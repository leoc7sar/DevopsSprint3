#!/usr/bin/env bash
set -euo pipefail

# ====== VARIÁVEIS (edite os sufixos únicos) ======
RG="mottu-rg"
LOC="brazilsouth"
PLAN="mottu-plan-s3"
APP="mottu-api-s3-SEU_SUFIXO"          # precisa ser único no Azure
SQLS="mottusqls3SEU_SUFIXO"            # único global
SQLADMIN="sqladminfiap"
SQLPASS="Defina_Uma_Senha_Forte_Aqui!"

# ====== RESOURCE GROUP ======
az group create -n "$RG" -l "$LOC"

# ====== AZURE SQL (server + db) ======
az sql server create -g "$RG" -n "$SQLS" -l "$LOC" -u "$SQLADMIN" -p "$SQLPASS"
az sql db create -g "$RG" -s "$SQLS" -n mottu_db -e GeneralPurpose -f Gen5 -c 2 -z 5GB
az sql server firewall-rule create -g "$RG" -s "$SQLS" -n AllowAllAzureIPs --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0

# ====== APP SERVICE (Linux) ======
az appservice plan create -g "$RG" -n "$PLAN" -l "$LOC" --sku B1 --is-linux
az webapp create -g "$RG" -p "$PLAN" -n "$APP" --runtime "DOTNETCORE:8.0"

# ====== APP SETTINGS (conn string + segredos) ======
CONNSTR="Server=tcp:${SQLS}.database.windows.net,1433;Initial Catalog=mottu_db;Persist Security Info=False;User ID=${SQLADMIN};Password=${SQLPASS};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
az webapp config connection-string set -g "$RG" -n "$APP" --settings Default="$CONNSTR" --connection-string-type SQLAzure
az webapp config appsettings set -g "$RG" -n "$APP" --settings ASPNETCORE_ENVIRONMENT=Production Jwt__Key="mude-esta-chave-em-producao"

# ====== MIGRATIONS no Azure SQL (aplicando schema) ======
dotnet ef database update --connection "$CONNSTR"

# ====== PUBLISH & DEPLOY (Zip Deploy) ======
dotnet publish -c Release -o ./publish
cd publish && zip -r ../app.zip . >/dev/null && cd ..
az webapp deploy -g "$RG" -n "$APP" --src-path app.zip --type zip

echo "===================================================="
echo "OK! Sua API está no ar em: https://${APP}.azurewebsites.net"
echo "Swagger: https://${APP}.azurewebsites.net/swagger"
