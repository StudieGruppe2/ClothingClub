using Backend.Repositories;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]   // adressen bliver /clothes
public class ClothesController : ControllerBase
{
    private readonly IClubMemberRepository repository;

    public ClothesController(IClubMemberRepository repository)
    {
        this.repository = repository;
    }

    /// <summary>Henter alt tøj i klubben, på tværs af medlemmer.</summary>
    [HttpGet]
    public List<Clothes> GetAll()
    {
        return repository.GetAllClothes();
    }

    /// <summary>Henter ét stykke tøj ud fra id.</summary>
    [HttpGet("{itemID}")]
    public ActionResult<Clothes> GetById(int itemID)
    {
        Clothes? item = repository.GetClothesById(itemID);
        if (item == null)
        {
            return NotFound();
        }
        return item;
    }

    // Ruten er /clothes/{itemID}/borrow, så den matcher frontend-kaldet.
    // Put fordi vi opdatere et eksisterende clothes.objekt og ikke opretter et nyt loan-objekt
    [HttpPut("{itemID}/borrow")]
    public IActionResult BorrowClothes(int itemID, int memberId, DateTime dueDate)
    {
        //Udlånsdatoen sættes af serveren, så klienten ikke skal sende den.
        bool success = repository.BorrowClothes(itemID, memberId, DateTime.UtcNow, dueDate);

        // Returnerer en rigtig HTTP-status, så frontend kan se om det lykkedes.
        if (!success)
        {
            return BadRequest("Tøjet kunne ikke lånes (findes ikke, er udlånt, eller du ejer det selv).");
        }

        return Ok();
    }

    // rute (/clothes/{itemID}/return).
    [HttpPut("{itemID}/return")]
    public IActionResult ReturnClothes(int itemID, int memberId)
    {
        bool success = repository.ReturnClothes(itemID, memberId);

        if (!success)
        {
            return BadRequest("Tøjet kunne ikke afleveres.");
        }

        return Ok();
    }
    
    
}