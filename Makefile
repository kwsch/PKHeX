PREFIX := /usr
DESTDIR := 
install: 
	install -d "$(DESTDIR)$(PREFIX)/share/pkhex/"
	install -D -m 755 "launcher" "$(DESTDIR)$(PREFIX)/bin/pkhex"
	# pkhex version 22.12.18 latest full work with wine
	# require wine or wine-stable or winehq-stable 9.0 or + and winetricks 20240105 or +
	#download https://projectpokemon.org/home/files/file/1-pkhex/?do=download&version=6855&csrfKey=cd29ab3c78e0744900e1bfd604bbfd83 PKHeX (221218).zip
	#backup
	#https://github.com/amidevous/PKHeX/releases/download/24.01.12/PKHeX.221218.zip
	wget https://github.com/amidevous/PKHeX/releases/download/24.01.12/PKHeX.221218.zip -O PKHeX.221218.zip
	rm -f "PKHeX.exe"
	unzip PKHeX.221218.zip
	rm -f PKHeX.221218.zip
	install -D -m 644 "PKHeX.exe" "$(DESTDIR)$(PREFIX)/share/pkhex/PKHeX.exe"
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet40
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet45
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet452
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet46
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet461
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet462
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet471
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet472
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet48
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnetcoredesktop3
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnetdesktop6 win10
	install -D -m 644 "icon.png" "$(DESTDIR)$(PREFIX)/share/pixmaps/pkhex.png"
	install -D -m 644 "pkhex.desktop" "$(DESTDIR)$(PREFIX)/usr/share/applications/pkhex.desktop"
