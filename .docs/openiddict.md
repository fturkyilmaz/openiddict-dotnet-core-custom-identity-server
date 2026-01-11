# OpenIddict Çalışma Yapısı

## 1. Temel Bileşenler
- **OpenIddictApplications**  
  Client uygulamalarının kayıtlarını tutar.  
  Örnek alanlar: `ClientId`, `ClientSecret`, `RedirectUri`.

- **OpenIddictScopes**  
  Tanımlı scope’ları içerir.  
  Örnek: `clientapi`, `postman`.

- **OpenIddictAuthorizations**  
  Kullanıcıya verilen izinleri saklar.  
  Örnek alanlar: `Subject`, `Scopes`.

- **OpenIddictTokens**  
  Üretilen access/refresh token kayıtlarını tutar.  
  Örnek alanlar: `Type`, `Status`, `CreationDate`, `ExpirationDate`.

---

## 2. Token Yaşam Döngüsü
1. **Token Üretimi**  
   - `/connect/token` endpoint’i üzerinden password grant, client credentials veya authorization code flow ile token üretilir.  
   - Access ve refresh token bilgileri `OpenIddictTokens` tablosuna kaydedilir.

2. **Access Token Kullanımı**  
   - API çağrılarında `Authorization: Bearer <token>` header’ı ile erişim sağlanır.  
   - JWT token ise stateless doğrulama yapılır.  
   - Reference token kullanılıyorsa DB kontrolü yapılır.

3. **Refresh Token Kullanımı**  
   - `/connect/token` endpoint’ine `grant_type=refresh_token` ile gönderilir.  
   - Refresh token geçerliyse yeni access token üretilir.

4. **Logout / Revocation**  
   - `IOpenIddictTokenManager.TryRevokeAsync(token)` çağrısı ile token’ın `Status` alanı `revoked` yapılır.  
   - Refresh token revoke edildiğinde tekrar kullanıldığında `invalid_grant` döner.  
   - Access token revoke edilmesi için reference token veya authorization revocation kullanılmalıdır.

---

## 3. Test Senaryosu
- **Password grant ile token alımı** → Access ve refresh token DB’ye kaydedilir.  
- **/auth/me çağrısı** → Token doğrulaması çalışır, kullanıcı bilgisi döner.  
- **Logout-everywhere çağrısı** → Kullanıcının tüm tokenları revoke edilir.  
- **Refresh token denemesi** → `invalid_grant` döner.  
- **Eski access token ile /auth/me** → JWT ise hâlâ geçerli olabilir; reference token kullanılıyorsa 401 döner.

---

## 4. Profesyonel Yaklaşım
- **Reference Token Kullanımı**  
  `options.UseReferenceAccessTokens()` ile access token DB üzerinden doğrulanır ve revoke sonrası geçersiz olur.

- **Authorization Revocation**  
  Token’ları authorization kaydına bağlayarak toplu revoke yapılabilir.

- **Validation Middleware**  
  `UseLocalServer()` ve `UseAspNetCore()` ayarları ile token doğrulama yapılır.

---

## 5. Özet
OpenIddict yapısı, uygulama kimlik doğrulama ve yetkilendirme süreçlerini yönetir.  
Token üretimi, kullanımı ve revoke işlemleri `OpenIddictTokens` tablosu üzerinden takip edilir.  
Profesyonel senaryolarda reference token veya authorization revocation tercih edilmelidir.
