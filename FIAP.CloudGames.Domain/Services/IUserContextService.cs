using FIAP.CloudGames.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Domain.Services;

public interface IUserContextService
{
    Guid UserId { get; set; }
    TypeUserEnum UserType { get; set; }
}
