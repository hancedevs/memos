using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using MongoDB.Driver;

namespace backend
{
    public class MongoDbContext
    {
        

    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["MongoDbSettings:ConnectionString"]);
        _database = client.GetDatabase(configuration["MongoDbSettings:DatabaseName"]);
    }

    public IMongoCollection<Planner> Planners => _database.GetCollection<Planner>("Planners");
    public IMongoCollection<WeddingStory> WeddingStories => _database.GetCollection<WeddingStory>("WeddingStories");
    public IMongoCollection<QRScan> QRScans => _database.GetCollection<QRScan>("QRCodes");
    }
}