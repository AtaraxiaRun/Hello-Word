using AspNetCore.MVC.JWT.Auth.Filter;
using AspNetCore.MVC.JWT.Auth.Models;
using AspNetCore.MVC.JWT.Auth.Untils;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.JWT.Auth.Controllers
{
    /// <summary>
    /// ��Ŀԭ��ַ��https://github.com/weixiaolong325/SessionAuthorized.Demo
    /// </summary>
    [MyActionAuthrizaFilterAttribute]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //��cookie��ȡ�û���Ϣ
            CurrentUser user = HttpContext.GetCurrentUserByCookie();
            //CurrentUser user = HttpContext.GetCurrentUserBySession();
            return View(user);
        }
    }
}
