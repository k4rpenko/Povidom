#include <iostream>
#include "Rest.h"
#include "Test.cpp"

int main() {
	// Server
	Rest rest;

	// Controllers
	Controllers::Test t;

	rest.runServer("127.0.0.1", 8080);
}