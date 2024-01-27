PKHeX
=====
<div>
  <span>English</span> / <a href=".github/README-es.md">Español</a> / <a href=".github/README-fr.md">Français</a> / <a href=".github/README-de.md">Deutsch</a> / <a href=".github/README-it.md">Italiano</a> / <a href=".github/README-zhHK.md">繁體中文</a> / <a href=".github/README-zh.md">简体中文</a>
</div>

![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Pokémon core series save editor, programmed in [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29).

Supports the following files:
* Save files ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* GameCube Memory Card files (\*.raw, \*.bin) containing GC Pokémon savegames.
* Individual Pokémon entity files (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* Mystery Gift files (\*.pgt, \*.pcd, \*.pgf, .wc\*) including conversion to .pk\*
* Importing GO Park entities (\*.gp1) including conversion to .pb7
* Importing teams from Decrypted 3DS Battle Videos
* Transferring from one generation to another, converting formats along the way.

Data is displayed in a view which can be edited and saved.
The interface can be translated with resource/external text files so that different languages can be supported.

Pokémon Showdown sets and QR codes can be imported/exported to assist in sharing.

PKHeX expects save files that are not encrypted with console-specific keys. Use a savedata manager to import and export savedata from the console ([Checkpoint](https://github.com/FlagBrew/Checkpoint), save_manager, [JKSM](https://github.com/J-D-K/JKSM), or SaveDataFiler).

**We do not support or condone cheating at the expense of others. Do not use significantly hacked Pokémon in battle or in trades with those who are unaware hacked Pokémon are in use.**

## Screenshots

![Main Window](https://i.imgur.com/HZs37cM.png)

## Install

Windows binary latest version

https://projectpokemon.org/home/files/file/1-pkhex/

Linux packages version 22.12.18 latest work on wine

https://software.opensuse.org//download.html?project=home%3Aamidevousgmail%3Apkhex&package=pkhex

## Building

PKHeX is a Windows Forms application which requires [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0).

The executable can be built with any compiler that supports C# 12.

### Build Configurations

Use the Debug or Release build configurations when building. There isn't any platform specific code to worry about!

## Dependencies

PKHeX's QR code generation code is taken from [QRCoder](https://github.com/codebude/QRCoder), which is licensed under [the MIT license](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt).

PKHeX's shiny sprite collection is taken from [pokesprite](https://github.com/msikma/pokesprite), which is licensed under [the MIT license](https://github.com/msikma/pokesprite/blob/master/LICENSE).

PKHeX's Pokémon Legends: Arceus sprite collection is taken from the [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) project and its abundance of collaborators and contributors.

### IDE

PKHeX can be opened with IDEs such as [Visual Studio](https://visualstudio.microsoft.com/downloads/) by opening the .sln or .csproj file.

## Building for Linux or MacOSX online version 22.12.18 latest work on wine

install wine 9.0 (multiarch require) or + and winetricks 20240105 or +

```
git clone https://github.com/kwsch/PKHeX.git
DESTDIR=$PWD/build make DESTDIR=$PWD/build install
sudo mv $PWD/build/* /
```

complétement manuel

```
wget https://github.com/amidevous/PKHeX/releases/download/24.01.12/PKHeX.221218.zip -O PKHeX.221218.zip
rm -f "PKHeX.exe"
unzip PKHeX.221218.zip
rm -f PKHeX.221218.zip
install -D -m 644 "PKHeX.exe" "$HOME/.local/share/pkhex/PKHeX.exe"
install -d "$HOME/.local/share/pkhex/"
install -d "$HOME/.local/share/pkhex/wineprefixes/pkhex"
install -d "$HOME/.local/share/pkhex/wineprefixes/pkhex/drive_c"
wget "https://raw.githubusercontent.com/amidevous/PKHeX/master/launcher" -O "launcher"
sudo install -D -m 755 "launcher" "/usr/bin/pkhex"
wget "https://raw.githubusercontent.com/amidevous/PKHeX/master/icon.png" -O "icon.png"
sudo install -D -m 644 "icon.png" "/usr/share/pixmaps/pkhex.png"
wget "https://raw.githubusercontent.com/amidevous/PKHeX/master/pkhex.desktop" -O "pkhex.desktop"
sudo install -D -m 644 "pkhex.desktop" "/usr/usr/share/applications/pkhex.desktop"
WINEPREFIX="$HOME/.local/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet40 dotnet45 dotnet452 dotnet46 dotnet461 dotnet462 dotnet471 dotnet472 dotnet48 dotnetcoredesktop3 dotnetdesktop6 win10 >/dev/null 2>&1
```
