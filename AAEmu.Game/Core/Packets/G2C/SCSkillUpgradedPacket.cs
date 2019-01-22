using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Game;
using AAEmu.Game.Models.Game.Skills;

namespace AAEmu.Game.Core.Packets.G2C
{
    public class SCSkillUpgradedPacket : GamePacket
    {
        private readonly Skill _skill;

        public SCSkillUpgradedPacket(Skill skill) : base(0x100, 1)
        {
            _skill = skill;
        }

        public override PacketStream Write(PacketStream stream)
        {
            stream.Write(_skill.Id);
            stream.Write(_skill.Level);
            return stream;
        }
    }
}