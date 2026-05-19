# Tworzenie nowej aplikacji webowej w ASP.NET

---

W tym poradniku dowiesz się jak wygląda ogólna struktura projektu, oraz jak go uruchomić.

---

# Wymagania
- Zainstalowana platforma [dotnet](https://dotnet.microsoft.com/en-us/download)
- Zainstalowany [docker](https://docs.docker.com/get-started/get-docker/)

---

# 1. Struktura projektu

Projekt jest podzielony na dwie główne części. Frontend i backend znajdujące się odpowiednio w folderach .

---

# 2. Backend

## 1. Program.cs

Plik Program.cs to wejście do programu. W nim są dodawane metody odpowiedzialne za konfigurację całej aplikacji.

## 2. Authorization

W folderze Authorization znajdują się definicje polityk, które dodaje się w pliku Program.cs.
Zdefiniowane zostały dwie przykładowe polityki dla obu mechanizmów kontroli.

| Mechanizm | Polityka            | Opis działania                                                                             |
|-----------|---------------------|--------------------------------------------------------------------------------------------|
| RBA C     | WorkingHoursHandler | Pozwala na pobranie zasobu tylko w wybranych godzinach.                                    |
| ABAC      | UserOrAdminHandler  | Pozwala na pobranie zasobu administratorowi lub użytkownikom do których dany zasób należy. |

## 3. Controllers

W folderze Controllers znajdują się wystawione przez aplikację endpointy.
### 1. GoogleAuthController.cs
### 2. USersController.cs

## 4. TokenService.cs

---

# 3. UserService

Dobrą praktyką jest rozbicie aplikacji na moduły. Ten moduł przechowuje klasy związane z obsługą użytkowników.