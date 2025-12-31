PKHeX(포케헥스)
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

포켓몬 코어 시리즈 세이브 에디터,  [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29)으로 프로그래밍됨.

다음 파일을 지원합니다:
* 세이브 파일 ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* GC 포켓몬 세이브 게임이 들어 있는 게임큐브 메모리 카드 파일(\*.raw, \*.bin)  포함
* 개별 포켓몬 엔티티 파일 (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* 이상한소포 파일(\*.pgt, \*.pcd, \*.pgf, .wc\*)을 .pk로 변환하는 기능 포함
* GO 파크 엔티티 가져오기 (\*.gp1) .pb7로 변환 포함
* Decrypted 3DS Battle Videos에서 팀 가져오기
* 한 세대에서 다른 세대로 이동하면서 그 과정에서 형식이 변환됩니다.

데이터는 편집하고 저장할 수 있는 보기로 표시됩니다.
인터페이스는 리소스/외부 텍스트 파일로 번역할 수 있어 다양한 언어를 지원할 수 있습니다.

포켓몬 쇼다운 세트와 QR 코드를 가져오고 내보낼 수 있어 공유에 도움을 줄 수 있습니다.

PKHeX는 콘솔 전용 키로 암호화되지 않은 세이브 파일을 요구합니다. ([Checkpoint](https://github.com/FlagBrew/Checkpoint), save_manager, [JKSM](https://github.com/J-D-K/JKSM), 또는 SaveDataFiler)를 사용하여 콘솔에서 세이브 데이터를 가져오고 내보낼 수 있습니다.

**저희는 타인을 희생시키는 부정행위를 지지하거나 묵인하지 않습니다. 해킹된 포켓몬이 사용 중이라는 사실을 모르는 사람들과의 배틀 또는 통신에서 심각하게 해킹된 포켓몬을 사용하지 마십시오.**

## 스크린샷

![Main Window](https://i.imgur.com/vDiaS7k.png)

## 빌드

PKHeX는 [.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)이 필요한 Windows Forms 애플리케이션입니다.

실행 파일은 C# 14을 지원하는 모든 컴파일러로 빌드할 수 있습니다.

### 빌드 구성

빌드할 때 디버그 또는 릴리스 빌드 구성을 사용하세요. 플랫폼 전용 코드는 걱정할 필요가 없습니다!

## 종속성

PKHeX의 QR 코드 생성 코드는 [the MIT license](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt)에 따라 라이선스가 부여된 [QRCoder](https://github.com/codebude/QRCoder) 에서 가져왔습니다.

PKHeX의 이로치(색이다른) 스프라이트 컬렉션은 [the MIT license](https://github.com/msikma/pokesprite/blob/master/LICENSE)에 따라 라이선스가 부여된 [pokesprite](https://github.com/msikma/pokesprite)에서 가져왔습니다.

PKheX의 Pokémon LEGENDS 아르세우스 스프라이트 컬렉션은 [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) 프로젝트와 수많은 협력자 및 기여자의 도움을 받아 만들어졌습니다.

### IDE(통합 개발 환경)

PKHeX는 .sln 또는 .csproj 파일을 열어 [Visual Studio](https://visualstudio.microsoft.com/downloads/)와 같은 IDE(통합 개발 환경)로 열 수 있습니다.
