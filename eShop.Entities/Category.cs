using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Entities
{
    // tabela Categories care mosteneste din BaseEntity
    public class Category : BaseEntity
    {
        // categoria parinte din care face parte subcategoria
        public int? ParentCategoryID { get; set; }
        public virtual Category ParentCategory { get; set; }

        // este marcat ca produs recomandat
        public bool isFeatured { get; set; }

        // SanitizedName este utilizat in URL-ul generat
        public string SanitizedName { get; set; }

        // ordine afisare
        public int DisplaySeqNo { get; set; }
        
        // id-ul pozei care se ataseaza produsului
        public int? PictureID { get; set; }
        public virtual Picture Picture { get; set; }

        public virtual List<Product> Products { get; set; }
        public virtual List<CategoryRecord> CategoryRecords { get; set; }
    }

    // tabela CategoryRecord
    public class CategoryRecord : BaseEntity
    {
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }

        public int LanguageID { get; set; }

        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
    }
}
