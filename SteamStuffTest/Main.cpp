#include <thread>
#include "SteamStuff.h"
#include "RemotePlayInviteHandler.h"

int main()
{
	if (!SteamStuff_Init())
	{
		std::cout << "Failed to initialize SteamStuff.dll" << std::endl;
		return 1;
	}

	auto gameId = CGameID(SteamStuff_GetRunningGameID());
	std::cout << "Hello, World! Game ID: " << gameId.ToUint64() << std::endl;

	if (!gameId.IsValid())
	{
		std::cout << "No game running" << std::endl;
		return 1;
	}
	if (!gameId.IsSteamApp())
	{
		std::cout << "Non-steam game running" << std::endl;
		return 1;
	}

	SteamStuff_SetOnInviteCreated([](CSteamID invitee, const char* connectURL)
		{
			std::cout << "Invite created for " << invitee << " with URL: " << connectURL << std::endl;
		});
	SteamStuff_SetOnSessionClosed([]()
		{
			std::cout << "Session closed" << std::endl;
		});

	while (true)
	{
		SteamStuff_RunCallbacks();
		std::cout << "Running..." << std::endl;
		std::this_thread::sleep_for(std::chrono::seconds(1));
	}

	// Shutdown the Steam client
	SteamStuff_Shutdown();
}