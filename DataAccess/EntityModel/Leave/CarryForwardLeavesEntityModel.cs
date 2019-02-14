using DataAccess.Helper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.EntityModel.Leave
{
    public class CarryForwardLeavesEntityModel
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }
        public int EmployeeId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LeaveBalanceTillDate { get; set; }
        public int TakenCasualLeaves { get; set; }
        public int TakenSickLeaves { get; set; }
        public int TakenCompoffLeaves { get; set; }
        public int MaxCasualLeaves { get; set; }
        public int MaxSickLeaves { get; set; }
        public int MaxCompoffLeaves { get; set; }
    }
}
