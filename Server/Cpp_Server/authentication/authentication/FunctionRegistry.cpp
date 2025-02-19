#include "Server.h"

map<string, map<string, function<http::response<http::string_body>(const http::request<http::string_body>)>>> FunctionRegistry::functionMap;

void FunctionRegistry::METHOD_ADD(const string& functionName, const string& method, function<http::response<http::string_body>(const http::request<http::string_body>)> func) {
    functionMap[functionName][method] = func;
}

map<string, map<string, function<http::response<http::string_body>(const http::request<http::string_body>)>>>& FunctionRegistry::getFunctionMap() {
    return functionMap;
}