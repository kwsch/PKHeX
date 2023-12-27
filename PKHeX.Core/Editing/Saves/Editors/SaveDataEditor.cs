namespace PKHeX.Core;

/// <summary>
/// Environment for editing a <see cref="SaveFile"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class SaveDataEditor<T>
{
    public readonly SaveFile SAV;
    public readonly SlotEditor<T> Slots;
    public readonly IPKMView PKMEditor;

    public SaveDataEditor() : this(FakeSaveFile.Default) { }

    public SaveDataEditor(SaveFile sav)
    {
        SAV = sav;
        Slots = new SlotEditor<T>(sav);
        PKMEditor = new FakePKMEditor(SAV.BlankPKM);
    }

    public SaveDataEditor(SaveFile sav, IPKMView editor)
    {
        SAV = sav;
        Slots = new SlotEditor<T>(sav);
        PKMEditor = editor;
    }
}
