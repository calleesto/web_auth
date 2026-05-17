# Konfiguracja logowania Google OAuth2 dla aplikacji ASP.NET Core Web API.

---

# Wymagania

- Konto Google
- Projekt ASP.NET Core

---

# 1. Utworzenie projektu w Google Cloud

Wejdź na [stronę google cloud](https://console.cloud.google.com/):
![Main page](./assets/GoogleCloudMainPage.png "Main page")

## Kroki

1. Kliknij **Select Project** (wybierz projekt)
2. Kliknij **New Project** (nowy projekt)
   ![Select page](./assets/GoogleCloudSelectProject.png "Select project")
3. Podaj nazwę projektu i ewentualnie dodaj nazwę organizacji
   ![Select page](./assets/GoogleCloudNewProject.png "New project")
4. Kliknij **Create** (utwórz)

---

# 2. Włączenie Google OAuth API

Przejdź na [stronę protokołu OAuth](https://console.cloud.google.com/auth/overview):
![Select page](./assets/GoogleCloudOAuth.png "OAuth")

Następnie kliknij **Get started** (Rozpocznij):

---

# 3. Konfiguracja projektu

![Select page](./assets/GoogleCloudOAuthConfig.png "OAuth config")

## Uzupełnij pola

### App information (Informacje o aplikacji)

**App name** (Nazwa aplikacji)\
**User Support email** (Adres e-mail dla użytkowników potrzebujących pomocy)

### Audience (Odbiorcy)

**External** (Z zewnątrz)

### Contact information (Dane kontaktowe)

**Email addresses** (Adresy e-mail)

### Finish (Zakończ)

**Agree to user data policy** (Zaakceptuj zasady użytkowania)

Na koniec kliknij **Create** (Utwórz)

---

# 4. Utworzenie OAuth Client ID

Zostaniesz przeniesiony na stronę [protokołu OAuth](https://console.cloud.google.com/auth/overview)
![Select page](./assets/GoogleCloudOAuthOverview.png "OAuth overview")

## Kroki

1. Kliknij **Create OAuth client** (Utwórz klienta OAuth):
2. **Create OAuth client ID** (Utwórz identyfikator klienta OAuth)
   ![Select page](./assets/GoogleCloudOauthConfigId.png "OAuth overview")
W tym przypadku należy wybrać **Web app** (Aplikacja internetowa)

---

# 5. Dodanie Redirect URI

## Authorized JavaScript origins (Autoryzowane źródła JavaScriptu)

Dodaj:

```text
https://adres:Port
```

## Authorized redirect URIs (Autoryzowane identyfikatory URI przekierowania)

Dodaj:

```text
https://adres:Port/signin-google
```

> `/signin-google` jest wymagane przez ASP.NET Core Google middleware.

![Select page](./assets/GoogleCloudOauthClient.png "OAuth overview")

---

# 6. Pobranie Client ID i Client Secret

Po utworzeniu OAuth Client otrzymasz:

```text
Client ID
Client Secret
```

Przykład:

```text
Client ID:
123456789-abc.apps.googleusercontent.com

Client Secret:
GOCSPX-xxxxxxxx
```

---

# 7. Konfiguracja Secret Manager (zalecane)

Po otrzymaniu sekretów dodaj je do swojej aplikacji\
**Pod żadnym pozorem nie powinno się upubliczniać sekretów**

## Inicjalizacja secret manager

```bash
dotnet user-secrets init
```

## Dodanie ClientId

```bash
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
```

## Dodanie ClientSecret

```bash
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
```

---

# 8. Najczęstsze błędy

---

## Error 400: redirect_uri_mismatch

### Przyczyna

Niepoprawny redirect URI w Google Cloud.

### Rozwiązanie

Dodaj dokładnie:

```text
https://adres:Port/signin-google
```

---

## Access blocked: This app’s request is invalid

### Przyczyna

- Brak OAuth Consent Screen
- Brak testowego użytkownika
- Niepoprawny Client ID
- Niepoprawny redirect URI

### Rozwiązanie

Sprawdź czy sekrety są aktualne/poprawne.

---

## Failed to determine the https port for redirect

### Przyczyna

`UseHttpsRedirection()` bez konfiguracji HTTPS.

### Rozwiązanie

Skonfiguruj HTTPS lub

Usuń (niewskazane):

```csharp
app.UseHttpsRedirection();
```

# 9. Usuwanie aplikacji

Przejdź na [stronę zarządzania zasobami](https://console.cloud.google.com/auth/overview) zaznacz wybrany projekt i usuń go.