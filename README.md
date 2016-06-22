PKHeX
=====

Pokémon NDS/3DS save editor, programmed in [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29).

Supports the following files originating from the Nintendo NDS & 3DS:
* Save files ("main", .sav)
* Individual Pokémon entity files (.pk*)
* Mystery Gift files (.pgt, .pcd, .pgf, .wc6) including conversion to .pk*
* Importing teams from Decrypted Battle Videos (X/Y/OR/AS only)
* Transferring of previous generation entities (.pkm) to future generation formats and save files

Data is displayed in a view which can be edited and saved.
The interface can be translated with resource/external text files so that different languages can be supported.

Pokémon Showdown sets can be imported/exported in addition to QR codes.

Nintendo 3DS savedata containers use an AES MAC that cannot be emulated without the 3DS's keys, thus a resigning service is required (svdt, save_manager, or SaveDataFiler).

## Screenshots

![Main Window](http://i.imgur.com/GBub4le.png)

### License

PKHeX is licensed under GPLv3. Refer to LICENSE.md for more information.

## Building

PKHeX can be compiled with any compiler that supports C# 6.0.

### IDE

PKHeX can be opened with MS Visual Studio and [MonoDevelop](http://www.monodevelop.com/) by importing the project with the .sln or .csproj file.

### Command Line

You can use [xbuild of Mono](http://mono-framework.com/Microsoft.Build): `xbuild PKHeX.sln`.

### GNU/Linux

On Debian, you can install MonoDevelop and [Mono](http://www.mono-project.com/) runtime with `sudo apt-get install monodevelop`. GNU/Linux is not the main Operating System of developers of this program so there may be bugs; some may come from non GNU/Linux specific code of Mono (so developers using *BSD, Windows and OS X should be able to reproduce them).
