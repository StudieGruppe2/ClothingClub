using Core;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Backend.Repositories;

public class ClothesRepository : IClothesRepository
{
    private readonly IMongoCollection<Clothes> clothesCollection;

    public ClothesRepository()
    {
        // Fortæl MongoDB, at ItemID er nøglen (_id)
        if (!BsonClassMap.IsClassMapRegistered(typeof(Clothes)))
        {
            BsonClassMap<Clothes> map = new BsonClassMap<Clothes>();
            map.AutoMap();
            map.MapIdMember(typeof(Clothes).GetProperty("ItemID")!);
            BsonClassMap.RegisterClassMap(map);
        }

        MongoClient client = new MongoClient("mongodb://localhost:27017");
        IMongoDatabase database = client.GetDatabase("ClothingClub");
        clothesCollection = database.GetCollection<Clothes>("Clothes");
    }

    public List<Clothes> GetAll()
    {
        FilterDefinition<Clothes> filter = Builders<Clothes>.Filter.Empty;
        return clothesCollection.Find(filter).ToList();
    }

    public Clothes? GetById(int itemId)
    {
        FilterDefinition<Clothes> filter = Builders<Clothes>.Filter.Eq("_id", itemId);
        return clothesCollection.Find(filter).FirstOrDefault();
    }

    public void Add(Clothes item)
    {
        // Find det højeste ItemID og giv det nye stykke tøj det næste
        FilterDefinition<Clothes> filter = Builders<Clothes>.Filter.Empty;
        SortDefinition<Clothes> sort = Builders<Clothes>.Sort.Descending("_id");
        Clothes? last = clothesCollection.Find(filter).Sort(sort).FirstOrDefault();

        if (last == null)
        {
            item.ItemID = 1;
        }
        else
        {
            item.ItemID = last.ItemID + 1;
        }

        clothesCollection.InsertOne(item);
    }

    public bool Remove(int itemId)
    {
        FilterDefinition<Clothes> filter = Builders<Clothes>.Filter.Eq("_id", itemId);
        DeleteResult result = clothesCollection.DeleteOne(filter);
        return result.DeletedCount > 0;
    }
}