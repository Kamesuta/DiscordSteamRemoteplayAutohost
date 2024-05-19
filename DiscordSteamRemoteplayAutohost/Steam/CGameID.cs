using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordSteamRemoteplayAutohost.Steam
{
    [Serializable]
    public struct CGameID
    {
        public ulong m_GameID;

        public enum EGameIDType
        {
            k_EGameIDTypeApp = 0,
            k_EGameIDTypeGameMod = 1,
            k_EGameIDTypeShortcut = 2,
            k_EGameIDTypeP2P = 3,
        };

        public CGameID(ulong GameID)
        {
            m_GameID = GameID;
        }

        public CGameID(uint nAppID)
        {
            m_GameID = 0;
            SetAppID(nAppID);
        }

        public CGameID(uint nAppID, uint nModID)
        {
            m_GameID = 0;
            SetAppID(nAppID);
            SetType(EGameIDType.k_EGameIDTypeGameMod);
            SetModID(nModID);
        }

        public readonly uint AppID => (uint)(m_GameID & 0xFFFFFFul);

        public readonly EGameIDType Type => (EGameIDType)(m_GameID >> 24 & 0xFFul);

        public readonly uint ModID => (uint)(m_GameID >> 32 & 0xFFFFFFFFul);

        public readonly bool IsValid
        {
            get
            {
                // Each type has it's own invalid fixed point:
                return Type switch
                {
                    EGameIDType.k_EGameIDTypeApp => AppID != 0,
                    EGameIDType.k_EGameIDTypeGameMod => AppID != 0 && (ModID & 0x80000000) != 0,
                    EGameIDType.k_EGameIDTypeShortcut => (ModID & 0x80000000) != 0,
                    EGameIDType.k_EGameIDTypeP2P => AppID == 0 && (ModID & 0x80000000) != 0,
                    _ => false,
                };
            }
        }

        #region Private Setters for internal use
        private void SetAppID(uint other)
        {
            m_GameID = m_GameID & ~(0xFFFFFFul << 0) | (other & 0xFFFFFFul) << 0;
        }

        private void SetType(EGameIDType other)
        {
            m_GameID = m_GameID & ~(0xFFul << 24) | ((ulong)other & 0xFFul) << 24;
        }

        private void SetModID(uint other)
        {
            m_GameID = m_GameID & ~(0xFFFFFFFFul << 32) | (other & 0xFFFFFFFFul) << 32;
        }
        #endregion
    }
}
