using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Linq.SqlClient;

namespace Application1
{
    class Program
    {
        static List<B_CUSTOMER> Customers;
        static List<B_SALE> Sales;
        static List<B_PRODUCT> Products;
        static List<B_PROVIDER> Providers;
        static List<B_PROVIDERS_PRODUCT> ProvidersProducts;
        static List<B_SALE> HighestSales;

        static void Main(string[] args)
        {
            GetData();

            GetPurchases();
            GetCustomersWithoutSales();
            GetHighestSales();
            GetProvidersForHighestSales();
            Console.ReadKey();
        }

        static void GetData()
        {
            using (BusinessClassesDataContext context = new BusinessClassesDataContext())
            {
                Products = (from prods in context.B_PRODUCTs
                            where SqlMethods.Like(prods.Description, "%kit")
                                && prods.IdProduct > 100
                            orderby prods.Description ascending
                            select prods).ToList<B_PRODUCT>();
            }
        }

        static void GetProvidersForHighestSales()
        {           
            Console.WriteLine("\nProviders of products with highest sales:");
            foreach (B_PRODUCT product in HighestSales.Select(ht => ht.B_PRODUCT))
            {
                Console.WriteLine("- Providers of " + product.Description);
                foreach (B_PROVIDER provider in ProvidersProducts
                    .Where(pr => product.IdProduct == pr.IdProduct)
                    .OrderBy(pr => pr.B_PRODUCT.Description)
                    .Select(pr => pr.B_PROVIDER).Distinct())
                    Console.WriteLine("   - " + provider.Name);
            }
        }

        static void GetHighestSales()
        {
            HighestSales = Sales
                .Where(s => s.Price >= 400)
                .OrderBy(s => s.Price).ToList();

            Console.WriteLine("\nHighest Sales:");
            foreach (B_SALE saleData in HighestSales)
                Console.WriteLine(" - " + saleData.B_CUSTOMER.FirstName + " : " +
                    saleData.B_PRODUCT.Description + " : " +
                    saleData.Cuantity + " unit(s) " +
                    saleData.Price.ToString() + " $");
        }

        static void GetCustomersWithoutSales()
        {
            List<B_CUSTOMER> customersWithNoSales = Customers
                .Except(Sales.Select(sd => sd.B_CUSTOMER).Distinct())
                .ToList();

            Console.WriteLine("\nCustomers without sales:");
            foreach (B_CUSTOMER customer in customersWithNoSales)
                Console.WriteLine(" - " + customer.FirstName);
        }

        static void GetPurchases()
        {
            Console.WriteLine("\nPurchases:");

            foreach (B_CUSTOMER customer in Sales.Select(sd => sd.B_CUSTOMER)
                .Distinct())
            {
                Console.WriteLine(customer.FirstName + " : ");
                foreach (string purchase in Sales
                    .Where(s => s.B_CUSTOMER == customer)
                    .Select(sd => sd.B_PRODUCT.Description + " : " +
                        sd.Cuantity.ToString() + " unit(s) " +
                        sd.Price.ToString() + " $"))
                    Console.WriteLine(" - " + purchase);
            }
        }

        static void GetBusinessData()
        {
            using (BusinessClassesDataContext context = new BusinessClassesDataContext())
            {
                Customers = (from cust in context.B_CUSTOMERs
                                //where SqlMethods.Like
                                orderby cust.FirstName ascending, cust.LastName ascending
                                select cust).ToList<B_CUSTOMER>();

                Sales = (from sale in context.B_SALEs
                            orderby sale.Cuantity descending
                            select sale).ToList<B_SALE>();

                Products = (from prod in context.B_PRODUCTs
                            orderby prod.Description ascending
                            select prod).ToList<B_PRODUCT>();

                Providers = (from prov in context.B_PROVIDERs
                                orderby prov.Name
                                select prov).ToList<B_PROVIDER>();

                ProvidersProducts = (from provprod in context.B_PROVIDERS_PRODUCTs
                                        select provprod).ToList<B_PROVIDERS_PRODUCT>();

            }
        }
    }
}
