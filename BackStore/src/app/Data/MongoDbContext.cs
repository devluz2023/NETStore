using MongoDB.Driver;
using MyApi.Models;

public class MongoDbContext
{
    public IMongoCollection<Cart> Carts { get; }
    public IMongoCollection<Product> Products { get; }
    
     public IMongoCollection<Sale> Sales { get; }

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        Carts = database.GetCollection<Cart>("Carts");
        Products = database.GetCollection<Product>("Products");
        Sales = database.GetCollection<Sale>("Sales");
    }
}