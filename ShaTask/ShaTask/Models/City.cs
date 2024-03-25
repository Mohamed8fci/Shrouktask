using System;
using System.Collections.Generic;

namespace ShaTask.Models
{
    public partial class City
    {
        public City()
        {
            Branches = new HashSet<Branch>();
        }

        public int Id { get; set; }
        public string CityName { get; set; } = null!;

        public virtual ICollection<Branch> Branches { get; set; }
    }
}
