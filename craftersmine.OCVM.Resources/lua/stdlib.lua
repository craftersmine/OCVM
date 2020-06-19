-- Contains overrides to specific Lua functions

local std = {}

function std.checkArg(n, have, ...)
    have = type(have)
    result = checkArgType(have, ...);
    if not result then
        local msg = string.format("bad argument #%d (%s expected, got %s)", n, table.concat({...}, " or "), have)
        breakpoint(msg);
        error(msg, 3)
    else return;
    end
end

return std;