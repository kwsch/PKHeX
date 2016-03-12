using System.Linq;

namespace PKHeX
{
    public enum Severity
    {
        Indeterminate = -2,
        Invalid = -1,
        Fishy = 0,
        Valid = 1,
        NotImplemented = 2,
    }
    public class LegalityCheck
    {
        public Severity Judgement = Severity.Valid;
        public string Comment = "Valid";
        public bool Valid => Judgement >= Severity.Fishy;

        public LegalityCheck() { }
        public LegalityCheck(Severity s, string c)
        {
            Judgement = s;
            Comment = c;
        }
        public static LegalityCheck verifyECPID(PK6 pk)
        {
            // Secondary Checks
            if (pk.EncryptionConstant == 0)
                return new LegalityCheck(Severity.Fishy, "Encryption Constant is not set.");

            if (pk.PID == 0)
                return new LegalityCheck(Severity.Fishy, "PID is not set.");

            if (pk.Gen6)
                return new LegalityCheck();

            // When transferred to Generation 6, the Encryption Constant is copied from the PID.
            // The PID is then checked to see if it becomes shiny with the new Shiny rules (>>4 instead of >>3)
            // If the PID is nonshiny->shiny, the top bit is flipped.

            // Check to see if the PID and EC are properly configured.
            bool xorPID = ((pk.TID ^ pk.SID ^ (int)(pk.PID & 0xFFFF) ^ (int)(pk.PID >> 16)) & 0x7) == 8;
            bool valid = xorPID
                ? pk.EncryptionConstant == (pk.PID ^ 0x8000000)
                : pk.EncryptionConstant == pk.PID;

            if (!valid)
                if (xorPID)
                    return new LegalityCheck(Severity.Invalid, "PID should be equal to EC [with top bit flipped]!");
                else
                    return new LegalityCheck(Severity.Invalid, "PID should be equal to EC!");

            return new LegalityCheck();
        }
        public static LegalityCheck verifyNickname(PK6 pk)
        {
            LegalityCheck r = new LegalityCheck { Judgement = Severity.NotImplemented };
            // If the Pokémon is not nicknamed, it should match one of the language strings.
            if (pk.Nickname.Length == 0)
            {
                r.Judgement = Severity.Indeterminate;
                r.Comment = "Pokémon nickname is empty.";
            }
            return r;
        }
        public static LegalityCheck verifyEVs(PK6 pk)
        {
            var EVs = pk.EVs;
            if (EVs.Sum() == 0 && pk.Met_Level != pk.Stat_Level && pk.Stat_Level > 1)
                return new LegalityCheck(Severity.Fishy, "All EVs are zero, but leveled above Met Level");
            if (EVs.Sum() == 508)
                return new LegalityCheck(Severity.Fishy, "2 EVs remaining.");
            if (EVs.Sum() > 510)
                return new LegalityCheck(Severity.Invalid, "EV total cannot be above 510.");
            if (EVs.Any(ev => ev > 252))
                return new LegalityCheck(Severity.Invalid, "EVs cannot go above 252.");
            if (EVs.All(ev => pk.EVs[0] == ev) && EVs[0] != 0)
                return new LegalityCheck(Severity.Fishy, "EVs are all equal.");

            return new LegalityCheck();
        }
        public static LegalityCheck verifyIVs(PK6 pk)
        {
            if (pk.IVs.Sum() == 0)
                return new LegalityCheck(Severity.Fishy, "All IVs are zero.");
            if (pk.IVs[0] < 30 && pk.IVs.All(iv => pk.IVs[0] == iv))
                return new LegalityCheck(Severity.Fishy, "All IVs are equal.");
            return new LegalityCheck();
        }
        public static LegalityCheck verifyID(PK6 pk)
        {
            if (pk.TID == 0 && pk.SID == 0)
                return new LegalityCheck(Severity.Fishy, "TID and SID are zero.");
            if (pk.TID == 0)
                return new LegalityCheck(Severity.Fishy, "TID is zero.");
            if (pk.SID == 0)
                return new LegalityCheck(Severity.Fishy, "SID is zero.");
            return new LegalityCheck();
        }

        public static LegalityCheck verifyEncounter(PK6 pk)
        {
            if (!pk.Gen6)
                return new LegalityCheck {Judgement = Severity.NotImplemented};

            if (pk.WasLink)
            {
                if (pk.FatefulEncounter || Legal.getLinkMoves(pk).Length == 0) // Should NOT be Fateful, and should be in Database
                    return new LegalityCheck(Severity.Invalid, "Not a valid Link gift.");
            }
            if (pk.WasEvent || pk.WasEventEgg)
            {
                var vwc6 = Legal.getValidWC6s(pk);
                try
                {
                    WC6 card = vwc6.First(wc6 => wc6.RelearnMoves.SequenceEqual(pk.RelearnMoves));
                    return new LegalityCheck(Severity.Valid, $"Matches #{card.CardID.ToString("0000")} ({card.CardTitle})");
                }
                catch { /* No card. */ }
                return new LegalityCheck(Severity.Invalid, "Not a valid Wonder Card gift.");
            }
            if (pk.WasEgg)
            {
                // Check Hatch Locations
                if (pk.Version < 26) // XY
                {
                    if (Legal.ValidMet_XY.Contains(pk.Met_Location))
                        return new LegalityCheck(Severity.Valid, "Valid XY hatched egg.");
                }
                else if (pk.Version < 28)
                {
                    if (Legal.ValidMet_AO.Contains(pk.Met_Location))
                        return new LegalityCheck(Severity.Valid, "Valid ORAS hatched egg.");
                }
                return new LegalityCheck(Severity.Invalid, "Invalid location for hatched egg.");
            }

            // Not Implemented: Stationary/Gifts

            if (Legal.getWildEncounterValid(pk))
                return new LegalityCheck(Severity.Valid, "Valid encounter at location.");

            return new LegalityCheck(Severity.Invalid, "Not a valid encounter.");
        }
    }
}
