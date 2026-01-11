#!/usr/bin/env bash
set -euo pipefail

BASE_URL="https://localhost:57679"

# Kullanıcı bilgileri
USERNAME="admin@test.com"
PASSWORD="Password123!"
SCOPE="openid profile offline_access"
GRANT_TYPE="password"

echo "====================================="
echo "1) TOKEN ALINIYOR (password grant)"
echo "====================================="

TOKEN_RESPONSE=$(curl -sk --http1.1 -X POST "$BASE_URL/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=$GRANT_TYPE" \
  -d "username=$USERNAME" \
  -d "password=$PASSWORD" \
  -d "scope=$SCOPE")

echo "$TOKEN_RESPONSE"

ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.access_token')
REFRESH_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.refresh_token')

echo
echo "Access Token alındı:"
echo "$ACCESS_TOKEN"
echo
echo "Refresh Token alındı:"
echo "$REFRESH_TOKEN"

echo
echo "====================================="
echo "2) ACCESS TOKEN İLE /auth/me ÇAĞRISI"
echo "====================================="

ME_RESPONSE=$(curl -sk --http1.1 -X GET "$BASE_URL/auth/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Accept: application/json")

echo "$ME_RESPONSE"ap

USER_ID=$(echo "$ME_RESPONSE" | jq -r '.id')

echo
echo "USER_ID alındı:"
echo "$USER_ID"

echo
echo "====================================="
echo "3) LOGOUT EVERYWHERE (form-urlencoded ile userId)"
echo "====================================="

LOGOUT_RESPONSE=$(curl -sk --http1.1 -i -X POST "$BASE_URL/auth/logout-everywhere" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  --data-urlencode "userId=$USER_ID")

echo "$LOGOUT_RESPONSE"

echo
echo "====================================="
echo "4) REFRESH TOKEN İLE YENİ TOKEN DENEMESİ"
echo "   (logout-everywhere sonrası invalid_grant beklenir)"
echo "====================================="

REFRESH_RESPONSE=$(curl -sk --http1.1 -X POST "$BASE_URL/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=refresh_token" \
  --data-urlencode "refresh_token=$REFRESH_TOKEN")

echo "$REFRESH_RESPONSE"

echo
echo "====================================="
echo "5) ESKİ ACCESS TOKEN İLE /auth/me TEKRAR"
echo "====================================="

ME_AGAIN=$(curl -sk --http1.1 -i -X GET "$BASE_URL/auth/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Accept: application/json")

echo "$ME_AGAIN"

echo
echo "====================================="
echo "TEST TAMAMLANDI"
echo "====================================="
