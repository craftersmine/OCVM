import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.Base.LuaApi.OpenComputers');

local unicode = {}

function unicode.char(value, ...)
	if type(value) == "string" and value ~= nil then
		data = Unicode._char({...})
		return table.unpack(data);
	else
		return nil;
	end;
end;

function unicode.charWidth(value, ...)
	return Unicode.charWidth(tostring(value))
end

function unicode.isWide(value, ...)
	return Unicode.isWide(tostring(value));
end

function unicode.len(value)
	return Unicode.len(tostring(value));
end

return unicode;