namespace PKHeX
{
    public class EncounterStatic
    {
        public int Species;
        public int Level;

        public int Location = 0;
        public int Ability = 0;
        public int Form = 0;
        public bool? Shiny = null; // false = never, true = always, null = possible
        public int[] Relearn = new int[4];
        public int Gender = -1;
        public int EggLocation = 0;
        public Nature Nature = Nature.Random;
        public bool Gift = false;
        public int Ball = 4; // Gift Only
        public GameVersion Version = GameVersion.Any;
        public int[] IVs = { -1, -1, -1, -1, -1, -1 };
        public bool IV3;
        public int[] Contest = { 0, 0, 0, 0, 0, 0 };
        public int HeldItem { get; set; }

        public bool Fateful = false;
        public bool RibbonWishing = false;
    }
}
