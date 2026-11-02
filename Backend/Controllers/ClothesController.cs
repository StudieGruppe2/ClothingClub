using Backend.Repositories;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]   // adressen bliver /clothes
public class ClothesController : ControllerBase
{
    private readonly IClothesRepository repository;

    public ClothesController(IClothesRepository repository)
    {
        this.repository = repository;
    }

    /// <summary>Henter alt tøj.</summary>
    [HttpGet]
    public List<Clothes> GetAll()
    {
        return repository.GetAll();
    }

    /// <summary>Henter ét stykke tøj ud fra id.</summary>
    [HttpGet("{itemId}")]
    public ActionResult<Clothes> GetById(int itemId)
    {
        Clothes? item = repository.GetById(itemId);
        if (item == null)
        {
            return NotFound();
        }
        return item;
    }

    /// <summary>Opretter et nyt stykke tøj.</summary>
    [HttpPost]
    public Clothes Create(Clothes item)
    {
        repository.Add(item);
        return item;
    }

    /// <summary>Sletter et stykke tøj.</summary>
    [HttpDelete("{itemId}")]
    public IActionResult Delete(int itemId)
    {
        if (repository.Remove(itemId))
        {
            return Ok();
        }
        return NotFound();
    }
}