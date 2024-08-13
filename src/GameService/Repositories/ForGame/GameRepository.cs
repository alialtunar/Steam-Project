using AutoMapper;
using GameService.Base;
using GameService.Data;
using GameService.DTOs;
using GameService.Entities;
using GameService.Services;

namespace GameService.Repositories;


public class GameRepository : IGameRepository
{
   private GameDbContext _context;
   private IMapper _mapper;

   private IFileService _fileService;
   private BaseResponseModel _responseModel;
    public GameRepository(GameDbContext context, IMapper mapper, IFileService fileService, BaseResponseModel responseModel)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
        _responseModel = responseModel;
    }
    public async Task<BaseResponseModel> CreateGame(GameDTO game)
    {
        if(game.File.Length >  0)
        {
           string videoUrl = await _fileService.UploadVideo(game.File);
           var objDto = _mapper.Map<Game>(game);
           objDto.VideoUrl = videoUrl;
           await _context.Games.AddAsync(objDto);

           if(await _context.SaveChangesAsync() > 0){
             _responseModel.IsSuccess = true;
             _responseModel.Message ="Created Game Successfully";
             _responseModel.Data = objDto;
             return _responseModel;
           }
        }

             _responseModel.IsSuccess = false;
             return _responseModel;
    }
}
