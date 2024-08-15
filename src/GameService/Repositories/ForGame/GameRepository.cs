using AutoMapper;
using Contracts;
using GameService.Base;
using GameService.Data;
using GameService.DTOs;
using GameService.Entities;
using GameService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace GameService.Repositories;


public class GameRepository : IGameRepository
{
  private GameDbContext _context;
  private IMapper _mapper;

  private IFileService _fileService;
  private BaseResponseModel _responseModel;

  private readonly IPublishEndpoint _publishEndpoint;
    public GameRepository(GameDbContext context, IMapper mapper, IFileService fileService, BaseResponseModel responseModel, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
        _responseModel = responseModel;
        _publishEndpoint = publishEndpoint;
    }
    public async Task<BaseResponseModel> CreateGame(GameDTO game)
  {
    if (game.File.Length > 0)
    {
      string videoUrl = await _fileService.UploadVideo(game.File);
      var objDto = _mapper.Map<Game>(game);
      objDto.VideoUrl = videoUrl;
      await _context.Games.AddAsync(objDto);
      await _publishEndpoint.Publish(_mapper.Map<GameCreated>(objDto));


      if (await _context.SaveChangesAsync() > 0)
      {
        _responseModel.IsSuccess = true;
        _responseModel.Message = "Created Game Successfully";
        _responseModel.Data = objDto;
        return _responseModel;
      }
    }

    _responseModel.IsSuccess = false;
    return _responseModel;
  }

  public async Task<BaseResponseModel> RemoveGame(Guid gameId)
  {
    Game game = await _context.Games.FindAsync(gameId);

    if (game is not null)
    {

      _context.Games.Remove(game);
      await _publishEndpoint.Publish<GameDeleted>(new {Id = gameId.ToString()});
      if (await _context.SaveChangesAsync() > 0)
      {
        _responseModel.IsSuccess = true;
        _responseModel.Data = game;
        return _responseModel;
      }

    }

    _responseModel.IsSuccess = false;
    return _responseModel;
  }

  public async Task<BaseResponseModel> GetAllGames()
  {
    List<Game> games = await _context.Games.Include(x => x.Category).Include(x => x.GameImages).ToListAsync();

    if (games is not null)
    {
      _responseModel.Data = games;
      _responseModel.IsSuccess = true;
      return _responseModel;
    }
    _responseModel.IsSuccess = false;
    return _responseModel;
  }

  public async Task<BaseResponseModel> GetGamesByCategoryId (Guid categoryId)
  {
    List<Game> games = await _context.Games.Where(x => x.Category.Id == categoryId).ToListAsync();

    if (games is not null)
    {
      _responseModel.Data = games;
      _responseModel.IsSuccess = true;
      return _responseModel;
    }
    _responseModel.IsSuccess = false;
    return _responseModel;
  }

  public async Task<BaseResponseModel> UpdateGame(UpdateGameDTO game, Guid gameId)
  {
    Game gameObj = await _context.Games.FindAsync(gameId);
    if (gameObj is not null)
    {
      gameObj.Price = game.Price;
      gameObj.MinimumSystemRequirement = game.MinimumSystemRequirement;
      gameObj.GameAuthor = game.GameAuthor;
      gameObj.GameName = game.GameName;
      gameObj.RecommendedSystemRequirement = game.RecommendedSystemRequirement;
      gameObj.GameDescription = game.GameDescription;
      await _publishEndpoint.Publish(_mapper.Map<GameUpdated>(gameObj));
      if (await _context.SaveChangesAsync() > 0)
      {
        _responseModel.IsSuccess = true;
        _responseModel.Data = gameObj;
        return _responseModel;
      }
    }
    _responseModel.IsSuccess = false;

    return _responseModel;


  }
}
