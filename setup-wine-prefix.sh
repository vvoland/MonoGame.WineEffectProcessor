#!/bin/bash

set -e

MONOGAME_SETUP_URL=$(curl https://api.github.com/repos/MonoGame/MonoGame/releases 2>/dev/null | grep -o "http.*/MonoGameSetup.exe" | head -n 1)

XDG_CACHE_HOME=${XDG_CACHE_HOME:-$HOME/.cache}
DEFAULT_PREFIX="${XDG_CACHE_HOME}/monogame-wine"

prefix="$DEFAULT_PREFIX"
if [ -d "$prefix" ]; then
    echo "Directory $prefix already exists"
    read -p "Do you want to continue? [y/N]: " yesNo
    if [ ! "$yesNo" == "y" ]; then
        echo "Aborting..."
        exit 1
    fi
fi


mkdir -p "$prefix"

export WINEPREFIX="$prefix"
winetricks d3dcompiler_47
if [ ! $? -eq 0 ]; then
    echo "Installing d3dcompiler_47 failed. Please check your winetricks version"
    exit 2
fi

echo "Downloading setup from $MONOGAME_SETUP_URL..."
wget "$MONOGAME_SETUP_URL" -O "$WINEPREFIX/drive_c/MonoGameSetup.exe"
wine64 "C:\\MonoGameSetup.exe" /S
