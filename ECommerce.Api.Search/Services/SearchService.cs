using ECommerce.Api.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;
        private readonly ICustomersService customersService;

        public SearchService(IOrdersService ordersService,IProductsService productsService,ICustomersService customersService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
            this.customersService = customersService;
        }
        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int CustomerId)
        {
            var CustomersResult = await customersService.GetCustomerAsync(CustomerId);
            var OrdersResult = await ordersService.GetOrdersAsync(CustomerId);
            var ProductsResult = await productsService.GetProductsAsync();
            if (OrdersResult.IsSuccess)
            {
                foreach (var order in OrdersResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = ProductsResult.IsSuccess ?
                            ProductsResult.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name :
                            "Product Information is not available.";
                    }
                   
                }
                var result = new
                {
                    Customer = CustomersResult.IsSuccess ?
                                  CustomersResult.Customer :
                                  new { Name = "Customer Information is not available." },
                    Orders = OrdersResult.Orders
                };
               return (true, result);
            }
            return (false, null);
        }
    }
}
