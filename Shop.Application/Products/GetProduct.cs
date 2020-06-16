using Microsoft.EntityFrameworkCore;
using Shop.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Products
{
    public class GetProduct
    {
        private ApplicationDbContext _ctx;

        public GetProduct(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ProductViewModel> Do(string name)
        {
            var stockOnHold = _ctx.StockOnHolds.Where(x => x.ExpiryDate < DateTime.Now).ToList();

            if (stockOnHold.Count > 0)
            {
                var stockToReturn = _ctx.Stock.AsEnumerable().Where(x => stockOnHold.Any(y => y.StockId == x.Id)).ToList();

                foreach (var stock in stockToReturn)
                {
                    stock.Quantity = stock.Quantity + stockOnHold.FirstOrDefault(x => x.StockId == stock.Id).Qty;
                }

                _ctx.StockOnHolds.RemoveRange(stockOnHold);

                await _ctx.SaveChangesAsync();
            }

            return _ctx.Products
                .Include(x => x.Stock)
                .Where(x => x.Name == name)
                .Select(x => new ProductViewModel
                {
                    Name = x.Name,
                    Description = x.Description,
                    Value = $"R {x.Value.ToString("N2")}",
                    Stock = x.Stock.Select(y => new StockViewModel
                    {
                        Id = y.Id,
                        Description = y.Description,
                        InStock = y.Quantity > 0
                    })
                })
                .FirstOrDefault();
        }
           

        public class ProductViewModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Value { get; set; }
            public IEnumerable<StockViewModel> Stock { get; set; }
        }

        public class StockViewModel
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public bool InStock { get; set; }
        }
    }
}
