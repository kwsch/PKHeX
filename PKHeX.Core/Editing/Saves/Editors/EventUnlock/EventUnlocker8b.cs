namespace PKHeX.Core;

public sealed class EventUnlocker8b(SAV8BS sav) : EventUnlocker<SAV8BS>(sav)
{
    public bool UnlockReadySpiritomb => SAV.UgSaveData.TalkedNPC < 32;
    public bool UnlockReadyBoxLegend => SAV.FlagWork.GetFlag(308) && SAV.FlagWork.GetWork(84) != 5; // FE_D05R0114_SPPOKE_GET, WK_SCENE_D05R0114 (1-3 story related, 4 = captured, 5 = can retry)
    public bool UnlockReadyShaymin => SAV.FlagWork.GetFlag(545) || !(SAV.FlagWork.GetWork(276) == 1 && SAV.Zukan.HasNationalDex && SAV.Items.GetItemQuantity(452) == 1 && SAV.FlagWork.GetSystemFlag(5)); // HAIHUEVENT_ID_D30, Oak's Letter
    public bool UnlockReadyDarkrai => SAV.FlagWork.GetFlag(301) || !(SAV.FlagWork.GetWork(275) == 1 && SAV.Zukan.HasNationalDex && SAV.Items.GetItemQuantity(454) == 1); // HAIHUEVENT_ID_D18, Member Card
    public bool UnlockReadyArceus => SAV.FlagWork.GetFlag(531) || !(SAV.FlagWork.GetWork(188) == 0 && SAV.Zukan.HasNationalDex && SAV.Items.GetItemQuantity(455) == 1 && SAV.FlagWork.GetSystemFlag(5)); // FE_D05R0116_LEGEND_CLEAR, Azure Flute

    // 0 = inactive, 1 = roaming, 2 = KOed, 3 = captured
    public bool UnlockReadyRoamerMesprit => SAV.Encounter.Roamer1Encount != 1;
    public bool UnlockReadyRoamerCresselia => SAV.Encounter.Roamer2Encount != 1;
    public bool ResetReadyRoamerMesprit => SAV.Encounter.Roamer1Encount != 0;
    public bool ResetReadyRoamerCresselia => SAV.Encounter.Roamer2Encount != 0;

    public void UnlockBoxLegend()
    {
        SAV.FlagWork.SetFlag(308, false); // captured
        SAV.FlagWork.SetFlag(393, false); // clear vanish
        SAV.FlagWork.SetFlag(1623, false); // can retry
        SAV.FlagWork.SetWork(84, 5); // can retry
    }

    public void UnlockSpiritomb()
    {
        var trainers = SAV.UgSaveData.GetTrainers();
        for (int i = 0; i < 32; i++)
            trainers[i] = (byte)(i + 1);
    }

    public void UnlockShaymin()
    {
        SAV.Zukan.HasNationalDex = true; // dex
        SAV.FlagWork.SetSystemFlag(5, true); // clear
        SAV.FlagWork.SetWork(276, 1); // haihu
        SAV.Items.SetItemQuantity(452, 1); // letter
        SAV.FlagWork.SetFlag(545, false); // clear vanish
    }

    public void UnlockDarkrai()
    {
        SAV.Zukan.HasNationalDex = true; // dex
        SAV.FlagWork.SetWork(275, 1); // haihu
        SAV.Items.SetItemQuantity(454, 1); // member
        SAV.FlagWork.SetFlag(301, false); // clear vanish
    }

    public void UnlockArceus()
    {
        SAV.Zukan.HasNationalDex = true; // dex
        SAV.Items.SetItemQuantity(455, 1); // flute
        SAV.FlagWork.SetSystemFlag(5, true); // clear
        SAV.FlagWork.SetFlag(1508, true); // wildcard
        SAV.FlagWork.SetFlag(244, false); // captured
        SAV.FlagWork.SetFlag(531, false); // clear vanish
        SAV.FlagWork.SetWork(188, 0); // staircase
    }

    public void UnlockZones()
    {
        const int ZONE_START = 134; // FLAG_ARRIVE_C01
        const int ZONE_END = 757; // FLAG_ARRIVE_UGSECRETBASE04

        for (int i = ZONE_START; i <= ZONE_END; i++)
        {
            SAV.FlagWork.SetSystemFlag(i, true);
        }

        // uncover hidden zones
        SAV.FlagWork.SetWork(278, 1); // Fullmoon Island
        SAV.FlagWork.SetWork(279, 1); // Newmoon Island
        SAV.FlagWork.SetWork(280, 1); // Spring Path / Sendoff Spring
        SAV.FlagWork.SetWork(281, 1); // Seabreak Path / Flower Paradise
        SAV.FlagWork.SetWork(291, 1); // Pokémon League (Victory Road entrance)
        SAV.FlagWork.SetWork(292, 1); // Ramanas Park
    }

    public void RespawnRoamer()
    {
        RespawnMesprit();
        RespawnCresselia();
    }

    public void RespawnMesprit()
    {
        SAV.FlagWork.SetFlag(249, false); // clear met
        SAV.FlagWork.SetFlag(420, false); // clear vanish
        SAV.Encounter.Roamer1Encount = 0; // not actively roaming
    }

    public void RespawnCresselia()
    {
        SAV.FlagWork.SetFlag(245, false); // clear met
        SAV.FlagWork.SetFlag(532, false); // clear vanish
        SAV.Encounter.Roamer2Encount = 0; // not actively roaming
    }

    public void UnlockFashion()
    {
        const int FASHION_START = 1246;
        const int FASHION_END = 1257;
        for (int i = FASHION_START; i <= FASHION_END; i++)
            SAV.FlagWork.SetFlag(i, true);
    }
}
