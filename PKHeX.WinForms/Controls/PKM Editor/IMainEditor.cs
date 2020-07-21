using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public interface IMainEditor : IPKMView
    {
        void UpdateIVsGB(bool skipForm);
        PKM Entity { get; }
        SaveFile RequestSaveFile { get; }
    }
}