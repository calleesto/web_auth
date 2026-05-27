# Autoryzacja w aplikacjach webowych

---

Niniejszy projekt prezentuje przykładową implementację bezpiecznej aplikacji webowej w frameworku ASP.NET Core

Projekt składa się z dwóch części frontend (HTML / CSS / JavaScript) i backend (C# 14 / .NET 10)

### Backend
Backend implementuje kilka mechanizmów uwierzytelniania i autoryzacji w ASP.NET Core:

 - JWT Bearer Authentication
 - Google OAuth 2.0
 - Role Based Access Control (RBAC)
 - Attribute Based Access Control (ABAC)
 - Refresh Tokens

### sDodatkowe elementy

Projekt zawiera również:

 - własne AuthorizationHandler
 - własne AuthorizationRequirement
 - obsługę Claims
 - przykładową konfigurację CORS
 - przykładową bazę danych in-memory

---

W dalszym kroku przeczytaj [AppOverview.md](./Docs/AppOverview.md)