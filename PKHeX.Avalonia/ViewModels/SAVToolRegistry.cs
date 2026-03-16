using System;
using System.Collections.Generic;
using Avalonia.Controls;
using PKHeX.Core;
using PKHeX.Avalonia.ViewModels.Subforms;
using PKHeX.Avalonia.Views.Subforms;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// Registry of all SAV tool descriptors for the dynamic tool panel.
/// </summary>
public static class SAVToolRegistry
{
    /// <summary>
    /// Returns all tool descriptors for the SAV editor toolbar.
    /// </summary>
    public static List<SAVToolDescriptor> GetAllTools()
    {
        return
        [
            // --- Trainer Info ---
            new("Trainer Info",
                sav => sav.HasParty || sav is SAV7b,
                CreateTrainerEditor),

            // --- Item Pouch ---
            new("Items",
                sav => (sav.HasParty && sav is not SAV4BR) || sav is SAV7b,
                CreateInventoryEditor),

            // --- Pokedex ---
            new("Pokédex",
                sav => sav.HasPokeDex,
                CreatePokedexEditor),

            // --- Wondercards ---
            new("Wondercards",
                sav => sav is IMysteryGiftStorageProvider,
                sav => WithView<WondercardViewModel, WondercardView>(new WondercardViewModel(sav))),

            // --- Box Layout ---
            new("Box Layout",
                sav => sav is IBoxDetailName,
                sav => WithView<BoxLayoutViewModel, BoxLayoutView>(new BoxLayoutViewModel(sav))),

            // --- Event Flags ---
            new("Event Flags",
                sav => sav is IEventFlag37 or IEventFlagProvider37 or SAV1 or SAV2 or SAV8BS or SAV7b or SAV9ZA,
                CreateEventFlagsEditor),

            // --- Hall of Fame ---
            new("Hall of Fame",
                sav => sav is ISaveBlock6Main or SAV7 or SAV3 { IsMisconfiguredSize: false } or SAV1,
                CreateHallOfFameEditor),

            // --- Misc Editor ---
            new("Misc Editor",
                sav => sav is SAV2 { Version: GameVersion.C } or SAV3 or SAV4 or SAV5 or SAV8BS,
                CreateMiscEditor),

            // --- Secret Base (Gen 3) ---
            new("Secret Base (Gen 3)",
                sav => sav is SAV3 { LargeBlock: ISaveBlock3LargeHoenn },
                sav => WithView<SAVSecretBase3ViewModel, SAVSecretBase3View>(
                    new SAVSecretBase3ViewModel((SAV3)sav))),

            // --- Secret Base (Gen 6 ORAS) ---
            new("Secret Base (ORAS)",
                sav => sav is SAV6AO,
                sav => WithView<SAVSecretBase6ViewModel, SAVSecretBase6View>(
                    new SAVSecretBase6ViewModel((SAV6AO)sav))),

            // --- Roamer (Gen 3) ---
            new("Roamer (Gen 3)",
                sav => sav is SAV3,
                sav => WithView<SAVRoamer3ViewModel, SAVRoamer3View>(
                    new SAVRoamer3ViewModel((SAV3)sav))),

            // --- Roamer (Gen 6 XY) ---
            new("Roamer (XY)",
                sav => sav is SAV6XY,
                sav => WithView<SAVRoamer6ViewModel, SAVRoamer6View>(
                    new SAVRoamer6ViewModel((SAV6XY)sav))),

            // --- Honey Tree ---
            new("Honey Trees",
                sav => sav is SAV4Sinnoh,
                sav => WithView<HoneyTree4ViewModel, HoneyTree4View>(
                    new HoneyTree4ViewModel((SAV4Sinnoh)sav))),

            // --- Underground (Gen 4 Sinnoh) ---
            new("Underground (Gen 4)",
                sav => sav is SAV4Sinnoh,
                sav => WithView<Underground4ViewModel, Underground4View>(
                    new Underground4ViewModel((SAV4Sinnoh)sav))),

            // --- Underground (BDSP) ---
            new("Underground (BDSP)",
                sav => sav is SAV8BS,
                sav => WithView<Underground8bViewModel, Underground8bView>(
                    new Underground8bViewModel((SAV8BS)sav))),

            // --- Geonet ---
            new("Geonet",
                sav => sav is SAV4,
                sav => WithView<Geonet4ViewModel, Geonet4View>(
                    new Geonet4ViewModel((SAV4)sav))),

            // --- Unity Tower ---
            new("Unity Tower",
                sav => sav is SAV5,
                sav => WithView<UnityTower5ViewModel, UnityTower5View>(
                    new UnityTower5ViewModel((SAV5)sav))),

            // --- Chatter (Chatot) ---
            new("Chatter",
                sav => sav is SAV4 or SAV5,
                sav =>
                {
                    var chatter = (IChatter)(sav is SAV4 s4 ? s4.Chatter : ((SAV5)sav).Chatter);
                    return WithView<ChatterViewModel, ChatterView>(new ChatterViewModel(sav, chatter));
                }),

            // --- Mail Box ---
            new("Mail Box",
                sav => sav is SAV2 or SAV2Stadium or SAV3 or SAV4 or SAV5,
                CreateMailBoxEditor),

            // --- RTC ---
            new("RTC Editor",
                sav => sav is SAV3 { SmallBlock: ISaveBlock3SmallHoenn },
                sav => WithView<SAVRTC3ViewModel, SAVRTC3View>(new SAVRTC3ViewModel((SAV3)sav))),

            // --- DLC (Gen 5) ---
            new("DLC Content",
                sav => sav.Generation == 5,
                sav => WithView<DLC5ViewModel, DLC5View>(new DLC5ViewModel((SAV5)sav))),

            // --- C-Gear Image ---
            new("C-Gear Skin",
                sav => sav is SAV5,
                sav => WithView<CGearImage5ViewModel, CGearImage5View>(
                    new CGearImage5ViewModel((SAV5)sav))),

            // --- PokeBlocks (Gen 3) ---
            new("PokéBlocks (Gen 3)",
                sav => sav is SAV3 { LargeBlock: ISaveBlock3LargeHoenn },
                sav =>
                {
                    var s3 = (SAV3)sav;
                    return WithView<PokeBlock3CaseEditorViewModel, PokeBlock3CaseEditorView>(
                        new PokeBlock3CaseEditorViewModel(s3, (ISaveBlock3LargeHoenn)s3.LargeBlock));
                }),

            // --- PokeBlocks (ORAS) ---
            new("PokéBlocks (ORAS)",
                sav => sav is SAV6AO,
                sav => WithView<SAVPokeBlockORASViewModel, SAVPokeBlockORASView>(
                    new SAVPokeBlockORASViewModel((SAV6AO)sav))),

            // --- Pokepuffs ---
            new("Poképuffs",
                sav => sav is ISaveBlock6Main,
                sav => WithView<SAVPokepuff6ViewModel, SAVPokepuff6View>(
                    new SAVPokepuff6ViewModel((ISaveBlock6Main)sav))),

            // --- O-Powers ---
            new("O-Powers",
                sav => sav is ISaveBlock6Main,
                sav => WithView<SAVOPower6ViewModel, SAVOPower6View>(
                    new SAVOPower6ViewModel((ISaveBlock6Main)sav))),

            // --- Link Info ---
            new("Link Info",
                sav => sav is ISaveBlock6Main,
                sav => WithView<SAVLink6ViewModel, SAVLink6View>(new SAVLink6ViewModel(sav))),

            // --- Super Training ---
            new("Super Training",
                sav => sav is ISaveBlock6Main,
                sav => WithView<SAVSuperTrain6ViewModel, SAVSuperTrain6View>(
                    new SAVSuperTrain6ViewModel((SAV6)sav))),

            // --- Berry Field (XY) ---
            new("Berry Field",
                sav => sav is SAV6XY,
                sav => WithView<SAVBerryFieldXYViewModel, SAVBerryFieldXYView>(
                    new SAVBerryFieldXYViewModel((SAV6XY)sav))),

            // --- Apricorns (HGSS) ---
            new("Apricorns",
                sav => sav is SAV4HGSS,
                sav => WithView<Apricorn4ViewModel, Apricorn4View>(
                    new Apricorn4ViewModel((SAV4HGSS)sav))),

            // --- Poké Beans ---
            new("Poké Beans",
                sav => sav is SAV7,
                sav => WithView<SAVPokebean7ViewModel, SAVPokebean7View>(
                    new SAVPokebean7ViewModel((SAV7)sav))),

            // --- Zygarde Cells/Stickers ---
            new("Cells & Stickers",
                sav => sav is SAV7,
                sav => WithView<SAVZygardeCell7ViewModel, SAVZygardeCell7View>(
                    new SAVZygardeCell7ViewModel((SAV7)sav))),

            // --- Festival Plaza ---
            new("Festival Plaza",
                sav => sav is SAV7,
                sav => WithView<SAVFestivalPlaza7ViewModel, SAVFestivalPlaza7View>(
                    new SAVFestivalPlaza7ViewModel((SAV7)sav))),

            // --- Capture Records (Let's Go) ---
            new("Capture Records",
                sav => sav is SAV7b,
                sav => WithView<SAVCapture7GGViewModel, SAVCapture7GGView>(
                    new SAVCapture7GGViewModel((SAV7b)sav))),

            // --- Battle Pass (Battle Revolution) ---
            new("Battle Pass",
                sav => sav is SAV4BR,
                sav => WithView<BattlePass4ViewModel, BattlePass4View>(
                    new BattlePass4ViewModel((SAV4BR)sav))),

            // --- Gear (Battle Revolution) ---
            new("Gear",
                sav => sav is SAV4BR,
                sav => WithView<Gear4ViewModel, Gear4View>(
                    new Gear4ViewModel((SAV4BR)sav))),

            // --- PokéGear (HGSS) ---
            new("PokéGear",
                sav => sav is SAV4HGSS,
                sav => WithView<PokeGear4ViewModel, PokeGear4View>(
                    new PokeGear4ViewModel((SAV4HGSS)sav))),

            // --- Poffin Case (Gen 4 Sinnoh) ---
            new("Poffin Case",
                sav => sav is SAV4Sinnoh,
                sav => WithView<PoffinCase4ViewModel, PoffinCase4View>(
                    new PoffinCase4ViewModel((SAV4Sinnoh)sav))),

            // --- Poffins (BDSP) ---
            new("Poffins (BDSP)",
                sav => sav is SAV8BS,
                sav => WithView<Poffin8bViewModel, Poffin8bView>(
                    new Poffin8bViewModel((SAV8BS)sav))),

            // --- Seal Stickers (BDSP) ---
            new("Seal Stickers",
                sav => sav is SAV8BS,
                sav => WithView<SealStickers8bViewModel, SealStickers8bView>(
                    new SealStickers8bViewModel((SAV8BS)sav))),

            // --- Raids (SwSh Galar) ---
            new("Raids (Galar)",
                sav => sav is SAV8SWSH,
                sav => WithView<Raid8ViewModel, Raid8View>(
                    new Raid8ViewModel((SAV8SWSH)sav, MaxRaidOrigin.Galar))),

            // --- Raids (SwSh Isle of Armor) ---
            new("Raids (IoA)",
                sav => sav is SAV8SWSH { SaveRevision: >= 1 },
                sav => WithView<Raid8ViewModel, Raid8View>(
                    new Raid8ViewModel((SAV8SWSH)sav, MaxRaidOrigin.IsleOfArmor))),

            // --- Raids (SwSh Crown Tundra) ---
            new("Raids (CT)",
                sav => sav is SAV8SWSH { SaveRevision: >= 2 },
                sav => WithView<Raid8ViewModel, Raid8View>(
                    new Raid8ViewModel((SAV8SWSH)sav, MaxRaidOrigin.CrownTundra))),

            // --- Tera Raids (SV Paldea) ---
            new("Tera Raids (Paldea)",
                sav => sav is SAV9SV,
                sav => WithView<Raid9ViewModel, Raid9View>(
                    new Raid9ViewModel((SAV9SV)sav, TeraRaidOrigin.Paldea))),

            // --- Tera Raids 7-Star ---
            new("Tera Raids (7★)",
                sav => sav is SAV9SV,
                sav => WithView<RaidSevenStar9ViewModel, RaidSevenStar9View>(
                    new RaidSevenStar9ViewModel((SAV9SV)sav))),

            // --- Tera Raids (SV Kitakami) ---
            new("Tera Raids (Kitakami)",
                sav => sav is SAV9SV { SaveRevision: >= 1 },
                sav => WithView<Raid9ViewModel, Raid9View>(
                    new Raid9ViewModel((SAV9SV)sav, TeraRaidOrigin.Kitakami))),

            // --- Tera Raids (SV Blueberry) ---
            new("Tera Raids (Blueberry)",
                sav => sav is SAV9SV { SaveRevision: >= 2 },
                sav => WithView<Raid9ViewModel, Raid9View>(
                    new Raid9ViewModel((SAV9SV)sav, TeraRaidOrigin.BlueberryAcademy))),

            // --- Block Data ---
            new("Block Data",
                sav => sav is ISCBlockArray,
                CreateBlockDataEditor),

            // --- Fashion (SV) ---
            new("Fashion (SV)",
                sav => sav is SAV9SV,
                sav => WithView<Fashion9ViewModel, Fashion9View>(
                    new Fashion9ViewModel((SAV9SV)sav))),

            // --- Fashion (ZA) ---
            new("Fashion (ZA)",
                sav => sav is SAV9ZA,
                sav => WithView<Fashion9ViewModel, Fashion9View>(
                    new Fashion9ViewModel((SAV9ZA)sav))),

            // --- Donuts (ZA) ---
            new("Donuts",
                sav => sav is SAV9ZA { SaveRevision: >= 1 },
                sav => WithView<Donut9aViewModel, Donut9aView>(
                    new Donut9aViewModel((SAV9ZA)sav))),

            // --- Event Reset (Gen 1) ---
            new("Event Reset",
                sav => sav is SAV1,
                sav => WithView<SAVEventReset1ViewModel, SAVEventReset1View>(
                    new SAVEventReset1ViewModel((SAV1)sav))),

            // --- Friend Safari (Gen 6 XY) ---
            new("Friend Safari",
                sav => sav is SAV6XY,
                sav =>
                {
                    ((SAV6XY)sav).UnlockAllFriendSafariSlots();
                    return WithView<SimpleTrainerViewModel, SimpleTrainerView>(
                        new SimpleTrainerViewModel(sav));
                }),
        ];
    }

