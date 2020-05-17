using Shop.Database;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.ProductsAdmin
{
    public class UpdateProduct
    {
        private ApplicationDbContext _ctx;

        public UpdateProduct(ApplicationDbContext context)
        {
            _ctx = context;
        }

        public async Task<Response> Do(Request req)
        {
            var product = _ctx.Products.FirstOrDefault(x => x.Id == req.Id);

            product.Name = req.Name;
            product.Description = req.Description;
            product.Value = req.Value;

            await _ctx.SaveChangesAsync();
            return new Response
            {
                Id = product.Id,
                Description = product.Description,
                Name = product.Name,
                Value = product.Value,
            };
        }
        public class Request
        {
            public int Id { get; set; }
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
