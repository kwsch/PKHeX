PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Pokémon core series save editor, programmed in [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29).

Supports the following files:
* Save files ("main", \*.sav, \*.dat, \*.gci)
* Individual Pokémon entity files (.pk\*)
* Mystery Gift files (.pgt, .pcd, .pgf, .wc\*) including conversion to .pk\*
* Importing teams from Decrypted 3DS Battle Videos
* Transferring from one generation to another, converting formats along the way.

Data is displayed in a view which can be edited and saved.
The interface can be translated with resource/external text files so that different languages can be supported.

Pokémon Showdown sets and QR codes can be imported/exported to assist in sharing.

Nintendo 3DS savedata containers use an AES MAC that cannot be emulated without the 3DS's keys, thus a resigning service is required ([svdt](https://github.com/meladroit/svdt), save_manager, [JKSM](https://github.com/J-D-K/JKSM), or SaveDataFiler).

## Screenshots

![Main Window](http://i.imgur.com/QT3IxpR.png)

## Building

PKHeX is a Windows Forms application which requires .NET Framework v4.6.

The executable can be built with any compiler that supports C# 6.0.

### Build Configurations

Use the Debug or Release build configurations when building using the .NET Framework.  Use the Mono-Debug or Mono-Release build configurations when building using Mono.

## Dependencies

PKHeX's QR code generation code is taken from [QRCoder](https://github.com/codebude/QRCoder), which is licensed under [the MIT license](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt).

### IDE

PKHeX can be opened with IDEs such as [Visual Studio](https://www.visualstudio.com/) or [MonoDevelop](http://www.monodevelop.com/) by opening the .sln or .csproj file.

### GNU/Linux

Install [MonoDevelop](http://www.monodevelop.com/) and [Mono Runtime](http://www.mono-project.com/) with `flatpak install --user --from https://download.mono-project.com/repo/monodevelop.flatpakref`. GNU/Linux is not the main Operating System of developers of this program so there may be bugs; some may come from non GNU/Linux specific code of Mono (so developers using *BSD, Windows and OS X should be able to reproduce them).
