﻿using BEonTime.Data.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace BEonTime.Data.Entities
{
    [BsonIgnoreExtraElements]
    [BsonCollection("deviceUsers")]
    public class DeviceUser
    {
        public string UserId { get; set; }
        public int DeviceUserId { get; set; }
        public string Name { get; set; }
        public int Card { get; set; }
        public int GroupId { get; set; }
        public string Privilege { get; set; }
    }
}