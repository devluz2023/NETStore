// MyApi.Models/Cart.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace MyApi.Models
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } 

        public int UserId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Date { get; set; }

        public List<CartProduct> Products { get; set; } = new List<CartProduct>();
    }

    public class CartProduct
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}