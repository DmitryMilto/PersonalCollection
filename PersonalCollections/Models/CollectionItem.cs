using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models
{
    public class CollectionItem
    {
        [Key]
        public int IdCollection { get; set; }
        public string NameCollection { get; set; }
        public string Description { get; set; }
        public int IdThema { get; set; }
        public string Image { get; set; }
        public Thema Themas { get; set; }
        public string IdUser { get; set; }
        public List<Item> Items { get; set; }
        public CollectionItem()
        {
            Items = new List<Item>();
        }
    }
}
