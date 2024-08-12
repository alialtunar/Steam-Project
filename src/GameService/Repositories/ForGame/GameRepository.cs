using GameService.Base;
using GameService.DTOs;

namespace GameService.Repositories;


public class GameRepository : IGameRepository
{
    public Task<BaseResponseModel> CreateGame(GameDTO game)
    {
        throw new NotImplementedException();
    }
}
