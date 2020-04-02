using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models
{
    public class Item
    {
        [Key]
        public int IdItem { get; set; }
        public string NameItem { get; set; }
        public string Description { get; set; }
        public int IdCollectionItem { get; set; }
        public CollectionItem CollectionItems { get; set; }
        public List<Like> Likes { get; set; }
        public Item()
        {
            Likes = new List<Like>();
        }
    }
}
