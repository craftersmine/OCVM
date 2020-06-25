import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.Base.LuaApi.OpenComputers');

local unicode = {}

function unicode.char(...)
	data = Unicode._char({...})
	return table.unpack(data);
end;

function unicode.charWidth(value, ...)
	return Unicode.charWidth(value)
end

function unicode.isWide(value, ...)
	return Unicode.isWide(tostring(value));
end

function unicode.len(value)
	return Unicode.len(tostring(value));
end

function unicode.lower(value)
	return Unicode.lower(tostring(value));
end

function unicode.reverse(value)
	return Unicode.reverse(value);
end

function unicode.sub(str, start, endV)
	if endV == nil or endV == 0 then return Unicode.sub(str, start)
	else return Unicode.sub(str, start, endV);
	end;
end

function unicode.upper(str)
	return Unicode.upper(str);
end

function unicode.wlen(str)
	return Unicode.wlen(str);
end

function unicode.wtrunc(str, count)
	return Unicode.wtrunc(str, count);
end

return unicode;