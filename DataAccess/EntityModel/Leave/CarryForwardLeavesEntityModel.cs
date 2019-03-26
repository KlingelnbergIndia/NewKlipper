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


        public float TakenCasualLeaves { get; set; }
        public float TakenSickLeaves { get; set; }
        public float TakenCompoffLeaves { get; set; }


        public float MaxCasualLeaves { get; set; }
        public float MaxSickLeaves { get; set; }
        public float MaxCompoffLeaves { get; set; }
    }
}
