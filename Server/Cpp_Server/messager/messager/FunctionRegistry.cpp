#include "FunctionRegistry.h"

map<string, function<void(any)>> FunctionRegistry::functionMap;

void FunctionRegistry::registerFunction(const string& methodName, function<void(any)> func) {
    functionMap[methodName] = func;
    cout << "Registered function: " << methodName << endl;
}

map<string, function<void(any)>>& FunctionRegistry::getFunctionMap() {
    return functionMap;
}