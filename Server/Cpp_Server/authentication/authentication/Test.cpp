#include "Rest.h"
#include <boost/any.hpp>

namespace Controllers {
	class Test : public Rest {
	public:
        Test() {
            FunctionRegistry::METHOD_ADD("/API/test", "POST", [this](const http::request<http::string_body>& REQ) {
                return this->Test_post(REQ);
            });
        }

        http::response<http::string_body> Test_post(const http::request<http::string_body>& req){
            try {
                string name = req.body();

                cout << "Received parameter 'name': " << name << endl;
                auto response = HttpController::HttpStatus::Status::OK();
                cout << "Response created successfully" << response << endl;
                return response;
            }
            catch(const char* exception){
                cerr << "Error: " << exception << '\n';
            }
        }
	};
}