PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

本程式係基於 [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29) 語言進行編寫之寶可夢核心系列遊戲儲存資料編輯器。

本程式支援下述檔案：
* 儲存資料檔 ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* 含 GameCube 寶可夢游戲儲存資料檔之 GameCube 記憶卡檔 (\*.raw, \*.bin)
* 單個寶可夢資料檔 (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* 神秘禮物檔 (\*.pgt, \*.pcd, \*.pgf, .wc\*) 並轉換至 .pk\*
* 匯入 Go 公園檔 (\*.gp1) 並轉換至 .pb7
* 從已解密 3DS 對戰視訊中匯入隊伍信息
* 將寶可夢從當前世代往後傳送，並順次轉換檔案格式

各項數據將顯示於介面上以便編輯及儲存。
介面亦可透過挂載內置/外置文字檔案以支援顯示多種語言。

可匯入/匯出 配置資訊及 QR 碼以便進行共有分享。

PKHeX 所讀取檔案須未經主機唯一密鑰加密，因而請使用儲存資料管理軟體(如 [Checkpoint](https://github.com/FlagBrew/Checkpoint)， save_manager， [JKSM](https://github.com/J-D-K/JKSM)， 抑或是 SaveDataFiler)以從主機中匯入匯出儲存資料 .

**我們反對亦不會縱容透過作弊手段損害他人利益之行為。切勿將魔法寶可夢用於對戰，或連線交換至不知情之訓練家手中。**

## 螢幕擷取截圖

![主介面](https://i.imgur.com/RBcUanJ.png)

## 構建

PKHeX 係 Windows 窗體應用程式，其須依賴於 [.NET Framework v4.6](https://www.microsoft.com/en-us/download/details.aspx?id=48137) 而運行，同時本程式亦實驗式地支援 [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)。

程式可透過任意支援 C# 10 之編譯器構建。

### 構建配置

請使用 Debug 及 Release 配置通道進行構建，無須擔心任何平台獨有源代碼相關問題！

## 依賴庫

PKHeX 之 QR 碼生成庫來源於 [QRCoder](https://github.com/codebude/QRCoder)， 其以 [MIT 許可證](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt) 進行分發。

PKHeX 之異色電腦圖學精靈集合庫來源於 [pokesprite](https://github.com/msikma/pokesprite)， 其以 [MIT 許可證](https://github.com/msikma/pokesprite/blob/master/LICENSE) 進行分發。

PKHeX 之「寶可夢傳説：阿爾宙斯」電腦圖學精靈集合庫來源於 [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) 項目，及其多位各界協作者和貢獻者。
### IDE

PKHeX 可透過如 [Visual Studio](https://visualstudio.microsoft.com/downloads/) 等各類 IDE ，開啓 .sln 或 .csproj 檔案以打開。

### GNU/Linux

GNU/Linux 非本程式開發者主要作業系統，因而於 GNU/Linux 平臺上運行本程式時可能存在 Bug； 部分 Bug 亦並非來源於 GNU/Linux 平臺上 Mono/Wine 之特定源代碼，因而其他使用者可能無法復現你所遇到之 Bug。
