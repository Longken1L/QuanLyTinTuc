using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuanLyTinTuc.Models;

namespace QuanLyTinTuc.Controllers
{
    public class TinTucsController : Controller
    {
        private TinTucContext db = new TinTucContext();


        // =====================================================
        // DANH SÁCH + TÌM KIẾM + LỌC CHỦ ĐỀ
        // =====================================================
        public ActionResult Index(string searchString, string chuDe)
        {
            var tinTucs = db.TinTucs.AsQueryable();


            // TÌM KIẾM
            // Tìm theo cả TIÊU ĐỀ và CHỦ ĐỀ
            if (!String.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();

                tinTucs = tinTucs.Where(t =>
                    (t.TieuDe != null &&
                     t.TieuDe.Contains(searchString))
                    ||
                    (t.ChuDe != null &&
                     t.ChuDe.Contains(searchString))
                );
            }


            // LỌC CHỦ ĐỀ
            // Dùng khi bấm các channel bên trái
            // Ví dụ: Công nghệ, Giáo dục, Du lịch...
            if (!String.IsNullOrWhiteSpace(chuDe))
            {
                chuDe = chuDe.Trim();

                tinTucs = tinTucs.Where(t =>
                    t.ChuDe != null &&
                    t.ChuDe.Contains(chuDe)
                );
            }


            // Gửi lại dữ liệu sang View
            ViewBag.SearchString = searchString;
            ViewBag.ChuDe = chuDe;


            // Tin mới nhất nằm trên cùng
            return View(
                tinTucs
                    .OrderByDescending(t => t.NgayGui)
                    .ToList()
            );
        }



        // =====================================================
        // CHI TIẾT TIN TỨC
        // =====================================================
        public ActionResult Details(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }


            // Vì MaTinTuc trong SQL là NCHAR(10)
            // nên cần Trim khoảng trắng
            id = id.Trim();


            TinTuc tinTuc = db.TinTucs.Find(id);


            if (tinTuc == null)
            {
                return HttpNotFound();
            }


            return View(tinTuc);
        }



        // =====================================================
        // THÊM TIN - GET
        // =====================================================
        public ActionResult Create()
        {
            return View();
        }



        // =====================================================
        // THÊM TIN - POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "MaTinTuc,TieuDe,TomTat,NoiDung,NgayGui,NguoiGui,ChuDe")]
            TinTuc tinTuc,
            HttpPostedFileBase AnhUpload)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                if (AnhUpload != null &&
                    AnhUpload.ContentLength > 0)
                {
                    // Lấy tên file gốc
                    string originalFileName =
                        Path.GetFileName(
                            AnhUpload.FileName
                        );


                    // Thêm thời gian phía trước
                    // để tránh trùng tên ảnh
                    string fileName =
                        DateTime.Now.Ticks +
                        "_" +
                        originalFileName;


                    // Đường dẫn thư mục Uploads
                    string path =
                        Path.Combine(
                            Server.MapPath("~/Uploads/"),
                            fileName
                        );


                    // Lưu ảnh
                    AnhUpload.SaveAs(path);


                    // Lưu đường dẫn ảnh vào database
                    tinTuc.URLAnh =
                        "~/Uploads/" + fileName;
                }


                // Thêm vào database
                db.TinTucs.Add(tinTuc);

                db.SaveChanges();


                return RedirectToAction("Index");
            }


            return View(tinTuc);
        }



        // =====================================================
        // SỬA TIN - GET
        // =====================================================
        public ActionResult Edit(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }


            id = id.Trim();


            TinTuc tinTuc =
                db.TinTucs.Find(id);


            if (tinTuc == null)
            {
                return HttpNotFound();
            }


            return View(tinTuc);
        }



        // =====================================================
        // SỬA TIN - POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include =
                "MaTinTuc,TieuDe,TomTat,NoiDung,NgayGui,NguoiGui,URLAnh,ChuDe")]
            TinTuc tinTuc,
            HttpPostedFileBase AnhUpload)
        {
            if (ModelState.IsValid)
            {
                // Nếu người dùng chọn ảnh mới
                if (AnhUpload != null &&
                    AnhUpload.ContentLength > 0)
                {
                    string originalFileName =
                        Path.GetFileName(
                            AnhUpload.FileName
                        );


                    string fileName =
                        DateTime.Now.Ticks +
                        "_" +
                        originalFileName;


                    string path =
                        Path.Combine(
                            Server.MapPath("~/Uploads/"),
                            fileName
                        );


                    AnhUpload.SaveAs(path);


                    // Thay URL ảnh cũ bằng ảnh mới
                    tinTuc.URLAnh =
                        "~/Uploads/" + fileName;
                }


                // Nếu không upload ảnh mới
                // URLAnh cũ vẫn được giữ nhờ HiddenFor
                // trong Edit.cshtml


                db.Entry(tinTuc).State =
                    EntityState.Modified;


                db.SaveChanges();


                return RedirectToAction("Index");
            }


            return View(tinTuc);
        }



        // =====================================================
        // XÓA TIN - GET
        // =====================================================
        public ActionResult Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }


            id = id.Trim();


            TinTuc tinTuc =
                db.TinTucs.Find(id);


            if (tinTuc == null)
            {
                return HttpNotFound();
            }


            return View(tinTuc);
        }



        // =====================================================
        // XÓA TIN - POST
        // =====================================================
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }


            id = id.Trim();


            TinTuc tinTuc =
                db.TinTucs.Find(id);


            if (tinTuc != null)
            {
                db.TinTucs.Remove(tinTuc);

                db.SaveChanges();
            }


            return RedirectToAction("Index");
        }



        // =====================================================
        // GIẢI PHÓNG DATABASE CONTEXT
        // =====================================================
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