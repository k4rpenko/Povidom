#pragma once
#include <iostream>
#include "FunctionRegistry.h"
#include "message.model.cpp"

class message {
public:
    message();
    void new_message(const MessageModels& _message);
    void change_status();
};