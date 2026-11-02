using Backend.Repositories;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]   // adressen bliver /clubmember
public class ClubMemberController : ControllerBase
{
    private readonly IClubMemberRepository repository;

    public ClubMemberController(IClubMemberRepository repository)
    {
        this.repository = repository;
    }

    /// <summary>Henter alle medlemmer.</summary>
    [HttpGet]
    public List<ClubMember> GetAll()
    {
        return repository.GetAll();
    }

    /// <summary>Henter ét medlem ud fra id.</summary>
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

    /// <summary>Opretter et nyt medlem.</summary>
    [HttpPost]
    public ClubMember Create(ClubMember member)
    {
        repository.Add(member);
        return member;
    }

    /// <summary>Sletter et medlem og dets tøj.</summary>
    [HttpDelete("{memberID}")]
    public IActionResult Delete(int memberID)
    {
        repository.Remove(memberID);
        return Ok();
    }

    /// <summary>Tilføjer tøj til et medlem.</summary>
    [HttpPost("{memberID}/clothes")]
    public IActionResult AddClothes(int memberID, Clothes item)
    {
        repository.AddClothes(memberID, item);
        return Ok();
    }

    /// <summary>Fjerner tøj fra et medlem.</summary>
    [HttpDelete("{memberID}/clothes/{itemID}")]
    public IActionResult RemoveClothes(int memberID, int itemID)
    {
        repository.RemoveClothes(memberID, itemID);
        return Ok();
    }
}