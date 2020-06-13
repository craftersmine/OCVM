local component = require('component');

print(component.doc('000', '000'))
print(component.invoke('000', 'TestInvokation', 'testDataArg'));
print(component.list('eeprom')());