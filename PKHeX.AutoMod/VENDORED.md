# Vendored: PKHeX AutoMod legalization engine

This project vendors the **legalization engine** from santacrab2's PKHeX-Plugins (Auto Legality Mod,
originally by architdate). It is the cross-platform, WinForms-free core that turns a Pokémon Showdown
set into a legal `PKM`. The Windows plugin UI (`AutoModPlugins`, WinForms) is **not** vendored.

## Source

| | |
|---|---|
| Upstream repo | https://github.com/santacrab2/PKHeX-Plugins |
| Vendored commit (SHA) | `b78bd4c274b75adf4454ad9cefebe0cbbbacfa19` |
| Commit date | 2026-07-08 |
| Upstream project | `PKHeX.Core.AutoMod` |
| Upstream `PKHeX.Core` NuGet pin | `26.7.7` |
| License | GPL-3.0-or-later (compatible with this repo's GPLv3) |

## What was vendored

The entire `PKHeX.Core.AutoMod` project source tree, copied verbatim with its upstream folder layout
and namespace (`PKHeX.Core.AutoMod`) preserved so future re-syncs stay a near drop-in:

```
AutoMod/            legalization core (APILegality, Legalizer, ShowdownEdits, RegenTemplate, trainers, ...)
Enhancements/       supporting logic used by the engine (ModLogic, EasterEggs, BattleTemplateLegality, ...)
```

The separate upstream projects `PKHeX.Core.Enhancements` (teams / Smogon net) and `PKHeX.Core.Injection`
(LiveHeX) were **not** vendored — they belong to later phases (issues #123 / #124).

## Why this is not under the 1:1 mirror rule

Unlike `PKHeX.Core` (which this repo mirrors 1:1 against kwsch/PKHeX), this project is a **second
vendored sync target**. Upstream pins `PKHeX.Core` as a NuGet package at a fixed version; here it is
switched to a **project reference on this repo's `PKHeX.Core`** (built from the latest upstream SHA).
On each Core sync, re-check this engine against the new Core and adapt any API drift here.

## Adaptations made vs. upstream

1. **`PKHeX.AutoMod.csproj` is new / rewritten.** Upstream's `PKHeX.Core.AutoMod.csproj` only contained
   `<PackageReference Include="PKHeX.Core" Version="26.7.7" />`. Replaced with:
   - `<ProjectReference Include="..\PKHeX.Core\PKHeX.Core.csproj" />` (build against this repo's Core).
   - `TargetFramework net10.0`, `Nullable enable` (root `Directory.Build.props` supplies `LangVersion 14`).
   - `AssemblyName = PKHeX.AutoMod` (the solution project name) while `RootNamespace` stays
     `PKHeX.Core.AutoMod` so **no source file needed editing**.
   - `NoWarn = CS0618;CS1591` and `GenerateDocumentationFile=false`, kept **defensively** to insulate the
     solution's warning-clean build from future Core drift in this third-party code. As of the vendored
     commit against this repo's Core (`26.07.07`), the engine compiles with **0 warnings even without
     these suppressions** — no source warnings are currently being hidden.

2. **No `.cs` source edits.** Every file under `AutoMod/` and `Enhancements/` is byte-for-byte upstream.
   Our Core version (`26.07.07`) is close enough to upstream's pin (`26.7.7`) that there was **no API
   drift to patch**. If a future Core sync breaks compilation, fix it here (not in `PKHeX.Core`) and
   record the change in this file.

3. **`AutoMod/APILegality.cs`: pass trainer context to `IGenerateSeed64`.** PKHeX.Core commit
   `757a297` added an `ITrainerInfo` parameter so Legends: Arceus encounter generation can derive the
   save-specific shiny-roll count. The vendored call now forwards its existing source trainer.

## Re-sync procedure

1. `git clone https://github.com/santacrab2/PKHeX-Plugins` and check out the new commit.
2. Replace `AutoMod/` and `Enhancements/` here with the upstream `PKHeX.Core.AutoMod/{AutoMod,Enhancements}`.
3. Keep `PKHeX.AutoMod.csproj` (do not overwrite it with upstream's package-referencing csproj).
4. `dotnet build PKHeX.AutoMod/PKHeX.AutoMod.csproj -c Release`; adapt any API breaks and log them above.
5. Update the SHA / date / Core pin in the table.

## Credits

Auto Legality Mod by **architdate** and **santacrab2** and contributors. GPL-3.0.
