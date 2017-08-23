#ifndef __STATE_NETWORK_SERVER_H
#define __STATE_NETWORK_SERVER_H

#include "StateNetwork/StatePacket.h"
class StateNetworkServer
{
private:
	static const int PORT  = 2550;
	static const int RECV_PORT = 2551;

	int udpSocket;
	int udpRecvSocket;
public:
	StateNetworkServer(void);
	~StateNetworkServer(void);
	/// Opens the socket of this network server.
	void Open();
	/// Closes the socket of this network server.
	void Close();
	/// Sends the given state packet over this network server.
	void SendStatePacket(OutputStatePacket pack);
	/// Receives an input packet from the server into the given pointer.
	/// Returns true if successful
	bool ReceiveStatePacket(InputStatePacket *pack);

    StateNetworkServer*	HAL_GetStateNetworkServerInstance();
    
};

extern float pwmValues[10];
extern void StartUnityThread();

#endif