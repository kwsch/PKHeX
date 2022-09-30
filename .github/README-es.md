PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Editor de guardado de las series principales de Pokémon, programado en [C#](https://es.wikipedia.org/wiki/C_Sharp).

Soporta los siguientes archivos:
* Archivos de guardado ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* Archivos de Memory Card de GameCube (\*.raw, \*.bin) que contienen archivos de GC Pokémon.
* Archivos de entidades individuales de Pokémon (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* Archivos de Regalos Misteriosos (\*.pgt, \*.pcd, \*.pgf, .wc\*) incluyendo conversión a .pk\*
* Importar archivos de entidades de GO Park (\*.gp1) incluyendo conversión a .pb7
* Importar equipos desde archivos Decrypted 3DS Battle Videos
* Pasar de una generación a la siguiente, convirtiendo los archivos en el proceso.

Los datos son visualizados en una forma que permite modificarlos y guardarlos.
La interfaz gráfica puede ser traducida con archivos de texto externos para dar soporte a múltiples lenguajes.

Pokémon Showdown asigna un código QR que puede ser importado/exportado para ayudar al compartir.

PKHeX espera archivos de guardado que no estén cifrados con las claves específicas de la consola. Use un gestor de archivos de guardado para importar y exportar información de la consola ([Checkpoint](https://github.com/FlagBrew/Checkpoint), save_manager, [JKSM](https://github.com/J-D-K/JKSM), o SaveDataFiler).

**No apoyamos ni toleramos las trampas a expensas de otros. No uses un Pokémon modificado significativamente en batalla o en intercambios con quienes no estén al tanto de que estás usando un Pokémon modificado.**

## Capturas de Pantalla

![Pantalla principal](https://i.imgur.com/umit9S2.png)

## Building

PKHeX es una aplicación de Windows Forms que requiere [.NET Framework v4.6](https://www.microsoft.com/es-es/download/details.aspx?id=48137), con soporte experimental para [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0).

El archivo ejecutable puede ser construido con cualquier compilador que soporte C# 10.

### Configuraciones del Build

Para hacer el build puedes usar las configuraciones de Debug o de Release. ¡No hay que preocuparse por código específico de ninguna plataforma!

## Dependencias

La generación de códigos QR de PKHeX es la de [QRCoder](https://github.com/codebude/QRCoder), licenciado bajo [la licencia MIT](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt).

La colección de sprites de Pokémons Shiny de PKHeX fue tomada de [pokesprite](https://github.com/msikma/pokesprite), licenciado bajo [la licencia MIT](https://github.com/msikma/pokesprite/blob/master/LICENSE).

PKHeX's Pokémon Legends: Arceus sprite collection is taken from the [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) project and its abundance of collaborators and contributors.

### IDE

PKHeX se puede abrir con un IDE como [Visual Studio](https://visualstudio.microsoft.com/es/downloads/), abriendo los archivos .sln o .csproj.

### GNU/Linux

GNU/Linux no es el sistema operativo principal de los desarrolladores de este proyecto, así que probablemente haya errores o bugs; de los cuales algunos pueden provenir de código no específico de GNU/Linux desde Mono o de Wine, con lo cual puede haber otros usuarios que no puedan reproducir ese error.
