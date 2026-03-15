#!/bin/bash
APP_NAME="PKHeX"
PUBLISH_DIR="bin/publish/linux-x64"

dotnet publish ../PKHeX.Avalonia.csproj -c Release -r linux-x64 --self-contained -o "$PUBLISH_DIR"

# Create AppDir structure
APP_DIR="${APP_NAME}.AppDir"
mkdir -p "${APP_DIR}/usr/bin"
mkdir -p "${APP_DIR}/usr/share/icons/hicolor/256x256/apps"
cp -R "${PUBLISH_DIR}/"* "${APP_DIR}/usr/bin/"
cp pkhex.desktop "${APP_DIR}/"
# cp pkhex.png "${APP_DIR}/usr/share/icons/hicolor/256x256/apps/"

cat > "${APP_DIR}/AppRun" << 'APPRUN'
#!/bin/bash
HERE=$(dirname "$(readlink -f "$0")")
exec "$HERE/usr/bin/PKHeX.Avalonia" "$@"
APPRUN
chmod +x "${APP_DIR}/AppRun"

echo "Created ${APP_DIR} - use appimagetool to create .AppImage"
