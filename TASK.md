# Úkol: Backend – Unit/Integration testy pro Scores API

## Přehled
Vytvoř integration test projekt pro `TetrisBlazor.Server` API endpointy pomocí `WebApplicationFactory`.

## Pracovní adresář
```
/Users/ludek.kolarcik/.copilothub/worktrees/TestTeam/Backend
```

## Krok 1: Vytvoř test projekt

```bash
cd /Users/ludek.kolarcik/.copilothub/worktrees/TestTeam/Backend

dotnet new xunit -n TetrisBlazor.Tests.Server -o TetrisBlazor.Tests.Server --framework net10.0

# Přidej do solution
# Uprav TetrisBlazor.slnx – přidej řádek s test projektem:
# <Project Path="TetrisBlazor.Tests.Server/TetrisBlazor.Tests.Server.csproj" />

# Přidej package references
cd TetrisBlazor.Tests.Server
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package coverlet.collector

# Přidej project reference na Server a Shared
dotnet add reference ../TetrisBlazor.Server/TetrisBlazor.Server.csproj
dotnet add reference ../TetrisBlazor.Shared/TetrisBlazor.Shared.csproj
```

## Krok 2: Uprav TetrisBlazor.Tests.Server.csproj

Csproj musí mít:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.*-*" />
    <PackageReference Include="coverlet.collector" Version="6.*">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\TetrisBlazor.Server\TetrisBlazor.Server.csproj" />
    <ProjectReference Include="..\TetrisBlazor.Shared\TetrisBlazor.Shared.csproj" />
  </ItemGroup>
</Project>
```

## Krok 3: Uprav TetrisBlazor.Server pro testovatelnost

`WebApplicationFactory` potřebuje přístup k entry pointu. Přidej do `TetrisBlazor.Server.csproj`:
```xml
<ItemGroup>
  <InternalsVisibleTo Include="TetrisBlazor.Tests.Server" />
</ItemGroup>
```

A do `Program.cs` na konec souboru přidej:
```csharp
public partial class Program { }
```

## Krok 4: Vytvoř test soubory

### `TetrisBlazor.Tests.Server/ScoresApiTests.cs`

```csharp
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TetrisBlazor.Shared;

namespace TetrisBlazor.Tests.Server;

public class ScoresApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ScoresApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // GET /api/scores

    [Fact]
    public async Task GetScores_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/scores");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetScores_ReturnsJsonArray()
    {
        var response = await _client.GetAsync("/api/scores");
        var scores = await response.Content.ReadFromJsonAsync<List<ScoreEntry>>();
        Assert.NotNull(scores);
    }

    [Fact]
    public async Task GetScores_ReturnsAtMost10()
    {
        var response = await _client.GetAsync("/api/scores");
        var scores = await response.Content.ReadFromJsonAsync<List<ScoreEntry>>();
        Assert.NotNull(scores);
        Assert.True(scores.Count <= 10);
    }

    // POST /api/scores – happy path

    [Fact]
    public async Task PostScore_ValidData_Returns201()
    {
        var submission = new ScoreSubmission { Name = "TEST", Score = 1000, Level = 2, Lines = 10 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostScore_ValidData_ReturnsCreatedEntry()
    {
        var submission = new ScoreSubmission { Name = "PLAYER1", Score = 5000, Level = 5, Lines = 40 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        var entry = await response.Content.ReadFromJsonAsync<ScoreEntry>();
        Assert.NotNull(entry);
        Assert.Equal("PLAYER1", entry.Name);
        Assert.Equal(5000, entry.Score);
        Assert.Equal(5, entry.Level);
        Assert.Equal(40, entry.Lines);
        Assert.False(string.IsNullOrEmpty(entry.Date));
    }

    [Fact]
    public async Task PostScore_NameIsTrimmedAndCapped20Chars()
    {
        var longName = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // 26 chars
        var submission = new ScoreSubmission { Name = longName, Score = 100, Level = 1, Lines = 0 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        var entry = await response.Content.ReadFromJsonAsync<ScoreEntry>();
        Assert.NotNull(entry);
        Assert.Equal(20, entry.Name.Length);
    }

    [Fact]
    public async Task PostScore_NameWithWhitespace_IsTrimmed()
    {
        var submission = new ScoreSubmission { Name = "  ACE  ", Score = 100, Level = 1, Lines = 0 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        var entry = await response.Content.ReadFromJsonAsync<ScoreEntry>();
        Assert.NotNull(entry);
        Assert.Equal("ACE", entry.Name);
    }

    // POST /api/scores – validace

    [Fact]
    public async Task PostScore_EmptyName_Returns400()
    {
        var submission = new ScoreSubmission { Name = "", Score = 100, Level = 1, Lines = 0 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostScore_WhitespaceName_Returns400()
    {
        var submission = new ScoreSubmission { Name = "   ", Score = 100, Level = 1, Lines = 0 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostScore_NegativeScore_Returns400()
    {
        var submission = new ScoreSubmission { Name = "TEST", Score = -1, Level = 1, Lines = 0 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostScore_LevelZero_Returns400()
    {
        var submission = new ScoreSubmission { Name = "TEST", Score = 0, Level = 0, Lines = 0 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostScore_NegativeLines_Returns400()
    {
        var submission = new ScoreSubmission { Name = "TEST", Score = 0, Level = 1, Lines = -1 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostScore_ZeroScore_IsValid()
    {
        var submission = new ScoreSubmission { Name = "NEWBIE", Score = 0, Level = 1, Lines = 0 };
        var response = await _client.PostAsJsonAsync("/api/scores", submission);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

## Krok 5: Uprav TetrisBlazor.slnx

```xml
<Solution>
  <Project Path="TetrisBlazor.Client/TetrisBlazor.Client.csproj" />
  <Project Path="TetrisBlazor.Server/TetrisBlazor.Server.csproj" />
  <Project Path="TetrisBlazor.Shared/TetrisBlazor.Shared.csproj" />
  <Project Path="TetrisBlazor.Tests.Server/TetrisBlazor.Tests.Server.csproj" />
</Solution>
```

## Krok 6: Spusť testy

```bash
cd /Users/ludek.kolarcik/.copilothub/worktrees/TestTeam/Backend
dotnet test TetrisBlazor.Tests.Server/TetrisBlazor.Tests.Server.csproj --logger "console;verbosity=normal"
```

Všechny testy musí projít zeleně. Pokud ne, oprav problémy.

## Krok 7: Git commit

```bash
git add .
git commit -m "test: add integration tests for Scores API

- WebApplicationFactory-based integration tests
- GET /api/scores: returns 200, JSON array, max 10 items
- POST /api/scores: valid data 201, name trim/cap, all validation 400 cases

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
```

Do NOT push.
