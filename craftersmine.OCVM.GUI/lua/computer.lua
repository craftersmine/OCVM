import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.Base.LuaApi.OpenComputers');

local computer = {}

function computer.address()
	return Computer.address();
end

function computer.getBootAddress()
	return Computer.getBootAddress();
end

function computer.setBootAddress(address)
	if not address then address = ""; end;
	Computer.setBootAddress(address);
end

function computer.beep(frequency, duration)
	if not frequency then frequency = 440 end;
	if not duration then duration = 0.1 end;
	Computer.beep(frequency, duration);
end

function computer.uptime()
	return Computer.uptime();
end

function computer.tmpAddress()
	return "";
end

function computer.getFreeMemory()
	return Computer.getFreeMemory();
end

function computer.getTotalMemory()
	return Computer.getTotalMemory();
end

function computer.getDeviceInfo()
	return Computer.getDeviceInfo();
end

return computer;