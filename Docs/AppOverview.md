# Tworzenie nowej aplikacji webowej w ASP.NET

---

W tym poradniku dowiesz się jak wygląda ogólna struktura projektu, oraz jak go uruchomić.

---

# Wymagania
- Zainstalowana platforma [dotnet SDK](https://dotnet.microsoft.com/en-us/download)
- Zainstalowany [Docker](https://docs.docker.com/get-started/get-docker/)

```bash
dotnet --version
docker --version
```

---

# 1. Struktura projektu

Projekt jest podzielony na dwie główne części. Frontend i backend znajdujące się odpowiednio w folderach .

Projekt prezentuje implementację:

- uwierzytelniania JWT,
- logowania przez Google OAuth,
- autoryzacji RBAC,
- autoryzacji ABAC,
- obsługi refresh tokenów,
- modularnej architektury aplikacji.

```text
web_auth/
│
├── Backend/
│   ├── Authorization/
│   │   ├── UserOrAdminHandler.cs
│   │   ├── UserOrAdminRequirement.cs
│   │   ├── WorkingHoursHandler.cs
│   │   └── WorkingHoursRequirement.cs
│   ├── Controllers/
│   │   ├── GoogleAuthController.cs
│   │   └── UsersController.cs
│   ├── Properties/
│   ├── Program.cs
│   ├── TokenService.cs
│   ├── appsettings.json
│   └── Dockerfile
│
├── UserService/
│   ├── InMemoryDatabase.cs
│   ├── LoggedUsers.cs
│   ├── LoginRequest.cs
│   ├── RefreshToken.cs
│   ├── User.cs
│   └── UserDto.cs
│
└── Frontend/
```

---

# 2. Backend

- obsługę endpointów REST API,
- uwierzytelnianie użytkowników,
- autoryzację,
- generowanie tokenów JWT,
- logowanie Google OAuth,
- konfigurację middleware aplikacji.

## 1. Program.cs

Plik `Program.cs` jest punktem wejścia aplikacji.

Znajduje się w nim konfiguracja:

- kontrolerów,
- uwierzytelniania JWT,
- logowania Google OAuth,
- polityk autoryzacji,
- CORS,
- dependency injection,
- middleware aplikacji.

Przykładowa konfiguracja usług:

```csharp
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddCors();
```

## 2. Authorization

Folder `Authorization` zawiera definicje polityk autoryzacji oraz ich handlery.

W projekcie zaimplementowano dwa przykładowe mechanizmy kontroli dostępu:

| Mechanizm | Handler | Opis |
|---|---|---|
| RBAC | `WorkingHoursHandler` | umożliwia dostęp do zasobu wyłącznie w określonych godzinach |
| ABAC | `UserOrAdminHandler` | umożliwia dostęp administratorowi lub właścicielowi zasobu |

Polityki są rejestrowane w pliku `Program.cs`.

Przykład:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOrAdmin", policy =>
    {
        policy.Requirements.Add(new UserOrAdminRequirement());
    });
});
```

## 3. Controllers

Folder `Controllers` zawiera endpointy REST API aplikacji.

### 1. GoogleAuthController.cs

Kontroler odpowiedzialny za logowanie przy użyciu konta Google OAuth.

Obsługiwane endpointy:

| Endpoint | Opis |
|---|---|
| `GET /auth/login-google` | przekierowanie użytkownika do logowania Google |
| `GET /auth/google-callback` | obsługa odpowiedzi po poprawnym logowaniu |

### 2. UsersController.cs

Kontroler odpowiedzialny za operacje związane z użytkownikami.

Przykładowe endpointy:

| Endpoint | Opis |
|---|---|
| `GET /api/public` | publiczny endpoint testowy |
| `POST /api/login` | logowanie użytkownika |
| `POST /api/logout` | wylogowanie użytkownika |
| `POST /api/refresh` | odświeżenie access tokenu |
| `GET /api/user/{id}` | pobranie danych użytkownika |
| `DELETE /api/user/{id}` | usunięcie użytkownika |
| `POST /api/logs` | pobranie informacji o aktywnych użytkownikach |

## 4. TokenService.cs

Klasa `TokenService` odpowiada za:

- generowanie access tokenów JWT,
- generowanie refresh tokenów,
- konfigurację czasu życia tokenów,
- dodawanie claims użytkownika do tokenu.

Przykładowe claims zapisane w tokenie:

```csharp
ClaimTypes.NameIdentifier
ClaimTypes.Name
ClaimTypes.Email
ClaimTypes.Role
```

---

# 3. UserService

Folder `UserService` zawiera klasy związane z użytkownikami oraz przykładową warstwą danych.

---

## Najważniejsze klasy

| Klasa | Opis |
|---|---|
| `User.cs` | model użytkownika |
| `UserDto.cs` | obiekt zwracany przez API |
| `LoginRequest.cs` | model danych logowania |
| `RefreshToken.cs` | model refresh tokenu |
| `LoggedUsers.cs` | obsługa aktywnych użytkowników |
| `InMemoryDatabase.cs` | przykładowa baza danych przechowywana w pamięci |

---

# 4. Konfiguracja aplikacji

Najważniejsze ustawienia aplikacji znajdują się w pliku:

```text
appsettings.json
```

Przykładowa konfiguracja:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ApiSettings": {
    "CorsOrigins": "http://localhost:63343",
    "AccessTokenExpiryMinutes": 5
  },
  "ABACSettings": {
    "startHour": 8,
    "endHour": 16
  }
}
```

---

# 5. Uruchomienie aplikacji

## 1. Przejdź do katalogu głównego
```bash
cd web_auth
```

## 2. Uruchom docker compose

```bash
docker compose up -d
```

---

# 6. Powiązana dokumentacja

Szczegółowe informacje znajdują się w osobnych dokumentach:

| Plik | Opis |
|---|---|
| `JWT.md` | uwierzytelnianie JWT oraz refresh tokeny |
| `GoogleAuth.md` | logowanie Google OAuth |
| `RBAC.md` | autoryzacja oparta o role |
| `ABAC.md` | autoryzacja oparta o atrybuty |