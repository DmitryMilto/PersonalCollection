using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models
{
    public class Like
    {
        [Key]
        public int IdLike { get; set; }
        public string UserName { get; set; }
        public int IdItem { get; set; }
        public Item Item { get; set; }
    }
}
