#!/bin/bash

LAUNCH_SETTINGS="../src/ShoppingProject.Web/Properties/launchSettings.json"

# launchSettings.json içinden applicationUrl değerini oku
APP_URL=$(jq -r '.profiles | to_entries[0].value.applicationUrl' $LAUNCH_SETTINGS)

# Eğer birden fazla URL varsa ilkini seç
APP_URL=$(echo $APP_URL | cut -d';' -f1)

# BASE_URL'i auth endpoint ile birleştir
BASE_URL="$APP_URL/auth"

# HTTPS ise self-signed sertifika için -k ekle
if [[ $BASE_URL == https* ]]; then
  CURL_OPTS="-k"
else
  CURL_OPTS=""
fi

echo "Using BASE_URL=$BASE_URL"

echo "=== Register ==="
curl $CURL_OPTS -s -X POST "$BASE_URL/register" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@example.com","password":"Pass123!"}' | jq .

echo "=== Token ==="
TOKEN_RESPONSE=$(curl $CURL_OPTS -s -X POST "$BASE_URL/token" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Pass123!"}')
echo "$TOKEN_RESPONSE" | jq .

ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.access_token')
REFRESH_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.refresh_token')

echo "=== Refresh ==="
curl $CURL_OPTS -s -X POST "$BASE_URL/refresh" \
  -H "Content-Type: application/json" \
  -d "{\"refresh_token\":\"$REFRESH_TOKEN\"}" | jq .

echo "=== Me ==="
curl $CURL_OPTS -s -X GET "$BASE_URL/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq .

echo "=== Revoke ==="
curl $CURL_OPTS -s -X POST "$BASE_URL/revoke" \
  -H "Content-Type: application/json" \
  -d "{\"refresh_token\":\"$REFRESH_TOKEN\"}" -i
