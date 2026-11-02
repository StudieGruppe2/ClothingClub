using Core;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Backend.Repositories;

public class ClubMemberRepositoryMongoDB : IClubMemberRepository
{
    private readonly IMongoCollection<ClubMember> memberCollection;

    public ClubMemberRepositoryMongoDB(IMongoDatabase database)
    {
        // Fortæl MongoDB, at MemberID er nøglen (_id)
        if (!BsonClassMap.IsClassMapRegistered(typeof(ClubMember)))
        {
            BsonClassMap<ClubMember> map = new BsonClassMap<ClubMember>();
            map.AutoMap();
            map.MapIdMember(typeof(ClubMember).GetProperty("MemberID")!);
            BsonClassMap.RegisterClassMap(map);
        }

        memberCollection = database.GetCollection<ClubMember>("Members");
    }

    public List<ClubMember> GetAll()
    {
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Empty;
        return memberCollection.Find(filter).ToList();
    }

    public ClubMember? GetById(int memberID)
    {
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Eq("_id", memberID);
        return memberCollection.Find(filter).FirstOrDefault();
    }

    public void Add(ClubMember member)
    {
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Empty;
        SortDefinition<ClubMember> sort = Builders<ClubMember>.Sort.Descending("_id");
        ClubMember? last = memberCollection.Find(filter).Sort(sort).FirstOrDefault();

        if (last == null)
        {
            member.ClubMemberId = 1;
        }
        else
        {
            member.ClubMemberId = last.ClubMemberId + 1;
        }

        if (member.Clothes == null)
        {
            member.Clothes = new List<Clothes>();
        }

        memberCollection.InsertOne(member);
    }

    // Sletter medlemmet. Tøjet ligger inde i medlemmet, så det forsvinder med.
    public void Remove(int memberID)
    {
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Eq("_id", memberID);
        memberCollection.DeleteOne(filter);
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

        item.ItemID = FindNextItemID();
        member.Clothes.Add(item);

        SaveMember(member);
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
    private int FindNextItemID()
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
}