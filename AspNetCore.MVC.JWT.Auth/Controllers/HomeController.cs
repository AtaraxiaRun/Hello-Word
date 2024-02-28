using AspNetCore.MVC.JWT.Auth.Filter;
using AspNetCore.MVC.JWT.Auth.Models;
using AspNetCore.MVC.JWT.Auth.Untils;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.JWT.Auth.Controllers
{
    /// <summary>
    /// 项目原地址：https://github.com/weixiaolong325/SessionAuthorized.Demo
    /// </summary>
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
