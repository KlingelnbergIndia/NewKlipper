using DataAccess.Helper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.Model;

namespace DataAccess.EntityModel.Attendance
{
    public class AttendanceRegularizationEntityModel
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }
        public int EmployeeID { get; set; }
        [BsonDateTimeOptions]
        public DateTime RegularizedDate { get; set; }
        [BsonDateTimeOptions]
        public string Remark { get; set; }
        public TimeSpan RegularizedHours { get; set; }
        public DateTime LogTime { get; set; } = DateTime.Now;
    }
}
