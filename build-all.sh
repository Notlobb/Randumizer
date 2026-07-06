#!/bin/bash
set -e

# ── Single source of truth for the release ─────────────────────────────────
APP_NAME="Randumizer"
VERSION="0.29-beta.5"
BUNDLE_ID="com.faxanadu.randumizer"
DOTNET="/usr/local/share/dotnet/dotnet"
CSPROJ="FaxanaduRando/FaxanaduRando.csproj"
# ───────────────────────────────────────────────────────────────────────────

# Which platforms to build. Override from the command line, e.g.:
#   ./build-all.sh windows linux
# Defaults to all three (macOS bundle only produced when run on macOS).
TARGETS=("$@")
if [ ${#TARGETS[@]} -eq 0 ]; then
    TARGETS=("windows" "linux" "macos")
fi

rm -rf build
mkdir -p build

build_windows() {
    for RID in win-x64 win-arm64; do
        echo "=== Windows: $RID ==="
        local OUT="build/win/${RID}"
        $DOTNET publish "$CSPROJ" \
            -c Release -r "$RID" --self-contained true \
            -p:PublishSingleFile=true \
            -p:IncludeNativeLibrariesForSelfExtract=true \
            -p:DebugType=none \
            -o "$OUT"
        mv "${OUT}/FaxanaduRando.exe" "${OUT}/${APP_NAME}.exe"
        local ZIP="${APP_NAME}-${VERSION}-${RID}.zip"
        (cd "$OUT" && zip -q "../${ZIP}" "${APP_NAME}.exe")
        echo "Created build/win/${ZIP}"
    done
}

build_linux() {
    for RID in linux-x64 linux-arm64; do
        echo "=== Linux: $RID ==="
        local OUT="build/linux/${RID}"
        $DOTNET publish "$CSPROJ" \
            -c Release -r "$RID" --self-contained true \
            -p:PublishSingleFile=true \
            -p:IncludeNativeLibrariesForSelfExtract=true \
            -p:DebugType=none \
            -o "$OUT"
        mv "${OUT}/FaxanaduRando" "${OUT}/${APP_NAME}"
        chmod +x "${OUT}/${APP_NAME}"
        local TAR="${APP_NAME}-${VERSION}-${RID}.tar.gz"
        tar -czf "build/linux/${TAR}" -C "$OUT" "${APP_NAME}"
        echo "Created build/linux/${TAR}"
    done
}

build_macos() {
    if [ "$(uname)" != "Darwin" ]; then
        echo "=== macOS: skipped (not running on macOS) ==="
        return
    fi

    # Detect architecture
    local ARCH RID
    ARCH=$(uname -m)
    if [ "$ARCH" = "arm64" ]; then RID="osx-arm64"; else RID="osx-x64"; fi
    echo "=== macOS: $RID ==="

    # Publish self-contained (no single file — native libs like libSkiaSharp
    # must stay loose on macOS).
    $DOTNET publish "$CSPROJ" \
        -c Release -r "$RID" --self-contained true \
        -o build/macos/publish

    local APP_DIR="build/macos/${APP_NAME}.app"
    mkdir -p "${APP_DIR}/Contents/MacOS"
    mkdir -p "${APP_DIR}/Contents/Resources"

    # Icon
    if [ -f "assets/AppIcon.icns" ]; then
        cp assets/AppIcon.icns "${APP_DIR}/Contents/Resources/AppIcon.icns"
    elif [ -f "assets/icon.png" ]; then
        local ICONSET="build/macos/AppIcon.iconset"
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

    cp -R build/macos/publish/* "${APP_DIR}/Contents/MacOS/"
    mv "${APP_DIR}/Contents/MacOS/FaxanaduRando" "${APP_DIR}/Contents/MacOS/${APP_NAME}"
    chmod +x "${APP_DIR}/Contents/MacOS/${APP_NAME}"

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

    codesign --force --deep -s - "${APP_DIR}"

    # DMG with an Applications symlink for drag-and-drop install
    local DMG_NAME="${APP_NAME}-${VERSION}-${RID}.dmg"
    local DMG_DIR="build/macos/dmg"
    rm -rf "${DMG_DIR}"
    mkdir -p "${DMG_DIR}"
    cp -R "${APP_DIR}" "${DMG_DIR}/"
    ln -s /Applications "${DMG_DIR}/Applications"
    hdiutil create -volname "${APP_NAME}" \
        -srcfolder "${DMG_DIR}" -ov -format UDZO \
        "build/macos/${DMG_NAME}"
    echo "Created build/macos/${DMG_NAME}"
}

for TARGET in "${TARGETS[@]}"; do
    case "$TARGET" in
        windows|win) build_windows ;;
        linux)       build_linux ;;
        macos|mac)   build_macos ;;
        *) echo "Unknown target: $TARGET (expected windows|linux|macos)" >&2; exit 1 ;;
    esac
done

echo ""
echo "All requested builds complete. Artifacts under build/"
