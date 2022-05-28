using eShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Services
{
    public static class DataContextHelper
    {
        public static eShopContext GetNewContext()
        {
            return new eShopContext();
        }
    }
}
