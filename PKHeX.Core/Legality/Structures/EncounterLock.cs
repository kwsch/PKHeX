namespace PKHeX.Core
{
    // Gender Locking
    public class EncounterLock
    {
        public int Species { get; set; }
        public int Nature { get; set; } = -1;
        public int Gender { get; set; } = -1;
    }
}
