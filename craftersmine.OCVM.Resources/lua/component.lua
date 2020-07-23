import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.Base.LuaApi.OpenComputers');

local component = {};

function component.doc(address, method)
	return Component.doc(address, method);
end

function component.invoke(address, method, ...)
	local arg = {...};
	--local res, reason = pcall(Component.invoke, address, method, arg);
	--breakpoint(res);
	--breakpoint(reason);
	--if not res and reason then
		--breakpoint(reason)
	--	error (reason.InnerException.Message, 2);
	--end
	local data = Component.invoke(address, method, arg);
	return data;
end

function component.list(filter, exact)
	if filter == nil then filter = '' end;
	if exact == nil then exact = false end;

	local list = Component.list(filter, exact);

	local key = nil;

	return setmetatable(list, {
		__call = function()
			key = next(list, key);
			if key then
				return key, list[key]
			end
		end
	});
end

function component.proxy(address)
	local type, reason = component.type(address);
	if not type then
		return nil, reason;
	end

	local slot, reason = component.slot(address);
	if not slot then
		return nil, reason;
	end

	local proxy = { address = address, type = type, slot = slot, fields = {} }

	local methods, reason = component.methods(address);
	if not methods then
		return nil, reason;
	end

	for method, info in pairs(methods) do
		if not info.getter and not info.setter then
			proxy[method] = setmetatable({ address = address, name = method }, {
				__call = function(self, ...)
					return component.invoke(self.address, self.name, ...);
				end,

				__tostring = function()
					return component.doc(self.address, self.name) or "function";
				end
			});
		else
			proxy.fields[method] = info;
		end
	end


	--local fields, reason = component.fields(address);
	--for field, info in pairs(fields) do
	--	proxy.fields[method] = info;
	--end
	
	setmetatable(proxy, 
	{
		__index = function (self, key)
			if self.fields[key] and self.fields[key].getter then
				return component.invoke(self.address, key);
			end
		end,

		__newindex = function(self, key, value)
			if self.fields[key] and self.fields[key].setter then
				component.invoke(self.address, key, value);
			elseif self.fields[key] and self.fields[key].getter then
				error("field is read-only");
			else
			rawset(self, key, value);
			end
		end,

		__pairs = function (self)
			local keyProxy, keyField, value;
			return function ()
				if not keyField then
					repeat
						keyProxy, value = next(self, keyProxy);
					until not keyProxy or keyProxy ~= "fields";
				end
				if not keyProxy then
					keyField, value = next(self.fields, keyField);
				end
				return keyProxy or keyField, value;
			end
		end
	});
	return proxy;
end

function component.type(address)
	if address == nil then address = '' end;
	local _type = Component.type(address);
	return _type[1], _type[2];
end

function component.slot(address)
	if address == nil then address = '' end;
	local _slot = Component.slot(address);
	return _slot[1], _slot[2];
end

function component.methods(address)
	if address == nil then address = '' end;
	local _methods = Component.methods(address);
	return _methods;
end

-- due to be uncommented on official API page, dropped from VM, works as dummy
-- just returns empty table and error if device not found
function component.fields(address)
	if address == nil then address = '' end;
	local data = Component.fields(address);
	if data[1] == true then 
		return {}, nil;
	else
		return nil, data[2];
	end
end

function component.get__(address, componentType)
	if address == nil then address = ''; end;
	if componentType == nil then componentType = ''; end;

	local data = Component.get(address, componentType);
	if data[1] == nil then
		return nil, data[2];
	else 
		return data[1], nil;
	end
end

function component.isAvailable__(componentType)
	if componentType == nil then componentType = '' end;
	return Component.isAvailable(componentType);
end

local componentProxy = {
	__index = function (self, key)
		if self.fields[key] and self.fields[key].getter then
			return component.invoke(self.address, key);
		end
	end,

	__newindex = function(self, key, value)
		if self.fields[key] and self.fields[key].setter then
			component.invoke(self.address, key, value);
		elseif self.fields[key] and self.fields[key].getter then
			error("field is read-only");
		else
			rawset(self, key, value);
		end
	end,

	__pairs = function (self)
		local keyProxy, keyField, value;
		return function ()
			if not keyField then
				repeat
					keyProxy, value = next(self, keyProxy);
				until not keyProxy or keyProxy ~= "fields";
			end
			if not keyProxy then
				keyField, value = next(self.fields, keyField);
			end
			return keyProxy or keyField, value;
		end
	end
}

local componentCallback = {
	__call = function(self, ...)
		return component.invoke(self.address, self.name, ...);
	end,

	__tostring = function()
		return component.doc(self.address, self.name) or "function";
	end
}

return component;