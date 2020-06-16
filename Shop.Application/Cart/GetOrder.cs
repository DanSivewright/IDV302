using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class GetOrder
    {
        private ISession _session;
        private ApplicationDbContext _ctx;

        public GetOrder(ISession session, ApplicationDbContext ctx)
        {
            _session = session;
            _ctx = ctx;
        }
    
        public class Response
        {
            public IEnumerable<Product> Products { get; set; }
            public CustomerInformation CustomerInformation { get; set; }

            public int GetTotalCharge() => Products.Sum(x => x.Value * x.Quantity);
        }
        public class Product
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public int StockId { get; set; }
            public int Value { get; set; }
        }

        public class CustomerInformation
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
        }
        public Response Do()
        {
            // TODO: Account for multiple items in the cart
            var cart = _session.GetString("cart");

            var cartList = JsonConvert.DeserializeObject<List<CartProduct>>(cart);

            var itemsInList = cartList.Select(x => x.StockId).ToList();

            var listOfProducts = _ctx.Stock
               .Include(x => x.Product)
               .Where(x => itemsInList.Contains(x.Id))
               .Select(x => new Product
               {
                   ProductId = x.ProductId,
                   StockId = x.Id,
                   Value = (int)(x.Product.Value * 100),
            }).ToList();

            //add this
            listOfProducts = listOfProducts.Select(x =>
            {
                x.Quantity = cartList.FirstOrDefault(y => y.StockId == x.StockId).Quantity;
                return x;
            }).ToList();

            var customerInfoString = _session.GetString("customer-info");

            var customerInformation = JsonConvert.DeserializeObject<Shop.Domain.Models.CustomerInformation>(customerInfoString);

            return new Response
            {
                Products = listOfProducts,
                CustomerInformation = new CustomerInformation
                {
                    FirstName = customerInformation.FirstName,
                    LastName = customerInformation.LastName,
                    Email = customerInformation.Email,
                    PhoneNumber = customerInformation.PhoneNumber,
                    Address1 = customerInformation.Address1,
                    Address2 = customerInformation.Address2,
                    City = customerInformation.City,
                    PostCode = customerInformation.PostCode
                }
            };
        }
    }
}
