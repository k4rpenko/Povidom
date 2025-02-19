#include <iostream>
#include "message.h"
#include "Hub.h"



int main() {
    message msg;

    Hub h;
    h.start(8099, "127.0.0.1", 1);
}