using Core;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Backend.Repositories;

public class ClubMemberRepository : IClubMemberRepository
{
    private readonly IMongoCollection<ClubMember> memberCollection;

    public ClubMemberRepository()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(ClubMember)))
        {
            BsonClassMap<ClubMember> map = new BsonClassMap<ClubMember>();
            map.AutoMap();
            map.MapIdMember(typeof(ClubMember).GetProperty("ClubMemberID")!);
            BsonClassMap.RegisterClassMap(map);
        }

        MongoClient client = new MongoClient("mongodb://localhost:27017");
        IMongoDatabase database = client.GetDatabase("ClothingClub");
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

    // Gemmer hele medlemmet (inkl. tøjlisten) igen
    private void SaveMember(ClubMember member)
    {
        FilterDefinition<ClubMember> filter = Builders<ClubMember>.Filter.Eq("_id", member.ClubMemberId);
        memberCollection.ReplaceOne(filter, member);
    }
}