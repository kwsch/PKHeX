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

## Construction

PKHeX est une application Windows Forms qui nécessite [.NET 9.0.](https://dotnet.microsoft.com/download/dotnet/9.0)

L'exécutable peut être construit avec n'importe quel compilateur prenant en charge C# 13.

### Construire les configurations

Utilisez les configurations Debug ou Release lors de la construction. Il n'y a pas de code spécifique à la plate-forme à craindre!

## Dépendances

Le code de génération du QR code de PKHeX est extrait de [QRCoder](https://github.com/codebude/QRCoder), qui est [sous licence MIT](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt).

La collection de sprites shiny de PKHeX est tirée de [pokesprite](https://github.com/msikma/pokesprite), qui est [sous licence MIT](https://github.com/msikma/pokesprite/blob/master/LICENSE).

PKHeX's Pokémon Legends: Arceus sprite collection is taken from the [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) project and its abundance of collaborators and contributors.

## IDE

PKHeX peut être ouvert avec des IDE tels que [Visual Studio](https://visualstudio.microsoft.com/fr/downloads/) en ouvrant le fichier .sln ou .csproj.
