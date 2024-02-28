using AspNetCore.MVC.JWT.Auth.Filter;
using AspNetCore.MVC.JWT.Auth.Models;
using AspNetCore.MVC.JWT.Auth.Untils;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.JWT.Auth.Controllers
{
    [MyActionAuthrizaFilterAttribute]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //从cookie获取用户信息
            CurrentUser user = HttpContext.GetCurrentUserByCookie();
            //CurrentUser user = HttpContext.GetCurrentUserBySession();
            return View(user);
        }
    }
}
