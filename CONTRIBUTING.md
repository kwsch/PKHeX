# Contributing to PKHeX-Avalonia

Thanks for helping improve the Avalonia UI for PKHeX! This document covers the contribution area most
open to community help: **UI translations**. (For code contributions, keep `PKHeX.Core` and the
vendored `PKHeX.AutoMod` byte-for-byte with upstream — never edit them; port behaviour into the
Avalonia layers instead.)

## UI localization (translating the app shell)

The app ships its own **UI-chrome** strings — menus, tab headers, buttons, dialog titles, settings
labels, and status/error messages — separately from PKHeX.Core's game data (species, moves, items,
etc.). Core data is already localized by `GameInfo.Strings`; **do not** try to translate species/move
names here.

### Where the strings live

```
PKHeX.Presentation/Localization/Strings/
  en.json        <- source of truth (English). Edit keys here first.
  ja.json  ko.json  fr.json  it.json  de.json  es.json  zh-Hans.json  zh-Hant.json
```

Each file is a flat JSON object of `"Key": "Translated value"`. The nine languages match the nine
data languages PKHeX.Core supports. English is authoritative; any key missing from another language
**falls back to the English string at runtime** (never blank, never the raw key).

The initial non-English files are a machine-assisted first pass — **native review is very welcome.**

### How it works (for reference)

- `LocalizedStrings` (in `PKHeX.Presentation/Localization/`) loads the JSON tables and exposes a
  string indexer with English fallback. It is BCL-only so ViewModels can use it directly
  (`LocalizedStrings.Instance["Key"]`) for dialog/status text.
- XAML uses the `{loc:Loc Key}` markup extension (`PKHeX.Avalonia/Localization/LocExtension.cs`), e.g.
  `Header="{loc:Loc Menu_File}"`. It binds through the active-language property so switching the
  language re-renders every visible string live, no restart.
- The active language follows the language selector (menu **Options → Language**, or **Settings →
  Appearance → Language**) and is persisted in `AppSettings.DisplayLanguage`.

### Fixing or adding a translation

1. Open the language file you want to improve (e.g. `fr.json`).
2. Find the key and edit its value. **Keep the key unchanged.**
3. Preserve, verbatim:
   - format placeholders `{0}`, `{1}` (e.g. `"Status_GameFormat": "Game: {0}"`);
   - escape sequences like `\n`;
   - game-version tokens and acronyms (`SWSH`, `BDSP`, `SV`, `HGSS`, `B2W2`, `Gen 1`…`Gen 9`, …),
     and the brand names `PKHeX`, `Avalonia`, `Showdown`, `LiveHeX`, `QR`, `PKM`.
4. Menu access-key mnemonics: a leading/embedded underscore (e.g. `"_File"`) marks the Alt-key
   letter. Keep exactly one underscore before a letter for Latin-script languages; drop it for
   CJK (Japanese/Korean/Chinese) menus.
5. Use official localized Pokémon terminology where it exists (Pokédex, Pokéblock, Poffin, Hall of
   Fame, Mystery Gift, Tera Raid, …).

### Adding a NEW string to the UI

1. Add the key + English value to `en.json`.
2. Reference it: `{loc:Loc My_New_Key}` in XAML, or `LocalizedStrings.Instance["My_New_Key"]` in a
   ViewModel. **Never hardcode a user-facing literal** — the audit test below will fail the build.
3. Add the key (translated, or copy the English value as a placeholder) to the other eight files so
   they stay key-complete.

### Guardrail test

`Tests/PKHeX.Avalonia.Tests/LocalizationAuditTests.cs` fails the build if a `.axaml` view or a
ViewModel introduces a hardcoded user-facing string instead of going through the resource system, and
`LocalizationServiceTests.cs` verifies all nine files are key-complete and that placeholders are
preserved.

The editor views and ViewModels are now migrated into the resource system, so
`Tests/PKHeX.Avalonia.Tests/localization-allowlist.txt` is near-empty — any remaining line is a
documented, justified exception (e.g. a glyph-only control or a technical literal). New views and
ViewModels are enforced by default. If you ever need to defer a file or justify a single literal:

1. Prefer migrating it: replace its literals with `{loc:Loc Key}` / `LocalizedStrings.Instance["Key"]`
   and add the keys to all nine JSON files.
2. Only if migration is genuinely not applicable, add a `RelativePath` (whole file) or
   `RelativePath|snippet` (single literal) line to `localization-allowlist.txt` with a comment saying why.
3. Run `dotnet test Tests/PKHeX.Avalonia.Tests` — it must stay green.

### Verifying

```
dotnet test Tests/PKHeX.Avalonia.Tests --filter "FullyQualifiedName~Localization"
```

Then run the app and switch languages via **Options → Language** to confirm the UI updates live.
