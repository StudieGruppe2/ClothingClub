using Core;

namespace Backend.Repositories;

public class ClubMemberRepositoryMemory : IClubMemberRepository
{
    // "Data-lageret": en liste i serverens hukommelse
    private List<ClubMember> members = new List<ClubMember>();

    private int nextMemberID = 1;
    private int nextItemID = 1;
    
    
 // Constructor - opretter testdata, når Memory-repositoryet oprettes
public ClubMemberRepositoryMemory()
{
    // Opretter tre klubmedlemmer
    ClubMember emma = new ClubMember
    {
        Name = "Emma Jensen",
        EmailAddress = "emma@test.dk",
        Phonenumber = "12345678"
    };

    ClubMember sofie = new ClubMember
    {
        Name = "Sofie Nielsen",
        EmailAddress = "sofie@test.dk",
        Phonenumber = "23456789"
    };

    ClubMember laura = new ClubMember
    {
        Name = "Laura Hansen",
        EmailAddress = "laura@test.dk",
        Phonenumber = "34567890"
    };

    // Tilføjer medlemmerne og tildeler dem ID
    Add(emma);
    Add(sofie);
    Add(laura);

    // Emmas tøj
    AddClothes(emma.ClubMemberId, new Clothes
    {
        Type = "Kjole",
        Description = "Lang sommerkjole",
        Size = "M",
        Color = "Rød"
    });

    AddClothes(emma.ClubMemberId, new Clothes
    {
        Type = "Blazer",
        Description = "Klassisk blazer",
        Size = "L",
        Color = "Sort"
    });

    // Sofies tøj
    AddClothes(sofie.ClubMemberId, new Clothes
    {
        Type = "Skjorte",
        Description = "Oversized skjorte",
        Size = "S",
        Color = "Blå"
    });

    AddClothes(sofie.ClubMemberId, new Clothes
    {
        Type = "Jeans",
        Description = "Straight fit jeans",
        Size = "M",
        Color = "Mørkeblå"
    });

    // Lauras tøj
    AddClothes(laura.ClubMemberId, new Clothes
    {
        Type = "Nederdel",
        Description = "Plisseret nederdel",
        Size = "M",
        Color = "Grøn"
    });

    AddClothes(laura.ClubMemberId, new Clothes
    {
        Type = "Striktrøje",
        Description = "Blød striktrøje",
        Size = "L",
        Color = "Beige"
    });

    // Opretter to testudlån
    BorrowClothes(1, sofie.ClubMemberId, DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
    BorrowClothes(5, emma.ClubMemberId, DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
}

    //Henter alle klubmedlemmer
    public List<ClubMember> GetAll()
    {
        return members;
        
    }

    // Henter klubmedlem udfra id
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

    // Tilføjer et klubmedlem 
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

    // Tilføjer tøj til medlem
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

    //Sletter tøj fra medlemmet og dets liste
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

    // Registrerer et udlån af tøj til et klubmedlem
    public bool BorrowClothes(int itemID, int borrowedByMemberId, DateTime borrowedOn, DateTime dueDate )
    {
        // Tjekker om låneren findes
        if (GetById(borrowedByMemberId) == null)
        {
            return false;
        }

        // Gennemgår alle klubmedlemmer
        foreach (ClubMember member in GetAll())
        {
            // Gennemgår medlemmets tøj
            foreach (Clothes item in member.Clothes)
            {
                // Finder det rigtige stykke tøj
                if (item.ItemID == itemID)
                {
                    // Tjekker om tøjet allerede er udlånt
                    if (item.BorrowedByMemberId != null)
                    {
                        return false;
                    }

                    // Tjekker om medlemmet forsøger at låne sit eget tøj
                    if (member.ClubMemberId == borrowedByMemberId)
                    {
                        return false;
                    }

                    // Registrerer låneren og udlånsdatoerne
                    // Bruger de datoer, der sendes ind, i stedet for at ignorere dem.
                    item.BorrowedByMemberId = borrowedByMemberId;
                    item.BorrowedOn = borrowedOn;
                    item.DueDate = dueDate;

                    return true;
                }
            }
        }

        // Returnerer false, hvis tøjet ikke findes
        return false;
    }
    
    // Registrerer aflevering af et stykke tøj
    public bool ReturnClothes(int itemID, int BorrowedByMemberId)
    {
        // Gennemgår alle klubmedlemmer
        foreach (ClubMember member in GetAll())
        {
            // Gennemgår medlemmets tøj
            foreach (Clothes item in member.Clothes)
            { 
                //Springer medlemmer uden tøjliste over
                if (member.Clothes == null)
                {
                    continue;
                }
                // Finder det rigtige stykke tøj
                if (item.ItemID == itemID)
                {
                    // Tjekker om tøjet er lånt af det angivne medlem
                    if (item.BorrowedByMemberId != BorrowedByMemberId)
                    {
                        return false;
                    }

                    // Nulstiller udlånsoplysningerne
                    item.BorrowedByMemberId = null;
                    item.BorrowedOn = null;
                    item.DueDate = null;

                    return true;
                }
            }
        }

        // Returnerer false, hvis tøjet ikke findes
        return false;
    }
    
}