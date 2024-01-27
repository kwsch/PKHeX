PREFIX := /usr
DESTDIR := 
install: 
	rm -rf .github PKHeX.Core PKHeX.Drawing.Misc PKHeX.Drawing.PokeSprite PKHeX.Drawing PKHeX.WinForms Tests .editorconfig .gitattributes .gitignore Directory.Build.props LICENSE PKHeX.sln README.md
	install -d "$(DESTDIR)$(PREFIX)/share/pkhex/"
	install -d "$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex"
	install -d "$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex/drive_c"
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
	echo winetrick create wait 2 hour max
	WINEPREFIX="$(DESTDIR)$(PREFIX)/share/pkhex/wineprefixes/pkhex" winetricks -q --force dotnet40 dotnet45 dotnet452 dotnet46 dotnet461 dotnet462 dotnet471 dotnet472 dotnet48 dotnetcoredesktop3 dotnetdesktop6 win10 >/dev/null 2>&1
	install -D -m 644 "icon.png" "$(DESTDIR)$(PREFIX)/share/pixmaps/pkhex.png"
	install -D -m 644 "pkhex.desktop" "$(DESTDIR)$(PREFIX)/usr/share/applications/pkhex.desktop"
