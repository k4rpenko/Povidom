#include "Status.h"

void HttpController::HttpStatus::Status::handle_request(beast::http::request<beast::http::string_body>& req, beast::http::response<beast::http::string_body>& res) {
    if (req.method() == http::verb::get) {
        res.result(http::status::ok);
        res.set(http::field::content_type, "text/plain");
        res.body() = "Hello, World!";
    }
    else {
        res.result(beast::http::status::method_not_allowed);
        res.set(beast::http::field::content_type, "text/plain");
        res.body() = "Method Not Allowed";
    }
    res.prepare_payload();
}

http::response<http::string_body> HttpController::HttpStatus::Status::OK(any message) {
    http::response<http::string_body> res{ http::status::ok, 11 };
    res.set(http::field::content_type, "text/plain");
    res.body() = message.has_value() ? any_cast<string>(message) : "OK";
    res.prepare_payload();
    return res;
}

http::response<http::string_body> HttpController::HttpStatus::Status::NotFound(any message) {
    http::response<http::string_body> res{ http::status::not_found, 11 };
    res.set(http::field::content_type, "text/plain");
    res.body() = message.has_value() ? any_cast<string>(message) : "404 Not Found";
    res.prepare_payload();
    return res;
}

http::response<http::string_body> HttpController::HttpStatus::Status::BadRequest(any message) {
    http::response<http::string_body> res{ http::status::bad_request, 11 };
    res.set(http::field::content_type, "text/plain");
    res.body() = message.has_value() ? any_cast<string>(message) : "400 Bad Request";
    res.prepare_payload();
    return res;
}

http::response<http::string_body> HttpController::HttpStatus::Status::ERROR_METHOD() {
    http::response<http::string_body> res{ http::status::method_not_allowed, 11 };
    res.set(http::field::content_type, "text/plain");
    res.body() = "404 Not Found";
    res.prepare_payload();
    return res;
}