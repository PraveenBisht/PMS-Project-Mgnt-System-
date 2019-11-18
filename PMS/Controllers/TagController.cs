using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMS.Models.Tag;
using PMS.Repository;

namespace ExpenseTracking.Controllers
{
    public class TagController : Controller
    {
        TagService _tagService = new TagService();

        public ActionResult Index()
        {
            return View("Index", _tagService.GetModel());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TagModel model)
        {
            if (ModelState.IsValid)
            {
                if (_tagService.CreateTag(model))
                {
                    return RedirectToAction("Index", "Tag");
                }
                else
                {
                    ModelState.AddModelError("", model.Message);
                }
            }

            return View("Create",model);
        }

        public ActionResult Edit(int id)
        {
            var tag = _tagService.GetModelByID(id);
            return View(tag);
        }

        [HttpPost]
        public ActionResult Edit(TagModel model)
        {
            if (_tagService.EditTag(model))
            {
                return RedirectToAction("Index", "Tag");
            }
            else
            {
                ModelState.AddModelError("", model.Message);
            }

            // If we got this far, something failed, redisplay form
            return View("Edit",model);
        }

        public ActionResult Delete(int id)
        {
            var tag = _tagService.GetModelByID(id);
            return View(tag);
        }

        [HttpPost]
        public ActionResult Delete(TagModel model)
        {
            if (_tagService.DeleteTag(model))
            {
                return RedirectToAction("Index", "Tag");
            }
            else
            {
                ModelState.AddModelError("", model.Message);
                return View("Delete",model);
            }
        }
    }
}
