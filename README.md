# Temat 17: Autoryzacja w aplikacjach webowych

## 1. Wybór środowiska programistycznego, języka i frameworku

Do realizacji projektu wybraliśmy technologie zapewniające wysoki poziom bezpieczeństwa "out-of-the-box":

* **Język:** C# 12 / .NET 8
* **Framework:** ASP.NET Core Web API (standard branżowy dla bezpiecznych usług backendowych).
* **Dokumentacja i Testy:** Swagger UI (OpenAPI). Wybrany jako główne narzędzie demonstracyjne, pozwalające na interaktywne sprawdzanie uprawnień bez konieczności budowania osobnego frontendu.
* **IDE:** JetBrains Rider / Visual Studio Code.
* **Kontrola wersji:** Git (GitHub).

## 2. Przegląd metod kontroli dostępu w systemach IT

1. **RBAC (Role-Based Access Control):** Dostęp oparty na rolach (np. Admin, User). Uprawnienia są przypisane do funkcji w systemie, a nie do konkretnych osób.
2. **ABAC (Attribute-Based Access Control / Dynamic-RBAC):** Dostęp oparty na atrybutach (kontekstowy). Decyzja zapada na podstawie cech użytkownika, czasu lub miejsca (np. "tylko w godzinach pracy").
3. **DAC (Discretionary Access Control):** Kontrola uznaniowa. Właściciel danego zasobu sam decyduje, kogo do niego dopuścić (np. udostępnianie folderu).
4. **MAC (Mandatory Access Control):** Kontrola obowiązkowa. Restrykcyjny system etykiet bezpieczeństwa (np. systemy wojskowe), gdzie system odgórnie narzuca dostęp.

**W projekcie zaimplementujemy modele RBAC oraz ABAC.**
### Dodatkowo planujemy przykładową implementacje standardu OAuth2 i/lub SAML

## 3. Implementacja aplikacji do celów demonstracyjnych

Aplikacja implementuje mechanizm autoryzacji oparty na tokenach **JWT (JSON Web Token)**.
* Serwer sprawdza tożsamość użytkownika.
* Wystawia zaszyfrowany token zawierający "Claims" (twierdzenia) o roli i atrybutach użytkownika.
* Przy każdym zapytaniu serwer weryfikuje token i decyduje o przyznaniu dostępu (kod 200 OK) lub odmowie (401 Unauthorized / 403 Forbidden).

## 4. Demonstracja różnych możliwości korzystania z aplikacji

Poniższa tabela przedstawia macierz uprawnień zaimplementowaną w systemie i testowaną za pomocą Swagger UI:

| Funkcjonalność (Endpoint) | Gość (Niezalogowany) | Użytkownik (User) | Administrator (Admin) | Uwagi (Metoda)                       |
| :--- |:--------------------:|:-----------------:|:---------------------:|:-------------------------------------|
| `GET /public` |         200          |        200        |          200          | Brak autoryzacji                     |
| `GET /user-profile` |         401          |        200        |         200         | **RBAC**                             |
| `DELETE /user` |         401          |        403        |          200          | **RBAC** (Tylko Admin)               |
| `POST /company-logs` |         401          |        403        |         200*          | **ABAC** (Tylko Admin + *godz. 8-16) |