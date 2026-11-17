# WebsiteAPI

Backend API voor mijn portfoliosite met CRUD-functionaliteit voor **Projects** en **Blogposts**.

## Functionaliteit

De API bevat:

- publieke GET-routes voor projecten en blogposts;
- registratie en login met ASP.NET Core Identity;
- Bearer-token-authenticatie;
- rollen `Editor` en `Admin`;
- policy `CanEditProjects`;
- beveiligde schrijf-routes;
- DTO's voor request- en responsegegevens;
- EF Core met PostgreSQL;
- HTTP-integratietests met xUnit;
- een aparte testdatabase voor integratietests.

## Vereisten

- .NET 10 SDK;
- PostgreSQL;
- PowerShell of een andere terminal.

## Project starten

Start de API:

```powershell
dotnet run
```

De API is daarna bereikbaar via het adres dat in de terminal wordt getoond, bijvoorbeeld:

```text
http://localhost:5224
```

Swagger staat op:

```text
http://localhost:5224/swagger/index.html
```

## PostgreSQL-databases

De applicatie gebruikt de normale database:

```text
websiteapi
```

De integratietests gebruiken een aparte database:

```text
websiteapi_tests
```

## Identity, gebruikers en rollen

Identity wordt geregistreerd in `Program.cs` met:

```csharp
builder.Services.AddIdentityApiEndpoints<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
```

De Identity-routes staan onder `/auth`.

### Beschikbare testgebruikers

De `DbSeeder` maakt de vaste gebruikers en rollen aan wanneer de API start.

| Gebruiker | E-mail | Wachtwoord | Rol |
|---|---|---|---|
| Gewone gebruiker | `joy@example.com` | `Joy123!` | geen rol |
| Editor | `editor@websiteapi.com` | `Editor123!` | `Editor` |
| Admin | `admin@websiteapi.com` | `Admin123!` | `Admin` |

De policy staat beide schrijversrollen toe:

```csharp
policy.RequireRole("Editor", "Admin")
```

Een gewone gebruiker is wel ingelogd, maar krijgt geen schrijfrechten.