    // -------------------------------------------------------------------------
    // Dispatcher methods for gen-specific tools
    // -------------------------------------------------------------------------

    private static (object, Window) CreateInventoryEditor(SaveFile sav)
    {
        try
        {
            return WithView<InventoryViewModel, InventoryView>(new InventoryViewModel(sav));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load inventory for {sav.GetType().Name} (Gen {sav.Generation}): {ex.Message}", ex);
        }
    }

    private static (object, Window) CreateTrainerEditor(SaveFile sav)
    {
        try
        {
            return sav switch
            {
                SAV9ZA s   => WithView<Trainer9aViewModel, Trainer9aView>(new Trainer9aViewModel(s)),
                SAV9SV s   => WithView<Trainer9ViewModel, Trainer9View>(new Trainer9ViewModel(s)),
                SAV8LA s   => WithView<Trainer8aViewModel, Trainer8aView>(new Trainer8aViewModel(s)),
                SAV8BS s   => WithView<Trainer8bViewModel, Trainer8bView>(new Trainer8bViewModel(s)),
                SAV8SWSH s => WithView<Trainer8ViewModel, Trainer8View>(new Trainer8ViewModel(s)),
                SAV7b s    => WithView<SAVTrainer7GGViewModel, SAVTrainer7GGView>(new SAVTrainer7GGViewModel(s)),
                SAV7 s     => WithView<SAVTrainer7ViewModel, SAVTrainer7View>(new SAVTrainer7ViewModel(s)),
                SAV6 s     => WithView<SAVTrainer6ViewModel, SAVTrainer6View>(new SAVTrainer6ViewModel(s)),
                SAV4BR s   => WithView<Trainer4BRViewModel, Trainer4BRView>(new Trainer4BRViewModel(s)),
                _          => WithView<SimpleTrainerViewModel, SimpleTrainerView>(new SimpleTrainerViewModel(sav)),
            };
        }
        catch
        {
            return WithView<SimpleTrainerViewModel, SimpleTrainerView>(new SimpleTrainerViewModel(sav));
        }
    }

