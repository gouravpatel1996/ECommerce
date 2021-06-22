using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using ECommerce.Api.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrdersDbContext dbContext;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrdersDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
            SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Orders.Any())
            {
                dbContext.Orders.Add(new Db.Order() { Id = 1, Items = new List<Db.OrderItem>() { new Db.OrderItem() { Id = 1, ProductId = 1, Quantity = 1, UnitPrice = 2 } },  Total = 3000, CustomerId=1,OrderDate=DateTime.Now });
                dbContext.Orders.Add(new Db.Order() { Id = 2, Items = new List<Db.OrderItem>() { new Db.OrderItem(){ Id = 2, ProductId = 2, Quantity = 20, UnitPrice = 2000 } }, Total = 1000, CustomerId = 2 , OrderDate = DateTime.Now });
                dbContext.Orders.Add(new Db.Order() { Id = 3, Items = new List<Db.OrderItem>() { new Db.OrderItem(){ Id = 3, ProductId = 3, Quantity = 20, UnitPrice = 2000 } }, Total = 20000, CustomerId = 3 , OrderDate = DateTime.Now });
                dbContext.Orders.Add(new Db.Order() { Id = 4, Items = new List<Db.OrderItem>() { new Db.OrderItem(){ Id = 4, ProductId = 4, Quantity = 20, UnitPrice = 2000 } }, Total = 3000, CustomerId = 4 , OrderDate = DateTime.Now });
                dbContext.Orders.Add(new Db.Order() { Id = 5, Items = new List<Db.OrderItem>() { new Db.OrderItem(){ Id = 5, ProductId = 5, Quantity = 20, UnitPrice = 2000 } }, Total = 5000, CustomerId = 5 , OrderDate = DateTime.Now });
                dbContext.SaveChanges();
            }
        }

        //public Task<(bool IsSuccess, Models.Order Order, string ErrorMessage)> GetOrderAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await dbContext.Orders.Where(x=>x.CustomerId== customerId).ToListAsync();
                if (orders != null && orders.Any())
                {
                    var results = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                    return (true, results, null);
                }
                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
