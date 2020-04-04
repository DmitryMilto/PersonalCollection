using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models.ViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<CollectionItem> CollectionItems { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public Page Page { get; set; }
    }
}
