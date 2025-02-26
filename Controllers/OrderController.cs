using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        private BookDBEntityEntities db = new BookDBEntityEntities();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getProductCategories()
        {
            List<Category> categories = new List<Category>();
            categories = db.Categories.OrderBy(a => a.CategoryName).ToList();
            return new JsonResult { Data = categories, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult getProducts(int categoryID)
        {
            List<Product> products = new List<Product>();
            products = db.Products.Where(a => a.CategoryID.Equals(categoryID)).OrderBy(a => a.ProductName).ToList();
            return new JsonResult { Data = products, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult save(OrderMaster order, HttpPostedFileBase file)
        {
            bool status = false;
            if (file != null)
            {
                string folderPath = Server.MapPath("~/Images/");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folderPath, fileName);
                file.SaveAs(filePath);
                order.AddressProofImage = fileName;
            }
            var isValidModel = TryUpdateModel(order);
            if (isValidModel)
            {
                db.OrderMasters.Add(order);
                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderMaster = db.OrderMasters
                  .Include(o => o.OrderDetails)
                  .FirstOrDefault(o => o.OrderID == id);

            if (orderMaster == null)
            {
                return HttpNotFound();
            }

            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.Products = new SelectList(db.Products, "ProductID", "ProductName");

            return View(orderMaster);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(OrderMaster orderMaster, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation Error: " + string.Join(", ", errors) });
            }

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Images/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    file.SaveAs(filePath);
                    orderMaster.AddressProofImage = "/Images/" + fileName;
                }

                db.Entry(orderMaster).State = EntityState.Modified;

                foreach (var orderDetail in orderMaster.OrderDetails)
                {
                    var existingDetail = db.OrderDetails
                                           .FirstOrDefault(od => od.OrderDetailID == orderDetail.OrderDetailID);

                    if (existingDetail != null)
                    {
                        existingDetail.Quantity = orderDetail.Quantity;
                        existingDetail.Rate = orderDetail.Rate;

                        var categoryId = int.Parse(Request.Form["Category_" + orderDetail.OrderDetailID]);
                        var productId = int.Parse(Request.Form["Product_" + orderDetail.OrderDetailID]);

                        existingDetail.Category = db.Categories.FirstOrDefault(c => c.CategoryID == categoryId);
                        existingDetail.Product = db.Products.FirstOrDefault(p => p.ProductID == productId);
                    }
                }

                db.SaveChanges();

                return Json(new { success = true, message = "Order Updated Successfully!", redirectUrl = Url.Action("Index", "Order") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        [Route("Details")]
        public ActionResult List()
        {
            var orderMasters = db.OrderMasters.ToList();

            var orderVMs = orderMasters.Select(o => new OrderVM
            {
                OrderID = o.OrderID,
                OrderDate = o.OrderDate,
                Description = o.Description,
                AddressProofImage = o.AddressProofImage,
            }).ToList();

            return View(orderVMs);
        }

        public ActionResult Delete(int id)
        {
            OrderMaster orderMaster = db.OrderMasters.Find(id);
            if (orderMaster == null)
            {
                return HttpNotFound();
            }
            return View(orderMaster);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            OrderMaster orderMaster = db.OrderMasters.Find(id);
            if (orderMaster == null)
            {
                return HttpNotFound();
            }
            db.OrderMasters.Remove(orderMaster);
            db.SaveChanges();
            return RedirectToAction("List");
        }


    }
}