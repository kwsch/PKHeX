#!/bin/bash
# Creates a macOS .app bundle for PKHeX Avalonia
APP_NAME="PKHeX"
PUBLISH_DIR="bin/publish/osx-arm64"
APP_DIR="${APP_NAME}.app"

# Build
dotnet publish ../PKHeX.Avalonia.csproj -c Release -r osx-arm64 --self-contained -o "$PUBLISH_DIR"

# Create .app structure
mkdir -p "${APP_DIR}/Contents/MacOS"
mkdir -p "${APP_DIR}/Contents/Resources"
cp -R "${PUBLISH_DIR}/"* "${APP_DIR}/Contents/MacOS/"
cp Info.plist "${APP_DIR}/Contents/"
# cp icon.icns "${APP_DIR}/Contents/Resources/" # Add icon later

echo "Created ${APP_DIR}"
