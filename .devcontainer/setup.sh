#!/usr/bin/env bash
# Dijalankan otomatis sekali saat Codespace dibuat (postCreateCommand).
set -e

dotnet tool install --global dotnet-ef --version "10.*" || dotnet tool update --global dotnet-ef --version "10.*"
export PATH="$PATH:$HOME/.dotnet/tools"

dotnet restore
dotnet ef database update

echo ""
echo "Siap. Jalankan aplikasi dengan:"
echo "  dotnet run --urls http://0.0.0.0:5000"
