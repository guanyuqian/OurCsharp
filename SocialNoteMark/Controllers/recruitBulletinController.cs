﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SocialNoteMark.Models;

namespace SocialNoteMark.Controllers
{
    public class recruitBulletinController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private List<Bulletin> result;

        public List<Bulletin> a { get; private set; }

        // GET: recruitBulletin
        public ActionResult Index()
        {
            result = db.Bulletins.SqlQuery("select * from Bulletins where type='1'and flag='0' AND UserName = '" + @User.Identity.Name + "';").ToList();
            result.Reverse();
            var userInfo = db.UserInfoes.First(u => u.UserName == User.Identity.Name);
            ViewBag.ImageUrl = userInfo.ImageUrl;
            return View(result);
        }
        public ActionResult interesting()
        {
            String UN = User.Identity.Name;
            int BulletionId = Int32.Parse(Request.Params["toId"]);
            List<Interest> b = db.Interests.SqlQuery("select * from Interests where UserName='" + UN + "'and BulletionID='" + BulletionId + "';").ToList();
            if (b.Count == 0)
            {
                db.Interests.Add(new Interest { UserName = User.Identity.Name, BulletionID = Int32.Parse(Request.Params["toId"]) });
                db.SaveChanges();
            }
            return RedirectToAction("AllRecruit"); ;
        }

   

      
        public ActionResult AllRecruit()
        {
            List<Interest> InterestList = db.Interests.ToList();
      
   

            result = db.Bulletins.SqlQuery("select * from Bulletins where type='1' and flag='0'").ToList();
            result.Reverse();

            return View(result);
        }
        public ActionResult History()
        {
            var UserName = User.Identity.Name;
            ViewBag.noteList = db.Bulletins.Where(u => u.UserName == UserName).OrderByDescending(u => u.CreateDate).ToList();
            return View(db.Bulletins.Where(u => u.UserName == UserName).OrderByDescending(u => u.CreateDate).ToList());
        }
      
        // GET: recruitBulletin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bulletin bulletin = db.Bulletins.Find(id);
            if (bulletin == null)
            {
                return HttpNotFound();
            }
            var nameList = new List<String>();
            var imageUrlList = new List<String>();
            List<Interest> friendList = db.Interests.Where(u => u.BulletionID == id).ToList();
            foreach (var fd in friendList)
            {
                var friendName = fd.UserName;
                var userInfo = db.UserInfoes.FirstOrDefault(u => u.UserName == friendName);
                nameList.Add(friendName);
                imageUrlList.Add(userInfo.ImageUrl);
            }
            ViewBag.NameList = nameList;
            ViewBag.ImageUrlList = imageUrlList;                          
            return View(bulletin);
        }

        // GET: recruitBulletin/Create
   
        public ActionResult Create()
        {

            return View();
        }

        // POST: recruitBulletin/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "BulletionID,UserName,Type,Flag,Name,Content,CreateDate")] Bulletin bulletin)
        {
            if (ModelState.IsValid)
            {
                bulletin.UserName = @User.Identity.Name;
                bulletin.Flag = 0;
                bulletin.Type = 1;
                bulletin.CreateDate = DateTime.Now;

         
                //bulletin.UserName=Session.GetEnumerator
                db.Bulletins.Add(bulletin);
                db.SaveChanges();
                return RedirectToAction(Request["nextPage"]);
            }

            return View(bulletin);
        }

        // GET: recruitBulletin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bulletin bulletin = db.Bulletins.Find(id);
            if (bulletin == null)
            {
                return HttpNotFound();
            }
            return View(bulletin);
        }

        // POST: recruitBulletin/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "BulletionID,UserName,Type,Flag,Name,Content,CreateDate")] Bulletin bulletin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bulletin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bulletin);
        }

        // GET: recruitBulletin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bulletin bulletin = db.Bulletins.Find(id);
            if (bulletin == null)
            {
                return HttpNotFound();
            }
            return View(bulletin);
        }

        // POST: recruitBulletin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bulletin bulletin = db.Bulletins.Find(id);
            db.Bulletins.Remove(bulletin);
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
    }
}
