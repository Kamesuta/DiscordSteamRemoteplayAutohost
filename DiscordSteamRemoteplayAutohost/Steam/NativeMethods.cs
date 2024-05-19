using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DiscordSteamRemoteplayAutohost
{
    internal partial class NativeMethods
    {
        // DllExport bool SteamStuff_Init();
        [LibraryImport("SteamStuff.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SteamStuff_Init();

        // DllExport void SteamStuff_Shutdown();
        [LibraryImport("SteamStuff.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void SteamStuff_Shutdown();

        // DllExport void SteamStuff_RunCallbacks();
        [LibraryImport("SteamStuff.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void SteamStuff_RunCallbacks();

        // DllExport uint64 SteamStuff_GetRunningGameID();
        [LibraryImport("SteamStuff.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial ulong SteamStuff_GetRunningGameID();

        // DllExport uint64 SteamStuff_SendInvite(uint64 invitee, uint64 gameID);
        [LibraryImport("SteamStuff.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial ulong SteamStuff_SendInvite(ulong invitee, ulong gameID);

        // DllExport void SteamStuff_CancelInvite(uint64 invitee, uint64 guestID);
        [LibraryImport("SteamStuff.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void SteamStuff_CancelInvite(ulong invitee, ulong guestID);

        // using OnInviteCreated = void(CSteamID invitee, const char* connectURL);
        public delegate void OnInviteCreated(ulong invitee, string connectURL);
        // DllExport void SteamStuff_SetOnInviteCreated(RemotePlayInviteHandler::OnInviteCreated* cb);
        [LibraryImport("SteamStuff.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void SteamStuff_SetOnInviteCreated(OnInviteCreated cb);

        // using OnSessionClosed = void();
        public delegate void OnSessionClosed();
        // DllExport void SteamStuff_SetOnSessionClosed(RemotePlayInviteHandler::OnSessionClosed* cb);
        [LibraryImport("SteamStuff.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void SteamStuff_SetOnSessionClosed(OnSessionClosed cb);
    }
}
