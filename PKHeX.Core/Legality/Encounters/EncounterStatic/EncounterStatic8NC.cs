using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Distributed Crystal Data)
    /// </summary>
    public sealed class EncounterStatic8NC : EncounterStatic8Nest<EncounterStatic8NC>
    {
        public override int Location { get => SharedNest; set { } }

        protected override bool IsMatchLocation(PKM pkm)
        {
            var loc = pkm.Met_Location;
            return loc == SharedNest || loc == Watchtower;
        }
    }
}