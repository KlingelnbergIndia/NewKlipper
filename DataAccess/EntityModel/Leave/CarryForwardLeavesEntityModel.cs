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
        public int RemainingEarnedLeaves { get; set; }
        public int RemainingCasualLeaves { get; set; }
        public int RemainingSickLeaves { get; set; }
    }
}
