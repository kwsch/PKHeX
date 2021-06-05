using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Pokédex structure used by <see cref="SAV4"/> games.
    /// </summary>
    public sealed class Zukan4 : ZukanBase
    {
        private readonly byte[] Data;
        private readonly int Offset;

        // General structure: u32 magic, 4*bitflags, u32 spinda, form flags, language flags, more form flags, upgrade flags

        /* 4 BitRegions with 0x40*8 bits
        * Region 0: Caught (Captured/Owned) flags
        * Region 1: Seen flags
        * Region 2: First Seen Gender
        * Region 3: Second Seen Gender
        * When setting a newly seen species (first time), we set the gender bit to both First and Second regions.
        * When setting an already-seen species, we set the Second region bit if the now-seen gender-bit is not equal to the first-seen bit.
        * 4 possible states: 00, 01, 10, 11
        * 00 - 1Seen: Male Only
        * 01 - 2Seen: Male First, Female Second
        * 10 - 2Seen: Female First, Male Second
        * 11 - 1Seen: Female Only
        * assuming the species is seen, (bit1 ^ bit2) + 1 = genders in dex
        */

        public const string GENDERLESS = "Genderless";
        public const string MALE = "Male";
        public const string FEMALE = "Female";
        private const int SIZE_REGION = 0x40;
        private const int COUNT_REGION = 4;
        private const int OFS_SPINDA = sizeof(uint) + (COUNT_REGION * SIZE_REGION);
        private const int OFS_FORM1 = OFS_SPINDA + sizeof(uint);

        private bool HGSS => SAV is SAV4HGSS;
        private bool DP => SAV is SAV4DP;

        public Zukan4(SAV4 sav, int offset) : base(sav, offset)
        {
            Data = sav.General;
            Offset = offset;
        }

        public uint Magic { get => BitConverter.ToUInt32(Data, Offset); set => BitConverter.GetBytes(value).CopyTo(Data, Offset); }

        public override bool GetCaught(int species) => GetRegionFlag(0, species - 1);
        public override bool GetSeen(int species) => GetRegionFlag(1, species - 1);
        public int GetSeenGenderFirst(int species) => GetRegionFlag(2, species - 1) ? 1 : 0;
        public int GetSeenGenderSecond(int species) => GetRegionFlag(3, species - 1) ? 1 : 0;
        public bool GetSeenSingleGender(int species) => GetSeenGenderFirst(species) == GetSeenGenderSecond(species);

        private bool GetRegionFlag(int region, int index)
        {
            var ofs = Offset + 4 + (region * SIZE_REGION) + (index >> 3);
            return FlagUtil.GetFlag(Data, ofs, index);
        }

        public void SetCaught(int species, bool value = true) => SetRegionFlag(0, species - 1, value);
        public void SetSeen(int species, bool value = true) => SetRegionFlag(1, species - 1, value);
        public void SetSeenGenderFirst(int species, int value = 0) => SetRegionFlag(2, species - 1, value == 1);
        public void SetSeenGenderSecond(int species, int value = 0) => SetRegionFlag(3, species - 1, value == 1);

        private void SetRegionFlag(int region, int index, bool value)
        {
            var ofs = Offset + 4 + (region * SIZE_REGION) + (index >> 3);
            FlagUtil.SetFlag(Data, ofs, index, value);
        }

        public uint SpindaPID { get => BitConverter.ToUInt32(Data, Offset + OFS_SPINDA); set => BitConverter.GetBytes(value).CopyTo(Data, Offset); }

        public static string[] GetFormNames4Dex(int species)
        {
            string[] formNames = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Array.Empty<string>(), 4);
            if (species == (int)Species.Pichu)
                formNames = new[] { MALE, FEMALE, formNames[1] }; // Spiky
            return formNames;
        }

        public int[] GetForms(int species)
        {
            const int brSize = 0x40;
            if (species == (int)Species.Deoxys)
            {
                uint val = (uint)(Data[Offset + 0x4 + (1 * brSize) - 1] | Data[Offset + 0x4 + (2 * brSize) - 1] << 8);
                return GetDexFormValues(val, 4, 4);
            }

            int FormOffset1 = Offset + 4 + (4 * brSize) + 4;
            switch (species)
            {
                case (int)Species.Shellos: // Shellos
                    return GetDexFormValues(Data[FormOffset1 + 0], 1, 2);
                case (int)Species.Gastrodon: // Gastrodon
                    return GetDexFormValues(Data[FormOffset1 + 1], 1, 2);
                case (int)Species.Burmy: // Burmy
                    return GetDexFormValues(Data[FormOffset1 + 2], 2, 3);
                case (int)Species.Wormadam: // Wormadam
                    return GetDexFormValues(Data[FormOffset1 + 3], 2, 3);
                case (int)Species.Unown: // Unown
                    return Data.Slice(FormOffset1 + 4, 0x1C).Select(i => (int)i).ToArray();
            }
            if (DP)
                return Array.Empty<int>();

            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
            int FormOffset2 = PokeDexLanguageFlags + 0x1F4;
            return species switch
            {
                (int)Species.Rotom => GetDexFormValues(BitConverter.ToUInt32(Data, FormOffset2), 3, 6),
                (int)Species.Shaymin => GetDexFormValues(Data[FormOffset2 + 4], 1, 2),
                (int)Species.Giratina => GetDexFormValues(Data[FormOffset2 + 5], 1, 2),
                (int)Species.Pichu when HGSS => GetDexFormValues(Data[FormOffset2 + 6], 2, 3),
                _ => Array.Empty<int>()
            };
        }

        public void SetForms(int species, int[] forms)
        {
            const int brSize = 0x40;
            switch (species)
            {
                case (int)Species.Deoxys: // Deoxys
                    uint newval = SetDexFormValues(forms, 4, 4);
                    Data[Offset + 0x4 + (1 * brSize) - 1] = (byte)(newval & 0xFF);
                    Data[Offset + 0x4 + (2 * brSize) - 1] = (byte)((newval >> 8) & 0xFF);
                    break;
            }

            int FormOffset1 = Offset + OFS_FORM1;
            switch (species)
            {
                case (int)Species.Shellos: // Shellos
                    Data[FormOffset1 + 0] = (byte)SetDexFormValues(forms, 1, 2);
                    return;
                case (int)Species.Gastrodon: // Gastrodon
                    Data[FormOffset1 + 1] = (byte)SetDexFormValues(forms, 1, 2);
                    return;
                case (int)Species.Burmy: // Burmy
                    Data[FormOffset1 + 2] = (byte)SetDexFormValues(forms, 2, 3);
                    return;
                case (int)Species.Wormadam: // Wormadam
                    Data[FormOffset1 + 3] = (byte)SetDexFormValues(forms, 2, 3);
                    return;
                case (int)Species.Unown: // Unown
                    int ofs = FormOffset1 + 4;
                    int len = forms.Length;
                    Array.Resize(ref forms, 0x1C);
                    for (int i = len; i < forms.Length; i++)
                        forms[i] = 0xFF;
                    Array.Copy(forms.Select(b => (byte)b).ToArray(), 0, Data, ofs, forms.Length);
                    return;
            }

            if (DP)
                return;

            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
            int FormOffset2 = PokeDexLanguageFlags + 0x1F4;
            switch (species)
            {
                case (int)Species.Rotom: // Rotom
                    BitConverter.GetBytes(SetDexFormValues(forms, 3, 6)).CopyTo(Data, FormOffset2);
                    return;
                case (int)Species.Shaymin: // Shaymin
                    Data[FormOffset2 + 4] = (byte)SetDexFormValues(forms, 1, 2);
                    return;
                case (int)Species.Giratina: // Giratina
                    Data[FormOffset2 + 5] = (byte)SetDexFormValues(forms, 1, 2);
                    return;
                case (int)Species.Pichu when HGSS: // Pichu
                    Data[FormOffset2 + 6] = (byte)SetDexFormValues(forms, 2, 3);
                    return;
            }
        }

        private static int[] GetDexFormValues(uint Value, int BitsPerForm, int readCt)
        {
            int[] Forms = new int[readCt];
            int n1 = 0xFF >> (8 - BitsPerForm);
            for (int i = 0; i < Forms.Length; i++)
            {
                int val = (int)(Value >> (i * BitsPerForm)) & n1;
                if (n1 == val && BitsPerForm > 1)
                    Forms[i] = -1;
                else
                    Forms[i] = val;
            }

            // (BitsPerForm > 1) was already handled, handle (BitsPerForm == 1)
            if (BitsPerForm == 1 && Forms[0] == Forms[1] && Forms[0] == 1)
                Forms[0] = Forms[1] = -1;

            return Forms;
        }

        private static uint SetDexFormValues(int[] Forms, int BitsPerForm, int readCt)
        {
            int n1 = 0xFF >> (8 - BitsPerForm);
            uint Value = 0xFFFFFFFF << (readCt * BitsPerForm);
            for (int i = 0; i < Forms.Length; i++)
            {
                int val = Forms[i];
                if (val == -1)
                    val = n1;

                Value |= (uint)(val << (BitsPerForm * i));
                if (i >= readCt)
                    throw new ArgumentException("Array count should be less than bitfield count", nameof(Forms));
            }
            return Value;
        }

        private static bool TryInsertForm(int[] forms, int form)
        {
            if (Array.IndexOf(forms, form) >= 0)
                return false; // already in list

            // insert at first empty
            var index = Array.IndexOf(forms, -1);
            if (index < 0)
                return false; // no free slots?

            forms[index] = form;
            return true;
        }

        public int GetUnownFormIndex(int form)
        {
            var ofs = Offset + OFS_FORM1 + 4;
            for (int i = 0; i < 0x1C; i++)
            {
                byte val = Data[ofs + i];
                if (val == form)
                    return i;
                if (val == 0xFF)
                    return -1;
            }
            return -1;
        }

        public int GetUnownFormIndexNext(int form)
        {
            var ofs = Offset + OFS_FORM1 + 4;
            for (int i = 0; i < 0x1C; i++)
            {
                byte val = Data[ofs + i];
                if (val == form)
                    return i;
                if (val == 0xFF)
                    return i;
            }

            return -1;
        }

        public void ClearUnownForms()
        {
            var ofs = Offset + OFS_FORM1 + 4;
            for (int i = 0; i < 0x1C; i++)
                Data[ofs + i] = 0xFF;
        }

        public bool GetUnownForm(int form) => GetUnownFormIndex(form) != -1;

        public void AddUnownForm(int form)
        {
            var index = GetUnownFormIndexNext(form);
            if (index == -1)
                return;

            var ofs = Offset + OFS_FORM1 + 4;
            Data[ofs + index] = (byte)form;
        }

        public override void SetDex(PKM pkm)
        {
            int species = pkm.Species;
            if (species is 0 or > Legal.MaxSpeciesID_4)
                return;
            if (pkm.IsEgg) // do not add
                return;

            var gender = pkm.Gender;
            var form = pkm.Form;
            var language = pkm.Language;
            SetDex(species, gender, form, language);
        }

        private void SetDex(int species, int gender, int form, int language)
        {
            SetCaught(species);
            SetSeenGender(species, gender);
            SetSeen(species);
            SetForms(species, form, gender);
            SetLanguage(species, language);
        }

        public void SetSeenGender(int species, int gender)
        {
            if (!GetSeen(species))
                SetSeenGenderNew(species, gender);
            else if (GetSeenSingleGender(species))
                SetSeenGenderSecond(species, gender);
        }

        public void SetSeenGenderNew(int species, int gender)
        {
            SetSeenGenderFirst(species, gender);
            SetSeenGenderSecond(species, gender);
        }

        public void SetSeenGenderNeither(int species)
        {
            SetSeenGenderFirst(species, 0);
            SetSeenGenderSecond(species, 0);
        }

        private void SetForms(int species, int form, int gender)
        {
            if (species == (int)Species.Unown) // Unown
            {
                AddUnownForm(form);
                return;
            }

            var forms = GetForms(species);
            if (forms.Length == 0)
                return;

            if (species == (int)Species.Pichu && HGSS) // Pichu (HGSS Only)
            {
                int formID = form == 1 ? 2 : gender;
                if (TryInsertForm(forms, formID))
                    SetForms(species, forms);
            }
            else
            {
                if (TryInsertForm(forms, form))
                    SetForms(species, forms);
            }
        }

        public void SetLanguage(int species, int language, bool value = true)
        {
            int lang = GetGen4LanguageBitIndex(language);
            SetLanguageBitIndex(species, lang, value);
        }

        public bool GetLanguageBitIndex(int species, int lang)
        {
            int dpl = 1 + Array.IndexOf(DPLangSpecies, species);
            if (DP && dpl < 0)
                return false;
            int FormOffset1 = Offset + OFS_FORM1;
            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);

            var ofs = PokeDexLanguageFlags + (DP ? dpl : species);
            return FlagUtil.GetFlag(Data, ofs, lang & 7);
        }

        public void SetLanguageBitIndex(int species, int lang, bool value)
        {
            int dpl = 1 + Array.IndexOf(DPLangSpecies, species);
            if (DP && dpl <= 0)
                return;
            int FormOffset1 = Offset + OFS_FORM1;
            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);

            var ofs = PokeDexLanguageFlags + (DP ? dpl : species);
            FlagUtil.SetFlag(Data, ofs, lang & 7, value);
        }

        public bool HasLanguage(int species) => GetSpeciesLanguageByteIndex(species) >= 0;

        private int GetSpeciesLanguageByteIndex(int species)
        {
            if (DP)
                return Array.IndexOf(DPLangSpecies, species);
            return species;
        }

        private static readonly int[] DPLangSpecies = { 23, 25, 54, 77, 120, 129, 202, 214, 215, 216, 228, 278, 287, 315 };

        public static int GetGen4LanguageBitIndex(int lang) => --lang switch
        {
            3 => 4, // invert ITA/GER
            4 => 3, // invert ITA/GER
            > 5 => 0, // Japanese
            < 0 => 1, // English
            _ => lang,
        };

        [Flags]
        public enum SetDexArgs
        {
            None,
            SeenAll = 1 << 0,

            CaughtNone = 1 << 1,
            CaughtAll = 1 << 2,

            SetNoLanguages = 1 << 3,
            SetAllLanguages = 1 << 4,
            SetSingleLanguage = 1 << 5,

            SetAllForms = 1 << 6,

            Complete = SeenAll | CaughtAll | SetAllLanguages | SetAllForms,
        }

        public void ModifyAll(int species, SetDexArgs args, int lang = 0)
        {
            if (args == SetDexArgs.None)
            {
                ClearSeen(species);
                return;
            }
            if ((args & SetDexArgs.SeenAll) != 0)
                CompleteSeen(species);

            if ((args & SetDexArgs.CaughtNone) != 0)
            {
                SetCaught(species, false);
                ClearLanguages(species);
            }
            else if ((args & SetDexArgs.CaughtAll) != 0)
            {
                SetCaught(species);
            }

            if ((args & SetDexArgs.SetNoLanguages) != 0)
            {
                ClearLanguages(species);
            }
            if ((args & SetDexArgs.SetAllLanguages) != 0)
            {
                SetLanguages(species);
            }
            else if ((args & SetDexArgs.SetSingleLanguage) != 0)
            {
                SetLanguage(species, lang);
            }

            if ((args & SetDexArgs.SetAllForms) != 0)
            {
                CompleteForms(species);
            }
        }

        private void CompleteForms(int species)
        {
            var forms = GetFormNames4Dex(species);
            if (forms.Length <= 1)
                return;

            var values = forms.Select((_, i) => i).ToArray();
            SetForms(species, values);
        }

        private void CompleteSeen(int species)
        {
            SetSeen(species);
            var pi = PersonalTable.HGSS[species];
            if (pi.IsDualGender)
            {
                SetSeenGenderFirst(species, 0);
                SetSeenGenderSecond(species, 1);
            }
            else
            {
                SetSeenGender(species, pi.FixedGender & 1);
            }
        }

        public void ClearSeen(int species)
        {
            SetCaught(species, false);
            SetSeen(species, false);
            SetSeenGenderNeither(species);

            SetForms(species, Array.Empty<int>());
            ClearLanguages(species);
        }

        private const int LangCount = 6;
        private void ClearLanguages(int species)
        {
            for (int i = 0; i < 8; i++)
                SetLanguageBitIndex(species, i, false);
        }

        private void SetLanguages(int species, bool value = true)
        {
            for (int i = 0; i < LangCount; i++)
                SetLanguageBitIndex(species, i, value);
        }

        // Bulk Manipulation
        public override void CompleteDex(bool shinyToo = false) => IterateAll(z => ModifyAll(z, SetDexArgs.Complete));
        public override void SeenNone() => IterateAll(ClearSeen);
        public override void CaughtNone() => IterateAll(z => SetCaught(z, false));
        public override void SeenAll(bool shinyToo = false) => IterateAll(CompleteSeen);

        public override void SetDexEntryAll(int species, bool shinyToo = false) => ModifyAll(species, SetDexArgs.Complete);
        public override void ClearDexEntryAll(int species) => ModifyAll(species, SetDexArgs.None);

        private static void IterateAll(Action<int> a)
        {
            for (int i = 1; i <= Legal.MaxSpeciesID_4; i++)
                a(i);
        }

        public override void SetAllSeen(bool value = true, bool shinyToo = false)
        {
            if (!value)
            {
                SeenNone();
                return;
            }
            IterateAll(CompleteSeen);
        }

        public override void CaughtAll(bool shinyToo = false)
        {
            SeenAll();
            IterateAll(z => SetCaught(z));
        }
    }
}
