# ContactList — Specyfikacja techniczna

## Stack

- **Backend**: Clean Architecture, CQRS (MediatR), REST API, C#, ASP.NET Core, FluentValidation, AutoMapper, Serilog, Swagger.
- **Frontend**: Angular 21, TypeScript.
- **Baza danych**: MSSQL, EF Core, Fluent API, code-first.
- **Uwierzytelnianie**: ASP.NET Identity, JWT Bearer, BCrypt dla `Contact.PasswordHash`, PBKDF2 dla hasła użytkownika.
- **Testy**: xUnit, FluentAssertions, Moq.

## Architektura

Clean Architecture + CQRS (MediatR):

```
src/
├── ContactList.Domain/          # Encje, Value Objects, wyjątki domenowe
├── ContactList.Application/     # CQRS (Commands/Queries), DTO, interfejsy, walidacja
├── ContactList.Infrastructure/  # EF Core, repozytoria, Identity, JWT, seeder
└── ContactList.Api/             # Kontrolery REST, middleware, Program.cs
client/                          # SPA (Angular)
tests/                           # xUnit: Domain, Application, Infrastructure
```

## Opis klas i metod (skrót)

### Domain (`ContactList.Domain`)

- **Encje** (`Entities/`): `Contact`, `Category`, `Subcategory`. Wszystkie dziedziczą po `BaseEntity` (Id: `Guid`). `Contact` agreguje wszystkie pola kontaktu (imię, nazwisko, Email, PasswordHash, PhoneNumber, data urodzenia, FK Category/Subcategory).
- **Value Objects** (`ValueObjects/`): `Email` (normalizacja do lowercase, walidacja formatu), `PhoneNumber` (format E.164), `Password` (wymusza złożoność: min. 8 znaków, duża litera, cyfra, znak specjalny).
- **Wyjątki** (`Exceptions/`): `DomainException` — sygnalizuje naruszenie reguły domenowej; mapowany w API na HTTP 400.

### Application (`ContactList.Application`)

