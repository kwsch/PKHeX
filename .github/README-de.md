PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Save Editor für die Pokémon Hauptreihe, geschrieben in [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29).

Die folgenden Dateien werden unterstützt:
* Spielstände ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* GameCube Memory Card Daten (\*.raw, \*.bin), die GC Pokémon Spielstände enthalten.
* Einzelne Pokémon (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4)
* Wunderkarten (\*.pgt, \*.pcd, \*.pgf, .wc\*) inklusive Konvertierung zu .pk\*
* Importieren von GO Park Entitäten (\*.gp1) inklusive Konvertierung zu .pb7
* Importieren von Teams aus entschlüsselten 3DS Battle Videos.
* Transferieren von einer Generation zur anderen. Dabei wird das Format automatisch umgewandelt.

Alle Daten werden so angezeigt, dass sie bearbeitet und gespeichert werden können.
Die Benutzeroberfläche unterstützt mehrere Sprachen, sie kann durch Text Dateien übersetzt werden.

Das Teilen der Pokémon wird durch das Importieren und Exportieren von Pokémon Showdown Sets und QR Codes erleichtert.

PKHeX erwartet Spielstände, die mit konsolenspezifischen Schlüsseln entschlüsselt wurden. Benutze also einen Savedata Manager um Spielstände von der Konsole zu exportieren und wieder zu importieren ([Checkpoint](https://github.com/FlagBrew/Checkpoint), save_manager, [JKSM](https://github.com/J-D-K/JKSM), oder SaveDataFiler).

**Wir unterstützen und dulden kein Cheating auf Kosten anderer. Benutze keine gehackten Pokémon in Kämpfen oder Tauschen mit anderen, die nicht wissen, dass du gehackte Pokémon benutzt.**

## Screenshots

![Main Window](https://i.imgur.com/A0Mmy0F.png)

## Erstellen

PKHeX ist eine Windows Forms Anwendung, die das [.NET Framework v4.6](https://www.microsoft.com/en-us/download/details.aspx?id=48137) benötigt, mit experimenteller Unterstützung für [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0).

Die Anwendung kann mit jedem Kompiler erstellt werde, der C# 8 unterstützt.

### Erstell Konfiguration

Benutze die Debug oder Release Konfiguration beim Erstellen. Es gibt keinen plattformspezifischen Code um den du dir dabei Sorgen machen musst!

## Anhängigkeiten

PKHeXs QR Code Generierung stammt von [QRCoder](https://github.com/codebude/QRCoder), welcher unter [der MIT Lizenz](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt) lizensiert ist.

PKHeX nutzt Shiny Sprites von [pokesprite](https://github.com/msikma/pokesprite), diese sind unter [der MIT Lizenz](https://github.com/msikma/pokesprite/blob/master/LICENSE) lizensiert.

### IDE

PKHeX kann mit IDEs wie [Visual Studio](https://visualstudio.microsoft.com/downloads/) durch das Öffnen der .sln oder .csproj Dateien geöffnet werden.

### GNU/Linux

GNU/Linux ist nicht das Hauptbetriebssystem der PKHeX Entwickler, daher könnten dort Bugs auftreten. Manche kommen möglicherweise von GNU/Linux spezifischem Code in Mono/Wine und können deshalb nicht von jedem reproduziert werden kann.
