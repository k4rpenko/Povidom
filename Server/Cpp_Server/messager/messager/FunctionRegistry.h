#pragma once
#include <map>
#include <functional>
#include <string>
#include <any>
#include <iostream>

using namespace std;

class FunctionRegistry {
private:
    static map<string, function<void(any)>> functionMap;

public:
    static void registerFunction(const string& methodName, function<void(any)> func);
    static map<string, function<void(any)>>& getFunctionMap();
};