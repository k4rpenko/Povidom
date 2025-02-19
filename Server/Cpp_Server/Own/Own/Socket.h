#include <Winsock2.h>
#include <WS2tcpip.h>
#include <iostream>
#include <string>

using namespace std;

namespace Own
{
	class Socket
	{
	private:
		struct sockaddr_in address;
		int sock;
		int connection;
	public:
		Socket(int domain, int service, int protocol, int port, u_long Interface) {}
		virtual int connect_network(int sock, struct sockaddr_in address) = 0;

		//test connection - тестування зєднання
		void test_connections(int);

		//Getter functions - отримати значення  з функцій
		struct sockaddr_in get_address() { return address; }
		int get_sock() { return sock; }
		int get_connection() { return connection; }
	};
}


