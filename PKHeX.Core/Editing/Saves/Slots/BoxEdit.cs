using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Represents a Box Editor that loads the contents for easy manipulation.
    /// </summary>
    public sealed class BoxEdit
    {
        private readonly SaveFile SAV;
        private readonly PKM[] CurrentContents;

        public BoxEdit(SaveFile sav)
        {
            SAV = sav;
            CurrentContents = new PKM[sav.BoxSlotCount];
        }

        public void Reload() => LoadBox(CurrentBox);

        public void LoadBox(int box)
        {
            if ((uint)box >= SAV.BoxCount)
                throw new ArgumentOutOfRangeException(nameof(box));

            SAV.AddBoxData(CurrentContents, box, 0);
            CurrentBox = box;
        }

        public PKM this[int index]
        {
            get => CurrentContents[index];
            set
            {
                CurrentContents[index] = value;
                SAV.SetBoxSlotAtIndex(value, index);
            }
        }

        public int CurrentBox { get; private set; }
        public int BoxWallpaper { get => SAV.GetBoxWallpaper(CurrentBox); set => SAV.SetBoxWallpaper(CurrentBox, value); }
        public string BoxName { get => SAV.GetBoxName(CurrentBox); set => SAV.SetBoxName(CurrentBox, value); }

        public int MoveLeft(bool max = false)
        {
            int newBox = max ? 0 : (CurrentBox + SAV.BoxCount - 1) % SAV.BoxCount;
            LoadBox(newBox);
            return newBox;
        }

        public int MoveRight(bool max = false)
        {
            int newBox = max ? SAV.BoxCount - 1 : (CurrentBox + 1) % SAV.BoxCount;
            LoadBox(newBox);
            return newBox;
        }
    }
}
