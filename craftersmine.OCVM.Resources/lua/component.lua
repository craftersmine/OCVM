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

	local list_mt = {};
	local key = nil;

	function list_mt.__call(t)
		key = next(list, key);
		if key then
			return key, list[key]
		end
	end

	return setmetatable(list, list_mt);
end

return component;