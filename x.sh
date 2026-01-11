#!/bin/bash

BASE_DIR="src/ShoppingProject.UseCases/Users/Commands"
SOURCE_FILE="src/ShoppingProject.UseCases/Users/AuthHandlers.cs"

# Klasörleri oluştur
mkdir -p $BASE_DIR/Me
mkdir -p $BASE_DIR/RevokeToken
mkdir -p $BASE_DIR/RefreshToken
mkdir -p $BASE_DIR/LogoutEverywhere
mkdir -p $BASE_DIR/ChangePassword
mkdir -p $BASE_DIR/ForgotPassword
mkdir -p $BASE_DIR/ResetPassword
mkdir -p $BASE_DIR/VerifyEmail
mkdir -p $BASE_DIR/ResendVerification
mkdir -p $BASE_DIR/TwoFactor

# MeQuery ve Handler
awk '/MeQuery/{flag=1} /RevokeTokenCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/Me/MeQuery.cs
awk '/MeQueryHandler/{flag=1} /RevokeTokenCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/Me/MeQueryHandler.cs

# RevokeToken
awk '/RevokeTokenCommand/{flag=1} /RefreshTokenCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/RevokeToken/RevokeTokenCommand.cs
awk '/RevokeTokenHandler/{flag=1} /RefreshTokenCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/RevokeToken/RevokeTokenHandler.cs

# RefreshToken
awk '/RefreshTokenCommand/{flag=1} /LogoutEverywhereCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/RefreshToken/RefreshTokenCommand.cs
awk '/RefreshTokenHandler/{flag=1} /LogoutEverywhereCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/RefreshToken/RefreshTokenHandler.cs

# LogoutEverywhere
awk '/LogoutEverywhereCommand/{flag=1} /ChangePasswordCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/LogoutEverywhere/LogoutEverywhereCommand.cs
awk '/LogoutEverywhereHandler/{flag=1} /ChangePasswordCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/LogoutEverywhere/LogoutEverywhereHandler.cs

# ChangePassword
awk '/ChangePasswordCommand/{flag=1} /ForgotPasswordCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/ChangePassword/ChangePasswordCommand.cs
awk '/ChangePasswordHandler/{flag=1} /ForgotPasswordCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/ChangePassword/ChangePasswordHandler.cs

# ForgotPassword
awk '/ForgotPasswordCommand/{flag=1} /ResetPasswordCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/ForgotPassword/ForgotPasswordCommand.cs
awk '/ForgotPasswordHandler/{flag=1} /ResetPasswordCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/ForgotPassword/ForgotPasswordHandler.cs

# ResetPassword
awk '/ResetPasswordCommand/{flag=1} /VerifyEmailCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/ResetPassword/ResetPasswordCommand.cs
awk '/ResetPasswordHandler/{flag=1} /VerifyEmailCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/ResetPassword/ResetPasswordHandler.cs

# VerifyEmail
awk '/VerifyEmailCommand/{flag=1} /ResendVerificationCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/VerifyEmail/VerifyEmailCommand.cs
awk '/VerifyEmailHandler/{flag=1} /ResendVerificationCommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/VerifyEmail/VerifyEmailHandler.cs

# ResendVerification
awk '/ResendVerificationCommand/{flag=1} /Enable2FACommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/ResendVerification/ResendVerificationCommand.cs
awk '/ResendVerificationHandler/{flag=1} /Enable2FACommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/ResendVerification/ResendVerificationHandler.cs

# TwoFactor
awk '/Enable2FACommand/{flag=1} /}/ {print; if(flag){flag=0}}' $SOURCE_FILE > $BASE_DIR/TwoFactor/Enable2FACommand.cs
awk '/Enable2FAHandler/{flag=1} /Disable2FACommand/{flag=0} flag' $SOURCE_FILE > $BASE_DIR/TwoFactor/Enable2FAHandler.cs
awk '/Disable2FACommand/{flag=1} /}/ {print; if(flag){flag=0}}' $SOURCE_FILE > $BASE_DIR/TwoFactor/Disable2FACommand.cs
awk '/Disable2FAHandler/{flag=1} /}/ {print; if(flag){flag=0}}' $SOURCE_FILE > $BASE_DIR/TwoFactor/Disable2FAHandler.cs

echo "✅ Command ve handler dosyaları oluşturuldu ve içerikleri kopyalandı."
