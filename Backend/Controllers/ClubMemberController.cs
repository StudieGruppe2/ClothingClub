using Backend.Repositories;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]   // adressen bliver /clubmember
public class ClubMemberController : ControllerBase
{
    // Dependency Inversion: Controlleren afhænger af interfacet (kontrakt)
    // i stedet for Memory eller MongoDB, så vi kan skifte repository
    // uden at ændre controllerens kode.
    private readonly IClubMemberRepository repository;

    public ClubMemberController(IClubMemberRepository repository)
    {        
        // Dependency Injection: Det valgte repository modtages udefra - gennem constructoren i stedet for at blive oprettet her.
        this.repository = repository;
    }

    //Henter alle medlemmer
    [HttpGet]
    public List<ClubMember> GetAll()
    {
        return repository.GetAll();
    }

    //Henter ét medlem ud fra id
    [HttpGet("{memberID}")]
    public ActionResult<ClubMember> GetById(int memberID)
    {
        ClubMember? member = repository.GetById(memberID);
        if (member == null)
        {
            return NotFound();
        }
        return member;
    }

    //Opretter et nyt medlem
    [HttpPost]
    public ClubMember Create(ClubMember member)
    {
        repository.Add(member);
        return member;
    }

    //Sletter et medlem og dets tøj
    [HttpDelete("{memberID}")]
    public IActionResult Delete(int memberID)
    {
        repository.Remove(memberID);
        return Ok();
    }

    /// Tilføjer tøj til et medlem
    [HttpPost("{memberID}/clothes")]
    public IActionResult AddClothes(int memberID, Clothes item)
    {
        repository.AddClothes(memberID, item);
        return Ok();
    }

    // Fjerner tøj fra et medlem
    [HttpDelete("{memberID}/clothes/{itemID}")]
    public IActionResult RemoveClothes(int memberID, int itemID)
    {
        repository.RemoveClothes(memberID, itemID);
        return Ok();
    }
}