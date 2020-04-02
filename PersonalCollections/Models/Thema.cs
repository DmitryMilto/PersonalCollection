using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models
{
    public class Thema
    {
        [Key]
        public int IdThema { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CollectionItem> CollectionItems { get; set; }

        public Thema()
        {
            CollectionItems = new List<CollectionItem>();
        }
    }
}
