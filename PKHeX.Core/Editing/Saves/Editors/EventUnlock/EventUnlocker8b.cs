namespace PKHeX.Core;

public class EventUnlocker8b : EventUnlocker<SAV8BS>
{
    public EventUnlocker8b(SAV8BS sav) : base(sav) { }

    public bool UnlockReadySpiritomb => SAV.UgSaveData.TalkedNPC < 32;
    public bool UnlockReadyBoxLegend => SAV.Work.GetFlag(308) && SAV.Work.GetWork(84) != 5; // FE_D05R0114_SPPOKE_GET, WK_SCENE_D05R0114 (1-3 story related, 4 = captured, 5 = can retry)
    public bool UnlockReadyShaymin => SAV.Work.GetFlag(545) || !(SAV.Work.GetWork(276) == 1 && SAV.Zukan.HasNationalDex && SAV.Items.GetItemQuantity(452) == 1 && SAV.Work.GetSystemFlag(5)); // HAIHUEVENT_ID_D30, Oak's Letter
    public bool UnlockReadyDarkrai => SAV.Work.GetFlag(301) || !(SAV.Work.GetWork(275) == 1 && SAV.Zukan.HasNationalDex && SAV.Items.GetItemQuantity(454) == 1); // HAIHUEVENT_ID_D18, Member Card

    // 0 = inactive, 1 = roaming, 2 = KOed, 3 = captured
    public bool UnlockReadyRoamerMesprit => SAV.Encounter.Roamer1Encount != 1;
    public bool UnlockReadyRoamerCresselia => SAV.Encounter.Roamer2Encount != 1;
    public bool ResetReadyRoamerMesprit => SAV.Encounter.Roamer1Encount != 0;
    public bool ResetReadyRoamerCresselia => SAV.Encounter.Roamer2Encount != 0;

    public void UnlockBoxLegend()
    {
        SAV.Work.SetFlag(308, false); // captured
        SAV.Work.SetFlag(393, false); // clear vanish
        SAV.Work.SetFlag(1623, false); // can retry
        SAV.Work.SetWork(84, 5); // can retry
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
        SAV.Work.SetSystemFlag(5, true); // clear
        SAV.Work.SetWork(276, 1); // haihu
        SAV.Items.SetItemQuantity(452, 1); // letter
        SAV.Work.SetFlag(545, false); // clear vanish
    }

    public void UnlockDarkrai()
    {
        SAV.Zukan.HasNationalDex = true; // dex
        SAV.Work.SetWork(275, 1); // haihu
        SAV.Items.SetItemQuantity(454, 1); // member
        SAV.Work.SetFlag(301, false); // clear vanish
    }

    public void RespawnRoamer()
    {
        RespawnMesprit();
        RespawnCresselia();
    }

    public void RespawnMesprit()
    {
        SAV.Work.SetFlag(249, false); // clear met
        SAV.Work.SetFlag(420, false); // clear vanish
        SAV.Encounter.Roamer1Encount = 0; // not actively roaming
    }

    public void RespawnCresselia()
    {
        SAV.Work.SetFlag(245, false); // clear met
        SAV.Work.SetFlag(532, false); // clear vanish
        SAV.Encounter.Roamer2Encount = 0; // not actively roaming
    }
}
