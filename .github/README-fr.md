PKHeX
=====
![Licence](https://img.shields.io/badge/License-GPLv3-blue.svg)

Éditeur de sauvegarde des jeux principaux de la série Pokémon, programmé en [C#](https://fr.wikipedia.org/wiki/C_sharp).

Les fichiers suivants sont pris en charge :
* Fichiers de sauvegarde (« main », \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* Fichiers de carte mémoire GameCube (\*.raw, \*.bin) contenant des sauvegardes de jeux Pokémon GameCube.
* Fichiers d'entités Pokémon individuels (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* Fichiers de Cadeau Mystère (\*.pgt, \*.pcd, \*.pgf, .wc\*), incluant la conversion en .pk\*
* Importation d'entités GO Park (\*.gp1), incluant la conversion en .pb7
* Importation d'équipes à partir de vidéos de combat 3DS déchiffrées
* Transfert d'une génération à l'autre, avec une conversion du format au passage.

Les données sont affichées sur une interface graphique, permettant de faire des modifications et des sauvegardes. L'interface peut être traduite avec des fichiers de ressources/textes externes afin que différentes langues puissent être prises en charge.

Les sets Pokémon Showdown! et les QR codes peuvent être importés/exportés pour faciliter le partage.

PKHeX demande des fichiers de sauvegarde qui ne sont pas chiffrés par des clés spécifiques aux consoles. Utilisez un gestionnaire de sauvegardes pour importer et exporter des sauvegardes depuis une console ([Checkpoint](https://github.com/FlagBrew/Checkpoint), save_manager, [JKSM](https://github.com/J-D-K/JKSM), ou SaveDataFiler).

**Nous ne soutenons ni ne tolérons la tricherie aux dépens des autres. N'utilisez pas de Pokémon piratés de manière significative en combat ou en échanges avec ceux qui ne savent pas que des Pokémon piratés sont utilisés.**

## Capture d'écran

![Fenêtre principale](https://i.imgur.com/CpUzqmY.png)

## Compilation

PKHeX est une application Windows Forms qui nécessite [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0).

L'exécutable peut être compilé avec n'importe quel compilateur prenant en charge C# 14.

### Configurations de la compilation

Utilisez la configuration Debug ou Release lors de la compilation. Aucun code spécifique à une plateforme n'est utilisée dans le programme, donc soyez sans crainte !

## Dépendances

Le code de génération des QR codes de PKHeX est extrait de [QRCoder](https://github.com/codebude/QRCoder), qui est [sous licence MIT](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt).

La collection de sprites chromatiques de PKHeX est tirée de [pokesprite](https://github.com/msikma/pokesprite), qui est [sous licence MIT](https://github.com/msikma/pokesprite/blob/master/LICENSE).

La collection de sprites Légendes Pokémon : Arceus de PKHeX est tirée du projet [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934), avec son abondance de collaborateurs et de contributeurs.

## IDE

PKHeX peut être ouvert avec des IDEs tels que [Visual Studio](https://visualstudio.microsoft.com/fr/downloads/) en ouvrant le fichier .sln ou .csproj.
