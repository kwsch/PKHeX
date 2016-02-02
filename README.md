PKHeX
=====

Pokémon X/Y/OR/AS SAV/PKX file editor, programmed in [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29).

Supports binary file loading of the following files originating from the Nintendo 3DS:
* Decrypted Save files (1MB/Cyber Save 0x65600/0x76000 Bytes)
* Individual entity files (232/260 bytes), saved as .pk6 and .ek6
* Conversion of Event Gift files (.wc6) to .pk6
* Decrypted Battle Videos
 
The following files from previous generations of games are supported:
* Transferring of previous generation entities (.pkm) to .pk6
* Conversion of Event Files (.pgt, .pcd, .pgf) to .pk6

Data is then displayed in a meaningful view, and can be edited and saved back to binary data.
Since the Nintendo 3DS savedata containers use an AES MAC that cannot be emulated without the 3DS's keys, a resigning service is required (svdt, save_manager, or SaveDataFiler).

The interface is translatable with resource/external text files so that different languages can be supported.

## Screenshots

![Main Window](http://i.snag.gy/dGdB4.jpg?raw=true)

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
