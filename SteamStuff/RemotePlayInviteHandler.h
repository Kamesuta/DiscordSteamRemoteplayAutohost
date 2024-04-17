#ifndef REMOTEPLAYINVITEHANDLER_H
#define REMOTEPLAYINVITEHANDLER_H

#include <Steamworks.h>

class RemotePlayInviteHandler
{
public:
    RemotePlayInviteHandler();
    virtual ~RemotePlayInviteHandler() {}

    /**
        @brief Send an invite to a friend to join a Remote Play session.
        @param invitee The Steam ID of the friend to invite.
        @param gameID The game ID of the game to play.
        @return The guest ID of the invitee or 0 if the invite failed.
    */
    uint64 SendInvite(CSteamID invitee, CGameID gameID);

    /**
        @brief Cancel an invite to a friend to join a Remote Play session.
        @param invitee The Steam ID of the friend to cancel the invite for.
        @param guestID The guest ID of the invitee.
    */
    void CancelInvite(CSteamID invitee, uint64 guestID);

    /**
    	@brief Callback for when a Remote Play invite result is received.
        @param invitee The Steam ID of the invitee.
        @param connectURL The URL to connect to the Remote Play session.
    */
    using OnInviteCreated = void(CSteamID invitee, const char* connectURL);
    void SetOnInviteCreated(OnInviteCreated* cb) { m_onInviteCreated = cb; }

    /**
        @brief Callback for when a Remote Play session is closed.
    */
    using OnSessionClosed = void();
    void SetOnSessionClosed(OnSessionClosed* cb) { m_onSessionClosed = cb; }

private:
    /**
        @brief Non-Steam App ID.
    */
    static const AppId_t m_nonsteamAppID = 480;

    /**
        @brief Next guest ID of the invitee.
    */
    uint64 m_remoteGuestID;

    OnInviteCreated* m_onInviteCreated;
    OnSessionClosed* m_onSessionClosed;

    STEAM_CALLBACK(RemotePlayInviteHandler, OnRemotePlayStop, RemoteClientStopStreamSession_t, m_remoteStopCb);
    STEAM_CALLBACK(RemotePlayInviteHandler, OnRemotePlayInviteResult, RemotePlayInviteResult_t, m_remoteInviteResultCb);
};

RemotePlayInviteHandler* GRemotePlayInviteHandler();

DllExport uint64 SteamStuff_SendInvite(uint64 invitee, uint64 gameID);
DllExport void SteamStuff_CancelInvite(uint64 invitee, uint64 guestID);
DllExport void SteamStuff_SetOnInviteCreated(RemotePlayInviteHandler::OnInviteCreated* cb);
DllExport void SteamStuff_SetOnSessionClosed(RemotePlayInviteHandler::OnSessionClosed* cb);

#endif // REMOTEPLAYINVITEHANDLER_H
