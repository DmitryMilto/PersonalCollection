using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models.ViewModel
{
    public class HomeViewModel
    {
        public List<CollectionItem> CollectionItems { get; set; }
        public List<Item> Items { get; set; }
    }
}
