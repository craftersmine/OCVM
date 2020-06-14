local libcomponent = require("component");

local function bootstrap()
	local eeprom = libcomponent.list("eeprom")();
	if eeprom then
		local code = libcomponent.invoke(eeprom, "get");

	end
end
