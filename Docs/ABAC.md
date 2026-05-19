# Attribute-Based Access Control (ABAC)

---

W tym poradniku dowiesz się czym jest ABAC i jak go zaimplementować.

---

# 1. Wprowadzenie

ABAC (*Attribute-Based Access Control*) to mechanizm kontroli dostępu oparty o atrybuty użytkownika, zasobu oraz kontekstu żądania.

W przeciwieństwie do RBAC, gdzie dostęp zależy wyłącznie od roli użytkownika, ABAC pozwala definiować bardziej szczegółowe reguły autoryzacji.

Przykładowe atrybuty wykorzystywane w ABAC:

- identyfikator użytkownika,
- właściciel zasobu,
- adres IP,
- godzina wykonania żądania,
- typ zasobu,
- stan użytkownika.

---

# 2. Implementacja ABAC w projekcie

W projekcie zaimplementowano politykę:

```csharp
UserOrAdmin
```

Polityka pozwala uzyskać dostęp do zasobu gdy:

- użytkownik posiada rolę `admin`,
- lub użytkownik jest właścicielem zasobu.

---

## 1. Rejestracja polityki

Polityka jest rejestrowana w pliku `Program.cs`.

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOrAdmin", policy =>
    {
        policy.Requirements.Add(new UserOrAdminRequirement());
    });
});
```

---

## 2. Requirement

Requirement definiuje wymaganie wymagane przez politykę autoryzacji.

```csharp
using Microsoft.AspNetCore.Authorization;

namespace Backend.Authorization;

public class UserOrAdminRequirement : IAuthorizationRequirement;
```

---

## 3. Authorization Handler

Logika autoryzacji znajduje się w klasie:

```csharp
UserOrAdminHandler
```

Handler sprawdza:

1. czy użytkownik jest zalogowany,
2. czy użytkownik posiada rolę `admin`,
3. czy identyfikator użytkownika zgadza się z identyfikatorem zasobu.

---

## 4. Implementacja handlera

```csharp
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Backend.Authorization;

public class UserOrAdminHandler : AuthorizationHandler<UserOrAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOrAdminRequirement requirement)
    {
        if (!context.User.Identity!.IsAuthenticated)
        {
            return Task.CompletedTask;
        }

        if (context.User.IsInRole("admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        Claim? userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        string? routeId = httpContext.Request.RouteValues["id"]?.ToString();

        if (routeId == null)
        {
            return Task.CompletedTask;
        }

        if (userIdClaim.Value == routeId)
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
// GET api/user/1
[HttpGet("user/{id:int}")]
[Authorize(Policy = "UserOrAdmin")]
public IActionResult Get(int id)
{
    User user = inMemoryDatabase.GetUserById(id);
    UserDto userDto = new(user);
    return Ok(userDto);
}
```

---

# 3. Jak działa autoryzacja

Przykład działania:

| Użytkownik | Żądanie | Wynik |
|---|---|---|
| admin | `/api/user/1` | dostęp przyznany |
| user id=1 | `/api/user/1` | dostęp przyznany |
| user id=2 | `/api/user/1` | dostęp zabroniony |

---

# 4. Zalety ABAC

Mechanizm ABAC pozwala:

- definiować bardziej szczegółowe reguły dostępu,
- kontrolować dostęp do konkretnych zasobów,
- uwzględniać kontekst żądania,
- implementować logikę właściciela zasobu,
- tworzyć dynamiczne polityki bezpieczeństwa.

---

# 5. Podsumowanie

W projekcie ABAC został wykorzystany do kontroli dostępu do zasobów użytkownika.

Mechanizm sprawdza:

- rolę użytkownika,
- identyfikator użytkownika,
- identyfikator zasobu z adresu URL.

Dzięki temu użytkownik może uzyskać dostęp wyłącznie do własnych danych, chyba że posiada uprawnienia administratora.