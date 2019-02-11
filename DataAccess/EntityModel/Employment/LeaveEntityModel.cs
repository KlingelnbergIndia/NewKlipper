using DataAccess.Helper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static DomainModel.Leave;

namespace DataAccess.EntityModel.Employment
{
    public class LeaveEntityModel
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }
        public int EmployeeId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LeaveDate { get; set; }

        public LeaveType TypeOfLeave { get; set; }
        public string Remark { get; set; }

    }
}
