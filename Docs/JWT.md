# JSON Web Token (JWT)

---

W tym poradniku dowiesz się czym jest RBAC i jak go zaimplementować.

---

# Wymagania

- Warto uprzednio przeczytać poradnik [README.md](./README.md)
- Projekt ASP.NET Core

---

# 1. Wprowadzenie

JWT (*JSON Web Token*) to standard służący do bezpiecznego przekazywania informacji pomiędzy klientem a serwerem w postaci podpisanego tokenu.

Token JWT jest wykorzystywany do:

- uwierzytelniania użytkownika,
- autoryzacji żądań API,
- przechowywania claims użytkownika,
- realizacji stateless authentication.

---

# 2. Budowa tokenu JWT

Token JWT składa się z trzech części rozdzielonych ```.```:

```text
HEADER.PAYLOAD.SIGNATURE
```

Przykład:

```text
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9
.
eyJzdWIiOiIxMjMiLCJuYW1lIjoiRnJhbmVrIEtpbW9ubyJ9
.
6B7mWHEjqO4XVqKLlywneWIBfp0309JV95swWTR5qXA
```

---

## 1. Header

Header zawiera informacje o typie tokenu oraz algorytmie podpisu.

Przykład:

```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

---

## 2. Payload

Payload zawiera claims użytkownika.

Przykładowe claims:

```json
{
  "sub": "123",
  "name": "Franek Kimono"
}
```

---

## 3. Signature

Signature służy do weryfikacji integralności tokenu.

Podpis jest tworzony przy użyciu:

- sekretnego klucza,
- algorytmu podpisu,
- danych z header oraz payload.

---

# 3. JWT w projekcie

W projekcie JWT został wykorzystany do:

- uwierzytelniania użytkowników,
- autoryzacji endpointów API,
- przechowywania ról użytkownika,
- przechowywania identyfikatora użytkownika.

---

# 4. Konfiguracja JWT

Konfiguracja znajduje się w pliku:

```text
appsettings.json
```

Przykład:

```json
{
  "ApiSettings": {
    "Issuer": "example-api",
    "Audience": "example-client",
    "AccessTokenExpiryMinutes": 15
  }
}
```

Sekret JWT powinien być przechowywany w secret manager
```bash
dotnet user-secrets init
dotnet user-secrets set "ApiSettings:Secret" "a-string-secret-at-least-256-bits-long"
```
**Pod żadnym pozorem nie powinno się upubliczniać sekretów**

---

# 5. Konfiguracja Authentication

JWT Authentication jest konfigurowane w pliku `Program.cs`.

```csharp
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["ApiSettings:Issuer"],
            ValidAudience = builder.Configuration["ApiSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:Secret"]!))
        };
    })
```

---

# 6. TokenService

Klasa `TokenService` odpowiada za:

- generowanie access tokenów,
- generowanie refresh tokenów,
- konfigurację czasu życia tokenów.

---

## 1. Generowanie tokenu JWT

Przykład generowania tokenu:

```csharp
    public string GenerateToken(User user)
    {
        List<Claim> claims =
        [
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new (ClaimTypes.Email, user.Email)
        ];

        foreach (string role in user.Roles)
        {
            Claim claim = new(ClaimTypes.Role, role);
            claims.Add(claim);
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _config["ApiSettings:Issuer"],
            audience: _config["ApiSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
```

---

## 2. Claims użytkownika

Token JWT przechowuje claims użytkownika.

W projekcie wykorzystywane są:

| Claim | Opis |
|---|---|
| `ClaimTypes.NameIdentifier` | identyfikator użytkownika |
| `ClaimTypes.Name` | nazwa użytkownika |
| `ClaimTypes.Email` | adres email |
| `ClaimTypes.Role` | role użytkownika |

Claims są wykorzystywane podczas autoryzacji endpointów.

---

# 7. Access Token

Access token:

- służy do autoryzacji żądań API,
- posiada ograniczony czas życia,
- jest przesyłany w nagłówku HTTP.

Przykład nagłówka:

```http
Authorization: Bearer TOKEN
```

---

# 8. Refresh Token

Refresh token służy do uzyskania nowego access tokenu bez konieczności ponownego logowania użytkownika.

---

## 1. Mechanizm działania

1. użytkownik loguje się,
2. serwer generuje access token,
3. serwer generuje refresh token,
4. po wygaśnięciu access tokenu klient wysyła refresh token,
5. serwer generuje nowy access token.

---

## 2. Generowanie refresh tokenu

Przykład implementacji:

```csharp
public string GenerateRefreshToken()
{
    byte[] randomNumber = new byte[64];

    using RandomNumberGenerator rng = RandomNumberGenerator.Create();

    rng.GetBytes(randomNumber);

    return Convert.ToBase64String(randomNumber);
}
```

Refresh token jest generowany przy użyciu kryptograficznie bezpiecznego generatora liczb losowych.

---

## 3. Endpoint logowania

Po poprawnym logowaniu użytkownik otrzymuje:

- access token,
- refresh token.

Przykład odpowiedzi:

```json
{
  "accessToken": "JWT_TOKEN",
  "refreshToken": "REFRESH_TOKEN"
}
```

---

## 4. Endpoint refresh

Endpoint:

```text
POST /api/refresh
```

pozwala wygenerować nowy access token przy użyciu refresh tokenu.

---

## 5. Walidacja refresh tokenu

Serwer sprawdza:

- czy refresh token istnieje,
- czy token nie został unieważniony,
- czy token nie wygasł.

Przykład:

```csharp
if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
{
    return Unauthorized();
}
```

---

## 6. Rotacja refresh tokenów

W projekcie zastosowano mechanizm rotacji refresh tokenów.

Po użyciu refresh token:

- poprzedni token zostaje unieważniony,
- generowany jest nowy refresh token.

Mechanizm zwiększa bezpieczeństwo aplikacji.

---

# Zalety JWT

JWT:

- pozwala tworzyć stateless authentication,
- dobrze współpracuje z REST API,
- upraszcza skalowanie aplikacji,
- umożliwia przechowywanie claims użytkownika,
- integruje się z ASP.NET Core Authentication.

---

# Wady JWT

JWT posiada również ograniczenia:

- tokenów nie można łatwo unieważnić,
- duże payloady zwiększają rozmiar tokenu,
- wyciek tokenu umożliwia przejęcie sesji,
- wymaga odpowiedniego zarządzania refresh tokenami.

---

# Podsumowanie

W projekcie JWT został wykorzystany do:

- uwierzytelniania użytkowników,
- autoryzacji endpointów API,
- przechowywania claims użytkownika,
- przechowywania ról użytkownika,
- implementacji refresh tokenów.

Mechanizm JWT stanowi podstawę systemu bezpieczeństwa aplikacji.