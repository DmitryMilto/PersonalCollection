using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models.ViewModel
{
    public class LikeViewModel
    {
        public int IdItem { get; set; }
        public string NameItem { get; set; }
        public string Description { get; set; }
        public int IdCollectionItem { get; set; }
        public bool like { get; set; }
        public string Image { get; set; }
        public string NameCollection { get; set; }
    }
}
