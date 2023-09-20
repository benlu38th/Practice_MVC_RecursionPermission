using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MVC_RecursionPermission.Models;

namespace MVC_RecursionPermission.Filter
{
    public class PermissionFilters : ActionFilterAttribute
    {
        private Context db = new Context();

        StringBuilder _sbBuilder = new StringBuilder();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //取得使用者識別
            int _ticketUserID = Convert.ToInt32(((FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.Name);

            //判斷使用者是否有該權限
            //取得目前URL的controllerName及actionName
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();

            //判斷使用者是否存在
            Boolean userIsExist = db.Members.Any(m => m.Id == _ticketUserID);
            if (userIsExist == false)
            {
                //登出驗證表單
                FormsAuthentication.SignOut();
            }

            //取得使用者擁有的權限
            var userPermissionsCode = db.Members.Where(m => m.Id == _ticketUserID).Select(m => m.Permission).FirstOrDefault()?.Split(',');//例如： ['A01','A02']


            Boolean userHasRoutePermission = false;
            if (userPermissionsCode != null)
            {
                //取出使用者擁有的權限的控制器名稱
                var userPermissionsControllerNames = db.Permissions
                    .Where(p => userPermissionsCode.Contains(p.Code))
                    .Select(p => p.ControllerName)
                    .ToList();

                //遍歷使用者擁有的權限的控制器名稱
                foreach (var userPermissionsControllerName in userPermissionsControllerNames)
                {
                    if (userPermissionsControllerName == controllerName)
                    {
                        userHasRoutePermission = true;
                        break;
                    }
                }
            }

            //沒有權限
            if (userHasRoutePermission == false)
            {
                //登出驗證表單
                FormsAuthentication.SignOut();

                //跳回Home頁
                filterContext.Result = new RedirectResult("~/Login/Index");
            }

            //沒有權限
            //以下組出Menu字串

            //先清除全域變數_sbBuilder
            _sbBuilder.Clear();
            //全抓
            List<Permission> permissions = db.Permissions.ToList();

            //第一層
            List<Permission> roots = permissions.Where(x => x.ParentId == null).ToList();//['A', 'B', 'C']

            //遍歷第一層
            foreach (var root in roots)
            {
                if (userPermissionsCode!=null && userPermissionsCode.Any(p => p.StartsWith(root.Code)))
                {
                    if (root.Permissions.Count > 0)//如果有孩子
                    {
                        _sbBuilder.Append($"<li class='submenu'><a href = '#'><i class='glyphicon glyphicon-list'></i>{root.Subject}<span class='caret pull-right'></span></a>");

                        // 處理孩子
                        GetPermissionString(root);


                        _sbBuilder.Append($"</li>");
                    }
                    else//如果沒孩子
                    {
                        _sbBuilder.Append($"<li><a href='{root.Url}'>{root.Subject}</a></li>");
                    }
                }
            }


            filterContext.Controller.ViewBag.Menu = _sbBuilder.ToString();
        }
        private void GetPermissionString(Permission node)
        {
            //取得使用者識別
            int _ticketUserID = Convert.ToInt32(((FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.Name);

            //取得使用者擁有的權限
            var userPermissions = db.Members.Where(m => m.Id == _ticketUserID).Select(m => m.Permission).FirstOrDefault()?.Split(',');//例如： ['A01','A02']

            _sbBuilder.Append("<ul class='nav'>");

            foreach (var nodeKid in node.Permissions)
            {
                if (userPermissions.Any(p => p.StartsWith(nodeKid.Code)))
                {
                    if (nodeKid.Permissions.Count > 0)//如果有孩子
                    {
                        _sbBuilder.Append($"<li class='submenu'><a href = '#'>{nodeKid.Subject}<span class='caret pull-right'></span></a>");
                        GetPermissionString(nodeKid);
                        _sbBuilder.Append("</li>");
                    }
                    else//如果沒孩子
                    {
                        _sbBuilder.Append($"<li><a href='{nodeKid.Url}'>{nodeKid.Subject}</a></li>");
                    }
                }
            }

            _sbBuilder.Append("</ul>");
        }
    }
}
