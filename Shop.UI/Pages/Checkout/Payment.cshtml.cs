using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Shop.Application.Cart;
using Stripe;
using Stripe.Checkout;

namespace Shop.UI.Pages.Checkout
{
    public class PaymentModel : PageModel
    {
        public string PublicKey { get; }

        public PaymentModel(IConfiguration config)
        {
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

        public IActionResult OnPost(string stripeEmail, string stripeToken)
        {
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
                Description = "Custom t-shirt",
                Amount = 500,
                Currency = "usd",
            };
            var chargeService = new ChargeService();
            Charge charge = chargeService.Create(chargeOptions);

            return RedirectToPage("/Index");
        }
    }
}
