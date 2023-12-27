PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Editor di Salvataggi Pokémon per la serie principale, programmato in [C#](https://it.wikipedia.org/wiki/C_sharp).

Supporta i seguenti tipi di file:
* File di salvataggio ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* File di Memory Card GameCube (\*.raw, \*.bin) contenenti File di Salvataggio Pokémon.
* File di Entità Pokémon individuali (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* File di Dono Segreto (\*.pgt, \*.pcd, \*.pgf, .wc\*) inclusa conversione in .pk\*
* Importazione di Entità del Go Park (\*.gp1) inclusa conversione in .pb7
* Importazione di squadre da Video Lotta del 3DS decriptati
* Trasferimento da una generazione all'altra, convertendo i formati propriamente.

I dati sono mostrati in una finestra che può essere modificata e salvata.
L'interfaccia può essere tradotta tramite risorse/file di testo esterni, così che sia possibile supportare diverse lingue.

Set di Pokémon Showdown e QR Code possono essere importati/esportati per agevolare la condivisione di file.

PKHeX si aspetta file di salvataggio non criptati con le chiavi specifiche della console. È possibile utilizzare un gestore di salvataggi per importare ed esportare dati di salvataggio dalla console (come [Checkpoint](https://github.com/FlagBrew/Checkpoint) o [JKSM](https://github.com/J-D-K/JKSM)).

**Non supportiamo e non perdoniamo l'utilizzo di cheat a scapito di altri giocatori. Non utilizzare Pokémon alterati in lotte o scambi con giocatori che non ne sono a conoscenza.**

## Screenshots

![Main Window](https://i.imgur.com/ICmQ41m.png)

## Building

PKHeX è un applicazione Windows Form che necessita del [.NET Desktop Runtime 8.0](https://dotnet.microsoft.com/download/dotnet/8.0).

L'eseguibile può essere compilato con qualsiasi compiler che supporti C# 12.

### Configurazioni di Build

È possibile utilizzare la configurazione Debug o la configurazione Release per compilare la soluzione. Non c'è alcun codice specifico per piattaforma di cui doversi preoccupare!

## Dipendenze

Il codice per la generazione di QR Code è preso da [QRCoder](https://github.com/codebude/QRCoder), concesso in licenza sotto [the MIT license](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt).

La collezione di sprite shiny è presa da [pokesprite](https://github.com/msikma/pokesprite), concessa in licenza sotto [the MIT license](https://github.com/msikma/pokesprite/blob/master/LICENSE).

La collezione di sprite per Leggende Pokémon: Arceus è presa dal progetto [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934), grazie a tutti i collaboratori e contribuenti.

### IDE

PKHeX può essere aperto con IDE come [Visual Studio](https://visualstudio.microsoft.com/it/downloads/) aprendo il file .sln o il file .csproj.
