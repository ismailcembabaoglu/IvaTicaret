﻿using IvaETicaret.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using IvaETicaret.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IvaETicaret.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles=Diger.Role_Bayi)]
    public class BayiReportController : Controller
    {
        private readonly ApplicationDbContext _context;


        public BayiReportController(ApplicationDbContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            ReportFiterAdminVM reportFiterAdminVM = new ReportFiterAdminVM();
            reportFiterAdminVM.BitisDate = DateTime.Now.Date;
            reportFiterAdminVM.BaslangicDate = DateTime.Now.Date;
            return View(reportFiterAdminVM);
        }
        public IActionResult DayReport()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
            var report = _context.OrderHeaders.Where(c => c.OrderStatus == Diger.Durum_TeslimEdildi && c.OrderDate.Date == DateTime.Now.Date && c.StoreId == storeId).Include(c => c.Adress).Include(c => c.ApplicationUser).Include(c => c.OdemeTur).Include(c => c.Store).ToList();
            var ToplamTutar = _context.OrderHeaders.Where(c => c.OrderStatus == Diger.Durum_TeslimEdildi && c.OrderDate.Date == DateTime.Now.Date && c.StoreId == storeId).Sum(c => c.OrderTotal);
            ReportVm rpt = new ReportVm();
            rpt.OrderHeaders = report;
            rpt.ToplamSatisTutar = ToplamTutar;
            rpt.AlinacakTutar = (ToplamTutar * 10) / 100;

            string a = Guid.NewGuid().ToString() + ".pdf";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdfreports/" + a);
            var stream = new FileStream(path, FileMode.Create);
            Document document = new Document(PageSize.A4);

            PdfWriter.GetInstance(document, stream);
            document.Open();
            Paragraph paragraph = new Paragraph("Günlük Satış Raporu(Tüm Bayiler)");
            Paragraph paragraph2 = new Paragraph("           ");

            PdfPTable pdfPTable = new PdfPTable(8);
            pdfPTable.AddCell("Id");
            pdfPTable.AddCell("Kullanıcı Adı");
            pdfPTable.AddCell("Tarih");
            pdfPTable.AddCell("Statüs");
            pdfPTable.AddCell("İsim Soyisim");
            pdfPTable.AddCell("Ödeme Türü");
            pdfPTable.AddCell("Mağaza Adı");
            pdfPTable.AddCell("Toplam Tutar");
            foreach (var item in rpt.OrderHeaders)
            {
                pdfPTable.AddCell(item.Id.ToString());
                pdfPTable.AddCell(item.ApplicationUser.Email);
                pdfPTable.AddCell(item.OrderDate.ToString("dd/MM/yyyy"));
                pdfPTable.AddCell(item.OrderStatus);
                pdfPTable.AddCell(item.Name + " " + item.SurName);
                pdfPTable.AddCell(item.OdemeTur.Name);
                pdfPTable.AddCell(item.Store.CompanyName);
                pdfPTable.AddCell(item.OrderTotal.ToString());
            }
            PdfPTable pdfPTable2 = new PdfPTable(2);
            pdfPTable2.AddCell("Genel Toplam");
            pdfPTable2.AddCell("Ödenecek Tutar");
            pdfPTable2.AddCell(rpt.ToplamSatisTutar.ToString() + " " + "TL");
            pdfPTable2.AddCell(rpt.AlinacakTutar.ToString() + " " + "TL");

            document.Add(paragraph);
            document.Add(paragraph2);
            document.Add(pdfPTable);
            document.Add(pdfPTable2);
            document.Close();

            return File("/pdfreports/" + a, "application/pdf", a);
        }
        public IActionResult Monthly()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
            var report = _context.OrderHeaders.Where(c => c.OrderStatus == Diger.Durum_TeslimEdildi && c.OrderDate.Date.Month == DateTime.Now.Date.Month && c.StoreId==storeId).Include(c => c.Adress).Include(c => c.ApplicationUser).Include(c => c.OdemeTur).Include(c => c.Store).ToList();
            var ToplamTutar = _context.OrderHeaders.Where(c => c.OrderStatus == Diger.Durum_TeslimEdildi && c.OrderDate.Date.Month == DateTime.Now.Date.Month && c.StoreId == storeId).Sum(c => c.OrderTotal);
            ReportVm rpt = new ReportVm();
            rpt.OrderHeaders = report;
            rpt.ToplamSatisTutar = ToplamTutar;
            rpt.AlinacakTutar = (ToplamTutar * 10) / 100;

            string a = Guid.NewGuid().ToString() + ".pdf";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdfreports/" + a);
            var stream = new FileStream(path, FileMode.Create);
            Document document = new Document(PageSize.A4);

            PdfWriter.GetInstance(document, stream);
            document.Open();
            Paragraph paragraph = new Paragraph("Aylık Satış Raporu(Tüm Bayiler)");
            Paragraph paragraph2 = new Paragraph("           ");

            PdfPTable pdfPTable = new PdfPTable(8);
            pdfPTable.AddCell("Id");
            pdfPTable.AddCell("Kullanıcı Adı");
            pdfPTable.AddCell("Tarih");
            pdfPTable.AddCell("Statüs");
            pdfPTable.AddCell("İsim Soyisim");
            pdfPTable.AddCell("Ödeme Türü");
            pdfPTable.AddCell("Mağaza Adı");
            pdfPTable.AddCell("Toplam Tutar");
            foreach (var item in rpt.OrderHeaders)
            {
                pdfPTable.AddCell(item.Id.ToString());
                pdfPTable.AddCell(item.ApplicationUser.Email);
                pdfPTable.AddCell(item.OrderDate.ToString("dd/MM/yyyy"));
                pdfPTable.AddCell(item.OrderStatus);
                pdfPTable.AddCell(item.Name + " " + item.SurName);
                pdfPTable.AddCell(item.OdemeTur.Name);
                pdfPTable.AddCell(item.Store.CompanyName);
                pdfPTable.AddCell(item.OrderTotal.ToString());
            }
            PdfPTable pdfPTable2 = new PdfPTable(2);
            pdfPTable2.AddCell("Genel Toplam");
            pdfPTable2.AddCell("Ödenecek Tutar");
            pdfPTable2.AddCell(rpt.ToplamSatisTutar.ToString() + " " + "TL");
            pdfPTable2.AddCell(rpt.AlinacakTutar.ToString() + " " + "TL");

            document.Add(paragraph);
            document.Add(paragraph2);
            document.Add(pdfPTable);
            document.Add(pdfPTable2);
            document.Close();

            return File("/pdfreports/" + a, "application/pdf", a);
        }
        public IActionResult Year()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
            var report = _context.OrderHeaders.Where(c => c.OrderStatus == Diger.Durum_TeslimEdildi && c.OrderDate.Date.Year == DateTime.Now.Date.Year && c.StoreId == storeId).Include(c => c.Adress).Include(c => c.ApplicationUser).Include(c => c.OdemeTur).Include(c => c.Store).ToList();
            var ToplamTutar = _context.OrderHeaders.Where(c => c.OrderStatus == Diger.Durum_TeslimEdildi && c.OrderDate.Date.Year == DateTime.Now.Date.Year && c.StoreId == storeId).Sum(c => c.OrderTotal);
            ReportVm rpt = new ReportVm();
            rpt.OrderHeaders = report;
            rpt.ToplamSatisTutar = ToplamTutar;
            rpt.AlinacakTutar = (ToplamTutar * 10) / 100;

            string a = Guid.NewGuid().ToString() + ".pdf";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdfreports/" + a);
            var stream = new FileStream(path, FileMode.Create);
            Document document = new Document(PageSize.A4);

            PdfWriter.GetInstance(document, stream);
            document.Open();
            Paragraph paragraph = new Paragraph("Yıllık Satış Raporu(Tüm Bayiler)");
            Paragraph paragraph2 = new Paragraph("           ");

            PdfPTable pdfPTable = new PdfPTable(8);
            pdfPTable.AddCell("Id");
            pdfPTable.AddCell("Kullanıcı Adı");
            pdfPTable.AddCell("Tarih");
            pdfPTable.AddCell("Statüs");
            pdfPTable.AddCell("İsim Soyisim");
            pdfPTable.AddCell("Ödeme Türü");
            pdfPTable.AddCell("Mağaza Adı");
            pdfPTable.AddCell("Toplam Tutar");
            foreach (var item in rpt.OrderHeaders)
            {
                pdfPTable.AddCell(item.Id.ToString());
                pdfPTable.AddCell(item.ApplicationUser.Email);
                pdfPTable.AddCell(item.OrderDate.ToString("dd/MM/yyyy"));
                pdfPTable.AddCell(item.OrderStatus);
                pdfPTable.AddCell(item.Name + " " + item.SurName);
                pdfPTable.AddCell(item.OdemeTur.Name);
                pdfPTable.AddCell(item.Store.CompanyName);
                pdfPTable.AddCell(item.OrderTotal.ToString());
            }
            PdfPTable pdfPTable2 = new PdfPTable(2);
            pdfPTable2.AddCell("Genel Toplam");
            pdfPTable2.AddCell("Ödenecek Tutar");
            pdfPTable2.AddCell(rpt.ToplamSatisTutar.ToString() + " " + "TL");
            pdfPTable2.AddCell(rpt.AlinacakTutar.ToString() + " " + "TL");

            document.Add(paragraph);
            document.Add(paragraph2);
            document.Add(pdfPTable);
            document.Add(pdfPTable2);
            document.Close();

            return File("/pdfreports/" + a, "application/pdf", a);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(ReportFilterCustomerVM reportFiterAdminVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
            IQueryable<OrderHeader>? orders;
            ReportVm rpt = new ReportVm();
            if (String.IsNullOrEmpty( reportFiterAdminVM.Name)&& String.IsNullOrEmpty(reportFiterAdminVM.Surname))
            {
                orders = _context.OrderHeaders
                                .Include(c => c.Adress)
                                .Include(c => c.Store)
                                .Include(c => c.ApplicationUser)
                                .Include(c => c.OdemeTur)
                                .Where(c =>
                                c.OrderStatus == reportFiterAdminVM.OrderStatus &&
                                c.OrderDate.Date >= reportFiterAdminVM.BaslangicDate.Date && c.OrderDate.Date <= reportFiterAdminVM.BitisDate.Date &&
                                c.StoreId == storeId);
                
                rpt.OrderHeaders = orders.ToList();
                rpt.ToplamSatisTutar = orders.Sum(c => c.OrderTotal);
                rpt.AlinacakTutar = (orders.Sum(c => c.OrderTotal) * 10) / 100;
            }
            else if (!String.IsNullOrEmpty(reportFiterAdminVM.Name) && !String.IsNullOrEmpty(reportFiterAdminVM.Surname))
            {
                orders = _context.OrderHeaders
                               .Include(c => c.Adress)
                               .Include(c => c.Store)
                               .Include(c => c.ApplicationUser)
                               .Include(c => c.OdemeTur)
                               .Where(c =>
                               c.OrderStatus == reportFiterAdminVM.OrderStatus &&
                               c.OrderDate.Date >= reportFiterAdminVM.BaslangicDate.Date && c.OrderDate.Date <= reportFiterAdminVM.BitisDate.Date &&
                               c.StoreId == storeId && c.Name.Contains(reportFiterAdminVM.Name) && c.SurName.Contains(reportFiterAdminVM.Surname));

                rpt.OrderHeaders = orders.ToList();
                rpt.ToplamSatisTutar = orders.Sum(c => c.OrderTotal);
                rpt.AlinacakTutar = (orders.Sum(c => c.OrderTotal) * 10) / 100;
            }
            else if (!String.IsNullOrEmpty(reportFiterAdminVM.Name) && String.IsNullOrEmpty(reportFiterAdminVM.Surname))
            {
                orders = _context.OrderHeaders
                               .Include(c => c.Adress)
                               .Include(c => c.Store)
                               .Include(c => c.ApplicationUser)
                               .Include(c => c.OdemeTur)
                               .Where(c =>
                               c.OrderStatus == reportFiterAdminVM.OrderStatus &&
                               c.OrderDate.Date >= reportFiterAdminVM.BaslangicDate.Date && c.OrderDate.Date <= reportFiterAdminVM.BitisDate.Date &&
                               c.StoreId == storeId && c.Name.Contains(reportFiterAdminVM.Name));

                rpt.OrderHeaders = orders.ToList();
                rpt.ToplamSatisTutar = orders.Sum(c => c.OrderTotal);
                rpt.AlinacakTutar = (orders.Sum(c => c.OrderTotal) * 10) / 100;
            }
            else if (String.IsNullOrEmpty(reportFiterAdminVM.Name) && !String.IsNullOrEmpty(reportFiterAdminVM.Surname))
            {
                orders = _context.OrderHeaders
                               .Include(c => c.Adress)
                               .Include(c => c.Store)
                               .Include(c => c.ApplicationUser)
                               .Include(c => c.OdemeTur)
                               .Where(c =>
                               c.OrderStatus == reportFiterAdminVM.OrderStatus &&
                               c.OrderDate.Date >= reportFiterAdminVM.BaslangicDate.Date && c.OrderDate.Date <= reportFiterAdminVM.BitisDate.Date &&
                               c.StoreId == storeId && c.SurName.Contains(reportFiterAdminVM.Surname));

                rpt.OrderHeaders = orders.ToList();
                rpt.ToplamSatisTutar = orders.Sum(c => c.OrderTotal);
                rpt.AlinacakTutar = (orders.Sum(c => c.OrderTotal) * 10) / 100;
            }



            string a = Guid.NewGuid().ToString() + ".pdf";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdfreports/" + a);
            var stream = new FileStream(path, FileMode.Create);
            Document document = new Document(PageSize.A4);

            PdfWriter.GetInstance(document, stream);
            document.Open();
            document.AddTitle(reportFiterAdminVM.BaslangicDate.Date.ToString() + "-" + reportFiterAdminVM.BitisDate.Date.ToString() + " Tarihleri Arası Satış Raporu");
            Paragraph paragraph = new Paragraph(reportFiterAdminVM.BaslangicDate.Date.ToString() + "-" + reportFiterAdminVM.BitisDate.Date.ToString() + " Tarihleri Arası Satış Raporu");
            Paragraph paragraph2 = new Paragraph("           ");
            PdfPTable pdfPTable = new PdfPTable(8);
            pdfPTable.AddCell("Id");
            pdfPTable.AddCell("Kullanıcı Adı");
            pdfPTable.AddCell("Tarih");
            pdfPTable.AddCell("Statüs");
            pdfPTable.AddCell("İsim Soyisim");
            pdfPTable.AddCell("Ödeme Türü");
            pdfPTable.AddCell("Mağaza Adı");
            pdfPTable.AddCell("Toplam Tutar");
            foreach (var item in rpt.OrderHeaders)
            {
                pdfPTable.AddCell(item.Id.ToString());
                pdfPTable.AddCell(item.ApplicationUser.Email);
                pdfPTable.AddCell(item.OrderDate.ToString("dd/MM/yyyy"));
                pdfPTable.AddCell(item.OrderStatus);
                pdfPTable.AddCell(item.Name + " " + item.SurName);
                pdfPTable.AddCell(item.OdemeTur.Name);
                pdfPTable.AddCell(item.Store.CompanyName);
                pdfPTable.AddCell(item.OrderTotal.ToString());
            }
            PdfPTable pdfPTable2 = new PdfPTable(2);
            pdfPTable2.AddCell("Genel Toplam");
            pdfPTable2.AddCell("Ödenecek Tutar");
            pdfPTable2.AddCell(rpt.ToplamSatisTutar.ToString() + " " + "TL");
            pdfPTable2.AddCell(rpt.AlinacakTutar.ToString() + " " + "TL");

            document.Add(paragraph);
            document.Add(paragraph2);
            document.Add(pdfPTable);
            document.Add(pdfPTable2);
            document.Close();

            return File("/pdfreports/" + a, "application/pdf", a);
        }
    }
}
