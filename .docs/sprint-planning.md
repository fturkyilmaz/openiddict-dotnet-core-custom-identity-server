Kurumsal / zero-trust bakış açısıyla, issue’lara **birebir bölünebilir** netlikte yazdım.

https://docs.openiddictcomponents.com/adminui/Claim_Types/Managing_Claimtypes/

---

````md
# 🛡️ Zero-Trust Identity Server – Sprint Planı

Bu doküman, OpenIddict tabanlı custom identity server için
kurumsal güvenlik gereksinimlerini adım adım hayata geçirmek amacıyla hazırlanmıştır.

Amaç:
- Zero-trust API güvenliği
- İnce taneli yetkilendirme
- Token yaşam döngüsünün tam kontrolü
- Denetlenebilir (audit-friendly) auth altyapısı

---

## 🏁 Genel Durum (Mevcut)

✅ Password Grant (custom handler)  
✅ Refresh Token Rotation  
✅ Logout Everywhere  
✅ Token Revocation  
✅ Role-based Authorization (Admin)  
✅ OpenIddict Validation  
✅ Access / Refresh token testleri (curl + bash)

---

# 🚀 Sprint 1 – Authorization Fundamentals (Core Security)

### 🎯 Hedef
API erişimini **scope**, **audience** ve **permission** bazlı kesin kurallarla güvence altına almak.

---

## 1️⃣ Scope & Audience Disiplini

### Hedef
Token’ın:
- doğru API’ye (`aud`)
- doğru yetki alanıyla (`scope`)
eriştiğini garanti altına almak.

### Yapılacaklar
- Scope modeli:
  - `api.users.read`
  - `api.users.write`
  - `api.admin`
- Audience modeli:
  - `shopping-api`
  - `shopping-admin`
- Authorization policy’leri:
  ```csharp
  policy.RequireClaim("scope", "api.users.read");
  policy.RequireClaim("aud", "shopping-api");
````

* OpenIddict token üretiminde `aud` ve `scope` enforce.

### Kabul Kriterleri

* Yanlış `aud` → 401
* Eksik `scope` → 403
* Doğru token → 200

### Testler

* Integration:

  * wrong aud
  * missing scope
  * valid scope + aud

### Commit

```
feat(auth): enforce scope and audience based API authorization
```

---

## 2️⃣ Permission-Based Authorization (Role’süz dünya)

### Hedef

`Role` yerine **permission claim** kullanımı.

> Mikroservis mimarisi için zorunlu.

### Yapılacaklar

* DB:

  * `Permissions`
  * `UserPermissions`
* Token issuance:

  ```json
  "permission": ["users.read", "users.manage"]
  ```
* Custom policy:

  ```csharp
  RequirePermission("users.manage")
  ```

### Kabul Kriterleri

* Permission yok → 403
* Permission eklendi → yeni token ile 200

### Testler

* Unit: PermissionHandler
* Integration: permission ekle/çıkar senaryosu

### Commit

```
feat(auth): add permission-based authorization model
```

---

## 3️⃣ Authorization Test Coverage

### Hedef

Auth regresyonlarını erken yakalamak.

### Yapılacaklar

* Integration test project:

  * Admin → 200
  * User → 403
  * Invalid aud → 401
* In-memory OpenIddict validation setup

### Commit

```
test(auth): add integration tests for authorization policies
```

---

# 🔐 Sprint 2 – Token Security & Abuse Prevention

### 🎯 Hedef

Çalınmış token, brute force ve abuse senaryolarını engellemek.

---

## 4️⃣ Refresh Token Reuse Detection

### Hedef

Aynı refresh token tekrar kullanılırsa:

* tüm session’lar revoke edilsin.

### Yapılacaklar

* Refresh usage tracking
* Reuse tespiti → `logout everywhere`
* Security event üretimi

### Kabul Kriterleri

* Reuse → `invalid_grant`
* Eski access token’lar 401

### Commit

```
feat(security): detect refresh token reuse and revoke sessions
```

---

## 5️⃣ Rate Limiting & Brute Force Protection

### Hedef

`/connect/token` ve login endpoint’lerini korumak.

### Yapılacaklar

* IP + client bazlı rate limit
* Failed login threshold
* Temporary ban

### Kabul Kriterleri

* Limit aşıldı → 429
* Çok hata → geçici blok

### Commit

```
feat(security): add rate limiting and brute force protection
```

---

# 📊 Sprint 3 – Audit, Sessions & Key Management

### 🎯 Hedef

Kurumsal denetlenebilirlik ve operasyonel güvenlik.

---

## 6️⃣ Audit & Security Event Stream

### Hedef

Her kritik auth olayının izlenebilir olması.

### Event’ler

* login_success
* login_failed
* token_issued
* token_revoked
* refresh_reuse
* logout_everywhere

### Commit

```
feat(audit): add structured security event logging
```

---

## 7️⃣ Advanced Session Management

### Hedef

* Max concurrent login
* Device-based logout
* Logout other devices

### Commit

```
feat(session): implement advanced session management
```

---

## 8️⃣ Signing Key Rotation (JWKS)

### Hedef

* Güvenli anahtar rotasyonu
* Zero downtime

### Yapılacaklar

* JWKS endpoint
* `kid` support
* Grace period

### Commit

```
feat(keys): implement JWKS and signing key rotation
```

---

# 🧭 Yol Haritası Özeti

| Sprint   | Odak                                   |
| -------- | -------------------------------------- |
| Sprint 1 | Authorization (scope, aud, permission) |
| Sprint 2 | Token abuse & security                 |
| Sprint 3 | Audit, session, crypto                 |

---

## ✅ Sonuç

Bu plan tamamlandığında sistem:

* Zero-trust uyumlu
* Kurumsal denetimlere hazır
* Mikroservis dostu
* Production-grade identity server

---
