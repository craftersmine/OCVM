-- Contains overrides to specific Lua functions

local std = {}

local tooLongWithoutYielding = setmetatable({},  { __tostring = function() return "too long without yielding" end})

function std.checkArg(n, have, ...)
    have = type(have)
    result = checkArgType(have, ...);
    if not result then
        local msg = string.format("bad argument #%d (%s expected, got %s)", n, table.concat({...}, " or "), have)
        breakpoint(msg);
        error(msg, 3)
    else return;
    end
end

function std.xpcall(f, msgh, ...)
    breakpoint("xpcall called");
    local handled = false
    checkArg(2, msgh, "function")
    local result = table.pack(xpcall(f, function(...)
      if rawequal((...), tooLongWithoutYielding) then
        return tooLongWithoutYielding
      elseif handled then
        return ...
      else
        handled = true
        return msgh(...)
      end
    end, ...))
    if rawequal(result[2], tooLongWithoutYielding) then
      result = table.pack(result[1], select(2, pcallTimeoutCheck(pcall(msgh, tostring(tooLongWithoutYielding)))))
    end
    return table.unpack(result, 1, result.n)
end

return std;