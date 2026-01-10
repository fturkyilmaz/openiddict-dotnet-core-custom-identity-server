## 🧹 Database Table Audit for OpenIddict Auth Integration

### 🎯 Goal
Simplify and align the database schema with OpenIddict-based authentication and authorization. Remove redundant or conflicting tables, and ensure only necessary entities are maintained.

---

### ✅ Required Tables

| Table Name                  | Reason |
|----------------------------|--------|
| `Users`                    | Core user entity for authentication. |
| `UserRoles`                | Maps users to roles. |
| `Roles`                    | Role definitions for authorization. |
| `UserClaims`               | Stores custom claims per user. |
| `OpenIddictApplications`  | Registered clients for OpenIddict. |
| `OpenIddictAuthorizations`| Tracks refresh tokens and consent. |
| `OpenIddictTokens`        | Stores access and refresh tokens. |
| `OpenIddictScopes`        | Defines available scopes. |
| `Clients`                 | Client metadata (ClientId, secret, redirect URIs). |
| `AuditLogs`               | Tracks login, token, and revoke actions. |
| `__EFMigrationsHistory`   | Required by EF Core. |
| `__EFMigrationsLock`      | Required by EF Core. |
| `sqlite_sequence`         | Required by SQLite for auto-increment IDs. |

---

### ⚠️ Optional Tables

| Table Name         | Reason |
|--------------------|--------|
| `ClientScopes`     | Only needed if client-specific scope filtering is required. |
| `Keys`             | Optional if key rotation or external key management is used. |
| `Scopes`           | May be redundant if `OpenIddictScopes` is sufficient. |

---

### ❌ Unnecessary Tables

| Table Name     | Reason |
|----------------|--------|
| `Contributors` | Not related to auth or OpenIddict. |
| `Tokens`       | Conflicts with `OpenIddictTokens`; likely legacy or custom. |

---

### 📌 Action Items

- [ ] Drop unnecessary tables (`Contributors`, `Tokens`).
- [ ] Review optional tables (`ClientScopes`, `Keys`, `Scopes`) and decide based on project needs.
- [ ] Ensure all required OpenIddict tables are seeded and indexed properly.
- [ ] Update EF Core migrations to reflect cleaned schema.

---

### 🧠 Notes
This cleanup ensures that OpenIddict is the **sole authority** for token and auth management, while user identity remains in the custom `Users` table. It improves maintainability, onboarding clarity, and avoids token conflicts.

