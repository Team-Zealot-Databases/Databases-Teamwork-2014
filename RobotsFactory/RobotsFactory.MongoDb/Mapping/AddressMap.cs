﻿namespace RobotsFactory.MongoDb.Mapping
{
    using System;
    using System.Linq;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class AddressMap
    {
        [BsonConstructor]
        public AddressMap(int addressId, string addressText, int cityId)
        {
            this.AddressId = addressId;
            this.AddressText = addressText;
            this.CityId = cityId;
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public int AddressId { get; set; }

        public string AddressText { get; set; }

        public int CityId { get; set; }
    }
}