#include "Socket.h"

using namespace Own;

Socket::Socket(int domain, int service, int protocol, int port, u_long Interface) {

	//Adrres strucktur-Структура адреси
	address.sin_family = domain;
	address.sin_port = htons(port);
	address.sin_addr.S_un.S_addr = htonl(Interface);

	//Establish connection-встановлення зєднання
	sock = socket(domain, service, protocol);
	test_connections(sock);

	connection = connect_network(sock, address);
	test_connections(connection);
}

void Socket::test_connections(int Item) {
	if (Item < 0) {
		perror("Faild to connect...");
		exit(EXIT_FAILURE);
	}
}