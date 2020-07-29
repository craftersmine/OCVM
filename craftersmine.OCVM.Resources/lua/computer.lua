import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.Base.LuaApi.OpenComputers');

local computer = {}

function computer.address()
	return Computer.address();
end

function computer._getBootAddress()
	return Computer.getBootAddress();
end

function computer._setBootAddress(address)
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
	return Computer.tmpAddress();
end

function computer.freeMemory()
	return Computer.getFreeMemory();
end

function computer.totalMemory()
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
	--breakpoint("pushed signal " .. tostring(name));
	--breakpoint({...})
	--breakpoint(name);
	Computer.pushSignal(tostring(name), {...});
end

function computer.pullSignal(timeout)
	local deadline = computer.uptime() + (type(timeout) == "number" and timeout or math.huge);
	repeat
		local name, sig = Computer.pullSignal();
		if sig ~= nil and (name ~= nil and type(name) == "string") then
			packedSig = table.pack(name, table.unpack(sig));
			--breakpoint(packedSig)
			if packedSig.n > 0 then
				return table.unpack(packedSig, 1, packedSig.n);
			end
		end
	until computer.uptime() >= deadline;
end

function computer.getProgramLocations()
	return {};
end

return computer;