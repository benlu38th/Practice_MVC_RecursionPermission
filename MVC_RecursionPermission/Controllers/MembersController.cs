using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MVC_RecursionPermission.Filter;
using MVC_RecursionPermission.Models;

namespace MVC_RecursionPermission.Controllers
{
    [Authorize]
    [PermissionFilters]
    public class MembersController : Controller
    {
        private Context db = new Context();

        StringBuilder _sbBuilder = new StringBuilder();

        // GET: Members
        public ActionResult Index()
        {
            // 創建一個Dictionary，用來存儲member的id和他們permissions
            Dictionary<int, string> memberPermissions = new Dictionary<int, string>();

            var membersInfo = db.Members.ToList();

            foreach (var memberInfo in membersInfo)
            {
                string[] permissionsArray = db.Members
                    .Where(m => m.Id == memberInfo.Id)
                    .Select(m => m.Permission)
                    .FirstOrDefault()
                    ?.Split(',');

                if (permissionsArray != null)
                {
                    var permissionSubjects = db.Permissions
                    .Where(p => permissionsArray.Contains(p.Code))
                    .Select(p => p.Subject)
                    .ToList();

                    string outputPermission = string.Join(",", permissionSubjects);

                    memberPermissions[memberInfo.Id] = outputPermission;
                }
                else
                {
                    memberPermissions[memberInfo.Id] = "";
                }
            }

            ViewBag.MemberPermissions = memberPermissions;

            return View(db.Members.ToList());
        }

        // GET: Members/Create
        public ActionResult Create()
        {
            ViewBag.PermissionData = GetPermissionData();

            return View();
        }

        // POST: Members/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Account,Password,Permission")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(member);
        }

        // GET: Members/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }

            ViewBag.PermissionData = GetPermissionData();

            ////自己想的
            //string oldPermissionsString = db.Members.Where(m => m.Id == id).Select(m => m.Permission).FirstOrDefault();

            //string output = "[";
            //if (oldPermissionsString != null)
            //{
            //    string[] oldPermissions = oldPermissionsString.Split(',');

            //    for (int i = 1; i <= oldPermissions.Length; i++)
            //    {
            //        output += "'";
            //        output += oldPermissions[i - 1];
            //        output += "'";
            //        if (i != oldPermissions.Length)
            //        {
            //            output += ",";
            //        }
            //    }
            //}
            //output += "]";

            //GPT優化後的
            string oldPermissionsString = db.Members.Where(m => m.Id == id).Select(m => m.Permission).FirstOrDefault();

            string output = "";

            if (oldPermissionsString != null)
            {
                string[] oldPermissions = oldPermissionsString.Split(',');
                output = $"[ {string.Join(",", oldPermissions.Select(p => "'" + p + "'"))} ]";
            }
            else
            {
                output = "[]";
            }

            ViewBag.OldPermissions = output;

            return View(member);
        }

        // POST: Members/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Account,Password,Permission")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(member);
        }

        // GET: Members/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = db.Members.Find(id);
            db.Members.Remove(member);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string GetPermissionData()
        {
            //全抓
            List<Permission> permissions = db.Permissions.ToList();

            //第一層
            List<Permission> roots = permissions.Where(x => x.ParentId == null).ToList();

            _sbBuilder.Append("[");

            foreach (Permission root in roots)//遍歷兄弟姊妹
            {
                GetPermissionString(root);

                if (!root.Equals(roots.Last()))//如果root跟roots的最後一個不一樣，結尾加上逗號
                {
                    _sbBuilder.Append(",");
                }
            }
            _sbBuilder.Append("]");

            return _sbBuilder.ToString();
        }
        private void GetPermissionString(Permission node)
        {
            //塞兄弟姊妹的資料
            _sbBuilder.Append($"{{'id': '{node.Code}', 'text': '{node.Subject}'");

            if (node.Permissions.Count > 0)//如果有孩子
            {
                _sbBuilder.Append(", 'children':[");

                foreach (var nodePermission in node.Permissions)//遍歷孩子
                {
                    GetPermissionString(nodePermission);//抓孩子

                    if (!nodePermission.Equals(node.Permissions.Last()))//如果nodePermission跟node.Permissions的最後一個不一樣，結尾加上逗號
                    {
                        _sbBuilder.Append(",");
                    }
                }

                _sbBuilder.Append("]");
            }
            _sbBuilder.Append("}");
        }
    }
}
