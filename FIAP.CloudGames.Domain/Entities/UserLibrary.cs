using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIAP.CloudGames.Domain.Entities;

public class UserLibrary
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid GameId { get; private set; }
    public Game Game { get; private set; } = null!;

    public DateTime AcquisitionDate { get; private set; }

    public decimal PricePaid { get; private set; }

    protected UserLibrary() { }

    public UserLibrary(Guid userId, Guid gameId, decimal pricePaid)
    {
        UserId = userId;
        GameId = gameId;
        PricePaid = pricePaid;
        AcquisitionDate = DateTime.UtcNow;
    }
}
