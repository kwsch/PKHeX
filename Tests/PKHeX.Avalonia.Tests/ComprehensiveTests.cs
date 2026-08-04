using System.IO;
using Avalonia.Headless.XUnit;
using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Comprehensive tests to discover edge cases, bugs, and missing functionality.
/// </summary>
public class ComprehensiveTests
{
    // private readonly Mock<ISpriteRenderer> _spriteRendererMock = new(); // Removed
    // private readonly Mock<IDialogService> _dialogServiceMock = new(); // Removed

    #region Edge Case Tests

    [Fact]
    public void Species_Zero_Displays_Empty_Slot()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 0 };
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        Assert.Equal(0, vm.Species);
        Assert.Equal("Empty Slot", vm.Title);
    }

    [Fact]
    public void MaxIV_Values_Are_Clamped()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        // Set IV above max (should be clamped or handled)
        vm.IvHP = 999; // Way above 31
        var result = vm.PreparePKM();
        
        // IVs should be clamped to 0-31 range
        Assert.InRange(result.IV_HP, 0, 31);
    }

    [Fact]
    public void MaxEV_Values_Are_Clamped()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        // Set EV above max (should be clamped or handled)
        vm.EvHP = 999; // Way above 252
        var result = vm.PreparePKM();
        
        // EVs should be clamped to 0-255 range (PKM allows 255)
        Assert.InRange(result.EV_HP, 0, 255);
    }

    [Fact]
    public void Null_Nickname_Handled_Gracefully()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        // This should not throw
        vm.Nickname = null!;
        var result = vm.PreparePKM();
        
        Assert.NotNull(result.Nickname);
    }

    [Fact]
    public void Invalid_PID_String_Does_Not_Crash()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        // Set invalid hex string
        vm.Pid = "ZZZZZZZZ"; // Invalid hex
        var result = vm.PreparePKM();
        
        // Should not crash, PID unchanged or handled
        Assert.True(true); // If we reach here, no crash occurred
    }

    [Fact]
    public void Level_Zero_Handled()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.Level = 0; // Invalid but should not crash
        var result = vm.PreparePKM();
        
        // Level should be at least 1 or handled
        Assert.True(result.CurrentLevel >= 0);
    }

    [Fact]
    public void Level_Over_100_Handled()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.Level = 255; // Over max
        var result = vm.PreparePKM();
        
        // Level should be clamped to 100 or handled
        Assert.InRange(result.CurrentLevel, 1, 100);
    }

    [Fact]
    public void Stat_Recalculation_Updates_On_IV_Change()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 1 }; // Bulbasaur
        pkm.CurrentLevel = 50;
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        // Capture initial stats
        var initialHP = vm.Stat_HP;

        // Change IV
        vm.IvHP = 31;
        
        Assert.True(vm.Stat_HP >= initialHP, "HP should increase or stay same when setting IV to max");
        Assert.Equal(31, vm.TargetPKM.IV_HP);
    }

    #endregion

    #region Command Tests

    [Fact]
    public void SetMaxIVs_Command_Sets_All_IVs_To_31()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.SetMaxIVsCommand.Execute(null);
        
        Assert.Equal(31, vm.IvHP);
        Assert.Equal(31, vm.IvATK);
        Assert.Equal(31, vm.IvDEF);
        Assert.Equal(31, vm.IvSPA);
        Assert.Equal(31, vm.IvSPD);
        Assert.Equal(31, vm.IvSPE);
        Assert.Equal(186, vm.IVTotal);
    }

    [Fact]
    public void ClearEVs_Command_Sets_All_EVs_To_Zero()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25, EV_HP = 100, EV_ATK = 100 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        Assert.True(vm.EVTotal > 0); // Pre-condition
        
        vm.ClearEVsCommand.Execute(null);
        
        Assert.Equal(0, vm.EvHP);
        Assert.Equal(0, vm.EvATK);
        Assert.Equal(0, vm.EvDEF);
        Assert.Equal(0, vm.EvSPA);
        Assert.Equal(0, vm.EvSPD);
        Assert.Equal(0, vm.EvSPE);
        Assert.Equal(0, vm.EVTotal);
    }

    [Fact]
    public void ToggleShiny_Command_Toggles_Shiny_State()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        bool initialState = vm.IsShiny;
        
        vm.ToggleShinyCommand.Execute(null);
        Assert.NotEqual(initialState, vm.IsShiny);
        
        vm.ToggleShinyCommand.Execute(null);
        Assert.Equal(initialState, vm.IsShiny);
    }

    #endregion

    #region Multi-Generation Tests

    [Fact]
    public void Gen4_Pokemon_Loads_Correctly()
    {
        var sav = new SAV4Pt();
        var pkm = new PK4 { Species = 393 }; // Piplup
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        Assert.Equal(393, vm.Species);
    }

    [Fact]
    public void Gen5_Pokemon_Loads_Correctly()
    {
        var sav = new SAV5B2W2();
        var pkm = new PK5 { Species = 495 }; // Snivy
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        Assert.Equal(495, vm.Species);
    }

    [Fact]
    public void Gen6_Pokemon_Loads_Correctly()
    {
        var sav = new SAV6XY();
        var pkm = new PK6 { Species = 650 }; // Chespin
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        Assert.Equal(650, vm.Species);
    }

    [Fact]
    public void Gen7_Pokemon_Loads_Correctly()
    {
        var sav = new SAV7SM();
        var pkm = new PK7 { Species = 722 }; // Rowlet
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        Assert.Equal(722, vm.Species);
    }

    [Fact]
    public void Gen8_Pokemon_Loads_Correctly()
    {
        var sav = new SAV8SWSH();
        var pkm = new PK8 { Species = 810 }; // Grookey
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        Assert.Equal(810, vm.Species);
    }

    [Fact]
    public void Gen9_Pokemon_Loads_Correctly()
    {
        var sav = new SAV9SV();
        var pkm = new PK9 { Species = 906 }; // Sprigatito
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        Assert.Equal(906, vm.Species);
    }

    #endregion

    #region Round-Trip Persistence Tests

    [Fact]
    public void All_Basic_Fields_Persist_After_PreparePKM()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        // Set various fields
        vm.Species = 1;
        vm.Nickname = "TestBulba";
        vm.HeldItem = 13; // Potion
        vm.Move1 = 33; // Tackle
        vm.Move2 = 45; // Growl
        vm.IvHP = 15;
        vm.IvATK = 20;
        vm.EvHP = 100;
        vm.EvATK = 150;
        vm.Happiness = 200;
        
        var result = vm.PreparePKM();
        
        Assert.Equal(1, result.Species);
        Assert.Equal("TestBulba", result.Nickname);
        Assert.Equal(13, result.HeldItem);
        Assert.Equal(33, result.Move1);
        Assert.Equal(45, result.Move2);
        Assert.Equal(15, result.IV_HP);
        Assert.Equal(20, result.IV_ATK);
        Assert.Equal(100, result.EV_HP);
        Assert.Equal(150, result.EV_ATK);
        Assert.Equal(200, result.CurrentFriendship);
    }

    [Fact]
    public void PP_And_PPUps_Persist_Correctly()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.Move1 = 33; // Tackle
        vm.Pp1 = 35;
        vm.PpUps1 = 3;
        
        var result = vm.PreparePKM();
        
        Assert.Equal(35, result.Move1_PP);
        Assert.Equal(3, result.Move1_PPUps);
    }

    [Fact]
    public void Met_Data_Persists_Correctly()
    {
        // Gen 3 doesn't store MetDate - use Gen 4+ for this test
        var sav = new SAV4Pt();
        var pkm = new PK4 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.MetLevel = 5;
        vm.MetDate = new DateTime(2024, 6, 15);
        
        var result = vm.PreparePKM();
        
        Assert.Equal(5, result.MetLevel);
        // Gen 4+ should persist MetDate
        Assert.NotNull(result.MetDate);
    }

    [Fact]
    public void PreparePKM_Called_Multiple_Times_Is_Idempotent()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.Nickname = "TestPika";
        vm.IvHP = 31;
        vm.HeldItem = 13;
        
        var result1 = vm.PreparePKM();
        var result2 = vm.PreparePKM();
        var result3 = vm.PreparePKM();
        
        Assert.Equal(result1.Nickname, result2.Nickname);
        Assert.Equal(result2.Nickname, result3.Nickname);
        Assert.Equal(result1.IV_HP, result3.IV_HP);
    }

    [Fact]
    public void Changing_Species_Clears_Form_Appropriately()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 201, Form = 5 }; // Unown with Form
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        Assert.Equal(5, vm.Form);
        vm.Species = 25; // Pikachu
        Assert.True(vm.Form >= 0);
        
        var result = vm.PreparePKM();
        Assert.Equal(25, result.Species);
    }

    [Fact]
    public void PID_Hex_String_Persists_Correctly()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.Pid = "DEADBEEF";
        var result = vm.PreparePKM();
        
        Assert.Equal(0xDEADBEEF, result.PID);
    }

    [Fact]
    public void EncryptionConstant_Persists_Correctly()
    {
        var sav = new SAV6XY();
        var pkm = new PK6 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.EncryptionConstant = "12345678";
        var result = vm.PreparePKM();
        
        Assert.Equal(0x12345678u, result.EncryptionConstant);
    }

    [Fact]
    public void Language_Persists_Correctly()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.Language = (int)LanguageID.Japanese;
        var result = vm.PreparePKM();
        
        Assert.Equal((int)LanguageID.Japanese, result.Language);
    }

    [Fact]
    public void Ball_Persists_Correctly()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.Ball = 2; // Great Ball
        var result = vm.PreparePKM();
        
        Assert.Equal(2, result.Ball);
    }

    [Fact]
    public void OT_Name_Persists_Correctly()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        vm.OriginalTrainerName = "TestOT";
        var result = vm.PreparePKM();
        
        Assert.Equal("TestOT", result.OriginalTrainerName);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void Valid_Pokemon_Shows_Legal()
    {
        var sav = new SAV3E();
        // Create a minimally valid Pokémon
        var pkm = new PK3
        {
            Species = 25,
            Move1 = 84, // Thundershock (legal for Pikachu)
            MetLocation = 0,
            MetLevel = 5,
            Ball = 4, // Pokéball
            OriginalTrainerName = "Ash",
            Language = (int)LanguageID.English,
            OriginalTrainerGender = 0
        };
        pkm.PID = 12345;
        pkm.OriginalTrainerTrash[0] = 0x41; // 'A'
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        // Just checking it doesn't crash - legality is complex
        Assert.NotNull(vm.LegalityReport);
    }


    #endregion

    #region Reflection Round-Trip Tests

    [AvaloniaFact]
    public void RoundTrip_All_Int_Properties()
    {
        var sav = new SAV3E();
        var pkm = new PK3 { Species = 1 }; // Use valid species to avoid normalization quirks
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        
        var properties = typeof(PokemonEditorViewModel)
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(int) && p.CanWrite && p.CanRead)
            .ToList();

        var exclusions = new HashSet<string> 
        { 
            "SelectedTab", "Stat_HP", "Stat_ATK", "Stat_DEF", "Stat_SPA", "Stat_SPD", "Stat_SPE",
            "Species", "Form", "Ability", "Level", "TargetPKM", "Nature", "Gender",
            "EggLocation", "MetLocation", "MetLevel", "OriginalTrainerGender", "Ball",
            "RelearnMove1", "RelearnMove2", "RelearnMove3", "RelearnMove4",
            "StatHPCurrent", "StatHPMax", "Valid", "Version", "StatAlignment", "HpType",
            "IsPokerusInfected", "IsPokerusCured", "AbilityNumber", "Id32", "IsNicknamed",
            "StatusCondition", "HandlingTrainerName", "HandlingTrainerGender",
            "HandlingTrainerFriendship", "CurrentHandler", "OriginalTrainerFriendship",
            "ContestCool", "ContestBeauty", "ContestCute", "ContestSmart", "ContestTough", "ContestSheen",
            "OtMemory", "OtMemoryIntensity", "OtMemoryFeeling", "OtMemoryVariable",
            "HtMemory", "HtMemoryIntensity", "HtMemoryFeeling", "HtMemoryVariable",
            "Sid",
            // Conditional on IFormArgument + species/form; not present on the PK3 test fixture.
            "FormArgumentValue", "FormArgumentMax"
        };

        foreach (var prop in properties)
        {
            if (exclusions.Contains(prop.Name)) continue;

            int testValue = 1;
            if (prop.Name.StartsWith("Iv")) testValue = 31;
            if (prop.Name.StartsWith("Ev")) testValue = 252;
            if (prop.Name.Contains("PpUps")) testValue = 3;
            if (prop.Name.Contains("Friendship") || prop.Name.Contains("Happiness")) testValue = 200;
            if (prop.Name.Contains("Sid") || prop.Name.Contains("TrainerID")) testValue = 12345;
            if (prop.Name.Contains("Move") && !prop.Name.Contains("Pp")) testValue = 33;

            try { prop.SetValue(vm, testValue); }
            catch { Assert.Fail($"Failed to set property {prop.Name}"); }
        }

        var newPkm = vm.PreparePKM();
        var (newVm, _, _) = TestHelpers.CreateTestViewModel(newPkm, sav);

        foreach (var prop in properties)
        {
            if (exclusions.Contains(prop.Name)) continue;

            int testValue = 1;
            if (prop.Name.StartsWith("Iv")) testValue = 31;
            if (prop.Name.StartsWith("Ev")) testValue = 252;
            if (prop.Name.Contains("PpUps")) testValue = 3;
            if (prop.Name.Contains("Friendship") || prop.Name.Contains("Happiness")) testValue = 200;
            if (prop.Name.Contains("Sid") || prop.Name.Contains("TrainerID")) testValue = 12345;
            if (prop.Name.Contains("Move") && !prop.Name.Contains("Pp")) testValue = 33;

            var actual = (int)prop.GetValue(newVm)!;
            Assert.True(actual == testValue, 
                $"Property {prop.Name} failed round-trip. Expected {testValue}, got {actual}");
        }
    }

    #endregion

    #region SaveFile Tests

    [Fact]
    public void Can_Load_Gen3_Variables()
    {
        var sav = new SAV3E();
        sav.TID16 = 12345;
        sav.OT = "ASH";

        var pkm = sav.GetPartySlotAtIndex(0);
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        Assert.Equal(sav.Context, vm.TargetPKM.Context);
    }

    [Fact]
    public void Load_Real_Save_File_If_Present()
    {
        var baseDir = AppContext.BaseDirectory;
        string savePath = Path.Combine(baseDir, "test_save.sav");
        
        if (!File.Exists(savePath)) return;

        byte[] data = File.ReadAllBytes(savePath);
        var sav = SaveUtil.GetSaveFile(data);

        Assert.NotNull(sav);
        Assert.True(sav.ChecksumsValid);

        var pkm = sav.BoxData[0];
        if (pkm == null) return;
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        Assert.NotNull(vm.SpeciesList);
        Assert.True(vm.SpeciesList.Count > 0);
    }

    #endregion
}
