using FilterService.Models;
using FilterService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilterService.Controllers;

[ApiController]
[Route("[controller]")]
public class FilterController : ControllerBase
{
    private readonly IFilterGameService _filterService;
    public FilterController(IFilterGameService filterService)
    {
        _filterService = filterService;
    }


    [HttpPost]
    public async Task<ActionResult> FilterGameServ(GameFilterItem filterItem){
        var response = await _filterService.SearchAsync(filterItem);
        return Ok(response);
    }
}