#pragma once
#include <map>
#include <functional>
#include <string>
#include <boost/beast.hpp>
#include <nlohmann/json.hpp>

namespace http = boost::beast::http;
using namespace std;
using json = nlohmann::json;

class FunctionRegistry {
private:
    static map<string, map<string, function<http::response<http::string_body>(const http::request<http::string_body>)>>> functionMap;

public:
    static void METHOD_ADD(const string& functionName, const string& method, function<http::response<http::string_body>(const http::request<http::string_body>)> func);
    static map<string, map<string, function<http::response<http::string_body>(const http::request<http::string_body>)>>>& getFunctionMap();
};