using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Entities.CustomEntities
{
    public class CategoryWithChildren
    {
        public Category Category { get; set; }
        public List<Category> Children { get; set; }
    }
}
