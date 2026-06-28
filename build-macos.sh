#!/bin/bash
set -e

APP_NAME="Randumizer"
BUNDLE_ID="com.faxanadu.randumizer"
VERSION="0.29"
DOTNET="/usr/local/share/dotnet/dotnet"

# Detect architecture
ARCH=$(uname -m)
if [ "$ARCH" = "arm64" ]; then
    RID="osx-arm64"
else
    RID="osx-x64"
fi

echo "Building for $RID..."

# Clean previous builds
rm -rf build
mkdir -p build

# Publish self-contained (no single file — native libs like libSkiaSharp need to be loose)
$DOTNET publish FaxanaduRando/FaxanaduRando.csproj \
    -c Release \
    -r $RID \
    --self-contained true \
    -o build/publish

# Create .app bundle structure
APP_DIR="build/${APP_NAME}.app"
mkdir -p "${APP_DIR}/Contents/MacOS"
mkdir -p "${APP_DIR}/Contents/Resources"

# Copy icon if it exists
if [ -f "assets/AppIcon.icns" ]; then
    cp assets/AppIcon.icns "${APP_DIR}/Contents/Resources/AppIcon.icns"
elif [ -f "assets/icon.png" ]; then
    # Generate icns from PNG
    ICONSET="build/AppIcon.iconset"
    mkdir -p "$ICONSET"
    sips -z 16 16     assets/icon.png --out "$ICONSET/icon_16x16.png" > /dev/null
    sips -z 32 32     assets/icon.png --out "$ICONSET/icon_16x16@2x.png" > /dev/null
    sips -z 32 32     assets/icon.png --out "$ICONSET/icon_32x32.png" > /dev/null
    sips -z 64 64     assets/icon.png --out "$ICONSET/icon_32x32@2x.png" > /dev/null
    sips -z 128 128   assets/icon.png --out "$ICONSET/icon_128x128.png" > /dev/null
    sips -z 256 256   assets/icon.png --out "$ICONSET/icon_128x128@2x.png" > /dev/null
    sips -z 256 256   assets/icon.png --out "$ICONSET/icon_256x256.png" > /dev/null
    sips -z 512 512   assets/icon.png --out "$ICONSET/icon_256x256@2x.png" > /dev/null
    sips -z 512 512   assets/icon.png --out "$ICONSET/icon_512x512.png" > /dev/null
    cp assets/icon.png "$ICONSET/icon_512x512@2x.png"
    iconutil -c icns "$ICONSET" -o "${APP_DIR}/Contents/Resources/AppIcon.icns"
fi

# Copy all published files into the bundle
cp -R build/publish/* "${APP_DIR}/Contents/MacOS/"

# Rename the main executable
mv "${APP_DIR}/Contents/MacOS/FaxanaduRando" "${APP_DIR}/Contents/MacOS/${APP_NAME}"
chmod +x "${APP_DIR}/Contents/MacOS/${APP_NAME}"

# Create Info.plist
cat > "${APP_DIR}/Contents/Info.plist" << PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>${APP_NAME}</string>
    <key>CFBundleDisplayName</key>
    <string>${APP_NAME}</string>
    <key>CFBundleIdentifier</key>
    <string>${BUNDLE_ID}</string>
    <key>CFBundleVersion</key>
    <string>${VERSION}</string>
    <key>CFBundleShortVersionString</key>
    <string>${VERSION}</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleExecutable</key>
    <string>${APP_NAME}</string>
    <key>CFBundleIconFile</key>
    <string>AppIcon</string>
    <key>LSMinimumSystemVersion</key>
    <string>12.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSPrincipalClass</key>
    <string>NSApplication</string>
</dict>
</plist>
PLIST

# Ad-hoc code sign
codesign --force --deep -s - "${APP_DIR}"

echo "App bundle created at: ${APP_DIR}"

# Create DMG
DMG_NAME="${APP_NAME}-${VERSION}-${RID}.dmg"
DMG_DIR="build/dmg"
rm -rf "${DMG_DIR}"
mkdir -p "${DMG_DIR}"
cp -R "${APP_DIR}" "${DMG_DIR}/"

# Create a symlink to Applications for drag-and-drop install
ln -s /Applications "${DMG_DIR}/Applications"

# Create the DMG
hdiutil create -volname "${APP_NAME}" \
    -srcfolder "${DMG_DIR}" \
    -ov -format UDZO \
    "build/${DMG_NAME}"

echo ""
echo "DMG created at: build/${DMG_NAME}"
echo "Done!"
