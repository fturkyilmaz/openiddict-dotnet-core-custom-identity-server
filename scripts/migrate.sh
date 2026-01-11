#!/usr/bin/env bash
set -euo pipefail

# Script'i scripts klasöründen çalıştırdığını varsayıyorum
INFRA_PROJ="../src/ShoppingProject.Infrastructure/ShoppingProject.Infrastructure.csproj"
WEB_PROJ="../src/ShoppingProject.Web/ShoppingProject.Web.csproj"

MIGRATION_NAME="AutoMigration_$(date +%Y%m%d%H%M%S)"

echo "→ Yeni migration oluşturuluyor: $MIGRATION_NAME"
dotnet ef migrations add "$MIGRATION_NAME" \
  --project "$INFRA_PROJ" \
  --startup-project "$WEB_PROJ" \
  --context AppDbContext

echo "→ Database update ediliyor..."
dotnet ef database update \
  --project "$INFRA_PROJ" \
  --startup-project "$WEB_PROJ" \
  --context AppDbContext

echo "✅ Migration ve update tamamlandı."
