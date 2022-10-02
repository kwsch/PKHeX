PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Editor di Salvataggi Pokémon per la serie principale, programmato in [C#](https://it.wikipedia.org/wiki/C_sharp).

Supporta i seguenti file:
* File di salvataggio ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* File di Memory Card GameCube (\*.raw, \*.bin) contenenti File di Salvataggio Pokémon.
* File di Entità Pokémon individuali (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* File di Dono Segreto (\*.pgt, \*.pcd, \*.pgf, .wc\*) inclusa conversione in .pk\*
* Importazione di Entità del Go Park (\*.gp1) inclusa conversione in .pb7
* Importazione di squadre da Video Lotta del 3DS decriptati
* Trasferimento da una generazione all'altra, convertendo i formati propriamente.

I dati sono mostrati in una finestra che può essere modificata e salvata.
L'interfaccia può essere tradotta con risorse/file di testo esterni, così che sia possibile supportare diversi linguaggi.

Set di Pokémon Showdown e QR Code possono essere importati/esportati per agevolare la condivisione di file.

PKHeX si aspetta file di salvataggio non criptati con le chiavi specifiche della console. È possibile usare un gestore di salvataggi per importare ed esportare dati di salvataggio dalla console ([Checkpoint](https://github.com/FlagBrew/Checkpoint), save_manager, [JKSM](https://github.com/J-D-K/JKSM), o SaveDataFiler).

**Non supportiamo e non perdoniamo l'utilizzo di cheat a scapito di altri giocatori. Non utilizzare Pokémon modificati significativamente in lotte o scambi con giocatori che non ne sono a conoscenza.**

## Screenshots

![Main Window](https://i.imgur.com/RBcUanJ.png)

## Building

PKHeX è un applicazione Windows Form e necessita [.NET Framework v4.6](https://www.microsoft.com/it-it/download/details.aspx?id=48137), con supporto sperimentale per [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0).

L'eseguibile può essere compilato con qualsiasi compiler che supporti C# 10.

### Configurazioni di Build

Puoi utilizzare la configurazione Debug o la configurazione Release per compilare. Non c'è alcun codice specifico per piattaforma di cui doversi preoccupare!

## Dipendenze

Il codice per la generazione di QR Code è preso da [QRCoder](https://github.com/codebude/QRCoder), concesso in licenza sotto [the MIT license](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt).

La collezione di sprite shiny è presa da [pokesprite](https://github.com/msikma/pokesprite), concessa in licenza sotto [the MIT license](https://github.com/msikma/pokesprite/blob/master/LICENSE).

La collezione di sprite per Leggende Pokémon: Arceus è presa dal [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) project grazie all'abbondanza dei collaboratori e dei contribuenti.

### IDE

PKHeX può essere aperto con IDE come [Visual Studio](https://visualstudio.microsoft.com/it/downloads/) aprendo il file .sln o il file .csproj.

### GNU/Linux

GNU/Linux non è il Sistema Operativo principale dei developer di questo programma, quindi potrebbero esserci bug; alcuni potrebbero provenire da codice GNU/Linux non specifico da Mono/Wine, per cui alcuni utenti potrebbero non essere in grado di riprodurre l'errore riscontrato.
