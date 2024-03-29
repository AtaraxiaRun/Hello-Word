﻿namespace AspNetCore.MVC.JWT.Auth.Models
{
    /// <summary>
    /// 登录用户的信息
    /// </summary>
    public class CurrentUser
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
    }
}
