using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Autocomplete.Domain
{
    public class RealEstateEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FormerName { get; set; }

        public string Market { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string StreetAddress { get; set; }

        public byte Type { get; set; }
    }
}
