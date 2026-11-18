using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Domain.ReadModels;

public sealed class GameListRow
{
    public Guid Id { get; set; }
    public string Title {get; set;}
    public string Description {get; set;}
    public decimal OriginalPrice {get; set;}
    public decimal? PriceWithDiscount {get; set;}
    public DateTime ReleaseDate {get; set;}
}
