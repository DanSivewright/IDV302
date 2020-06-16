using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Shop.Application.Cart;
using Shop.Application.Orders;
using Shop.Database;
using Stripe;
using Stripe.Checkout;

namespace Shop.UI.Pages.Checkout
{
    public class PaymentModel : PageModel
    {
        private ApplicationDbContext _ctx;

        public string PublicKey { get; }

        public PaymentModel(IConfiguration config, ApplicationDbContext ctx)
        {
            _ctx = ctx;
            PublicKey = config["Stripe:PublicKey"].ToString();
        }

        public IActionResult OnGet()
        {
            var information = new GetCustomerInformation(HttpContext.Session).Do();

            if(information == null)
            {
                return RedirectToPage("/Checkout/CustomerInformation");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(string stripeEmail, string stripeToken)
        {
            var CartOrder = new Application.Cart.GetOrder(HttpContext.Session, _ctx).Do();
            var customerOptions = new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            };
            var customerService = new CustomerService();
            Customer customer = customerService.Create(customerOptions);

            var chargeOptions = new ChargeCreateOptions
            {
                Customer = customer.Id,
                Description = "Shop Purchase",
                Amount = CartOrder.GetTotalCharge(),
                Currency = "zar",
            };
            var chargeService = new ChargeService();
            Charge charge = chargeService.Create(chargeOptions);

            var sessionId = HttpContext.Session.Id;

            // Create order
            await new CreateOrder(_ctx).Do(new CreateOrder.Request
            {
                StripeReference = charge.OrderId,
                SessionId = sessionId,

                FirstName = CartOrder.CustomerInformation.FirstName,
                LastName = CartOrder.CustomerInformation.LastName,
                Email = CartOrder.CustomerInformation.Email,
                PhoneNumber = CartOrder.CustomerInformation.PhoneNumber,
                Address1 = CartOrder.CustomerInformation.Address1,
                Address2 = CartOrder.CustomerInformation.Address2,
                City = CartOrder.CustomerInformation.City,
                PostCode = CartOrder.CustomerInformation.PostCode,

                Stocks = CartOrder.Products.Select(x => new CreateOrder.Stock
                {
                    StockId = x.StockId,
                    Quantity = x.Quantity
                }).ToList()
            });

            return RedirectToPage("/Index");
        }
    }
}
