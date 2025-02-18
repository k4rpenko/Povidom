#include "Hub.h"
#include <termcolor/termcolor.hpp>
#include "FunctionRegistry.h"

void Hub::monitoring(websocket::stream<tcp::socket>& ws) {
    while (true) {
        beast::flat_buffer buffer;
        ws.read(buffer);

        auto out = beast::buffers_to_string(buffer.cdata());
        json received_json;
        try {
            received_json = json::parse(out);
        }
        catch (const json::parse_error& e) {
            cerr << "Invalid JSON: " << e.what() << endl;
            ws.write(net::buffer("Invalid JSON format"));
            continue;
        }
        auto method_name = received_json["TYPE"].get<string>();

        auto& functionMap = FunctionRegistry::getFunctionMap();
        auto func_it = functionMap.find(method_name);

        if (func_it != functionMap.end()) {
            try {
                func_it->second(received_json["CONTEXT"]);
            }
            catch (const bad_any_cast& e) {
                cerr << "Bad any_cast: " << e.what() << endl;
                ws.write(net::buffer("Bad any_cast"));
            }
            catch (const json::exception& e) {
                cerr << "JSON parsing error: " << e.what() << endl;
                ws.write(net::buffer("JSON parsing error"));
            }
        }
        else {
            ws.write(net::buffer("Method not found"));
        }
    }

}

void Hub::start(unsigned short PORT, const string& URL, short nodes) {
    
    string art = R"(
__          __       _       _____               _           _
\ \        / /      | |     / ____|             | |         | |
 \ \  /\  / /   ___ | |__  | (___    ___    ___ | | __  ___ | |_
  \ \/  \/ /   / _ \| '_ \  \___ \  / _ \  / __|| |/ / / _ \| __|
   \  /\  /   |  __/| |_) | ____) || (_) || (__ |   < |  __/| |_
    \/  \/     \___||_.__/ |_____/  \___/  \___||_|\_\ \___| \__|    
                                          
    )";
    cout << termcolor::white << termcolor::bold << art << termcolor::reset << endl;
    cout << termcolor::white << termcolor::bold << "Port: " << termcolor::reset;
    cout << termcolor::green << termcolor::bold << PORT << termcolor::reset << endl;
    cout << termcolor::white << termcolor::bold << "Url: " << termcolor::reset;
    cout << termcolor::green << termcolor::bold << URL << termcolor::reset << endl;
    cout << termcolor::white << termcolor::bold << "Status: " << termcolor::reset;
    cout << termcolor::green << termcolor::bold << "Ok" << termcolor::reset << endl;
    const auto address = net::ip::make_address(URL);
    const auto port = static_cast<unsigned short>(PORT);

    tcp::acceptor acceptor{ ioc, {address, port} };
    while (true) {
        tcp::socket socket{ ioc };
        acceptor.accept(socket);
        cout << "Socket accepted" << endl;

        thread{ [q = move(socket), this]() mutable {
            try 
            {
                websocket::stream<tcp::socket> ws {move(q)};
                ws.accept();

                monitoring(ws);
            }
            catch (const beast::system_error& e) {
                if (e.code() == websocket::error::closed) {
                    cout << "WebSocket closed." << endl;
                }
                else {
                    cerr << "WebSocket error: " << e.what() << endl;
                }
            }
        } }.detach();
    }
}
