#include "Hub.h"
#include <iostream>
#include "message.h"

int main() {
    //WebSocket class
    message msg;

    Hub h;
    h.start(8099, "127.0.0.1", 1);
}