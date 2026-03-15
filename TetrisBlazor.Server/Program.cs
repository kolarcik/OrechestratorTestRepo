using System.Text.Json;
using TetrisBlazor.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseCors();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

var scoresFile = Path.Combine(AppContext.BaseDirectory, "scores.json");
const int MaxRecords = 100;

List<ScoreEntry> LoadScores()
{
    try
    {
        if (!File.Exists(scoresFile)) return [];
        var json = File.ReadAllText(scoresFile);
        return JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? [];
    }
    catch { return []; }
}

void SaveScores(List<ScoreEntry> scores)
{
    try { File.WriteAllText(scoresFile, JsonSerializer.Serialize(scores, new JsonSerializerOptions { WriteIndented = true })); }
    catch { /* ignore I/O errors */ }
}

app.MapGet("/api/scores", () =>
{
    var scores = LoadScores();
    return scores.OrderByDescending(s => s.Score).Take(10).ToList();
});

app.MapPost("/api/scores", (ScoreSubmission submission) =>
{
    if (string.IsNullOrWhiteSpace(submission.Name))
        return Results.BadRequest(new { error = "name is required" });
    if (submission.Score < 0)
        return Results.BadRequest(new { error = "score must be non-negative" });
    if (submission.Level < 1)
        return Results.BadRequest(new { error = "level must be >= 1" });

    var entry = new ScoreEntry
    {
        Name = submission.Name.Trim()[..Math.Min(20, submission.Name.Trim().Length)],
        Score = submission.Score,
        Level = submission.Level,
        Lines = submission.Lines,
        Date = DateTime.UtcNow.ToString("yyyy-MM-dd")
    };

    var scores = LoadScores();
    scores.Add(entry);
    if (scores.Count > MaxRecords)
        scores = scores.OrderByDescending(s => s.Score).Take(MaxRecords).ToList();

    SaveScores(scores);
    return Results.Created("/api/scores", entry);
});

app.MapFallbackToFile("index.html");

app.Run();