    private static (object, Window) CreatePokedexEditor(SaveFile sav)
    {
        try
        {
            return sav switch
            {
                SAV9ZA s   => WithView<Pokedex9aViewModel, Pokedex9aView>(new Pokedex9aViewModel(s)),
                SAV9SV s   => WithView<PokedexSVViewModel, PokedexSVView>(new PokedexSVViewModel(s)),
                SAV8LA s   => WithView<PokedexLAViewModel, PokedexLAView>(new PokedexLAViewModel(s)),
                SAV8BS s   => WithView<PokedexBDSPViewModel, PokedexBDSPView>(new PokedexBDSPViewModel(s)),
                SAV8SWSH s => WithView<PokedexSWSHViewModel, PokedexSWSHView>(new PokedexSWSHViewModel(s)),
                SAV7b s    => WithView<SAVPokedexGGViewModel, SAVPokedexGGView>(new SAVPokedexGGViewModel(s)),
                SAV7 s     => WithView<SAVPokedexSMViewModel, SAVPokedexSMView>(new SAVPokedexSMViewModel(s)),
                SAV6AO s   => WithView<SAVPokedexORASViewModel, SAVPokedexORASView>(new SAVPokedexORASViewModel(s)),
                SAV6XY s   => WithView<SAVPokedexXYViewModel, SAVPokedexXYView>(new SAVPokedexXYViewModel(s)),
                SAV5 s     => WithView<Pokedex5ViewModel, Pokedex5View>(new Pokedex5ViewModel(s)),
                SAV4 s     => WithView<Pokedex4ViewModel, Pokedex4View>(new Pokedex4ViewModel(s)),
                _          => WithView<SimplePokedexViewModel, SimplePokedexView>(new SimplePokedexViewModel(sav)),
            };
        }
        catch
        {
            return WithView<SimplePokedexViewModel, SimplePokedexView>(new SimplePokedexViewModel(sav));
        }
    }

