# Google OAuth Authentication

---

W tym poradniku dowiesz się czym jest Google OAuth 2.0 i jak je zaimplementować

---

# Wymagania

- Warto uprzednio przeczytać poradnik [Google API.md](./Google%20API.md)
- Warto uprzednio przeczytać poradnik [JWT.md](./JWT.md)
- Projekt ASP.NET Core

---

# 1. Wprowadzenie

Google OAuth 2.0 pozwala użytkownikom logować się do aplikacji przy użyciu konta Google.

Mechanizm eliminuje konieczność:

- tworzenia własnych haseł,
- przechowywania danych logowania,
- implementowania klasycznego systemu rejestracji.

Po poprawnym uwierzytelnieniu aplikacja otrzymuje dane użytkownika, takie jak:

- adres email,
- nazwa użytkownika,
- identyfikator konta Google.

---

# 2. Przepływ logowania

Proces logowania wygląda następująco:

1. użytkownik wybiera logowanie Google,
2. aplikacja przekierowuje użytkownika do Google,
3. użytkownik loguje się na konto Google,
4. Google przekazuje dane użytkownika do aplikacji,
5. aplikacja generuje JWT,
6. użytkownik otrzymuje access token oraz refresh token.

---

**Poniższa sekcja zakłada zaznajomienie się z tą instrukcją [Google API.md](./Google%20API.md)**

# 3. Konfiguracja Authentication

Konfiguracja Google Authentication znajduje się w pliku `Program.cs`.

```csharp
builder.Services.AddAuthentication(options =>
    {
        ...
    })
    .AddJwtBearer("Bearer", options =>
    {
        ...
    })
    .AddCookie("External")
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.SignInScheme = "External";
    });
```

---

## External Cookie

Google Authentication wykorzystuje tymczasowy mechanizm cookie:

```csharp
.AddCookie("External")
```

Cookie służy do przechowywania danych użytkownika pomiędzy:

- przekierowaniem do Google,
- powrotem użytkownika do aplikacji.

---

# 4. GoogleAuthController

Kontroler `GoogleAuthController` odpowiada za:

- rozpoczęcie logowania Google,
- obsługę callback endpointu,
- generowanie JWT po poprawnym logowaniu.

---

## 1. Endpoint logowania

Endpoint:

```text
GET /auth/login-google
```

przekierowuje użytkownika do Google.

Implementacja:

```csharp
[HttpGet("login-google")]
public IActionResult LoginGoogle()
{
    AuthenticationProperties properties = new()
    {
        RedirectUri = "/auth/google-callback"
    };

    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
}
```

---

###  Challenge

Metoda:

```csharp
Challenge()
```

powoduje przekierowanie użytkownika do strony logowania Google.

Po poprawnym logowaniu użytkownik zostaje przekierowany na:

```text
/auth/google-callback
```

---

## 2. Callback Endpoint

Endpoint:

```text
GET /auth/google-callback
```

obsługuje odpowiedź Google po zakończeniu logowania.

---

## 3. Odczytywanie danych użytkownika

Dane użytkownika są pobierane z claims zwróconych przez Google.

Przykład:

```csharp
AuthenticateResult result = await HttpContext.AuthenticateAsync("External");

if (!result.Succeeded)
{
    return Unauthorized();
}

ClaimsPrincipal principal = result.Principal!;

string email = principal.FindFirst(ClaimTypes.Email)!.Value;
string name = principal.FindFirst(ClaimTypes.Name)!.Value;
```

---

## 4. Tworzenie użytkownika

Po poprawnym logowaniu aplikacja:

1. sprawdza czy użytkownik istnieje,
2. tworzy użytkownika jeśli nie istnieje,
3. generuje token JWT,
4. generuje refresh token.

Przykład:

```csharp
User? user = inMemoryDatabase.GetUserByEmail(email);

if (user == null)
{
    user = inMemoryDatabase.CreateGoogleUser(name, email);
}
```

---

## 5. Generowanie JWT

Po poprawnym uwierzytelnieniu generowany jest token JWT.

---

# 5. Claims użytkownika

Google przekazuje dane użytkownika w postaci claims.

Najczęściej wykorzystywane:

| Claim | Opis |
|---|---|
| `ClaimTypes.Name` | nazwa użytkownika |
| `ClaimTypes.Email` | adres email |
| `ClaimTypes.NameIdentifier` | identyfikator użytkownika |

---

## 6. Zabezpieczenia i dobre praktyki

Podczas implementacji Google OAuth należy:

- używać HTTPS,
- przechowywać `ClientSecret` poza repozytorium,
- weryfikować poprawność callback URL.

---

# 7. Zalety Google Authentication

Logowanie Google:

- upraszcza proces logowania,
- eliminuje konieczność przechowywania haseł,
- zwiększa wygodę użytkownika,
- wykorzystuje zabezpieczenia Google.

---

# 8. Wady Google Authentication

Mechanizm posiada również ograniczenia:

- zależność od zewnętrznego dostawcy,
- konieczność konfiguracji OAuth,
- konieczność zabezpieczenia callback endpointów.
