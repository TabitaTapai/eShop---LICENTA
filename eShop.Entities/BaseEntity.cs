using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Entities
{
    // entitatile din clasa BaseEntity vor fi mostenite in aproape toate clasele urmatoare
    public class BaseEntity
    {
        // id-ul
        public int ID { get; set; }
        // este activa
        public bool IsActive { get; set; }
        // este stearsa
        public bool IsDeleted { get; set; }
        // timestamp cand a fost modificat
        public DateTime? ModifiedOn { get; set; }
    }
}
