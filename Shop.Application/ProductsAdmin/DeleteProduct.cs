using Shop.Database;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.ProductsAdmin
{
    public class DeleteProduct
    {
        private ApplicationDbContext _ctx;

        public DeleteProduct(ApplicationDbContext context)
        {
            _ctx = context;
        }

        public async Task<bool> Do(int id)
        {
            var Product = _ctx.Products.FirstOrDefault(x => x.Id == id);
            _ctx.Products.Remove(Product);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
