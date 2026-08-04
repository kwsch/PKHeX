# LiveHeX: clean-room sys-botbase client (not vendored)

## Decision: clean-room, not vendored

Unlike the PKHeX.AutoMod legalization engine (which is vendored byte-for-byte from santacrab2/PKHeX-Plugins), the LiveHeX connectivity here is a CLEAN-ROOM re-implementation in this project's Infrastructure layer under namespace `PKHeX.Infrastructure.LiveHex`. The upstream `PKHeX.Core.Injection` project (same repo, commit b78bd4c274b75adf4454ad9cefebe0cbbbacfa19) was evaluated for vendoring but rejected because:

1. It requires a `LibUsbDotNet` (v2.2.29) package reference for its USB controller (`UsbBotMini`); USB is explicitly out of scope for v1, and that package targets old frameworks and risks NU1701 restore warnings against net10.0, which would break this repo's 0-warning build requirement.
2. It also carries 3DS `NTRClient`/`NTRAPIFramework` code (~500 LOC), dead weight for the Switch-only v1.
3. `RamOffsets.GetSwitchInterface`/`GetCommunicator` hard-reference `UsbBotMini` and `NTRClient`, so a byte-for-byte subset excluding those files would not compile without editing vendored sources — breaking the byte-for-byte rule.
4. The sys-botbase wire protocol is tiny and public (ASCII `peek`/`poke`/`peekMain`/`peekAbsolute`/`pokeAbsolute` + hex over TCP port 6000), so a clean-room TCP client is small, dependency-free, and fully unit-testable against a loopback fake — matching the required Application-port / Infrastructure-adapter architecture.

## Attribution

The sys-botbase text-command grammar and the per-game console-RAM offset/pointer constants (box-start heap offsets and pointer expressions for Sword/Shield, Brilliant Diamond/Shining Pearl, Legends: Arceus, Scarlet/Violet) are factual data mirrored from the upstream PKHeX-Plugins `PKHeX.Core.Injection` project (RamOffsets / LPBasic / LPPointer / LPBDSP) at commit b78bd4c274b75adf4454ad9cefebe0cbbbacfa19, and from the sys-botbase project by olliz0r. Credit: Auto-Legality / LiveHeX tooling by architdate, santacrab2 and contributors (GPL-3.0); sys-botbase by olliz0r. This repository is GPL-3.0, compatible.

## Files (clean-room)

- PKHeX.Infrastructure/LiveHex/SwitchCommand.cs — encodes sys-botbase text commands
- PKHeX.Infrastructure/LiveHex/HexCodec.cs — hex encode/decode for payloads and responses
- PKHeX.Infrastructure/LiveHex/SysBotConnection.cs — TCP client implementing IConsoleConnection
- PKHeX.Infrastructure/LiveHex/SysBotConnectionFactory.cs — factory for real connections
- PKHeX.Infrastructure/LiveHex/PointerResolver.cs — resolves pointer-expression chains
- PKHeX.Infrastructure/LiveHex/LiveHexGameProfiles.cs — game-support matrix + RAM offset/pointer tables
- PKHeX.Infrastructure/LiveHex/LiveHexBoxAddressing.cs — reads/writes a box via the three addressing modes
- PKHeX.Infrastructure/LiveHex/LiveHexService.cs — ILiveHexService orchestration
- Application-layer ports: PKHeX.Application/Abstractions/LiveHex/{IConsoleConnection,IConsoleConnectionFactory,ILiveHexService,LiveHexConnectionException}.cs

## Game-support matrix (v1, Wi-Fi / sys-botbase TCP only; USB out of scope)

| Game | Save type | Addressing | Firmware versions |
| --- | --- | --- | --- |
| Sword/Shield | SAV8SWSH | flat heap offset | 1.1.1, 1.2.1, 1.3.2 |
| Brilliant Diamond/Shining Pearl | SAV8BS | per-Pokémon pointer table | 1.0.0, 1.1.0, 1.1.1, 1.1.2, 1.1.3, 1.2.0, 1.3.0 |
| Legends: Arceus | SAV8LA | pointer-resolved contiguous | 1.0.0, 1.0.1, 1.0.2, 1.1.1 |
| Scarlet/Violet | SAV9SV | pointer-resolved contiguous | 1.0.1, 1.1.0, 1.2.0, 1.3.0, 1.3.1, 1.3.2, 2.0.1, 2.0.2, 3.0.0, 3.0.1, 4.0.0 |

## Re-sync note

When PKHeX-Plugins is re-synced, re-check the offset/pointer tables in LiveHexGameProfiles.cs against upstream RamOffsets/LP* for the pinned commit and add any newly-supported firmware versions (data-only changes).
