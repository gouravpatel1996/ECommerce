using AutoMapper;
using ECommerce.Api.Customers.Db;
using ECommerce.Api.Customers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Customers.Providers
{
    public class CustomersProvider: ICustomersProvider
    {
        private readonly CustomersDbContext dbContext;
        private readonly ILogger<CustomersProvider> logger;
        private readonly IMapper mapper;

        public CustomersProvider(CustomersDbContext dbContext,ILogger<CustomersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
            SeedData();
        }
        private void SeedData()
        {
            if (!dbContext.Customers.Any())
            {
                dbContext.Customers.Add(new Db.Customer() { Id = 1, Name = "Keyboard", Address = "3000 street" });
                dbContext.Customers.Add(new Db.Customer() { Id = 2, Name = "Mouse", Address = "1000 street" });
                dbContext.Customers.Add(new Db.Customer() { Id = 3, Name = "Monitor", Address = "20000 street" });
                dbContext.Customers.Add(new Db.Customer() { Id = 4, Name = "Modem", Address = "3000 street" });
                dbContext.Customers.Add(new Db.Customer() { Id = 5, Name = "UPS", Address = "5000 street" });
                dbContext.SaveChanges();
            }
        }
        public async Task<(bool IsSuccess, IEnumerable<Models.Customer> Customers, string ErrorMessage)> GetCustomersAsync()
        { 
            try
            {
                logger?.LogInformation("Querying Customers");
                var customers = await dbContext.Customers.ToListAsync();
                if (customers!=null && customers.Any())
                {
                    logger?.LogInformation($"{customers.Count} Customer(s) found");
                    var result = mapper.Map<IEnumerable<Db.Customer>, IEnumerable<Models.Customer>>(customers);
                    return (true, result, null);
                }
                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false,null,ex.Message);
            }
        }
        public async Task<(bool IsSuccess, Models.Customer Customer, string ErrorMessage)> GetCustomerAsync(int id)
        {
            try
            {
                logger?.LogInformation("Querying Customers");
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c=>c.Id==id);
                if (customer != null)
                {
                    logger?.LogInformation($"{customer.Name} Customer found");
                    var result = mapper.Map<Db.Customer,Models.Customer>(customer);
                    return (true, result, null);
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
