using System.Text;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.Logic.Packets.PacketHandlers;

namespace LeagueSandbox.GameServer.Logic.Packets.PacketDefinitions.S2C
{
    public class SpellAnimation : BasePacket
    {
        public SpellAnimation(AttackableUnit u, string animationName)
            : base(PacketCmd.PKT_S2C_SPELL_ANIMATION, u.NetId)
        {
            Write((byte)0xC4); // unk  <--
            Write((uint)0); // unk     <-- One of these bytes is a flag
            Write((uint)0); // unk     <--
            Write(1.0f); // Animation speed scale factor
            foreach (var b in Encoding.Default.GetBytes(animationName))
                Write(b);
            Write((byte)0);
        }
    }
}