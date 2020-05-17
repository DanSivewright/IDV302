using Shop.Database;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.ProductsAdmin
{
    public class CreateProduct
    {
        private ApplicationDbContext _ctx;

        public CreateProduct(ApplicationDbContext context)
        {
            _ctx = context;
        }

        public async Task<Response> Do(Request req)
        {
            var product = new Product
            {
                Name = req.Name,
                Description = req.Description,
                Value = req.Value
            };

            _ctx.Products.Add(product);
            
            await _ctx.SaveChangesAsync();

            return new Response
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Value = product.Value
            };
        }
        public class Request
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Value { get; set; }
        }

        public class Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Value { get; set; }
        }
    }
}
