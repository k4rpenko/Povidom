#pragma once
#include <iostream>
#include <string>
#include <boost/beast.hpp>

namespace beast = boost::beast;
namespace http = boost::beast::http;
using namespace std;

namespace HttpController {
	namespace HttpStatus {
		class Status
		{
		private:
			void handle_request(http::request<http::string_body>& req, http::response<http::string_body>& res);
		public:
			static http::response<http::string_body> OK(any message = any{});
			static http::response<http::string_body> NotFound(any message = any{});
			static http::response<http::string_body> BadRequest(any message = any{});
			static http::response<http::string_body> ERROR_METHOD();
		};
	}
}