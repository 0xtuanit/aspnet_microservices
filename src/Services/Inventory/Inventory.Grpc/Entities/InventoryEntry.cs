using Contracts.Domains;
using Infrastructure.Extensions;
using MongoDB.Bson.Serialization.Attributes;

namespace Inventory.Grpc.Entities;

// Only read data
[BsonCollection("InventoryEntries")]
public class InventoryEntry : MongoEntity
{
    [BsonElement("itemNo")]
    public string? ItemNo { get; set; }
    
    [BsonElement("quantity")]
    public int Quantity { get; set; }
}