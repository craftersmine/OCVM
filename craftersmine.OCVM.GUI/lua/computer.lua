import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.Base.LuaApi.OpenComputers');

local computer = {}

function computer.address()
	return Computer.address();
end

function computer.getBootAddress()
	return Computer.getBootAddress();
end

return computer;