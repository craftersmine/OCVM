fsProxy = component.proxy(component.list("filesystem")());
res, res1, res2, res3, res4, res5 = fsProxy.open("","");
data = { res, res1, res2, res3, res4, res5 }
breakpoint(data);

-- local function init do