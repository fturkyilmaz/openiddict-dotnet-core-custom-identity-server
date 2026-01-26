#!/bin/bash

# Permission-Based Authorization Tests
# This script tests the permission-based authorization model

echo "🎯 Testing Permission-Based Authorization"
echo "======================================"

BASE_URL="https://localhost:57679"

echo ""
echo "1. Getting token with permission claims..."
TOKEN_RESPONSE=$(curl -s -X POST "$BASE_URL/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&username=testuser&password=Pass123!&scope=openid profile email")

ACCESS_TOKEN=$(echo $TOKEN_RESPONSE | jq -r '.access_token')

if [ "$ACCESS_TOKEN" != "null" ]; then
    echo "✅ Token obtained successfully"
else
    echo "❌ Failed to get token"
    echo $TOKEN_RESPONSE
    exit 1
fi

echo ""
echo "2. Testing users.read permission endpoint..."
RESPONSE=$(curl -s -w "%{http_code}" -X GET "$BASE_URL/api/protected/users" \
  -H "Authorization: Bearer $ACCESS_TOKEN")

HTTP_CODE="${RESPONSE: -3}"
if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ Users read endpoint accessible with users.read permission"
else
    echo "❌ Users read endpoint returned $HTTP_CODE"
fi

echo ""
echo "3. Testing users.write permission endpoint..."
RESPONSE=$(curl -s -w "%{http_code}" -X POST "$BASE_URL/api/protected/users" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"test"}')

HTTP_CODE="${RESPONSE: -3}"
if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ Users write endpoint accessible with users.write permission"
else
    echo "❌ Users write endpoint returned $HTTP_CODE"
fi

echo ""
echo "4. Testing users.manage permission endpoint (delete)..."
RESPONSE=$(curl -s -w "%{http_code}" -X DELETE "$BASE_URL/api/protected/users/123" \
  -H "Authorization: Bearer $ACCESS_TOKEN")

HTTP_CODE="${RESPONSE: -3}"
if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ User delete endpoint accessible with users.manage permission"
else
    echo "❌ User delete endpoint returned $HTTP_CODE"
fi

echo ""
echo "5. Debug claims to see permission claims..."
curl -s -X GET "$BASE_URL/api/protected/debug" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.claims[] | select(.Type=="permission")'

echo ""
echo "🎯 Permission-Based Authorization Test Complete"