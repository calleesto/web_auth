# Authorization in Web Applications

## 1. Choice of Development Environment, Language, and Framework

For this project, we chose technologies that provide a high level of security "out-of-the-box" and allow for a full separation of layers (Frontend/Backend):

* **Backend:** C# 12 / .NET 10, ASP.NET Core Web API (the industry standard for secure backend services).
* **Frontend:** HTML / CSS / JavaScript utilizing the Fetch API to handle asynchronous requests and authorization headers.

## 2. Overview of Access Control Methods in IT Systems

1. **RBAC (Role-Based Access Control):** Access based on roles (e.g., Admin, User). Permissions are assigned to functions in the system, rather than to specific individuals.
2. **ABAC (Attribute-Based Access Control / Dynamic-RBAC):** Contextual, attribute-based access. Decisions are made based on user characteristics, time, or location (e.g., "only during working hours" or "access only to personal data").
3. **DAC (Discretionary Access Control):** Discretionary control. The owner of a specific resource decides who is granted access to it (e.g., sharing a file folder).
4. **MAC (Mandatory Access Control):** Mandatory control. A restrictive system of security labels (e.g., military systems) where the system dictates access from the top down.

**This project implements the RBAC and ABAC models.** Additionally, the **OAuth2** standard has been integrated, enabling Single Sign-On (SSO) using a Google account.

## 3. Implementation of the Application for Demonstration Purposes

The application implements an authorization mechanism based on **JWT (JSON Web Tokens)** and cookies (for Google integration).
* The server verifies the user's identity.
* It issues an encrypted JWT token containing "Claims" about the user's role (e.g., `admin`) and identifier (`NameIdentifier`).
* With every request, the server verifies the token and decides whether to grant access (**200 OK**) or deny it due to lack of authentication (**401 Unauthorized**) or lack of authorization/permissions (**403 Forbidden**).

## 4. Demonstration of Application Usage Scenarios

The table below presents the precise permission matrix implemented in the system. It pays special attention to the distinction between authentication errors (401) and authorization errors (403):

| Functionality (Endpoint) | Guest (Not logged in) | User | Administrator (Admin) | Notes (Method / Requirements) |
| :--- | :---: | :---: | :---: | :--- |
| `GET api/public` | 200 | 200 | 200 | No authorization required. Open access for everyone. |
| `GET api/user/{id}` | 401 | **200** (own ID)<br>**403** (other ID) | 200 | **RBAC + ABAC:** User only has access to their own resource. Admin has access to all. |
| `DELETE api/user/{id}` | 401 | 403 | 200 | **RBAC:** Strict requirement of the `admin` role. |
| `POST api/logs` | 401 | 403 | **200** (hours 8-16)<br>**403** (other hours) | **RBAC + ABAC:** Requires the `admin` role **AND** the action must occur during designated server working hours. |
| `POST /login` | 200 | 200 | 200 | Traditional authentication: database verification and JWT issuance. |
| `GET /auth/login-google` | 302 -> 200 | - | - | **OAuth2:** Redirect to Google servers and issuance of Claims after a successful Callback. |
| `POST /logout` | 400 | 200 | 200 | Requires an active session (token), otherwise the server returns `400 Bad Request`. |