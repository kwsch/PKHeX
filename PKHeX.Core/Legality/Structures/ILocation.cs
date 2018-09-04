namespace PKHeX.Core
{
    public interface ILocation
    {
        int Location { get; set; }
        int EggLocation { get; set; }
    }

    public static partial class Extensions
    {
        public static int GetLocation(this ILocation encounter)
        {
            if (encounter == null)
                return -1;
            return encounter.Location != 0
                ? encounter.Location
                : encounter.EggLocation;
        }

        internal static string GetEncounterLocation(this ILocation Encounter, int gen, int version = -1)
        {
            int loc = Encounter.GetLocation();
            if (loc < 0)
                return null;

            if (version == (int)GameVersion.CXD) // handle C/XD locations
            {
                var locs = GameInfo.Strings.metCXD_00000;
                return loc >= locs.Length ? null : locs[loc];
            }

            return GameInfo.GetLocationName(loc != Encounter.Location, loc, gen, gen);
        }
    }
}
