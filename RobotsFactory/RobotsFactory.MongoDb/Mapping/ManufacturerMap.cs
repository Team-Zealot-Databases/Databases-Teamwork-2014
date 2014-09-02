﻿namespace RobotsFactory.MongoDb.Mapping
{
    using System;
    using System.Linq;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class ManufacturerMap
    {
        [BsonConstructor]
        public ManufacturerMap(int manufacturerId, string name, int addressId)
        {
            this.ManufacturerId = manufacturerId;
            this.Name = name;
            this.AddressId = addressId;
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public int ManufacturerId { get; set; }

        public string Name { get; set; }

        public int AddressId { get; set; }
    }
}