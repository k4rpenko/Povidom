#pragma once
#include <boost/asio.hpp>
#include <boost/beast.hpp>
#include <iostream>
#include <termcolor/termcolor.hpp>
#include "FunctionRegistry.h"
#include "Status.h"

using tcp = boost::asio::ip::tcp;
namespace asio = boost::asio;
namespace beast = boost::beast;
namespace http = boost::beast::http;
using namespace std;

class Server : public HttpController::HttpStatus::Status {
private:
    void monitorRequest(const string& address, int port, const string& method, const string& target);
    void handleSession(tcp::socket socket);
public:
    void runServer(const string& address, int port);
};