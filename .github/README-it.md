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

## Installare

Ultima versione binaria di Windows

https://projectpokemon.org/home/files/file/1-pkhex/

Pacchetti Linux versione 22.12.18 ultimo lavoro su wine

https://software.opensuse.org//download.html?project=home%3Aamidevousgmail%3Apkhex&package=pkhex

## Edificio

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


## Creazione per Linux o MacOSX versione online 22.12.18 ultimo lavoro su wine

installa wine 9.0 (richiede multiarch) o + e winetricks 20240105 o +

```
git clone https://github.com/kwsch/PKHeX.git
DESTDIR=$PWD/build make DESTDIR=$PWD/build install
sudo mv $PWD/build/* /
```


manuale completo

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
