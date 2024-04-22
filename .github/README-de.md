PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Save Editor für die Pokémon Hauptreihe, geschrieben in [C#](https://de.wikipedia.org/wiki/C-Sharp).

Die folgenden Dateien werden unterstützt:
* Spielstände ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* GameCube Memory Card Daten (\*.raw, \*.bin), die GC Pokémon Spielstände enthalten.
* Einzelne Pokémon (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* Wunderkarten (\*.pgt, \*.pcd, \*.pgf, .wc\*), inklusive Konvertierung zu .pk\*
* Import von GO Park Pokémon (\*.gp1) inklusive Konvertierung zu .pb7
* Import von Teams aus entschlüsselten 3DS Battle Videos.
* Transfer von einer Generation zur anderen, mit automatischer Umwandlung des Formats.

Alle Daten werden so angezeigt, dass sie bearbeitet und gespeichert werden können.
Die Benutzeroberfläche unterstützt mehrere Sprachen und kann durch Text Dateien übersetzt werden.

Pokémon können aus Pokémon Showdown Sets und QR Codes exportiert bzw. importiert werden, was das Teilen erleichtert.

PKHeX erwartet entschlüsselte Spielstände. Da diese konsolenspezifisch verschlüsselt sind, wird ein Savedata Manager benötigt, um Spielstände von der Konsole zu exportieren und wieder zu importieren ([Checkpoint](https://github.com/FlagBrew/Checkpoint), save_manager, [JKSM](https://github.com/J-D-K/JKSM), oder SaveDataFiler).

**Wir unterstützen und dulden kein Cheating auf Kosten anderer. Benutze keine gehackten Pokémon in Kämpfen oder Tauschen mit anderen, die nicht wissen, dass du gehackte Pokémon benutzt.**

## Screenshots

![Main Window](https://i.imgur.com/7ErmRJI.png)

## Erstellen

PKHeX ist eine Windows Forms Anwendung, welche die [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) runtime benötigt.

Die Anwendung kann mit jedem Kompiler erstellt werden, der C# 12 unterstützt.

### Erstell Konfiguration

Benutze die Debug oder Release Konfiguration beim Erstellen. Es gibt keinen plattformspezifischen Code um den du dir dabei Sorgen machen musst!

## Anhängigkeiten

PKHeXs QR Code Generierung stammt von [QRCoder](https://github.com/codebude/QRCoder), welcher unter [der MIT Lizenz](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt) lizensiert ist.

PKHeX nutzt Shiny Sprites von [pokesprite](https://github.com/msikma/pokesprite), diese sind unter [der MIT Lizenz](https://github.com/msikma/pokesprite/blob/master/LICENSE) lizensiert.

PKHeXs Pokémon Legends: Arceus Sprites kommen vom [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) Projekt und seiner Vielzahl an Mitarbeitern und Mitwirkenden.

### IDE

PKHeX kann mit IDEs wie [Visual Studio](https://visualstudio.microsoft.com/de/downloads/) durch das Öffnen der .sln oder .csproj Dateien geöffnet werden.