    private static (object, Window) CreateEventFlagsEditor(SaveFile sav)
    {
        try
        {
            return sav switch
            {
                SAV9ZA s => WithView<FlagWork9aViewModel, FlagWork9aView>(new FlagWork9aViewModel(s)),
                SAV8BS s => WithView<FlagWork8bViewModel, FlagWork8bView>(new FlagWork8bViewModel(s)),
                SAV7b s  => WithView<EventWorkViewModel, EventWorkView>(new EventWorkViewModel(s, s.EventWork)),
                SAV1 s   => WithView<SAVEventReset1ViewModel, SAVEventReset1View>(new SAVEventReset1ViewModel(s)),
                SAV2 s   => CreateEventFlags2Editor(s),
                IEventFlagProvider37 provider => CreateEventFlagFromProvider(sav, provider),
                IEventFlag37 ef => WithView<EventFlagsViewModel, EventFlagsView>(new EventFlagsViewModel(sav, ef, sav.Version)),
                _ => throw new ArgumentException("Unsupported save type for event flags."),
            };
        }
        catch
        {
            // Fallback: try generic event flags if available
            if (sav is IEventFlag37 ef2)
                return WithView<EventFlagsViewModel, EventFlagsView>(new EventFlagsViewModel(sav, ef2, sav.Version));
            throw;
        }
    }

