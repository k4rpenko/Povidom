#include "Server.h"

namespace HttpStatus = HttpController::HttpStatus;

void Server::monitorRequest(const string& address, int port, const string& method, const string& target) {
    cout << termcolor::white << termcolor::bold << address << ":" << port << termcolor::reset << " ";
    if (method == "POST") {
        cout << termcolor::yellow << termcolor::bold << "POST" << termcolor::reset << " ";
    }
    else if (method == "GET") {
        cout << termcolor::blue << termcolor::bold << "GET" << termcolor::reset << " ";
    }
    else if (method == "PUT") {
        cout << termcolor::on_bright_yellow << termcolor::bold << "PUT" << termcolor::reset << " ";
    }
    else if (method == "DELETE") {
        cout << termcolor::red << termcolor::bold << "DELETE" << termcolor::reset << " ";
    }
    else {
        cout << termcolor::magenta << termcolor::bold << method << termcolor::reset << " ";
    }
    cout << termcolor::green << termcolor::bold << target << termcolor::reset << endl;
}

void Server::handleSession(tcp::socket socket) {
    try {
        beast::flat_buffer buffer;

        http::request<http::string_body> req;
        http::read(socket, buffer, req);

        string target = req.target();
        string method = req.method_string();
        http::response<http::string_body> res;

        auto& functionMap = FunctionRegistry::getFunctionMap();
        auto func_it = functionMap.find(target);
        if (func_it != functionMap.end() && func_it->second.count(method)) {
            monitorRequest(socket.remote_endpoint().address().to_string(), socket.remote_endpoint().port(), method, target);
            cout << "Executing function for target: " << target << ", method: " << method << endl;
            res = func_it->second[method](req);
            cout << "Function executed successfully" << endl;

        }
        else {
            res = HttpStatus::Status::NotFound();
        }
        http::write(socket, res);
        socket.shutdown(tcp::socket::shutdown_send);
    }
    catch (const std::exception& e) {
        cerr << "Session error: " << e.what() << endl;
    }
}

void Server::runServer(const string& address, int port) {
    try {
        boost::asio::io_context ioc;
        tcp::acceptor acceptor(ioc, { asio::ip::make_address(address), static_cast<unsigned short>(port) });
        string art = R"(
 _____   ______   _____  _______
|  __ \ |  ____| / ____||__   __|
| |__) || |__   | (___     | |
|  _  / |  __|   \___ \    | |
| | \ \ | |____  ____) |   | |
|_|  \_\|______||_____/    |_|
                                     
    )";
        cout << termcolor::white << termcolor::bold << art << termcolor::reset << endl;
        cout << termcolor::white << termcolor::bold << "Port: " << termcolor::reset;
        cout << termcolor::green << termcolor::bold << port << termcolor::reset << endl;
        cout << termcolor::white << termcolor::bold << "Url: " << termcolor::reset;
        cout << termcolor::green << termcolor::bold << address << termcolor::reset << endl;
        cout << termcolor::white << termcolor::bold << "Status: " << termcolor::reset;
        cout << termcolor::green << termcolor::bold << "Ok" << termcolor::reset << endl;


        while (true) {
            tcp::socket socket(ioc);
            acceptor.accept(socket);
            thread(&Server::handleSession, this, std::move(socket)).detach();
        }
    }
    catch (const    exception& e) {
        cerr << "Server error: " << e.what() << endl;
    }
}