- **Features/**: każdy use-case to para `Command`/`Query` + `Handler` (+ `Validator` dla komend).
  - `Contacts/Commands/{CreateContact,UpdateContact,DeleteContact}` — CRUD kontaktów.
  - `Contacts/Queries/{GetContactsList,GetContactById}` — lista + szczegóły.
  - `Categories/Queries/GetCategories`, `Subcategories/Queries/GetSubcategoriesByCategory` — słowniki.
  - `Auth/Commands/{Register,Login}` — rejestracja i logowanie; zwracają `AuthResponseDto` (JWT + czas wygaśnięcia).
- **Common/Behaviours/**: pipeline MediatR — `UnhandledExceptionBehaviour` (log + rethrow), `LoggingBehaviour` (log before/after), `ValidationBehaviour` (agregacja błędów FluentValidation → `ValidationException`).
- **Common/Exceptions/**: `NotFoundException`, `ConflictException`, `ValidationException`, `UnauthorizedException`.
- **Contracts/**: interfejsy wstrzykiwane przez Infrastructure:
  - `Persistence/`: `IUnitOfWork`, `IContactRepository`, `ICategoryRepository`, `ISubcategoryRepository`.
  - `Identity/`: `IUserService` (Register/Login), `AuthRequestDto`, `AuthResponseDto`.
  - `Security/`: `IPasswordHasher` (tylko `Hash` — hasło kontaktu jest write-only, nigdy nie jest weryfikowane przy logowaniu).

### Infrastructure (`ContactList.Infrastructure`)

- **Persistence/ApplicationDbContext** — `DbSet`y dla `Contact`, `Category`, `Subcategory`.
- **Persistence/Configurations/** — mapowania EF.
- **Persistence/Repositories/** — `ContactRepository`, `CategoryRepository`, `SubcategoryRepository`.
- **Persistence/UnitOfWork** — `SaveChangesAsync` opakowuje `DbContext.SaveChangesAsync`.
- **Persistence/Seed/** — `DictionaryIds` (stałe `Guid`y kategorii i podkategorii Służbowy: Szef / Klient / Współpracownik / Partner).
- **Identity/ApplicationUser** — `IdentityUser<Guid>`, oddzielny od `Contact` (logowanie ≠ dane kontaktu).
- **Identity/JwtTokenService** — generuje JWT (HMAC SHA256); claims: `sub`, `email`, `jti`, `iat`.
- **Identity/UserService** — `RegisterAsync`, `LoginAsync` (generyczny komunikat anti-enumeration).
- **Security/BcryptPasswordHasher** — `BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 11)`.

### API (`ContactList.Api`)

- **Controllers/**:
  - `AuthController` — `POST /api/auth/register`, `POST /api/auth/login` (AllowAnonymous).
  - `ContactsController` — `[Authorize]` domyślnie; `GET /api/contacts` i `GET /api/contacts/{id}` oznaczone `[AllowAnonymous]`. `POST/PUT/DELETE` wymagają JWT.
  - `CategoriesController` — `GET /api/categories`, `GET /api/categories/{id}/subcategories`.
- **Common/GlobalExceptionHandler** — `IExceptionHandler`; mapuje `NotFoundException → 404`, `ConflictException → 409`, `ValidationException → 400 (ValidationProblemDetails)`, `DomainException → 400`, `UnauthorizedException → 401`, pozostałe → 500.
- **Program.cs** — konfiguruje Serilog, rejestruje warstwy (`AddApiServices`, `AddApplicationServices`, `AddInfrastructureServices`), przy starcie wykonuje `Database.MigrateAsync()` + `DataSeeder.SeedAsync()`, włącza Swagger (Dev), CORS dla `http://localhost:4200`, `UseAuthentication/UseAuthorization`.

### Frontend (`client/`)

- **core/api/** — serwisy HTTP: `auth.service`, `contacts.service`, `categories.service`.
- **core/auth/** — `auth-state.service` (sygnał zalogowania), `token-storage` (localStorage).
- **core/interceptors/** — `auth.interceptor` (dokleja `Authorization: Bearer`), `error.interceptor` (globalne toasty), `loading.interceptor` (spinner).
- **features/auth/** — `login.page`, `register.page` (reactive forms).
- **features/contacts/** — `contacts-list.page`, `contact-card.component`, `contact-details-modal.component`, `contact-form-modal.component` (dodawanie/edycja).
- **shared/ui/** — `confirm-modal`, `spinner`, `toast`.

### Testy (`tests/`)

- **ContactList.Domain.Tests** — testy encji (`ContactTests`, `CategoryTests`, `SubcategoryTests`) i value objectów (`EmailTests`, `PasswordTests`, `PhoneNumberTests`). Weryfikują niezmienniki w konstruktorach, walidację formatów, rzucane `DomainException`.
- **ContactList.Application.Tests** — testy handlerów (CQRS) per feature: `Features/{Auth,Contacts,Categories,Subcategories}/...` + pipeline behaviours (`Common/Behaviours`) i profili AutoMapper (`Common/Mappings`). Repozytoria i `IUserService` mockowane przez Moq.
- **ContactList.Infrastructure.Tests** — testy integracyjne repozytoriów (`ContactRepositoryTests`, `CategoryRepositoryTests`, `SubcategoryRepositoryTests`) na `Microsoft.EntityFrameworkCore.InMemory`.

## Wykorzystane biblioteki

### Backend (NuGet)

- **MediatR 12.4.1** — CQRS, pipeline behaviours. Ostatnia darmowa wersja (v13+ wymaga licencji komercyjnej).
- **FluentValidation 11.11.0** + **FluentValidation.DependencyInjectionExtensions** — walidacja komend.
- **AutoMapper 13.0.1** — mapowanie encja ↔ DTO. Ostatnia darmowa wersja (v14+ komercyjny).
- **Microsoft.EntityFrameworkCore 10.0.6** + **SqlServer** + **Tools** — ORM + migracje.
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.6** — zarządzanie użytkownikami.
- **Microsoft.AspNetCore.Authentication.JwtBearer 10.0.6**, **Microsoft.IdentityModel.Tokens 8.17.0**, **System.IdentityModel.Tokens.Jwt 8.17.0** — JWT.
- **BCrypt.Net-Next 4.1.0** — hashowanie hasła kontaktu (pole `Contact.PasswordHash`).
- **Serilog.AspNetCore 9.0.0** — logowanie.
- **Swashbuckle.AspNetCore 7.2.0** — Swagger UI z wsparciem JWT.

### Testy

- **xUnit**, **Moq**, **FluentAssertions**, **Microsoft.EntityFrameworkCore.InMemory** (testy repozytoriów).

### Frontend (npm)

- @angular/* 21.x.
- rxjs 7.8.
- typescript.

## Sposób kompilacji i uruchomienia

### Wymagania

- .NET SDK 10
- Node.js 20+, npm 11+
- SQL Server LocalDB

### Backend

Z katalogu głównego repozytorium:

```bash
# przywrócenie paczek + build
dotnet build

# uruchomienie wszystkich testów (Domain + Application + Infrastructure)
dotnet test

# start API (przy pierwszym uruchomieniu automatycznie odpala migracje + seed)
dotnet run --project src/ContactList.Api
```

Domyślne URL-e (patrz `src/ContactList.Api/Properties/launchSettings.json`):

- Swagger UI: `https://localhost:<port>/swagger`
- API root: `https://localhost:<port>/api`


### Frontend

Z katalogu `client/`:

```bash
npm install
npm start          # ng serve → http://localhost:4200
```

Frontend oczekuje API pod adresem skonfigurowanym w `client/src/environments/`.

## Bezpieczeństwo

- Hasła użytkowników aplikacji (logowanie) zarządzane przez ASP.NET Identity — domyślny hasher (PBKDF2), reguły złożoności włączone (wielka/mała litera, cyfra, znak specjalny, min. 8 znaków).
- Hasło w polu `Contact.PasswordHash` hashowane BCrypt (workFactor 11), nigdy nie zwracane w DTO.
- JWT HMAC-SHA256; sekret z `appsettings.Development.json` (w produkcji — `appsettings.Production.json` lub zmienne środowiskowe).
- Walidacja wejścia dwuwarstwowa: FluentValidation + niezmienniki w konstruktorach Domain.
- CORS ograniczony do `http://localhost:4200` w Development.
- Globalny handler wyjątków — nie wycieka stack trace do klienta.
- Komunikat błędu logowania generyczny ("Invalid email or password").

## Dane słownikowe

Seedowane przy starcie aplikacji:

- **Kategorie**: Służbowy, Prywatny, Inny
- **Podkategorie Służbowy**: Szef, Klient, Współpracownik, Partner
- **Kontakty demo** (3 wpisy — żeby lista nie była pusta przy pierwszym uruchomieniu):
  - `Seed_Jan Seed_Kowalski` — Służbowy / Szef
  - `Seed_Anna Seed_Nowak` — Prywatny
  - `Seed_Marek Seed_Sąsiad` — Prywatny