    private static (object, Window) CreateEventFlags2Editor(SAV2 sav)
    {
        return WithView<EventFlags2ViewModel, EventFlags2View>(
            new EventFlags2ViewModel(sav, sav, sav, sav.Version));
    }

    private static (object, Window) CreateEventFlagFromProvider(SaveFile sav, IEventFlagProvider37 provider)
    {
        var ef = provider.EventWork;
        return WithView<EventFlagsViewModel, EventFlagsView>(new EventFlagsViewModel(sav, ef, sav.Version));
    }

    private static (object, Window) CreateHallOfFameEditor(SaveFile sav)
    {
        try
        {
            return sav switch
            {
                SAV7 s => WithView<SAVHallOfFame7ViewModel, SAVHallOfFame7View>(new SAVHallOfFame7ViewModel(s)),
                SAV6 s => WithView<SAVHallOfFame6ViewModel, SAVHallOfFame6View>(new SAVHallOfFame6ViewModel(s)),
                SAV3 s => WithView<SAVHallOfFame3ViewModel, SAVHallOfFame3View>(new SAVHallOfFame3ViewModel(s)),
                SAV1 s => WithView<SAVHallOfFame1ViewModel, SAVHallOfFame1View>(new SAVHallOfFame1ViewModel(s)),
                _ => throw new ArgumentException("Unsupported save type for Hall of Fame."),
            };
        }
        catch
        {
            throw;
        }
    }

