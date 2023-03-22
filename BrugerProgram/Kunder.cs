using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace BrugerProgram
{
    public class Kunde
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string?  PostalCode { get; set; }
        public string? City { get; set; }
        public string? StreetName { get; set; }


        public Kunde() { }

        public Kunde(string name, string email, string postalCode, string city, string streetName)
        {
            Name = name;   
            Email = email;
            PostalCode = postalCode;
            City = city;    
            StreetName = streetName;
                
        }



    }


   
}
