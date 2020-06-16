using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shop.Database;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Cart
{
    public class AddToCartd
    {
        private ISession _session;
        private ApplicationDbContext _ctx;

        public AddToCartd(ISession session, ApplicationDbContext ctx)
        {
            _session = session;
            _ctx = ctx;
        }


        public class Request
        {
            public int StockId { get; set; }
            public int Quantity { get; set; }
        }

        public async Task<bool> Do(Request request)
        {
            var stockOnHold = _ctx.StockOnHolds.Where(x => x.SessionId == _session.Id).ToList();
            var stockToHold = _ctx.Stock.Where(x => x.Id == request.StockId).FirstOrDefault();

            if (stockToHold.Quantity < request.Quantity)
            {
                // Return not enough stock 
                return false;
            }

            _ctx.StockOnHolds.Add(new StockOnHold
            {
                StockId = stockToHold.Id,
                SessionId = _session.Id,
                Qty = request.Quantity, 
                ExpiryDate = DateTime.Now.AddMinutes(20)
            });

            stockToHold.Quantity = stockToHold.Quantity - request.Quantity;

            foreach (var stock in stockOnHold)
            {
                stock.ExpiryDate = DateTime.Now.AddMinutes(20);
            }

            await _ctx.SaveChangesAsync();

            var cartList = new List<CartProduct>();
            var stringObject = _session.GetString("cart");

            if (!string.IsNullOrEmpty(stringObject))
            {
                cartList = JsonConvert.DeserializeObject<List<CartProduct>>(stringObject);
            }

            if(cartList.Any(x => x.StockId == request.StockId))
            {
                cartList.Find(x => x.StockId == request.StockId).Quantity += request.Quantity;
            } else
            {
                cartList.Add(new CartProduct
                {
                    StockId = request.StockId,
                    Quantity = request.Quantity
                });
            }


            stringObject = JsonConvert.SerializeObject(cartList);

            _session.SetString("cart", stringObject);

            return true;
        }
    }
}
