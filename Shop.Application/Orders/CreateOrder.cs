using Shop.Database;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Orders
{
    public class CreateOrder
    {
        private ApplicationDbContext _ctx;

        public CreateOrder(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Request
        {
            public string StripeReference { get; set; }
            public string SessionId { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }

            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }

            public List<Stock> Stocks { get; set; }
        }

        public class Stock
        {
            public int StockId { get; set; }
            public int Quantity { get; set; }
        }
        public async Task<bool> Do(Request req)
        {
            var stockOnHold = _ctx.StockOnHolds.Where(x => x.SessionId == req.SessionId).ToList();

            _ctx.StockOnHolds.RemoveRange(stockOnHold);

            var order = new Order
            {
                OrderRef = CreateOrderReference(),
                StripeReference = req.StripeReference,

                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                PhoneNumber = req.PhoneNumber,
                Address1 = req.Address1,
                Address2 = req.Address2,
                City = req.City,
                PostCode = req.PostCode,

                OrderProducts = req.Stocks.Select(x => new OrderProduct
                {
                    StockId = x.StockId,
                    Qty = x.Quantity
                }).ToList()
            };

            _ctx.Orders.Add(order);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public string CreateOrderReference()
        {
            var characters = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
            var result = new char[12];
            var random = new Random();

            do
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = characters[random.Next(characters.Length)];
                }
            } while (_ctx.Orders.Any(x => x.OrderRef == new string(result)));

            return new string(result);
        }
    }
}
