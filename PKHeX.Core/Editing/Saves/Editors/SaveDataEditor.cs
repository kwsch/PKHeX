namespace PKHeX.Core
{
    /// <summary>
    /// Environment for editing a <see cref="SaveFile"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SaveDataEditor<T>
    {
        public readonly SaveFile SAV;
        public readonly SlotEditor<T> Slots;

        public IPKMView PKMEditor { get; set; }

        public SaveDataEditor(SaveFile sav)
        {
            SAV = sav;
            Slots = new SlotEditor<T>(sav);
        }
    }
}