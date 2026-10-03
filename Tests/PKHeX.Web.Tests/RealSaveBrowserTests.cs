using System.Globalization;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Published app round trips on the private real saves, compared byte-for-byte with the same edit made through native Core.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.RealSave)]
public sealed class RealSaveBrowserTests(PublishedAppFixture app)
{
    public static IEnumerable<object[]> RealCases()
    {
        foreach (var engine in PublishedAppFixture.Engines)
        {
            foreach (var prefix in PublishedAppFixture.Prefixes)
            {
                foreach (var family in RealSaves.Families)
                {
                    yield return [engine, prefix, family];
                }
            }
        }
    }

    [TierTheory(TestCategory.RealSave)]
    [MemberData(nameof(RealCases))]
    public async Task RealSavePublishedRoundTrip(string engine, string prefix, string family)
    {
        // Private fixture: validated natively before any browser run.
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;

        var index = SaveFixtures.WritableSlot(native);
        var nativeSlot = SaveFixtures.Slot(native, index);
        var sourcePk = nativeSlot.Read(native);
        var expectedNoOp = native.Clone().Write().ToArray();

        // Native reference edit: the same nickname change, made directly through Core.
        var changed = native.Clone();
        var editedPk = SaveFixtures.Slot(changed, index).Read(changed);
        var nickname = editedPk.Nickname == "WASM Proof" ? "WASM Test" : "WASM Proof";
        editedPk.Nickname = nickname;
        editedPk.IsNicknamed = true;
        var beforeSet = changed.Data.ToArray();
        Assert.True(SaveFixtures.Slot(changed, index).WriteTo(changed, editedPk, EntityImportSettings.None));

        var start = changed.GetBoxSlotOffset(index.Box, index.Slot);
        var slotSize = changed.SIZE_BOXSLOT;
        Assert.True(changed.PartyCount == native.PartyCount, "Party count changed.");
        Assert.True(beforeSet.AsSpan(0, start).SequenceEqual(changed.Data[..start]), "Native edit changed data before the target slot.");
        Assert.True(beforeSet.AsSpan(start + slotSize).SequenceEqual(changed.Data[(start + slotSize)..]), "Native edit changed data after the target slot (including party/dex/records).");
        var expectedEdited = changed.Clone().Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;

        // No-op round trip: open, analyse, download unchanged.
        await Load(page, fixture.Bytes);
        await Expect(page.Locator("#overview-family")).ToHaveTextAsync(SupportMatrix.Families.Single(f => f.SaveType == native.GetType()).Games);
        // Compared with native Core values; the private values are kept out of the failure messages.
        Assert.True(await page.Locator("#overview-trainer").TextContentAsync() == native.OT, "Overview trainer differs from native (value withheld).");
        Assert.True(await page.Locator("#overview-tid").TextContentAsync() == native.DisplayTID.ToString("D5"), "Overview TID differs from native (value withheld).");
        Assert.True(await page.Locator("#overview-sid").TextContentAsync() == native.DisplaySID.ToString("D5"), "Overview SID differs from native (value withheld).");
        Assert.True(await page.Locator("#overview-money").TextContentAsync() == string.Create(CultureInfo.InvariantCulture, $"{native.Money:#,0} Pokédollars"), "Overview money differs from native (value withheld).");
        await Select(page, index);
        // Every inspector value matches native Core on the same slot; the private values are kept out of the failure messages.
        var inspected = InspectorText.Sections(SaveFixtures.Open(fixture.Bytes).Select(index).Inspect()).SelectMany(s => s.Rows);
        foreach (var row in inspected)
        {
            Assert.True(await page.Locator($"#{row.Id}").TextContentAsync() == row.Value, $"Inspector {row.Id} differs from native (value withheld).");
        }
        await CheckLegality(page, sourcePk, native, nativeSlot.Type);
        var noOp = await Download(page);
        Assert.True(noOp.AsSpan().SequenceEqual(expectedNoOp), "No-op browser/native output differs (bytes withheld).");

        // Edited round trip: reopen the no-op output, edit the nickname, apply, download.
        await Load(page, noOp);
        await Expect(page.Locator("#message")).ToHaveTextAsync("Save loaded locally; checksums valid.");
        await Select(page, index);
        await page.Locator("#nickname").FillAsync(nickname);
        await page.Locator("#nicknamed").CheckAsync();
        await CheckLegality(page, editedPk, changed, nativeSlot.Type);
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        var edited = await DownloadEdited(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native output differs (bytes withheld).");

        // The edited export changes only the nickname fields of the target slot.
        var reopened = SaveFixtures.Parse(edited);
        var reopenedPk = SaveFixtures.Slot(reopened, index).Read(reopened);
        Assert.True(reopened.ChecksumsValid && reopenedPk.Nickname == nickname);
        Assert.True(reopened.PartyCount == native.PartyCount, "Export changed party count.");
        Assert.True(PartyBytes(reopened).SequenceEqual(PartyBytes(native)), "Export changed party data.");
        AssertOnlyRangeDiffers(noOp, edited, start, slotSize);
        AssertOnlyNicknameChanged(sourcePk, reopenedPk);

        // Replacing the edited session with its own export shows the edit persisted. The current revision was downloaded,
        // so the replace waits only for the confirmation that the export was checked.
        await Load(page, edited);
        await page.Locator("#exit-continue").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Unmodified session");
        await Select(page, index);
        Assert.True(await page.Locator("#nickname").InputValueAsync() == nickname, "Downloaded nickname did not survive reopening.");
        await CheckLegality(page, reopenedPk, reopened, nativeSlot.Type);

        // Privacy, runtime errors, and reload clearing the session.
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred; no private traces retained.");
        await page.ReloadAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync();
        await Expect(page.Locator("#overview-title")).ToHaveCountAsync(0);
        await Expect(page.Locator("#box-grid")).ToHaveCountAsync(0);

        // The reload is a second boot: check it the same way, then that it left nothing behind.
        await app.AssertStaticBootAsync(session);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred after reload.");
        fixture.AssertUnchanged();

        var evidence = TestEnvironment.Optional(TestEnvironment.Evidence);
        if (evidence is not null)
        {
            Directory.CreateDirectory(evidence);
            var result = new
            {
                family, engine, browserVersion = session.BrowserVersion, basePath = "/" + prefix, bootMs = session.BootMs, totalMs = session.ElapsedMs,
                slotKind = "box", noOpMatchesNative = true, editedMatchesNative = true, reopened = true, originalUnchanged = true, privacyPassed = true,
            };
            var file = $"{family}-{engine}-{(prefix.Length == 0 ? "root" : "subpath")}.json";
            await File.WriteAllTextAsync(Path.Combine(evidence, file), JsonSerializer.Serialize(result));
        }
    }

    /// <summary>
    /// The first party member of each private save: a nickname edit through the published app matches the same native Core edit byte for
    /// byte, keeps its stored stats, HP and status, and changes only that party position. Values are withheld from messages.
    /// </summary>
    [TierTheory(TestCategory.RealSave)]
    [MemberData(nameof(RealCases))]
    public async Task RealSavePartyRoundTrip(string engine, string prefix, string family)
    {
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;
        Assert.True(native.PartyCount > 0, "The private save has no party member to edit.");
        var slot = SlotRef.InParty(0);
        var sourcePk = native.GetPartySlotAtIndex(0);
        var noOp = native.Clone().Write().ToArray();

        var changed = native.Clone();
        var editedPk = changed.GetPartySlotAtIndex(0);
        var nickname = editedPk.Nickname == "WASM Party" ? "WASM Test" : "WASM Party";
        editedPk.Nickname = nickname;
        editedPk.IsNicknamed = true;
        changed.SetPartySlotAtIndex(editedPk, 0, EntityImportSettings.None);
        var expectedEdited = changed.Clone().Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, noOp);
        await Select(page, slot);
        await page.Locator("#nickname").FillAsync(nickname);
        await page.Locator("#nicknamed").CheckAsync();
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        var party = NativeLegality.Of(changed, changed.GetPartySlotAtIndex(0), StorageSlotType.Party);
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(party.Verdict);
        Assert.True(await page.Locator("#legality-report-verbose").TextContentAsync() == party.VerboseReport, "Party legality differs from native (report withheld).");
        var edited = await DownloadEdited(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native party output differs (bytes withheld).");

        var reopened = SaveFixtures.Parse(edited);
        Assert.True(reopened.ChecksumsValid && reopened.PartyCount == native.PartyCount, "Export changed checksums or party count.");
        AssertOnlyNicknameChanged(sourcePk, reopened.GetPartySlotAtIndex(0));
        AssertOnlyRangeDiffers(noOp, edited, native.GetPartyOffset(0), native.SIZE_PARTY);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred; no private traces retained.");
        fixture.AssertUnchanged();
    }

    /// <summary>
    /// The first boxed Pokémon of each private save that is not an egg: friendship towards each stored trainer, edited through the published
    /// app, matches the same native Core edit byte for byte and changes only that slot. Values are withheld from messages.
    /// </summary>
    [TierTheory(TestCategory.RealSave)]
    [MemberData(nameof(RealCases))]
    public async Task RealSaveFriendshipRoundTrip(string engine, string prefix, string family)
    {
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;
        var index = FirstWritableNonEgg(native);
        var noOp = native.Clone().Write().ToArray();

        var changed = native.Clone();
        var editedPk = SaveFixtures.Slot(changed, index).Read(changed);
        var trainer = editedPk.OriginalTrainerFriendship == 200 ? 201 : 200;
        var hasHandler = editedPk.HandlingTrainerName.Length != 0;
        var handler = editedPk.HandlingTrainerFriendship == 100 ? 101 : 100;
        editedPk.OriginalTrainerFriendship = (byte)trainer;
        if (hasHandler)
        {
            editedPk.HandlingTrainerFriendship = (byte)handler;
        }
        Assert.True(SaveFixtures.Slot(changed, index).WriteTo(changed, editedPk, EntityImportSettings.None));
        var expectedEdited = changed.Clone().Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, noOp);
        await Select(page, index);
        await page.Locator("#ot-friendship").FillAsync(trainer.ToString(CultureInfo.InvariantCulture));
        if (hasHandler)
        {
            await page.Locator("#ht-friendship").FillAsync(handler.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            await Expect(page.Locator("#ht-friendship")).Not.ToBeEditableAsync();
        }
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        var edited = await DownloadEdited(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native friendship output differs (bytes withheld).");
        AssertOnlyRangeDiffers(noOp, edited, native.GetBoxSlotOffset(index.Box, index.Slot), native.SIZE_BOXSLOT);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred; no private traces retained.");
        fixture.AssertUnchanged();
    }

    /// <summary>
    /// The first boxed Pokémon that is not an egg and the first party member of each private save: a level step and a nature change on the
    /// boxed one and a level step on the party member, edited through the published app, match the same native Core edits (the party member
    /// recalculated by the PK6 party-stat policy) byte for byte and change only those two slots. Values are withheld from messages.
    /// </summary>
    [TierTheory(TestCategory.RealSave)]
    [MemberData(nameof(RealCases))]
    public async Task RealSaveLevelNatureRoundTrip(string engine, string prefix, string family)
    {
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;
        Assert.True(native.PartyCount > 0, "The private save has no party member to edit.");
        var index = FirstWritableNonEgg(native);
        var noOp = native.Clone().Write().ToArray();

        var changed = native.Clone();
        var boxed = SaveFixtures.Slot(changed, index).Read(changed);
        var boxedLevel = LevelStep(boxed.CurrentLevel);
        var nature = boxed.Nature == Nature.Adamant ? Nature.Modest : Nature.Adamant;
        boxed.EXP = Experience.GetEXP(boxedLevel, boxed.PersonalInfo.EXPGrowth);
        boxed.Nature = nature;
        Assert.True(SaveFixtures.Slot(changed, index).WriteTo(changed, boxed, EntityImportSettings.None));
        var member = changed.GetPartySlotAtIndex(0);
        var memberLevel = LevelStep(member.CurrentLevel);
        var (hp, status) = (member.Stat_HPCurrent, member.Status_Condition);
        member.EXP = Experience.GetEXP(memberLevel, member.PersonalInfo.EXPGrowth);
        member.ResetPartyStats();
        member.Status_Condition = status;
        member.Stat_HPCurrent = Math.Min(hp, member.Stat_HPMax);
        changed.SetPartySlotAtIndex(member, 0, EntityImportSettings.None);
        var expectedEdited = changed.Clone().Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, noOp);
        await Select(page, index);
        await page.Locator("#level").FillAsync(boxedLevel.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#nature").SelectOptionAsync(((int)nature).ToString(CultureInfo.InvariantCulture));
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        await Select(page, SlotRef.InParty(0));
        await page.Locator("#level").FillAsync(memberLevel.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        var edited = await DownloadEdited(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native level and nature output differs (bytes withheld).");

        // Only the two slots and the checksum footer may differ: take the party slot as edited, then check the rest against the box slot.
        var partyOffset = native.GetPartyOffset(0);
        var withParty = noOp.ToArray();
        edited.AsSpan(partyOffset, native.SIZE_PARTY).CopyTo(withParty.AsSpan(partyOffset));
        AssertOnlyRangeDiffers(withParty, edited, native.GetBoxSlotOffset(index.Box, index.Slot), native.SIZE_BOXSLOT);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred; no private traces retained.");
        fixture.AssertUnchanged();

        // One level up, or down from the highest level, so the edit always changes the level.
        static byte LevelStep(byte level) => level == Experience.MaxLevel ? (byte)(level - 1) : (byte)(level + 1);
    }

    /// <summary>
    /// The first boxed Pokémon that is not an egg and the first party member of each private save: an IV and an EV step on the boxed one and
    /// an IV step on the party member, edited through the published app, match the same native Core edits (the party member following the
    /// PK6 party-stat policy) byte for byte and change only those two slots. Values are withheld from messages.
    /// </summary>
    [TierTheory(TestCategory.RealSave)]
    [MemberData(nameof(RealCases))]
    public async Task RealSaveIvEvRoundTrip(string engine, string prefix, string family)
    {
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;
        Assert.True(native.PartyCount > 0, "The private save has no party member to edit.");
        var index = FirstWritableNonEgg(native);
        var noOp = native.Clone().Write().ToArray();

        var changed = native.Clone();
        var boxed = SaveFixtures.Slot(changed, index).Read(changed);
        var speedIv = IvStep(boxed.IV_SPE);
        // Lowering an EV is always accepted; with no EVs at all, 4 HP EVs stay within the total.
        var (evStat, evCore, ev) = boxed.EV_HP > 0 ? (0, 0, boxed.EV_HP - 1)
            : boxed.EV_ATK > 0 ? (1, 1, boxed.EV_ATK - 1)
            : boxed.EVTotal == 0 ? (0, 0, 4)
            : (5, 3, boxed.EV_SPE > 0 ? boxed.EV_SPE - 1 : Math.Min(4, EffortValues.Max510 - boxed.EVTotal));
        boxed.IV_SPE = speedIv;
        boxed.SetEV(evCore, ev);
        Assert.True(SaveFixtures.Slot(changed, index).WriteTo(changed, boxed, EntityImportSettings.None));
        var member = changed.GetPartySlotAtIndex(0);
        var storedStats = member.GetStats(member.PersonalInfo);
        var hpIv = IvStep(member.IV_HP);
        member.IV_HP = hpIv;
        if (!member.GetStats(member.PersonalInfo).AsSpan().SequenceEqual(storedStats))
        {
            // The policy recalculates only when the edit changes the calculation; otherwise the stored battle state is kept as stored.
            var (hp, status) = (member.Stat_HPCurrent, member.Status_Condition);
            member.ResetPartyStats();
            member.Status_Condition = status;
            member.Stat_HPCurrent = Math.Min(hp, member.Stat_HPMax);
        }
        changed.SetPartySlotAtIndex(member, 0, EntityImportSettings.None);
        var expectedEdited = changed.Clone().Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, noOp);
        await Select(page, index);
        await page.Locator("#iv-5").FillAsync(speedIv.ToString(CultureInfo.InvariantCulture));
        await page.Locator($"#ev-{evStat}").FillAsync(ev.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        await Select(page, SlotRef.InParty(0));
        await page.Locator("#iv-0").FillAsync(hpIv.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        var edited = await DownloadEdited(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native IV and EV output differs (bytes withheld).");

        // Only the two slots and the checksum footer may differ: take the party slot as edited, then check the rest against the box slot.
        var partyOffset = native.GetPartyOffset(0);
        var withParty = noOp.ToArray();
        edited.AsSpan(partyOffset, native.SIZE_PARTY).CopyTo(withParty.AsSpan(partyOffset));
        AssertOnlyRangeDiffers(withParty, edited, native.GetBoxSlotOffset(index.Box, index.Slot), native.SIZE_BOXSLOT);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred; no private traces retained.");
        fixture.AssertUnchanged();

        // One up, or down from the highest IV, so the edit always changes the IV.
        static int IvStep(int iv) => iv == 31 ? 30 : iv + 1;
    }

    /// <summary>
    /// The first boxed Pokémon that is not an egg and the first party member of each private save: a move change and a held item change on the
    /// boxed one and a PP step on the party member, edited through the published app, match the same native Core edits byte for byte and
    /// change only those two slots. Values are withheld from messages.
    /// </summary>
    [TierTheory(TestCategory.RealSave)]
    [MemberData(nameof(RealCases))]
    public async Task RealSaveItemMoveRoundTrip(string engine, string prefix, string family)
    {
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;
        Assert.True(native.PartyCount > 0, "The private save has no party member to edit.");
        var index = FirstWritableNonEgg(native);
        var noOp = native.Clone().Write().ToArray();

        var changed = native.Clone();
        var boxed = SaveFixtures.Slot(changed, index).Read(changed);
        var move = boxed.Move1 == (ushort)Move.Thunderbolt ? (ushort)Move.Surf : (ushort)Move.Thunderbolt;
        var item = boxed.HeldItem == ItemMoveDraftTests.Leftovers ? ItemMoveDraftTests.ChoiceScarf : ItemMoveDraftTests.Leftovers;
        Assert.True(boxed.Move1_PPUps <= 3, "The boxed Pokémon's first move has more PP Ups than a move can take.");
        // A move change keeps the PP Ups of the slot (or clears them for an empty one) and gives full PP.
        var ppUps = boxed.Move1 == 0 ? 0 : boxed.Move1_PPUps;
        boxed.Move1 = move;
        boxed.Move1_PPUps = ppUps;
        boxed.Move1_PP = boxed.GetMovePP(move, ppUps);
        boxed.HeldItem = item;
        Assert.True(SaveFixtures.Slot(changed, index).WriteTo(changed, boxed, EntityImportSettings.None));
        var member = changed.GetPartySlotAtIndex(0);
        Assert.True(member.Move1 != 0, "The party member's first move slot is empty.");
        var max = member.GetMovePP(member.Move1, member.Move1_PPUps);
        // One down, or up from none, within the move's PP, so the edit always changes the PP and is accepted.
        var pp = member.Move1_PP == 0 ? 1 : Math.Min(member.Move1_PP, max) - 1;
        member.Move1_PP = pp;
        changed.SetPartySlotAtIndex(member, 0, EntityImportSettings.None);
        var expectedEdited = changed.Clone().Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, noOp);
        await Select(page, index);
        await page.Locator("#move-0").SelectOptionAsync(move.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#held-item").SelectOptionAsync(item.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        await Select(page, SlotRef.InParty(0));
        await page.Locator("#pp-0").FillAsync(pp.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        var edited = await DownloadEdited(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native item, move and PP output differs (bytes withheld).");

        // Only the two slots and the checksum footer may differ: take the party slot as edited, then check the rest against the box slot.
        var partyOffset = native.GetPartyOffset(0);
        var withParty = noOp.ToArray();
        edited.AsSpan(partyOffset, native.SIZE_PARTY).CopyTo(withParty.AsSpan(partyOffset));
        AssertOnlyRangeDiffers(withParty, edited, native.GetBoxSlotOffset(index.Box, index.Slot), native.SIZE_BOXSLOT);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred; no private traces retained.");
        fixture.AssertUnchanged();
    }

    /// <summary>
    /// The first boxed Pokémon that is not an egg and the first party member of each private save that can be male or female and whose form
    /// is not its gender: an ability slot step on the boxed one and a gender change on the party member, edited through the published app,
    /// match the same native Core edits byte for byte and change only those two slots. Values are withheld from messages.
    /// </summary>
    [TierTheory(TestCategory.RealSave)]
    [MemberData(nameof(RealCases))]
    public async Task RealSaveAbilityGenderRoundTrip(string engine, string prefix, string family)
    {
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;
        var index = FirstWritableNonEgg(native);
        var position = Enumerable.Range(0, native.PartyCount).FirstOrDefault(i => native.GetPartySlotAtIndex(i) is { IsEgg: false, PersonalInfo.IsDualGender: true, Species: not (ushort)Species.Meowstic }, -1);
        Assert.True(position >= 0, "The private save has no party member that can be male or female.");
        var noOp = native.Clone().Write().ToArray();

        var changed = native.Clone();
        var boxed = SaveFixtures.Slot(changed, index).Read(changed);
        // The next slot after the stored one (the first for a slot number that names none), so the edit always changes the ability's slot.
        var slot = AbilityVerifier.IsValidAbilityBits(boxed.AbilityNumber) ? ((boxed.AbilityNumber >> 1) + 1) % boxed.PersonalInfo.AbilityCount : 0;
        boxed.SetAbilityIndex(slot);
        Assert.True(SaveFixtures.Slot(changed, index).WriteTo(changed, boxed, EntityImportSettings.None));
        var member = changed.GetPartySlotAtIndex(position);
        var gender = member.Gender == EntityGender.Male ? EntityGender.Female : EntityGender.Male;
        member.Gender = gender;
        changed.SetPartySlotAtIndex(member, position, EntityImportSettings.None);
        var expectedEdited = changed.Clone().Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, noOp);
        await Select(page, index);
        await page.Locator("#ability").SelectOptionAsync(slot.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        await Select(page, SlotRef.InParty(position));
        await page.Locator("#gender").SelectOptionAsync(gender.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        var edited = await DownloadEdited(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native ability and gender output differs (bytes withheld).");

        // Only the two slots and the checksum footer may differ: take the party slot as edited, then check the rest against the box slot.
        var partyOffset = native.GetPartyOffset(position);
        var withParty = noOp.ToArray();
        edited.AsSpan(partyOffset, native.SIZE_PARTY).CopyTo(withParty.AsSpan(partyOffset));
        AssertOnlyRangeDiffers(withParty, edited, native.GetBoxSlotOffset(index.Box, index.Slot), native.SIZE_BOXSLOT);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred; no private traces retained.");
        fixture.AssertUnchanged();
    }

    /// <summary>
    /// The first boxed Pokémon that is not an egg and the first party member of each private save that is not an egg: a species change of each,
    /// previewed and confirmed through the published app, matches Core's species change (and, for the party member, the PK6 party-stat
    /// policy: stats recalculated, HP never raised, status kept) byte for byte and changes only those two slots. Values are withheld from messages.
    /// </summary>
    [TierTheory(TestCategory.RealSave)]
    [MemberData(nameof(RealCases))]
    public async Task RealSaveSpeciesFormRoundTrip(string engine, string prefix, string family)
    {
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;
        var index = FirstWritableNonEgg(native);
        var position = Enumerable.Range(0, native.PartyCount).FirstOrDefault(i => native.GetPartySlotAtIndex(i) is { IsEgg: false, PartyStatsPresent: true }, -1);
        Assert.True(position >= 0, "The private save has no party member that is not an egg.");
        var noOp = native.Clone().Write().ToArray();

        var changed = native.Clone();
        var boxed = SaveFixtures.Slot(changed, index).Read(changed);
        var boxedSpecies = boxed.Species == (ushort)Species.Linoone ? (ushort)Species.Zigzagoon : (ushort)Species.Linoone;
        boxed.ChangeSpeciesForm(boxedSpecies, 0, changed.Personal);
        Assert.True(SaveFixtures.Slot(changed, index).WriteTo(changed, boxed, EntityImportSettings.None));
        var member = changed.GetPartySlotAtIndex(position);
        var memberSpecies = member.Species == (ushort)Species.Chansey ? (ushort)Species.Magikarp : (ushort)Species.Chansey;
        var (hp, status) = (member.Stat_HPCurrent, member.Status_Condition);
        member.ChangeSpeciesForm(memberSpecies, 0, changed.Personal);
        member.ResetPartyStats();
        member.Status_Condition = status;
        member.Stat_HPCurrent = Math.Min(hp, member.Stat_HPMax);
        changed.SetPartySlotAtIndex(member, position, EntityImportSettings.None);
        var expectedEdited = changed.Clone().Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, noOp);
        await Select(page, index);
        await page.Locator("#species").SelectOptionAsync(boxedSpecies.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#species-confirm").ClickAsync();
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        await Select(page, SlotRef.InParty(position));
        await page.Locator("#species").SelectOptionAsync(memberSpecies.ToString(CultureInfo.InvariantCulture));
        await page.Locator("#species-confirm").ClickAsync();
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        var edited = await DownloadEdited(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native species output differs (bytes withheld).");

        // Only the two slots and the checksum footer may differ: take the party slot as edited, then check the rest against the box slot.
        var partyOffset = native.GetPartyOffset(position);
        var withParty = noOp.ToArray();
        edited.AsSpan(partyOffset, native.SIZE_PARTY).CopyTo(withParty.AsSpan(partyOffset));
        AssertOnlyRangeDiffers(withParty, edited, native.GetBoxSlotOffset(index.Box, index.Slot), native.SIZE_BOXSLOT);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred; no private traces retained.");
        fixture.AssertUnchanged();
    }

    /// <summary>The first writable boxed PK6 that is not an egg, since eggs are not edited.</summary>
    private static SlotRef FirstWritableNonEgg(SaveFile save)
    {
        for (var i = 0; i < save.SlotCount; i++)
        {
            var slot = SlotRef.InBox(i / save.BoxSlotCount, i % save.BoxSlotCount);
            var info = SaveFixtures.Slot(save, slot);
            var pk = info.Read(save);
            if (pk is PK6 { Species: not 0, ChecksumValid: true, IsEgg: false } && info.CanWriteTo(save) && info.CanWriteTo(save, pk) == WriteBlockedMessage.None)
            {
                return slot;
            }
        }
        throw new InvalidOperationException("Real fixture contains no writable boxed PK6 that is not an egg.");
    }
}
