﻿using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EatYourFeats.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(ILogger<DashboardModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}