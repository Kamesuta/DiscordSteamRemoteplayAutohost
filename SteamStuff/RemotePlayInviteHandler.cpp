#include "SteamStuff.h"
#include "RemotePlayInviteHandler.h"

RemotePlayInviteHandler::RemotePlayInviteHandler() :
    m_remoteGuestID(1),
    m_remoteInviteResultCb(this, &RemotePlayInviteHandler::OnRemotePlayInviteResult),
    m_remoteStopCb(this, &RemotePlayInviteHandler::OnRemotePlayStop),
    m_onInviteCreated(nullptr),
    m_onSessionClosed(nullptr)
{
}

uint64 RemotePlayInviteHandler::SendInvite(CSteamID invitee, CGameID gameID)
{
    RemotePlayPlayer_t rppInvitee = { invitee, m_remoteGuestID++, 0, 0, 0 };

    if (gameID.IsSteamApp() && gameID.AppID() != m_nonsteamAppID)
    {
        // Start Remote Play session
        GClientContext()->RemoteClientManager()->BCreateRemotePlayInviteAndSession(rppInvitee, gameID.AppID());
    }
    else
    {
        // non-Steam game is not supported
        return 0;
	}

    return rppInvitee.m_guestID;
}

void RemotePlayInviteHandler::CancelInvite(CSteamID invitee, uint64 guestID)
{
    if(GClientContext()->RemoteClientManager()->BIsStreamingSessionActive())
    {
        RemotePlayPlayer_t rppInvitee = { invitee, guestID, 0, 0, 0 };
        GClientContext()->RemoteClientManager()->CancelRemotePlayInviteAndSession(rppInvitee);
    }
}

void RemotePlayInviteHandler::OnRemotePlayInviteResult(RemotePlayInviteResult_t* inviteResultCb)
{
    if (inviteResultCb->m_eResult == k_ERemoteClientLaunchResultOK)
    {
        // Call the invite created callback
        if (m_onInviteCreated)
        {
			m_onInviteCreated(inviteResultCb->m_player.m_playerID, inviteResultCb->m_szConnectURL);
		}
    }
}

void RemotePlayInviteHandler::OnRemotePlayStop(RemoteClientStopStreamSession_t* streamStopCb)
{
    if (!GClientContext()->RemoteClientManager()->BIsStreamingSessionActive())
    {
        // Reset the guest ID
        m_remoteGuestID = 1;

        // Call the session closed callback
        if (m_onSessionClosed)
        {
			m_onSessionClosed();
		}
    }
}

// helper functions

RemotePlayInviteHandler* GRemotePlayInviteHandler()
{
    static RemotePlayInviteHandler handler;
    return &handler;
}

// exported functions

uint64 SteamStuff_SendInvite(uint64 invitee, uint64 gameID)
{
    return GRemotePlayInviteHandler()->SendInvite(CSteamID(invitee), CGameID(gameID));
}

void SteamStuff_CancelInvite(uint64 invitee, uint64 guestID)
{
    GRemotePlayInviteHandler()->CancelInvite(CSteamID(invitee), guestID);
}

void SteamStuff_SetOnInviteCreated(RemotePlayInviteHandler::OnInviteCreated* cb)
{
    GRemotePlayInviteHandler()->SetOnInviteCreated(cb);
}

void SteamStuff_SetOnSessionClosed(RemotePlayInviteHandler::OnSessionClosed* cb)
{
    GRemotePlayInviteHandler()->SetOnSessionClosed(cb);
}
