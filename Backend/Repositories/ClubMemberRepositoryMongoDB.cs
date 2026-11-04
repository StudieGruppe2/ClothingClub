using Core;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Backend.Repositories;

public class ClubMemberRepositoryMongoDB : IClubMemberRepository
{
    //Database forbindelse - skal rettes i program så den virker
    private readonly IMongoCollection<ClubMember> memberCollection;

    public ClubMemberRepositoryMongoDB(IMongoDatabase database)
    {
        // Fortæl MongoDB, at ClubMemberId er nøglen (_id)
        if (!BsonClassMap.IsClassMapRegistered(typeof(ClubMember)))
        {
            BsonClassMap<ClubMember> map = new BsonClassMap<ClubMember>();
            map.AutoMap();
            map.MapIdMember(typeof(ClubMember).GetProperty("ClubMemberId")!);
            BsonClassMap.RegisterClassMap(map);
        }

        memberCollection = database.GetCollection<ClubMember>("Members");
    }
    
   
    //Henter alle klubmedlemmer
    public List<ClubMember> GetAll()
    {
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Empty;
        return memberCollection.Find(filter).ToList();
    }
    
    
    // Henter klubmedlem udfra medlemsid
    public ClubMember? GetById(int ClubMemberId)
    {
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Eq("_id", ClubMemberId);
        return memberCollection.Find(filter).FirstOrDefault();
    }
    

    // Tilføjer et klubmedlem og tildeler et ID
    public void Add(ClubMember member)
    {
        // Opretter et tomt filter, så alle medlemmer medtages
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Empty;
        
        // Sorterer medlemmerne efter ID, fra højeste til laveste
        SortDefinition<ClubMember> sort = Builders<ClubMember>.Sort.Descending("_id");

        // Finder medlemmet med det højeste ID og gemmer det i "last".
        // Hvis databasen er tom, bliver last = null.
        ClubMember? last = memberCollection.Find(filter).Sort(sort).FirstOrDefault();

        // Hvis der ikke findes medlemmer, starter ID på 1
        if (last == null)
        {
            member.ClubMemberId = 1;
        }
        else
        {
            // Ellers får det nye medlem et ID, der er 1 højere end det højeste
            member.ClubMemberId = last.ClubMemberId + 1;
        }

        // Sørger for, at medlemmet har en tom tøjliste, hvis listen er null
        if (member.Clothes == null)
        {
            member.Clothes = new List<Clothes>();
        }

        // Gemmer det nye medlem i MongoDB
        memberCollection.InsertOne(member);
    }

    
    // Sletter medlemmet. Tøjet ligger inde i medlemmet, så det forsvinder med.
    public void Remove(int ClubMemberId)
    {
        // Opretter et filter, der finder medlemmet med det angivne ID
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Eq("_id", ClubMemberId);
        memberCollection.DeleteOne(filter);
    }
    
    // Tilføjer tøj til klubmedlemmet udfra medlemsid'et
    public void AddClothes(int ClubMemberId, Clothes item)
    {
        ClubMember? member = GetById(ClubMemberId); // finder medlemmet
       
        //Hvis medlemmet ikke fidnes slutter metoden
        if (member == null)
        {
            return;
        }
        // hvis medlemmet ikke har en øjliste laves en ny
        if (member.Clothes == null)
        {
            member.Clothes = new List<Clothes>();
        }

        //Finder næste tøj-id og tildeler det
        item.ItemID = FindNextItemID(); // Kalder vores metode herfor
        member.Clothes.Add(item);

        //Gemmer det opdateret medlem og tøjet
        SaveMember(member);
    }

    
    //Sletter tøjet
    public void RemoveClothes(int ClubMemberId, int itemID)
    {
        ClubMember? member = GetById(ClubMemberId); //Finder medlemmet
        
        // Slutter metoden hvis medlem og tøjlisten er null
        if (member == null || member.Clothes == null)
        {
            return;
        }

        // Opretter en variabel til at gemme tøjet, vi leder efter
        Clothes? found = null;
        foreach (Clothes c in member.Clothes)
        {
            // Kontrollerer, om tøjets ID matcher det angivne itemID
            if (c.ItemID == itemID)
            {
                // Gemmer det fundne tøj i variablen found
                found = c;
            }
        }

        // Hvis tøjet blev fundet, fjernes det fra listen
        if (found != null)
        {
            member.Clothes.Remove(found);
            SaveMember(member);
        }
    }
    

    // Samler alt tøj fra alle medlemmer i én liste
    public List<Clothes> GetAllClothes()
    {
        List<Clothes> result = new List<Clothes>();

        foreach (ClubMember member in GetAll())
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

    // Gemmer hele medlemmet (inkl. tøjlisten) i databasen igen
    private void SaveMember(ClubMember member)
    {
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Eq("_id", member.ClubMemberId);
        memberCollection.ReplaceOne(filter, member);
    }

    // Finder det højeste ItemID blandt alle medlemmers tøj og lægger 1 til
    private int FindNextItemID() // Private fordi det er en "hjælpemetode" til vores addClothes
    {
        int highest = 0;

        foreach (Clothes c in GetAllClothes())
        {
            if (c.ItemID > highest)
            {
                highest = c.ItemID;
            }
        }

        return highest + 1;
    }
    
    //Udlåner tøjet
public bool BorrowClothes(int itemID, int borrowedByMemberId, DateTime borrowedOn, DateTime dueDate)
{
    // Tjekker om låneren findes
    if (GetById(borrowedByMemberId) == null)
    {
        return false;
    }

    // Gennemgår medlemmernes tøj
    foreach (ClubMember member in GetAll())
    {
        if (member.Clothes == null)
        {
            continue;
        }

        foreach (Clothes item in member.Clothes)
        {
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

                // Registrerer låneren og datoerne
                item.BorrowedByMemberId = borrowedByMemberId;
                item.BorrowedOn = borrowedOn;
                item.DueDate = dueDate;

                // Gemmer ændringen i MongoDB
                SaveMember(member);
                return true;
            }
        }
    }

    // Returnerer false, hvis tøjet ikke findes
    return false;
}

// Registrerer aflevering af et stykke tøj
public bool ReturnClothes(int itemID, int borrowedByMemberId)
{
    foreach (ClubMember member in GetAll())
    {
        if (member.Clothes == null)
        {
            continue;
        }

        foreach (Clothes item in member.Clothes)
        {
            if (item.ItemID == itemID)
            {
                // Kontrollerer, at tøjet er lånt af det angivne medlem
                if (item.BorrowedByMemberId != borrowedByMemberId)
                {
                    return false;
                }

                // Nulstiller udlånsoplysningerne
                item.BorrowedByMemberId = null;
                item.BorrowedOn = null;
                item.DueDate = null;

                // Gemmer ændringerne i MongoDB
                SaveMember(member);
                return true;
            }
        }
    }

    // Returnerer false, hvis tøjet ikke blev fundet
    return false;
}
}