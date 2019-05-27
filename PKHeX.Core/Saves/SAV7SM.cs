namespace PKHeX.Core
{
    public class SAV7SM : SAV7
    {
        public SAV7SM(byte[] data) : base(data) => Initialize();
        public SAV7SM() : base(SaveUtil.SIZE_G7SM) => Initialize();
        public override SaveFile Clone() => new SAV7SM((byte[])Data.Clone());

        private void Initialize()
        {
            Personal = PersonalTable.SM;
            HeldItems = Legal.HeldItems_SM;

            Items = new MyItem7SM(this, Bag);
            Zukan = new Zukan7(this, PokeDex, PokeDexLanguageFlags);
            Records = new Record6(this, Record, Core.Records.MaxType_SM);
        }

        protected override int EventFlagMax => 3968;
        public override int MaxMoveID => Legal.MaxMoveID_7;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_7;
        public override int MaxItemID => Legal.MaxItemID_7;
        public override int MaxAbilityID => Legal.MaxAbilityID_7;
    }
}