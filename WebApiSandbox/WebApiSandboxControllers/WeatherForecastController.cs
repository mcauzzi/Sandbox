﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoriesExceptions;
using WebApiSandboxRepositoryInterfaces;
using WebApiSandboxViewModels;

namespace WebApiSandboxControllers;

[ApiController]
[Authorize(Roles ="User" )]
[Route("[controller]", Name = "WeatherForecast")]
public class WeatherForecastController:Controller
{
    public IForecastsRepository Repository { get; }

    public WeatherForecastController(IForecastsRepository repository)
    {
        Repository = repository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<ForecastViewModel>> Get([FromQuery]int rows, [FromQuery]int offset)
    {
        return await Repository.Get(rows, offset);
    }
    
    [HttpGet("city")]
    public async Task<IEnumerable<ForecastViewModel>> GetByCity(int cityId, [FromQuery]int rows, [FromQuery]int offset)
    {
        return await Repository.GetByCity(cityId, rows, offset);
    }
    
    [HttpGet("date")]
    public async Task<IEnumerable<ForecastViewModel>> GetByDate(DateOnly date,[FromQuery]int rows, [FromQuery]int offset)
    {
        return await Repository.GetByDate(date, rows, offset);
    }
    
    [HttpGet("summary")]
    
    public async Task<IEnumerable<ForecastViewModel>> GetBySummary(string summary,[FromQuery] int rows, [FromQuery]int offset)
    {
        return await Repository.GetBySummary(summary, rows, offset);
    }
    
    [HttpPost]
    [Authorize(Roles ="Admin" )]
    public async Task<IActionResult> Post([FromBody] ForecastViewModel forecast)
    {
        try
        {
            await Repository.Add(forecast);
        }
        catch (NotFoundException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
        return Ok();
    }
}
