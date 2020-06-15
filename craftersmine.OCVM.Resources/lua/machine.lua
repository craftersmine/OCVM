local component = require('component');
local computer = require('computer');

local wrapUserdata, wrapSingleUserdata, unwrapUserdata, wrappedUserdataMeta

wrappedUserdataMeta = {
  -- Weak keys, clean up once a proxy is no longer referenced anywhere.
  __mode="k",
  -- We need custom persist logic here to avoid ERIS trying to save the
  -- userdata referenced in this table directly. It will be repopulated
  -- in the load methods of the persisted userdata wrappers (see below).
  [persistKey and persistKey() or "LuaJ"] = function()
    return function()
      -- When using special persistence we have to manually reassign the
      -- metatable of the persisted value.
      return setmetatable({}, wrappedUserdataMeta)
    end
  end
}
local wrappedUserdata = setmetatable({}, wrappedUserdataMeta)

local function processResult(result)
  result = wrapUserdata(result) -- needed for metamethods.
  if not result[1] then -- error that should be re-thrown.
    error(result[2], 0)
  else -- success or already processed error.
    return table.unpack(result, 2, result.n)
  end
end

local function invoke(target, direct, ...)
  local result
  if direct then
    local args = table.pack(...) -- for unwrapping
    args = unwrapUserdata(args)
    result = table.pack(target.invoke(table.unpack(args, 1, args.n)))
    args = nil -- clear upvalue, avoids trying to persist it
    if result.n == 0 then -- limit for direct calls reached
      result = nil
    end
    -- no need to wrap here, will be wrapped in processResult
  end
  if not result then
    local args = table.pack(...) -- for access in closure
    result = select(1, coroutine.yield(function()
      args = unwrapUserdata(args)
      local result = table.pack(target.invoke(table.unpack(args, 1, args.n)))
      args = nil -- clear upvalue, avoids trying to persist it
      result = wrapUserdata(result)
      return result
    end))
  end
  return processResult(result)
end

local function udinvoke(f, data, ...)
  local args = table.pack(...)
  args = unwrapUserdata(args)
  local result = table.pack(f(data, table.unpack(args)))
  args = nil -- clear upvalue, avoids trying to persist it
  return processResult(result)
end

-- Metatable for additional functionality on userdata.
local userdataWrapper = {
  __index = function(self, ...)
    return udinvoke(userdata.apply, wrappedUserdata[self], ...)
  end,
  __newindex = function(self, ...)
    return udinvoke(userdata.unapply, wrappedUserdata[self], ...)
  end,
  __call = function(self, ...)
    return udinvoke(userdata.call, wrappedUserdata[self], ...)
  end,
  __gc = function(self)
    local data = wrappedUserdata[self]
    wrappedUserdata[self] = nil
    userdata.dispose(data)
  end,
  -- This is the persistence protocol for userdata. Userdata is considered
  -- to be 'owned' by Lua, and is saved to an NBT tag. We also get the name
  -- of the actual class when saving, so we can create a new instance via
  -- reflection when loading again (and then immediately wrap it again).
  -- Collect wrapped callback methods.
  [persistKey and persistKey() or "LuaJ"] = function(self)
    local className, nbt = userdata.save(wrappedUserdata[self])
    -- The returned closure is what actually gets persisted, including the
    -- upvalues, that being the classname and a byte array representing the
    -- nbt data of the userdata value.
    return function()
      return wrapSingleUserdata(userdata.load(className, nbt))
    end
  end,
  -- Do not allow changing the metatable to avoid the gc callback being
  -- unset, leading to potential resource leakage on the host side.
  __metatable = "userdata",
  __tostring = function(self)
    local data = wrappedUserdata[self]
    return tostring(select(2, pcall(tostring, data)))
  end
}

local userdataCallback = {
  __call = function(self, ...)
    local methods = spcall(userdata.methods, wrappedUserdata[self.proxy])
    for name, direct in pairs(methods) do
      if name == self.name then
        return invoke(userdata, direct, self.proxy, name, ...)
      end
    end
    error("no such method", 1)
  end,
  __tostring = function(self)
    return userdata.doc(wrappedUserdata[self.proxy], self.name) or "function"
  end
}

function wrapSingleUserdata(data)
  -- Reuse proxies for lower memory consumption and more logical behavior
  -- without the need of metamethods like __eq, as well as proper reference
  -- behavior after saving and loading again.
  for k, v in pairs(wrappedUserdata) do
    -- We need a custom 'equals' check for userdata because metamethods on
    -- userdata introduced by JNLua tend to crash the game for some reason.
    if v == data then
      return k
    end
  end
  local proxy = {type = "userdata"}
  local methods = spcall(userdata.methods, data)
  for method in pairs(methods) do
    proxy[method] = setmetatable({name=method, proxy=proxy}, userdataCallback)
  end
  wrappedUserdata[proxy] = data
  return setmetatable(proxy, userdataWrapper)
end

function wrapUserdata(values)
  local processed = {}
  local function wrapRecursively(value)
    if type(value) == "table" then
      if not processed[value] then
        processed[value] = true
        for k, v in pairs(value) do
          value[k] = wrapRecursively(v)
        end
      end
    elseif type(value) == "userdata" then
      return wrapSingleUserdata(value)
    end
    return value
  end
  return wrapRecursively(values)
end

function unwrapUserdata(values)
  local processed = {}
  local function unwrapRecursively(value)
    if wrappedUserdata[value] then
      return wrappedUserdata[value]
    end
    if type(value) == "table" then
      if not processed[value] then
        processed[value] = true
        for k, v in pairs(value) do
          value[k] = unwrapRecursively(v)
        end
      end
    end
    return value
  end
  return unwrapRecursively(values)
end