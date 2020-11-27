using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;


namespace BEonTime.Data.Entities
{
    public class BEonTimeUser : MongoUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CareerStarted { get; set; }
        public DeviceUser DeviceUser { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
