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

    // Put fordi vi opdatere et eksisterende clothes.objekt og ikke opretter et nyt loan-objekt
    [HttpPut]
    public bool BorrowClothes(int BorrowedByMemberId, int itemID, DateTime BorrowedOn, DateTime DueDate)
    {
        return repository.BorrowClothes(BorrowedByMemberId, itemID, BorrowedOn, DueDate);
    }
    
    [HttpPut]
    public bool ReturnClothes(int itemID, int borrowedByMemberId)
    {
        return repository.ReturnClothes(itemID, borrowedByMemberId);
    }
}