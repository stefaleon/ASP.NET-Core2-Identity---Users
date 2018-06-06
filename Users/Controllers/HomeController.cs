﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Users.Controllers
{
    public class HomeController : Controller
    {
        //[Authorize]
        //public ViewResult Index() =>
        //    View(new Dictionary<string, object> { ["Placeholder"] = "Placeholder" });
        
        
        [Authorize]
        public IActionResult Index() => View(GetData(nameof(Index)));

        // /Home/OtherAction
        [Authorize(Roles = "Users")]
        public IActionResult OtherAction() => View("Index", GetData(nameof(OtherAction)));

        private Dictionary<string, object> GetData(string actionName) => new Dictionary<string, object>
        {
            ["Action"] = actionName,
            ["User"] = HttpContext.User.Identity.Name,
            ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
            ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
            ["In Users Role"] = HttpContext.User.IsInRole("Users"),
            ["In Admins Role"] = HttpContext.User.IsInRole("Admins")
        };
    }
}
