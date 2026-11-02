using Core;

namespace Backend.Repositories;

public class ClubMemberRepositoryMemory : IClubMemberRepository
{
    // "Databasen": en liste i serverens hukommelse
    private List<ClubMember> members = new List<ClubMember>();

    private int nextMemberID = 1;
    private int nextItemID = 1;

    public List<ClubMember> GetAll()
    {
        return members;
    }

    public ClubMember? GetById(int memberID)
    {
        foreach (ClubMember member in members)
        {
            if (member.ClubMemberId == memberID)
            {
                return member;
            }
        }
        return null;
    }

    public void Add(ClubMember member)
    {
        member.ClubMemberId = nextMemberID;
        nextMemberID++;

        if (member.Clothes == null)
        {
            member.Clothes = new List<Clothes>();
        }

        members.Add(member);
    }

    // Sletter medlemmet. Tøjet ligger inde i medlemmet, så det forsvinder med.
    public void Remove(int memberID)
    {
        ClubMember? member = GetById(memberID);
        if (member != null)
        {
            members.Remove(member);
        }
    }

    public void AddClothes(int memberID, Clothes item)
    {
        ClubMember? member = GetById(memberID);
        if (member == null)
        {
            return;
        }

        if (member.Clothes == null)
        {
            member.Clothes = new List<Clothes>();
        }

        item.ItemID = nextItemID;
        nextItemID++;

        member.Clothes.Add(item);
    }

    public void RemoveClothes(int memberID, int itemID)
    {
        ClubMember? member = GetById(memberID);
        if (member == null || member.Clothes == null)
        {
            return;
        }

        Clothes? found = null;
        foreach (Clothes c in member.Clothes)
        {
            if (c.ItemID == itemID)
            {
                found = c;
            }
        }

        if (found != null)
        {
            member.Clothes.Remove(found);
        }
    }

    // Samler alt tøj fra alle medlemmer i én liste
    public List<Clothes> GetAllClothes()
    {
        List<Clothes> result = new List<Clothes>();

        foreach (ClubMember member in members)
        {
            if (member.Clothes == null)
            {
                continue;
            }

            foreach (Clothes c in member.Clothes)
            {
                result.Add(c);
            }
        }

        return result;
    }

    // Finder ét stykke tøj, uanset hvilket medlem der ejer det
    public Clothes? GetClothesById(int itemID)
    {
        foreach (Clothes c in GetAllClothes())
        {
            if (c.ItemID == itemID)
            {
                return c;
            }
        }
        return null;
    }
}