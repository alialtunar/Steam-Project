using GameService.Controllers;
using GameService.DTOs;
using GameService.Repositories;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("[controller]")]

public class GameController : ControllerBase
{
    private IGameRepository _gameRepository;
    public GameController(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    [HttpPost]
    public async Task<ActionResult> CreateGame([FromForm]GameDTO game)
    {
        var response = await _gameRepository.CreateGame(game);
        return Ok(response);
    }

    [HttpDelete("{gameId}")]
    public async Task<ActionResult> RemoveGame([FromRoute]Guid gameId)
    {
      var response = await _gameRepository.RemoveGame(gameId);
      return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllGames()
    {
        var response = await _gameRepository.GetAllGames();
        return Ok(response);
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult> GetGamesByCategoryId([FromRoute] Guid categoryId)
    {
        var response = await _gameRepository.GetGamesByCategoryId(categoryId);
        return Ok(response);
    }

    [HttpPut("{gameId}")]
    public async Task<ActionResult> UpdateGame(UpdateGameDTO model,[FromRoute] Guid gameId)
    {
       var response = await _gameRepository.UpdateGame(model, gameId);
       return Ok(response); 
    }
}