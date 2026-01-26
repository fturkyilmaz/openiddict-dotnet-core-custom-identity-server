#!/bin/bash

# Scope & Audience Discipline Tests
# This script tests the authorization policies

echo "🛡️  Testing Scope & Audience Discipline"
echo "======================================"

BASE_URL="https://localhost:57679"

echo ""
echo "1. Getting token with api.users.read scope..."
TOKEN_RESPONSE=$(curl -s -X POST "$BASE_URL/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&username=testuser&password=Pass123!&scope=openid profile email api.users.read")

ACCESS_TOKEN=$(echo $TOKEN_RESPONSE | jq -r '.access_token')

if [ "$ACCESS_TOKEN" != "null" ]; then
    echo "✅ Token obtained successfully"
else
    echo "❌ Failed to get token"
    echo $TOKEN_RESPONSE
    exit 1
fi

echo ""
echo "2. Testing users endpoint with correct scope..."
RESPONSE=$(curl -s -w "%{http_code}" -X GET "$BASE_URL/api/protected/users" \
  -H "Authorization: Bearer $ACCESS_TOKEN")

HTTP_CODE="${RESPONSE: -3}"
if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ Users endpoint accessible with correct scope"
else
    echo "❌ Users endpoint returned $HTTP_CODE"
fi

echo ""
echo "3. Testing admin endpoint without admin scope..."
RESPONSE=$(curl -s -w "%{http_code}" -X GET "$BASE_URL/api/protected/admin" \
  -H "Authorization: Bearer $ACCESS_TOKEN")

HTTP_CODE="${RESPONSE: -3}"
if [ "$HTTP_CODE" = "403" ]; then
    echo "✅ Admin endpoint properly protected (403 Forbidden)"
else
    echo "❌ Admin endpoint should return 403 but returned $HTTP_CODE"
fi

echo ""
echo "4. Testing debug endpoint to see claims..."
curl -s -X GET "$BASE_URL/api/protected/debug" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq .

echo ""
echo "5. Getting token with wrong audience (simulated)..."
echo "Note: This test demonstrates audience validation concept"
echo "In practice, audience validation happens at the token validation level"

echo ""
echo "🎯 Scope & Audience Discipline Test Complete"