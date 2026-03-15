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
