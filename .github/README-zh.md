PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

本程序是基于 [C#](https://zh.wikipedia.org/wiki/C♯) 语言进行编写的宝可梦核心系列游戏存档编辑器。

支持以下存档类型：
* 存档文件 ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* GameCube 宝可梦游戏存档包含 GameCube 记忆存档 (\*.raw, \*.bin)
* 单个宝可梦实体文件 (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* 神秘礼物文件 (\*.pgt, \*.pcd, \*.pgf, .wc\*) 并转换为 .pk\*
* 导入 Go Park存档 (\*.gp1) 并转换为 .pb7
* 从已破解的 3DS 对战视频中导入队伍
* 支持宝可梦在不同世代的间转移，并转换文件格式

各项数据显示于窗口界面上以便编辑及保存。
 该界面可以通过 内部/外部 文本文件进行翻译，以便支持不同的语言。

可以导入/导出宝可梦Showdown代码和二维码以协助共享。

PKHeX 所读取存档文件必须是未经主机唯一密钥加密，因此请使用存档管理软件(如 [Checkpoint](https://github.com/FlagBrew/Checkpoint)， save_manager， [JKSM](https://github.com/J-D-K/JKSM)或 SaveDataFiler)以从主机中导入导出存档 .

**我们反对且不会纵容通过作弊手段损害他人利益的行为。切勿将魔法宝可梦用于对战，或通过连接交换给不知情的训练家手中。**

## 截图

![主介面](https://i.imgur.com/SfskT2Q.png)

## 构建

PKHeX 是 Windows 窗口应用程序，依赖于 [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)。

可以使用任何支持 C# 12 的编译器生成可执行文件。

### 构建配置

请使用 Debug 及 Release 配置通道进行构建，无须担心任何平台独有源代码相关问题！

## 依赖库

PKHeX 的 QR 码生成库来源于 [QRCoder](https://github.com/codebude/QRCoder)， 依据 [MIT 许可证](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt) 进行授权。

PKHeX 的异色精灵图示集合库来源于 [pokesprite](https://github.com/msikma/pokesprite)， 依据 [MIT 许可证](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt) 进行授权。

PKHeX 的“宝可梦传说：阿尔宙斯”精灵图片集来源于 [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) 项目，及其多位各界协作者和贡献者。
### IDE

PKHeX 可以通过打开 .sln 或 .csproj 文件来使用 [Visual Studio](https://visualstudio.microsoft.com/downloads/) 等 IDE 打开。
