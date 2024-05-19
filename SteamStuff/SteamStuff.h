#ifndef STEAMSTUFF_H
#define STEAMSTUFF_H

// Define DllExport to export functions from the DLL.
#ifdef DLL_EXPORT
    #define DllExport extern "C" __declspec(dllexport)
#else
    #define DllExport extern "C" __declspec(dllimport)
#endif

#include <Steamworks.h>

class ClientContext
{
public:
    ClientContext();
    virtual ~ClientContext();

    ISteamUser019* SteamUser();
    ISteamFriends015* SteamFriends();

    /**
        @brief Get the Remote Client Manager interface.
        @return The Remote Client Manager interface.
    */
    IClientRemoteClientManager* RemoteClientManager();

    /**
    	@brief Initialize the Steam client.
        @return True if the Steam client was initialized successfully.
    */
    bool Init();
    /**
        @brief Shutdown the Steam client.
    */
    void Shutdown();
    /**
        @brief Run the Steam client callbacks.
    */
    void RunCallbacks();

    /**
        @brief Get the game ID of the running game.
        @return The game ID of the running game.
    */
    CGameID GetRunningGameID();

private:
    HSteamPipe m_hPipe;
    HSteamUser m_hUser;

    ISteamClient019* m_pSteamClient;
    ISteamUser019* m_pSteamUser;
    ISteamFriends015* m_pSteamFriends;

    IClientEngine* m_pClientEngine;
    IClientRemoteClientManager* m_pClientRemoteManager;
    
    bool m_ShuttingDown;
    bool m_Initialized;
};

ClientContext* GClientContext();

DllExport bool SteamStuff_Init();
DllExport void SteamStuff_Shutdown();
DllExport void SteamStuff_RunCallbacks();
DllExport uint64 SteamStuff_GetRunningGameID();

#endif // !STEAMSTUFF_H
