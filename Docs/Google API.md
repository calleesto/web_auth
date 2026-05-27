# Konfiguracja logowania Google OAuth2 dla aplikacji ASP.NET Core Web API.

---

W tym poradniku dowiesz się jak stworzyć i skonfigurować nowy projekt w celu uruchmienia Google OAuth API.

---

# Wymagania

- Konto Google
- Projekt ASP.NET Core

---

# 1. Utworzenie projektu w Google Cloud

Wejdź na [stronę google cloud](https://console.cloud.google.com/):
![Main page](./assets/GoogleCloudMainPage.png "Main page")

## Kroki

1. Kliknij **Wybierz projekt** (Select Project)
2. Kliknij **Nowy projekt** (New Project)
   ![Select page](./assets/GoogleCloudSelectProject.png "Select project")
3. Podaj nazwę projektu i ewentualnie dodaj nazwę organizacji
   ![Select page](./assets/GoogleCloudNewProject.png "New project")
4. Kliknij **Utwórz** (Create)

---

# 2. Włączenie Google OAuth API i konfiguracja projektu

Przejdź na [stronę protokołu OAuth](https://console.cloud.google.com/auth/overview):
![Select page](./assets/GoogleCloudOAuth.png "OAuth")

Następnie kliknij **Rozpocznij** (Get started):

![Select page](./assets/GoogleCloudOAuthConfig.png "OAuth config")

## Uzupełnij pola

### Informacje o aplikacji (App information)

**Nazwa aplikacji** (App name)\
**Adres e-mail dla użytkowników potrzebujących pomocy** (User Support email)

### Odbiorcy (Audience)

**Z zewnątrz** (External), by dowolny użytkownik mógł się zalogować do naszej alpikacji.

### Dane kontaktowe (Contact information)

**Adresy e-mail** (Email addresses)

### Zakończ (Finish)

**Zaakceptuj zasady użytkowania** (Agree to user data policy)

Na koniec kliknij **Utwórz** (Create)

---

Zostaniesz przeniesiony na stronę [protokołu OAuth](https://console.cloud.google.com/auth/overview)
![Select page](./assets/GoogleCloudOAuthOverview.png "OAuth overview")

1. Kliknij **Utwórz klienta OAuth** (Create OAuth client):
2. **Utwórz identyfikator klienta OAuth** (Create OAuth client ID)
   ![Select page](./assets/GoogleCloudOauthConfigId.png "OAuth overview")
W tym przypadku należy wybrać **Aplikacja internetowa** (Web app)

---

# 5. Dodanie Redirect URI

## Autoryzowane źródła JavaScriptu (Authorized JavaScript origins)

Dodaj:

```text
https://adres:Port
```

## Autoryzowane identyfikatory URI przekierowania (Authorized redirect URIs)

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

# 8. Najczęstsze błędy podczas działania aplikacji związane z Google OAuth API

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

Przejdź na [stronę zarządzania zasobami](https://console.cloud.google.com/cloud-resource-manager) zaznacz wybrany projekt i usuń go.

---

W dalszym kroku przeczytaj [GoogleAuth.md](./GoogleAuth.md)