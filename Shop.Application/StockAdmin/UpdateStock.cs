﻿using Shop.Database;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.StockAdmin
{
    public class UpdateStock
    {
        private ApplicationDbContext _ctx;

        public UpdateStock(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Response> Do(Request req)
        {
            var stocks = new List<Stock>();

            foreach (var stock in req.Stock)
            {
                stocks.Add(new Stock
                {
                    Id = stock.Id,
                    Description = stock.Description,
                    Quantity = stock.Quantity,
                    ProductId = stock.ProductId
                });
            }

            _ctx.Stock.UpdateRange(stocks);

            await _ctx.SaveChangesAsync();

            return new Response
            {
                Stock = req.Stock
            };  
        }

        public class StockViewModel
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }
        }

        public class Request
        {
            public IEnumerable<StockViewModel> Stock { get; set; }
        }

        public class Response
        {
            public IEnumerable<StockViewModel> Stock { get; set; }
        }
    }
}
