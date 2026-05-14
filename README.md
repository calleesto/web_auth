# Temat 17: Autoryzacja w aplikacjach webowych

## 1. Wybór środowiska programistycznego, języka i frameworku

Do realizacji projektu wybraliśmy technologie zapewniające wysoki poziom bezpieczeństwa "out-of-the-box" oraz pozwalające na pełną separację warstw (Frontend/Backend):

* **Backend:** C# 12 / .NET 10, ASP.NET Core Web API (standard branżowy dla bezpiecznych usług backendowych).
* **Frontend:** Vanilla HTML/JavaScript z wykorzystaniem Fetch API do obsługi asynchronicznych żądań i nagłówków autoryzacyjnych.
* **Dokumentacja i Testy:** Swagger UI (OpenAPI) dla backendu, interaktywny dashboard w przeglądarce dla frontendu.
* **IDE:** JetBrains Rider / Visual Studio Code.
* **Kontrola wersji:** Git (GitHub).

## 2. Przegląd metod kontroli dostępu w systemach IT

1. **RBAC (Role-Based Access Control):** Dostęp oparty na rolach (np. Admin, User). Uprawnienia są przypisane do funkcji w systemie, a nie do konkretnych osób.
2. **ABAC (Attribute-Based Access Control / Dynamic-RBAC):** Dostęp oparty na atrybutach (kontekstowy). Decyzja zapada na podstawie cech użytkownika, czasu lub miejsca (np. "tylko w godzinach pracy" lub "dostęp tylko do własnych danych").
3. **DAC (Discretionary Access Control):** Kontrola uznaniowa. Właściciel danego zasobu sam decyduje, kogo do niego dopuścić (np. udostępnianie folderu).
4. **MAC (Mandatory Access Control):** Kontrola obowiązkowa. Restrykcyjny system etykiet bezpieczeństwa (np. systemy wojskowe), gdzie system odgórnie narzuca dostęp.

**W projekcie zaimplementowano modele RBAC oraz ABAC.** Dodatkowo zintegrowano standard **OAuth2**, umożliwiając logowanie przy użyciu konta Google (SSO).

## 3. Implementacja aplikacji do celów demonstracyjnych

Aplikacja implementuje mechanizm autoryzacji oparty na tokenach **JWT (JSON Web Token)** oraz plikach cookie (dla integracji z Google).
* Serwer sprawdza tożsamość użytkownika.
* Wystawia zaszyfrowany token JWT zawierający "Claims" (twierdzenia) o roli (np. `admin`) i identyfikatorze użytkownika (`NameIdentifier`).
* Przy każdym zapytaniu serwer weryfikuje token i decyduje o przyznaniu dostępu (**200 OK**) lub odmowie ze względu na brak autentykacji (**401 Unauthorized**) albo brak autoryzacji/uprawnień (**403 Forbidden**).

## 4. Demonstracja różnych możliwości korzystania z aplikacji

Poniższa tabela przedstawia precyzyjną macierz uprawnień zaimplementowaną w systemie. Zwraca ona szczególną uwagę na rozróżnienie błędów uwierzytelniania (401) i autoryzacji (403):

| Funkcjonalność (Endpoint) | Gość (Niezalogowany) | Użytkownik (User) | Administrator (Admin) | Uwagi (Metoda / Wymagania) |
| :--- | :---: | :---: | :---: | :--- |
| `GET api/public` | 200 | 200 | 200 | Brak autoryzacji. Otwarty dostęp dla wszystkich. |
| `GET api/user/{id}` | 401 | **200** (własne ID)<br>**403** (cudze ID) | 200 | **RBAC + ABAC:** Użytkownik ma dostęp tylko do swojego zasobu. Admin ma dostęp do wszystkich. |
| `DELETE api/user/{id}` | 401 | 403 | 200 | **RBAC:** Ścisły wymóg posiadania roli `admin`. |
| `POST api/logs` | 401 | 403 | **200** (godz. 8-16)<br>**403** (inne godz.) | **RBAC + ABAC:** Wymaga roli `admin` **ORAZ** akcji w wyznaczonych godzinach pracy serwera. |
| `POST /login` | 200 | 200 | 200 | Autentykacja tradycyjna: weryfikacja bazy danych i wydanie tokenu JWT. |
| `GET /auth/login-google` | 302 -> 200 | - | - | **OAuth2:** Przekierowanie do serwerów Google i wydanie Claims po poprawnym Callbacku. |
| `POST /logout` | 400 | 200 | 200 | Wymaga aktywnej sesji (tokenu), inaczej serwer zwraca `400 Bad Request`. |