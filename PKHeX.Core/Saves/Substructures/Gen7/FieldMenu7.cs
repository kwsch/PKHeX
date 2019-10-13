namespace PKHeX.Core
{
    public sealed class FieldMenu7 : SaveBlock
    {
        public FieldMenu7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public FieldMenu7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        // USUM ONLY
        public string RotomOT
        {
            get => SAV.GetString(Offset + 0x30, 0x1A);
            set => SAV.SetString(value, SAV.OTLength).CopyTo(Data, Offset + 0x30);
        }
    }
}