    private static (object, Window) CreateMiscEditor(SaveFile sav)
    {
        try
        {
            return sav switch
            {
                SAV8BS s => WithView<Misc8bViewModel, Misc8bView>(new Misc8bViewModel(s)),
                SAV5 s   => WithView<Misc5ViewModel, Misc5View>(new Misc5ViewModel(s)),
                SAV4 s   => WithView<Misc4ViewModel, Misc4View>(new Misc4ViewModel(s)),
                SAV3 s   => WithView<SAVMisc3ViewModel, SAVMisc3View>(new SAVMisc3ViewModel(s)),
                SAV2 s   => WithView<SAVMisc2ViewModel, SAVMisc2View>(new SAVMisc2ViewModel(s)),
                _ => throw new ArgumentException("Unsupported save type for Misc editor."),
            };
        }
        catch
        {
            throw;
        }
    }

    private static (object, Window) CreateMailBoxEditor(SaveFile sav)
    {
        MailDetail[] mails;
        int partyCount;
        switch (sav)
        {
            case SAV2 s2:
                mails = new MailDetail[6 + 10];
                for (int i = 0; i < mails.Length; i++)
                    mails[i] = new Mail2(s2, i);
                partyCount = 6;
                break;
            case SAV2Stadium s2s:
                mails = new MailDetail[SAV2Stadium.MailboxHeldMailCount + SAV2Stadium.MailboxMailCount];
                for (int i = 0; i < mails.Length; i++)
                    mails[i] = new Mail2(s2s, i);
                partyCount = SAV2Stadium.MailboxHeldMailCount;
                break;
            case SAV3 s3:
                mails = new MailDetail[6 + 10];
                for (int i = 0; i < mails.Length; i++)
                    mails[i] = s3.LargeBlock.GetMail(i);
                partyCount = 6;
                break;
            case SAV4 s4:
                var p4 = s4.PartyData;
                mails = new MailDetail[p4.Count + 20];
                for (int i = 0; i < p4.Count; i++)
                    mails[i] = new Mail4(((PK4)p4[i]).HeldMail.ToArray());
                for (int i = p4.Count, j = 0; i < mails.Length; i++, j++)
                    mails[i] = s4.GetMail(j);
                partyCount = p4.Count;
                break;
            case SAV5 s5:
                var p5 = s5.PartyData;
                mails = new MailDetail[p5.Count + 20];
                for (int i = 0; i < p5.Count; i++)
                    mails[i] = new Mail5(((PK5)p5[i]).HeldMail.ToArray());
                for (int i = p5.Count, j = 0; i < mails.Length; i++, j++)
                    mails[i] = s5.GetMail(j);
                partyCount = p5.Count;
                break;
            default:
                throw new ArgumentException("Unsupported save type for Mail Box.");
        }
        return WithView<MailBoxViewModel, MailBoxView>(new MailBoxViewModel(sav, mails, partyCount));
    }

    private static (object, Window) CreateBlockDataEditor(SaveFile sav)
    {
        try
        {
            var blockArray = (ISCBlockArray)sav;
            return WithView<BlockDump8ViewModel, BlockDump8View>(new BlockDump8ViewModel(blockArray));
        }
        catch
        {
            throw;
        }
    }

    // -------------------------------------------------------------------------
    // Helper
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a View of type <typeparamref name="TView"/> and sets its DataContext to the provided ViewModel.
    /// Returns the (ViewModel, View) tuple.
    /// </summary>
    private static (object ViewModel, Window View) WithView<TVM, TView>(TVM vm)
        where TVM : class
        where TView : Window, new()
    {
        var view = new TView { DataContext = vm };
        return (vm, view);
    }
}
