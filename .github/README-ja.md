PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

[C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29) でプログラミングされた、ポケットモンスターの本編シリーズ用セーブデータエディタ。

以下のファイルをサポートしています：
- セーブファイル ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
- GC版ポケモンのセーブデータを含むゲームキューブ メモリーカードファイル (\*.raw, \*.bin)
- 個別のポケモンデータファイル (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
- .pk\* への変換を含む、ふしぎなおくりものファイル (\*.pgt, \*.pcd, \*.pgf, .wc\*)
- .pb7 への変換を含む、GOパークデータのインポート (\*.gp1)
- 復号化された3DSバトルビデオからのパーティのインポート
- 世代間の転送、および途中のフォーマット変換。

データはビューに表示され、編集および保存が可能です。
さまざまな言語をサポートするため、リソースや外部テキストファイルを使用してインターフェースを翻訳できます。

共有を支援するため、Pokémon ShowdownのセットやQRコードのインポート/エクスポートが可能です。

PKHeXは、本体固有のキーで暗号化されていないセーブファイルを必要とします。本体からセーブデータをインポートおよびエクスポートするには、セーブデータマネージャー（[Checkpoint](https://github.com/FlagBrew/Checkpoint)、save_manager、[JKSM](https://github.com/J-D-K/JKSM)、またはSaveDataFiler）を使用してください。

**私たちは、他者の不利益となる不正行為をサポートまたは容認しません。改造されたポケモンが含まれていることを相手が知らない状態で、対戦や交換に著しく改造されたポケモンを使用しないでください。**

## スクリーンショット

![Main Window](https://i.imgur.com/pIHdoTp.png)

## ビルド

PKHeXは [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) を必要とするWindowsフォームアプリケーションです。

実行ファイルは、C# 14をサポートする任意のコンパイラでビルドできます。

### ビルド構成

ビルド時には Debug または Release のビルド構成を使用してください。プラットフォーム固有のコードについて心配する必要はありません！

## 依存関係

PKHeXのQRコード生成コードは、[MITライセンス](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt)の下でライセンスされている [QRCoder](https://github.com/codebude/QRCoder) から取得しています。

PKHeXの色違いスプライトコレクションは、[MITライセンス](https://github.com/msikma/pokesprite/blob/master/LICENSE)の下でライセンスされている [pokesprite](https://github.com/msikma/pokesprite) から取得しています。

PKHeXのPokémon LEGENDS アルセウスのスプライトコレクションは、[National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) プロジェクトと、その多くの協力者および貢献者から取得しています。

### IDE

PKHeXは、.sln または .csprojファイルを開くことで、[Visual Studio](https://visualstudio.microsoft.com/downloads/)などのIDEで開くことができます。