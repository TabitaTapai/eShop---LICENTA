using eShop.Entities.APIEntities;
using eShop.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eShop.Web.Areas.API.Models
{
    public class CartItemsViewModel
    {
        public CartItemsViewModel()
        {
            CartItems = new List<CartItemEntity>();
        }

        public List<CartItemEntity> CartItems { get; set; }
        public bool PromoApplied { get; set; }
        public PromoEntity Promo { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal FinalTotal { get; set; }
    }
}