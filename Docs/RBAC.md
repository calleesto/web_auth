# Role-Based Access Control (RBAC)

---

W tym poradniku dowiesz się czym jest RBAC i jak go zaimplementować.

---

## 1. Wprowadzenie

RBAC (*Role-Based Access Control*) to mechanizm kontroli dostępu oparty o role użytkowników.

Każdy użytkownik może posiadać jedną lub wiele ról, które definiują jego uprawnienia w systemie.

Przykładowe role:

- `admin`
- `user`
- `moderator`

Dostęp do zasobów jest przyznawany na podstawie przypisanych ról.

---

# 2. Implementacja RBAC w projekcie

W projekcie zaimplementowano politykę:

```csharp
AdminWorkingHours
```

Polityka pozwala administratorowi wykonywać operacje wyłącznie w określonych godzinach.

---

## 1. Rejestracja polityki

Polityka jest rejestrowana w pliku `Program.cs`.

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminWorkingHours", policy =>
    {
        policy.Requirements.Add(new WorkingHoursRequirement());
    });
});
```

---

## 2. Requirement

Requirement definiuje wymaganie wymagane przez politykę autoryzacji.

```csharp
using Microsoft.AspNetCore.Authorization;

namespace Backend.Authorization;

public class WorkingHoursRequirement : IAuthorizationRequirement;
```

---

## 3. Authorization Handler

Logika autoryzacji znajduje się w klasie:

```csharp
WorkingHoursHandler
```

Handler sprawdza:

1. czy użytkownik posiada rolę `admin`,
2. czy żądanie zostało wykonane w określonych godzinach.

---

## 4. Implementacja handlera

```csharp
using Microsoft.AspNetCore.Authorization;

namespace Backend.Authorization;

public class WorkingHoursHandler : AuthorizationHandler<WorkingHoursRequirement>
{
    private readonly IConfiguration _configuration;

    public WorkingHoursHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkingHoursRequirement requirement)
    {
        if (!context.User.IsInRole("admin"))
        {
            return Task.CompletedTask;
        }

        int startHour = _configuration.GetValue<int>("ABACSettings:StartHour");
        int endHour = _configuration.GetValue<int>("ABACSettings:EndHour");

        int now = DateTime.UtcNow.Hour;

        if (now >= startHour && now <= endHour)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

---

## 5. Użycie polityki

Polityka może zostać użyta przy endpointach API.

Przykład:

```csharp
[HttpPost("logs")]
[Authorize(Policy = "AdminWorkingHours")]
public IActionResult Logs()
{
    return Ok(_loggedUsers.GetStatus());
}
```

---

# 3. Jak działa autoryzacja

Przykład działania:

| Użytkownik | Godzina | Wynik |
|---|---|---|
| admin | 10:00 | dostęp przyznany |
| admin | 20:00 | dostęp zabroniony |
| user | 10:00 | dostęp zabroniony |

---

# 4. Zalety RBAC

Mechanizm RBAC:

- upraszcza zarządzanie uprawnieniami,
- pozwala grupować użytkowników według ról,
- integruje się z ASP.NET Core Authorization,
- umożliwia łatwe zabezpieczanie endpointów,
- poprawia czytelność kodu.

---

# 5. Ograniczenia RBAC

RBAC dobrze sprawdza się przy prostych systemach uprawnień, jednak może być niewystarczający gdy:

- dostęp zależy od właściciela zasobu,
- wymagane są dynamiczne reguły,
- autoryzacja zależy od kontekstu żądania.

W takich przypadkach stosuje się ABAC.

---

# 6. Podsumowanie

W projekcie RBAC został wykorzystany do:

- kontroli dostępu administratora,
- zabezpieczania endpointów API,
- implementacji polityki dostępu czasowego,
- przechowywania ról użytkownika w tokenie JWT.

Mechanizm pozwala łatwo definiować dostęp do zasobów na podstawie ról użytkownika.