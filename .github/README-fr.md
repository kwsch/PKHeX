PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Éditeur de sauvegarde de la série de base Pokémon, programmé en [C#](https://fr.wikipedia.org/wiki/C_sharp).

Prend en charge les fichiers suivants:
* Enregistrer les fichiers ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* Fichiers de carte mémoire GameCube (\*.raw, \*.bin) contenant des sauvegardes de Pokémon GC.
* Fichiers d'entités Pokémon individuels (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* Fichiers de cadeau mystère (\*.pgt, \*.pcd, \*.pgf, .wc\*) y compris la conversion en .pk\*
* Importation d'entités GO Park (\*.gp1) incluant la conversion en .pb7
* Importation d'équipes à partir de 3DS Battle Videos
* Transfert d'une génération à l'autre, conversion des formats en cours de route.

Les données sont affichées dans une vue qui peut être modifiée et enregistrée. L'interface peut être traduite avec des fichiers de ressources/textes externes afin que différentes langues puissent être prises en charge.

Les ensembles Pokémon Showdown et les QR codes peuvent être importés/exportés pour faciliter le partage.

PKHeX attend des fichiers de sauvegarde qui ne sont pas chiffrés avec des clés spécifiques à la console. Utilisez un gestionnaire de données enregistrées pour importer et exporter des données enregistrées à partir de la console ([Checkpoint](https://github.com/FlagBrew/Checkpoint) ou [JKSM](https://github.com/J-D-K/JKSM)).

**Nous ne soutenons ni ne tolérons la tricherie aux dépens des autres. N'utilisez pas de Pokémon piratés de manière significative au combat ou dans des échanges avec ceux qui ne savent pas que des Pokémon piratés sont en cours d'utilisation.**

## Captures d'écran

![Main Window](https://i.imgur.com/YEdBzlt.png)

## Installer

Windows binaire derniére version

https://projectpokemon.org/home/files/file/1-pkhex/

Linux pacquet 22.12.18 derniére version fonctionelle sous wine

https://software.opensuse.org//download.html?project=home%3Aamidevousgmail%3Apkhex&package=pkhex

## Construction

PKHeX est une application Windows Forms qui nécessite [.NET 8.0.](https://dotnet.microsoft.com/download/dotnet/8.0)

L'exécutable peut être construit avec n'importe quel compilateur prenant en charge C# 12.

### Construire les configurations

Utilisez les configurations Debug ou Release lors de la construction. Il n'y a pas de code spécifique à la plate-forme à craindre!

## Dépendances

Le code de génération du QR code de PKHeX est extrait de [QRCoder](https://github.com/codebude/QRCoder), qui est [sous licence MIT](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt).

La collection de sprites shiny de PKHeX est tirée de [pokesprite](https://github.com/msikma/pokesprite), qui est [sous licence MIT](https://github.com/msikma/pokesprite/blob/master/LICENSE).

PKHeX's Pokémon Legends: Arceus sprite collection is taken from the [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) project and its abundance of collaborators and contributors.

## IDE

PKHeX peut être ouvert avec des IDE tels que [Visual Studio](https://visualstudio.microsoft.com/fr/downloads/) en ouvrant le fichier .sln ou .csproj.

## Constructionpour Linux ou MacOSX uniquement 22.12.18 derniére version fonctionelle sous wine

installer wine 9.0 (multiarch requis) ou + et winetricks 20240105 ou +

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
