Name:           pkhex
Version:        22.12.18
Release:        1%{?dist}

Summary:        Pokémon core series save editor, programmed in C#.

License:        LGPLv2+
URL:            https://github.com/amidevous/PKHeX
Source0:        https://github.com/amidevous/PKHeX/releases/download/24.01.12/pkhex-22.12.18.tar.xz

BuildArch:      noarch
AutoReqProv: no

# need arch-specific wine, not available everywhere:
# - adopted from wine.spec
ExclusiveArch:  %{ix86} x86_64 %{arm} aarch64
# - explicitly not ppc64* to hopefully not confuse koschei
ExcludeArch:    ppc64 ppc64le

BuildRequires:  make
BuildRequires:  desktop-file-utils

Requires:       wine-common wine winetricks
Requires:       cabextract gzip unzip wget which
Requires:       hicolor-icon-theme
Requires:       (kdialog if kdialog else zenity)

%description
Pokémon core series save editor, programmed in C#.
Supports the following files:
Save files ("main", *.sav, *.dsv, *.dat, *.gci, *.bin)
GameCube Memory Card files (*.raw, *.bin) containing GC Pokémon savegames.
Individual Pokémon entity files (.pk*, *.ck3, *.xk3, *.pb7, *.sk2, *.bk4, *.rk4)
Mystery Gift files (*.pgt, *.pcd, *.pgf, .wc*) including conversion to .pk*
Importing GO Park entities (*.gp1) including conversion to .pb7
Importing teams from Decrypted 3DS Battle Videos
Transferring from one generation to another, converting formats along the way.
Data is displayed in a view which can be edited and saved. The interface can be translated with resource/external text files so that different languages can be supported.
Pokémon Showdown sets and QR codes can be imported/exported to assist in sharing.
PKHeX expects save files that are not encrypted with console-specific keys. Use a savedata manager to import and export savedata from the console (Checkpoint, save_manager, JKSM, or SaveDataFiler).
We do not support or condone cheating at the expense of others. Do not use significantly hacked Pokémon in battle or in trades with those who are unaware hacked Pokémon are in use.


%prep
%setup -q

%build
# not needed

%install
%make_install


%files
/usr/bin/pkhex
/usr/share/applications/pkhex.desktop
/usr/share/pixmaps/pkhex.png
/usr/share/pkhex/PKHeX.exe
