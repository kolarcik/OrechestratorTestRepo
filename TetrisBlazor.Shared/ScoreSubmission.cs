using System.ComponentModel.DataAnnotations;

namespace TetrisBlazor.Shared;

public class ScoreSubmission
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Score { get; set; }

    [Range(1, int.MaxValue)]
    public int Level { get; set; }

    [Range(0, int.MaxValue)]
    public int Lines { get; set; }
}
