using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAccess.Helper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DataAccess.EntityModel.Employment
{
    [BsonIgnoreExtraElements]
    public class EmployeeEntityModel 
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }

        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [StringLength(50)]
        [Display(Name = "First Holiday")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [StringLength(50)]
        [Display(Name = "Last Holiday")]
        public string LastName { get; set; }
        public string Title { get; set; } = "";

        [BsonDateTimeOptions]
        public DateTime JoiningDate { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        public int DepartmentId { get; set; }
        
        public int ReportingSuperiorID { get; set; } = -1;

        public List<int> Reportees { get; set; } = new List<int>();

        public bool HasRole(string role)
        {
            return Roles.Contains(role);
        }

        public bool IsReportee(int id)
        {
            return Reportees.Contains(id);
        }

    }

    public enum Gender
    {
        Male = 1,
        Female = 2
    }

}