using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using PMS.Models.Expense;
using PMS.Repository;

namespace PMS.Controllers
{
    public class ExpenseController : Controller
    {
        ExpenseService _expenseService=new ExpenseService();
        PMSEntities db=new PMSEntities();
        //public ActionResult AddMoney()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {

        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //}
        //[HttpPost]
        //public ActionResult AddMoney(WithdrawalViewModel model)
        //{
     
        //    try
        //    {
        //        var Withdrawal = new Withdrawal_tbl();
        //        Withdrawal.CreatedDate = model.createddate;
        //        Withdrawal.Amount = model.amount;
        //        Withdrawal.UserId = model.UserID;
        //        db.Withdrawal_tbl.Add(Withdrawal);
        //        db.SaveChanges();

        //        return View();
        //    }
        //    catch (Exception e)
        //    {
        //       throw;
        //    }  
        //}
        public ActionResult Index()
        {
            var model = _expenseService.GetModelByUser(User.Identity.GetUserId());
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult CreateNewExpense(IndexViewsModel model)
        {
            if (_expenseService.CreateNewExpense(model, User.Identity.GetUserId()))
            {              
                return RedirectToAction("CreateNewExpense", "Expense");
            }
            else
            {
                ModelState.AddModelError("", model.Message);
            }

            var newModel = _expenseService.GetModelByUser(User.Identity.GetUserId());
            newModel.IsHasError = model.IsHasError;
            newModel.Message = model.Message;

            return View("CreateNewExpense", newModel);
        }


        public ActionResult ExpensesChart()
        {
            var model = _expenseService.GetModelByUser(User.Identity.GetUserId());

            var groupByTagList = from m in model.Expenses
                                 group m by m.Tag
                                 into groupTag
                                 select new {groupTag.Key, SumAmount = groupTag.Sum(e => e.Amount)};


            var labelList = groupByTagList.Select(e => e.Key).ToArray();
            var valueList = groupByTagList.Select(e => e.SumAmount).ToArray();

            var bytes = new Chart(width: 400, height: 200, theme:ChartTheme.Blue)
                .AddSeries(
                    chartType: "bar",
                    xValue: labelList,
                    yValues: valueList)
                .GetBytes("png");
            return File(bytes, "image/png");
        }
        [HttpPost]
        public JsonResult GetExpensesTable()
        {
            PMSEntities _entity = new PMSEntities();
            try
            {
                var userid = User.Identity.GetUserId();

                var expenses = _entity.Expense_tbl.Where(e => e.UserID == userid).Select((items) =>
                new ExpenseModel
                {
                    ID = items.ID,
                    ExpenseDate = items.ExpenseDate ?? DateTime.Now,
                    expensedatetime = items.ExpenseDate.ToString(),
                    Description = items.Description,
                    Amount = items.Amount ?? 0,
                    Tag = (items.Tag_tbl != null ? items.Tag_tbl.Description : string.Empty)
                }).ToList();
                return Json(expenses);
            }
            catch (Exception e)
            {



            }

            return Json(string.Empty);
        }

        public ActionResult Details(int id)
        {
            var model = new IndexViewsModel();
            model.ExpenseModelEntry = _expenseService.GetExpenseModelEditByUserEmail(User.Identity.GetUserId(), id);
            model.TagList = _expenseService.GetTagModel();
            return View("Details", model);
        }

        public ActionResult Edit(int id)
        {
            var model = new IndexViewsModel();
            model.ExpenseModelEntry = _expenseService.GetExpenseModelEditByUserEmail(User.Identity.GetUserId(), id);
            model.TagList = _expenseService.GetTagModel();
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(IndexViewsModel model)
        {
            var isHasError = false;
            var errMessage = string.Empty;

            if (_expenseService.UpdateNewExpense(model.ExpenseModelEntry, User.Identity.GetUserId(), out isHasError, out errMessage))
            {
                return RedirectToAction("Index", "Expense");
            }
            else
            {
                ModelState.AddModelError("", errMessage);
            }

            var inputmodel = new IndexViewsModel();
            inputmodel.ExpenseModelEntry = _expenseService.GetExpenseModelEditByUserEmail(User.Identity.GetUserId(), model.ExpenseModelEntry.ID);
            inputmodel.TagList = _expenseService.GetTagModel();
            // If we got this far, something failed, redisplay form
            return View("Edit", inputmodel);
        }

        public ActionResult Delete(int id)
        {
            var model = new IndexViewsModel();
            model.ExpenseModelEntry = _expenseService.GetExpenseModelEditByUserEmail(User.Identity.GetUserId(), id);
            model.TagList = _expenseService.GetTagModel();
            return View("Delete", model);
        }

        [HttpPost]
        public ActionResult Delete(IndexViewsModel model)
        {
            var isHasError = false;
            var errMessage = string.Empty;

            if (_expenseService.DeleteNewExpense(model.ExpenseModelEntry, out isHasError, out errMessage))
            {
                return RedirectToAction("Index", "Expense");
            }
            else
            {
                ModelState.AddModelError("", errMessage);
            }

            return View("Index", "Expense");
        }
    }
}
