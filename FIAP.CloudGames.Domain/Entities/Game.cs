using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Domain.Entities;

public class Game
{
    [Key]
    public Guid Id { get; private set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
