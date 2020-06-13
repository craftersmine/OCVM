import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.Base.LuaApi.OpenComputers');

local component = {};

function component.doc(address, method)
	return Component.doc(address, method);
end

function component.invoke(address, method, ...)
	local arg = {...};
	return Component.invoke(address, method, arg);
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
	
end

function component.fields(address)
	
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