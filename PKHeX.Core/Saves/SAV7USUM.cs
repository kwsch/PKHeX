namespace PKHeX.Core
{
    public class SAV7USUM : SAV7
    {
        public SAV7USUM(byte[] data) : base(data) => Initialize();
        public SAV7USUM() : base(SaveUtil.SIZE_G7USUM) => Initialize();
        public override SaveFile Clone() => new SAV7USUM((byte[])Data.Clone());

        private void Initialize()
        {
            Personal = PersonalTable.USUM;
            HeldItems = Legal.HeldItems_USUM;

            Items = new MyItem7USUM(this, Bag);
            Zukan = new Zukan7(this, PokeDex, PokeDexLanguageFlags);
            Records = new Record6(this, Record, Core.Records.MaxType_USUM);
        }

        protected override int EventFlagMax => 4928;
        public override int MaxMoveID => Legal.MaxMoveID_7_USUM;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_7_USUM;
        public override int MaxItemID => Legal.MaxItemID_7_USUM;
        public override int MaxAbilityID => Legal.MaxAbilityID_7_USUM;
    }
}