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

function computer.users()
	return table.unpack(Computer.users());
end

function computer.addUser(user)
	return Computer.addUser(user);
end

function computer.removeUser(user)
	return Computer.removeUser(user);
end

function computer.pushSignal(name, ...)
	Computer.pushSignal(name, {...});
end

function computer.pullSignal(timeout)
	if not timeout and type(timeout) ~= "number" then timeout = 0 end;
	name, data = Computer.pullSignal(timeout);
end

function computer.getProgramLocations()
	return {};
end

return computer;