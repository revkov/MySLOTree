using MySLOTree.Filters;
using MySLOTree.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MySLOTree.Controllers
{
    [InitializeSimpleNewsDB]
    public class NewsController : Controller
    {
        public ActionResult Index()
        {
            using (NewsContext context = new NewsContext())
            {
                NewsListModel model = new NewsListModel()
                {
                    News = context.News.Where(x => !x.IsDeleted).ToArray().Select(x => new NewsModel(x))
                };

                return View(model);

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int? parentId, string title)
        {
            using (NewsContext context = new NewsContext())
            {
                var newNews = new News()
                {
                    ParentId = parentId,
                    Title = title
                };
                context.News.Add(newNews);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Move(int nodeId, int? newParentId)
        {
            if (nodeId == newParentId)
            {
                return RedirectToAction("Index");
            }
            using (NewsContext context = new NewsContext())
            {
                if (newParentId.HasValue && ContainsChilds(context, nodeId, newParentId.Value))
                {
                    return RedirectToAction("Index");
                }
                var node = context.News.Where(x => x.Id == nodeId).Single();
                node.ParentId = newParentId;
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private bool ContainsChilds(NewsContext context, int parentId, int id)
        {
            bool result = false;
            var inner = context.News.Where(x => x.ParentId == parentId && !x.IsDeleted).ToArray();
            foreach (var node in inner)
            {
                if (node.Id == id && node.ParentId == parentId)
                {
                    return true;
                }
                result = ContainsChilds(context, node.Id, id);
            }

            return result;
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (NewsContext context = new NewsContext())
            {
                DeleteNodes(context, id);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private void DeleteNodes(NewsContext context, int id)
        {
            var inner = context.News.Where(x => x.ParentId == id && !x.IsDeleted).ToArray();
            foreach (var node in inner)
            {
                node.IsDeleted = true;
                DeleteNodes(context, node.Id);
            }
            var deleted = context.News.Where(x => x.Id == id && !x.IsDeleted).Single();
            deleted.IsDeleted = true;
        }
    }
}
