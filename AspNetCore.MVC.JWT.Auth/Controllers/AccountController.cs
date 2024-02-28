using AspNetCore.MVC.JWT.Auth.Models;
using AspNetCore.MVC.JWT.Auth.Untils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AspNetCore.MVC.JWT.Auth.Controllers
{
    public class AccountController : Controller
    {
        //登录页面
        public IActionResult Login()
        {
            return View();
        }
        //登录提交
        [HttpPost]
        public IActionResult LoginSub(IFormCollection fromData)
        {
            string userName = fromData["userName"].ToString();
            string passWord = fromData["password"].ToString();
            //真正写法是读数据库验证
            if (userName == "test" && passWord == "123456")
            {
                #region 传统session/cookies
                //登录成功,记录用户登录信息
                CurrentUser currentUser = new CurrentUser()
                {
                    Id = 123,
                    Name = "测试账号",
                    Account = userName
                };

                //写sessin
                // HttpContext.Session.SetString("CurrentUser", JsonConvert.SerializeObject(currentUser));
                //写cookies
                HttpContext.SetCookies("CurrentUser", JsonConvert.SerializeObject(currentUser));
                #endregion

                //跳转到首页
                return RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["err"] = "账号或密码不正确";
                //账号密码不对,跳回登录页
                return RedirectToAction("Login", "Account");
            }
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public IActionResult LogOut()
        {
            HttpContext.DeleteCookies("CurrentUser");
            //Session方式
            // HttpContext.Session.Remove("CurrentUser");
            return RedirectToAction("Login", "Account");
        }
    }
}
