#pragma once
#include <string>
#include <boost/beast/core.hpp>
#include <boost/beast/websocket.hpp>
#include <boost/asio.hpp>
#include <nlohmann/json.hpp>
#include <iostream>
#include <memory>
#include "FunctionRegistry.h"

namespace beast = boost::beast;
namespace websocket = beast::websocket;
namespace net = boost::asio;
using tcp = net::ip::tcp;
using json = nlohmann::json;
using namespace std;

class Hub
{
public:
    Hub() : ioc(1) {}
    void start(unsigned short PORT, const string& URL, short nodes);

private:
    net::io_context ioc;
    void monitoring(websocket::stream<tcp::socket>& ws);
};