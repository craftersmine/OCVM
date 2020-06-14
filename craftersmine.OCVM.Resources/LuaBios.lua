local component = require('component');

--print(component.type('0011'))
--print(component.slot('0011'))
p = component.proxy(component.list("eeprom")());
for k, v in pairs(p) do
	print(k, v);
end