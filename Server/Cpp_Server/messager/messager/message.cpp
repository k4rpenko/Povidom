#include "message.h"

void message::new_message(const MessageModels& _message) {
    cout << "Message called with context : " << _message.Context << endl;
}

void message::change_status() {

}

message::message() {
    FunctionRegistry::registerFunction("new_message", [this](any msg) {
        json context_json = any_cast<json>(msg);
        MessageModels message_model = context_json.get<MessageModels>();
        this->new_message(message_model);
    });
}