using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderID { get; set; }

        [NotMapped]
        public string ProductName { get; set; }
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        // Item Price este in Order Item deoarece putem incasa un pret mai mare sau mai mic decat Product Price 
        public decimal ItemPrice { get; set; }

        public int Quantity { get; set; }
    }
}
