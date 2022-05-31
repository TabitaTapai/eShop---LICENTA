namespace eShop.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<eShop.Data.eShopContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(eShop.Data.eShopContext context)
        {
            //  Aceasta metoda va fi apelata dupa migrarea la cea mai recenta versiune

            //  e poate utiliza metoda DbSet<T>.AddOrUpdate() din helper 
            //  pentru a evita seed duplicat
        }
    }
}
