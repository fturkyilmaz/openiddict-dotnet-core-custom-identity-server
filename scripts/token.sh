#!/bin/bash

# Kullanıcı bilgileri
USERNAME="admin@test.com"
PASSWORD="Password123!"
SCOPE="openid profile offline_access"
GRANT_TYPE="password"

# Token endpoint
TOKEN_URL="https://localhost:57679/connect/token"

# Token Response
RESPONSE=$(curl -s -X POST $TOKEN_URL \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=$GRANT_TYPE" \
  -d "username=$USERNAME" \
  -d "password=$PASSWORD" \
  -d "scope=$SCOPE")

echo "Response:"
echo $RESPONSE

# Access token'i JSON'dan çıkar
ACCESS_TOKEN=$(echo $RESPONSE | jq -r '.access_token')

echo "Access Token:"
echo $ACCESS_TOKEN
