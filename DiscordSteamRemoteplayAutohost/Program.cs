// See https://aka.ms/new-console-template for more information
using DiscordSteamRemoteplayAutohost;
using System;
using System.Runtime.InteropServices;

try
{
    if (!NativeMethods.SteamStuff_Init())
    {
        Console.WriteLine("Failed to initialize SteamStuff.dll");
        return;
    }

    var gameId = new CGameID(NativeMethods.SteamStuff_GetRunningGameID());
    Console.WriteLine($"Hello, World! Game ID: {gameId.m_GameID}");

    if (!gameId.IsValid)
    {
        Console.WriteLine("No game running");
        return;
    }
    if (gameId.Type != CGameID.EGameIDType.k_EGameIDTypeApp)
    {
        Console.WriteLine("Non-steam game running");
        return;
    }

    NativeMethods.SteamStuff_SetOnInviteCreated((invitee, connectURL) =>
    {
        Console.WriteLine($"Invite created for {invitee} with URL: {connectURL}");
    });
    NativeMethods.SteamStuff_SetOnSessionClosed(() =>
    {
        Console.WriteLine("Session closed");
    });

    NativeMethods.SteamStuff_SendInvite(0, gameId.m_GameID);

    while (true)
    {
        NativeMethods.SteamStuff_RunCallbacks();
        Console.WriteLine("Running...");
        System.Threading.Thread.Sleep(1000);
    }
}
finally
{
    NativeMethods.SteamStuff_Shutdown();
}
