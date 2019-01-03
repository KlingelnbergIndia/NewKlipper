using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAccess.Helper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DataAccess.EntityModel.Employment
{
    public class EmployeeEntityModel : Person
    {
        
        public string Title { get; set; } = "";

        [BsonDateTimeOptions]
        public DateTime JoiningDate { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        public int DepartmentId { get; set; }

        //public List<int> OtherAssociatedDepartments { get; set; } = new List<int>();


        public int SubDivisionId { get; set; }

        public DateTime LastUpdatedOn { get; set; } = DateTime.Now;

        public int ReportingSuperiorID { get; set; } = -1;

        public List<int> Reportees { get; set; } = new List<int>();

        [BsonDateTimeOptions]
        public DateTime LeavingDate { get; set; }

        public string ProvidentFundNumber { get; set; } = "'MH/PUN/305790/XXX";
        public string ProvidentFundUANNumber { get; set; } = "000000000000";
        public string PANNumber { get; set; } = "ABCDEFGHIJK";
        public string AadharNumber { get; set; } = "XXXX YYYY ZZZZ";

        public List<string> CompanyCreditCards { get; set; } = new List<string>();

        public bool HasRole(string role)
        {
            return Roles.Contains(role);
        }

        public bool IsInActiveService()
        {
            if (LeavingDate.Year == 1 && LeavingDate.Month == 1 && LeavingDate.Day == 1)
            {
                return true;
            }
            if (LeavingDate < DateTime.Now)
            {
                return false;
            }
            return true;
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

    public class Person
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }

        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public Gender Gender { get; set; } = Gender.Male;

        [StringLength(10)]
        public string Prefix { get; set; } = "Mr.";

        [BsonDateTimeOptions]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [BsonDateTimeOptions]
        public DateTime BirthDate { get; set; }

        public Address HomeAddress { get; set; } = new Address();
        public Address WorkAddress { get; set; } = new Address()
        {
            Unit = "Office No. 501",
            Building = "OM Chambers, T29/31",
            Street = "Bhosari Telco Road",
            Landmark = "Near Sharayu Toyota Showroom",
            Locality = "MIDC Bhosari",
            City = "Pune",
            State = "Maharashtra",
            Country = "India",
            ZipCode = "411026",
            MapLink = "https://goo.gl/maps/1WJqS32rMw12"
        };

        [StringLength(15)]
        public string MobilePhone { get; set; } = "+91 9999999999";

        [StringLength(15)]
        public string WorkPhone { get; set; } = "+91 9999999999";

        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "xyz@Klingelnberg.com";

        public byte[] Photo { get; set; } = null;

        //public List<SocialProfile> SocialConnections { get; set; }
    }

    public class Address
    {
        public int ID { get; set; }

        [StringLength(25)]
        public string Unit { get; set; }

        [StringLength(25)]
        public string Building { get; set; }

        [StringLength(25)]
        public string Street { get; set; }

        [StringLength(50)]
        public string Locality { get; set; }

        [StringLength(25)]
        public string Landmark { get; set; }

        [StringLength(25)]
        public string City { get; set; }

        [StringLength(25)]
        public string State { get; set; }

        [StringLength(20)]
        public string Country { get; set; }

        [StringLength(15)]
        public string ZipCode { get; set; }

        [DataType(DataType.Url)]
        public string MapLink { get; set; }
    }
}