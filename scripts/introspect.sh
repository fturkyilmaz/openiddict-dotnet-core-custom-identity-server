#!/usr/bin/env bash
set -euo pipefail

BASE_URL="https://localhost:57679"

USERNAME="admin@test.com"
PASSWORD="Password123!"
SCOPE="openid profile offline_access"
GRANT_TYPE="password"

CLIENT_ID="open-id-api"
CLIENT_SECRET="open-id-api-secret"

echo "====================================="
echo "1) TOKEN ALINIYOR (password grant)"
echo "====================================="

TOKEN_RESPONSE=$(curl -s --http1.1 -X POST "$BASE_URL/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=$GRANT_TYPE" \
  --data-urlencode "username=$USERNAME" \
  --data-urlencode "password=$PASSWORD" \
  --data-urlencode "scope=$SCOPE")

echo "$TOKEN_RESPONSE" | jq .

ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.access_token')
REFRESH_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.refresh_token')

echo
echo "Access Token: $ACCESS_TOKEN"
echo "Refresh Token: $REFRESH_TOKEN"

echo
echo "====================================="
echo "2) INTROSPECTION (önce active=true)"
echo "====================================="

INTRO_BEFORE=$(curl -sk --http1.1 -u "$CLIENT_ID:$CLIENT_SECRET" \
  -X POST "$BASE_URL/connect/introspect" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  --data-urlencode "token=$ACCESS_TOKEN")

echo "$INTRO_BEFORE" | jq .

echo
echo "====================================="
echo "3) LOGOUT EVERYWHERE"
echo "====================================="

USER_ID=$(curl -sk --http1.1 -X GET "$BASE_URL/auth/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq -r '.id')

LOGOUT_RESPONSE=$(curl -sk --http1.1 -X POST "$BASE_URL/auth/logout-everywhere" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  --data-urlencode "userId=$USER_ID")

echo "$LOGOUT_RESPONSE"

echo
echo "====================================="
echo "4) INTROSPECTION (logout sonrası active=false)"
echo "====================================="

INTRO_AFTER=$(curl -sk --http1.1 -u "$CLIENT_ID:$CLIENT_SECRET" \
  -X POST "$BASE_URL/connect/introspect" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  --data-urlencode "token=$ACCESS_TOKEN")

echo "$INTRO_AFTER" | jq .

echo
echo "====================================="
echo "TEST TAMAMLANDI"
echo "====================================="
