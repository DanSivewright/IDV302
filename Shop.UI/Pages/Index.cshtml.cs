using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

// Logic Imports 
using Shop.Application.Products;
using Shop.Database;

namespace Shop.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private ApplicationDbContext _ctx;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public IEnumerable<GetProducts.ProductViewModel> Products { get; set; }

        public void OnGet()
        {
            Products = new GetProducts(_ctx).Do();
        }
    }
}
