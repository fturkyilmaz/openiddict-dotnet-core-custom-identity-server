# 🔐 Clean Architecture + OpenIddict & User Management

Bu proje, **ASP.NET Core Clean Architecture** yaklaşımını temel alarak **OpenIddict tabanlı kimlik doğrulama** ve **kullanıcı yönetimi** için başlangıç noktası sağlar.  
Amaç, modern uygulamalarda **loosely-coupled**, **DDD uyumlu** ve **SOLID prensiplerine uygun** bir yapı kurarken aynı zamanda **authentication & authorization** süreçlerini entegre etmektir.

---

## 🚀 Özellikler
- **OpenIddict entegrasyonu**: OAuth2 / OpenID Connect protokolleri ile token tabanlı kimlik doğrulama.
- **User Management**: Kullanıcı kayıt, giriş, profil güncelleme, şifre resetleme.
- **Role & Claims Management**: Yetkilendirme için rol ve claim tabanlı kontrol.
- **Clean Architecture Katmanları**:
  - **Core**: Domain modelleri, entity, value object, domain event.
  - **Application (Use Cases)**: CQRS komutları ve sorguları, kullanıcı işlemleri.
  - **Infrastructure**: EF Core, OpenIddict store, repository implementasyonları.
  - **Web**: API endpointleri, kullanıcı yönetim controllerları.
- **Test Projeleri**: Unit ve integration testler.

---

## 📦 Başlangıç

### 1. Template Kurulumu
```bash
dotnet new install Ardalis.CleanArchitecture.Template
dotnet new clean-arch -o AuthProject
cd AuthProject
```

### 2. OpenIddict Paketleri
```bash
dotnet add AuthProject.Infrastructure package OpenIddict.AspNetCore
dotnet add AuthProject.Infrastructure package OpenIddict.EntityFrameworkCore
dotnet add AuthProject.Infrastructure package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

### 3. DbContext Entegrasyonu
📂 `Infrastructure/AppDbContext.cs`
```csharp
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
```

📂 `Infrastructure/Entities/ApplicationUser.cs`
```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
```

---

## 🔑 Authentication Flow
- **Register** → `/api/account/register`
- **Login** → `/api/account/login` → JWT / Access Token döner
- **Refresh Token** → `/api/account/refresh`
- **Role Management** → `/api/account/roles`

---

## 🧪 Migration & DB Update
```bash
dotnet ef migrations add InitialIdentitySetup -p AuthProject.Infrastructure -s AuthProject.Web
dotnet ef database update -p AuthProject.Infrastructure -s AuthProject.Web
```

---

## 🎯 Hedefler
- Clean Architecture ile **kimlik doğrulama** ve **kullanıcı yönetimini** modüler hale getirmek.
- Domain ve Application katmanlarını **UI’den bağımsız** tutmak.
- Enterprise-ready bir **auth & user management boilerplate** sağlamak.

---

## 📚 Daha Fazla
- [OpenIddict Docs](https://documentation.openiddict.com/)
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Clean Architecture Principles](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html)
