﻿using eShop.Entities;
using eShop.Entities.CustomEntities;
using eShop.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eShop.Web.ViewModels
{
    public class CartItemsViewModel : PageViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public List<int> ProductIDs { get; set; }
        public List<Product> Products { get; set; }
        public string PromoCode { get; set; }
        public Promo Promo { get; set; }
    }

    public class UpdateCartItemsViewModel : PageViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public string PromoCode { get; set; }
    }

    public class CheckoutViewModel : CartItemsViewModel
    {
        public bool CartHasProducts { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public bool PromoApplied { get; set; }

        public eShopUser User { get; set; }
        public decimal FinalAmount { get; set; }
    }

    public class ConfirmOrderViewModel
    {
        public List<int> ProductIDs { get; set; }
        public List<Product> Products { get; set; }
    }
}