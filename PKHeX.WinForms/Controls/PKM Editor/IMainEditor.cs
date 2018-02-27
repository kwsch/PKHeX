using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public interface IMainEditor
    {
        bool Unicode { get; }
        bool FieldsInitialized { get; }
        bool ChangingFields { get; set; }
        bool HaX { get; }

        PKM pkm { get; }

        void UpdateIVsGB(bool skipForm);
    }
}