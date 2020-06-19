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

function unicode.charWidth(value)
	if type(value) == "string" and value ~= nil then
		return Unicode.charWidth(value);
	else return nil end;
end

return unicode;