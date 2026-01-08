#!/bin/bash

BASE_URL="http://localhost:5000/auth"

echo "=== Register ==="
curl -s -X POST "$BASE_URL/register" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@example.com","password":"Pass123!"}' | jq .

echo "=== Token ==="
TOKEN_RESPONSE=$(curl -s -X POST "$BASE_URL/token" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Pass123!"}')
echo "$TOKEN_RESPONSE" | jq .

ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.access_token')
REFRESH_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.refresh_token')

echo "=== Refresh ==="
curl -s -X POST "$BASE_URL/refresh" \
  -H "Content-Type: application/json" \
  -d "{\"refresh_token\":\"$REFRESH_TOKEN\"}" | jq .

echo "=== Me ==="
curl -s -X GET "$BASE_URL/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq .

echo "=== Revoke ==="
curl -s -X POST "$BASE_URL/revoke" \
  -H "Content-Type: application/json" \
  -d "{\"refresh_token\":\"$REFRESH_TOKEN\"}" -i
