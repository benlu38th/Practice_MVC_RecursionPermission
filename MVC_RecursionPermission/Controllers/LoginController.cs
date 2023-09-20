using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MVC_RecursionPermission.Models;

namespace MVC_RecursionPermission.Controllers
{
    public class LoginController : Controller
    {
        private Context db = new Context();

        public ActionResult Logout()
        {
            // 清除缓存
            Response.Cache.SetNoStore();
            Response.Cache.AppendCacheExtension("no-cache");
            Response.Expires = -1;
            Response.Buffer = true;
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //登出驗證表單
            FormsAuthentication.SignOut();

            return RedirectToAction("Index");
        }


        // GET: Login/Index
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // 用戶已經登入，重定向到其他頁面
                return RedirectToAction("Index", "Members"); 
            }

            return View();
        }

        // POST: Login/Index
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id,Account,Password")] ViewLogin viewLogin)
        {
            if (ModelState.IsValid)
            {
                var userInfo = db.Members.FirstOrDefault(u => u.Account == viewLogin.Account && u.Password == viewLogin.Password);

                if (userInfo == null) //帳號或密碼錯誤
                {
                    ViewBag.AccountError = "帳號錯誤";
                    return View(viewLogin);
                }
                else//帳號密碼正確
                {
                    //宣告驗證票要夾帶的資料 (用;區隔)
                    string userData = userInfo.Account + ";" + userInfo.Permission;

                    //設定驗證票(夾帶資料，cookie 命名) // 需額外引入using System.Web.Configuration;
                    var tempt = SetAuthenTicket(userData, Convert.ToString(userInfo.Id));

                    //將 Cookie 寫入回應
                    Response.Cookies.Add(tempt);

                    return RedirectToAction("Index", "Members");

                }
            }

            return View(viewLogin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 設定認證票證並創建用戶的認證 Cookie。
        /// </summary>
        /// <param name="userData">要存儲在認證票證中的用戶特定數據。</param>
        /// <param name="userId">用戶的識別符。</param>
        /// <returns>包含加密認證票證的認證 Cookie。</returns>
        public static HttpCookie SetAuthenTicket(string userData, string userId)
        {
            // 聲明一個認證票證。
            // 注意：需要額外引入 using System.Web.Security;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userId, DateTime.Now, DateTime.Now.AddHours(1), false, userData);

            // 加密認證票證。
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            // 創建一個 Cookie。
            HttpCookie authenticationCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            // 寫入 Cookie 到回應。
            return authenticationCookie;
        }
    }
}
