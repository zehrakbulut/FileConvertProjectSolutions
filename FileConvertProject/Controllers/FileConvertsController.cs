using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileConvertProject.Data;
using FileConvertProject.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Components.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections;

namespace FileConvertProject.Controllers
{
    public class FileConvertsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public FileConvertsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: FileConverts
        public async Task<IActionResult> Index()
        {
            return _context.fileConverts != null ?
                        View(await _context.fileConverts.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.fileConverts'  is null.");
        }

        // GET: FileConverts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.fileConverts == null)
            {
                return NotFound();
            }

            var fileConvert = await _context.fileConverts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileConvert == null)
            {
                return NotFound();
            }

            return View(fileConvert);
        }

        // GET: FileConverts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FileConverts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Information,SoftwareNumber,HardwareNumber,Date,Username,Password")] FileConvert fileConvert)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fileConvert);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fileConvert);
        }

        // GET: FileConverts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.fileConverts == null)
            {
                return NotFound();
            }

            var fileConvert = await _context.fileConverts.FindAsync(id);
            if (fileConvert == null)
            {
                return NotFound();
            }
            return View(fileConvert);
        }
        public IActionResult UploadFile(int? id)
        {
            var fileConvert = _context.fileConverts.Find(id);
            return View(fileConvert);
        }


        [HttpPost]
        public IActionResult UploadFile(FileConvert fileconvert)
        {

            var fileC = _context.fileConverts.FirstOrDefault(c => c.Id == fileconvert.Id);
            string webrootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            if (files.Count > 0)
            {
                string fileNamex = Guid.NewGuid().ToString();
                var uploads = Path.Combine(webrootPath, @"Files");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, fileNamex + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                fileC.FilePath = @"Files\" + fileNamex + extension;
                _context.fileConverts.Update(fileC);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }


        public string ConvertToBinaryAndChangeExtension()
        {
            string uploadedFilePath = "C:\\Users\\ZEHRA\\OneDrive\\Masaüstü\\FH612 - 00217.dtf"; // Yüklenen dosyanın yolu

            try
            {
                byte[] fileContent = System.IO.File.ReadAllBytes(uploadedFilePath); // Dosyanın içeriğini binary olarak okuyun

                // Dosyanın uzantısını .bin olarak değiştirin
                string newFilePath = Path.ChangeExtension(uploadedFilePath, ".bin");

                // Dosyanın içeriği binary olarak yeni dosyaya yazılıyor
                System.IO.File.WriteAllBytes(newFilePath, fileContent);
                System.IO.File.Delete(uploadedFilePath);

                return "Dosyanın içeriği başarıyla binary formatına dönüştürüldü ve uzantısı .bin olarak değiştirildi.";
            }
            catch (Exception ex)
            {
                return "Hata oluştu: " + ex.Message;
            }
        }
        //public void changeExtension()
        //{
        //    string dosyaYolu = "C:\\Users\\ZEHRA\\OneDrive\\Masaüstü\\FH612 - 00217 - Kopya.dtf";
        //    string yeniUzanti = ".bin";

        //    // Dosyanın var olup olmadığını kontrol ediyoruz.
        //    if (System.IO.File.Exists(dosyaYolu))
        //    {
        //        try
        //        {
        //            // Yeni dosya yolu oluşturuluyor
        //            string yeniDosyaYolu = Path.ChangeExtension(dosyaYolu, yeniUzanti);
        //            // Dosyanın uzantısı değiştiriliyor
        //            System.IO.File.Move(dosyaYolu, yeniDosyaYolu);

        //            byte[] dosyaVerisi = System.IO.File.ReadAllBytes(yeniDosyaYolu);

        //        }
        //        catch (Exception ex)
        //        {
        //            // Hata yönetimi burada yapılmalı
        //            Console.WriteLine("Hata oluştu: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Dosya bulunamadı.");
        //    }
        //}

        public static bool IsBetweenAddresses(string target, string start, string end)
        {
            return string.Compare(target, start) >= 0 && string.Compare(target, end) <= 0;
        }

        public static bool IsExits(string startAddress, string endAddress, string targetValue, out string resultMessage)
        {
            string filePath = "C:\\Users\\ZEHRA\\OneDrive\\Masaüstü\\FH612 - 00217 - Kopya.bin";
            string dataInRange = ReadDataInRange(filePath, startAddress, endAddress);
            resultMessage = "";

            if (dataInRange.Contains(targetValue))
            {
                resultMessage = targetValue;
                return true;
            }

            resultMessage = "Hedef adres aralığında bulunmuyor.";
            return false;
        }

        public static string ReadDataInRange(string filePath, string startAddress, string endAddress)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                long start = Convert.ToInt64(startAddress, 16);
                long end = Convert.ToInt64(endAddress, 16);
                long lengthToRead = end - start + 1; // Okunacak verinin uzunluğu
                byte[] buffer = new byte[lengthToRead]; // Veriyi tutmak için bir arabellek oluştur
                fs.Seek(start, SeekOrigin.Begin); // Başlangıç konumuna git
                fs.Read(buffer, 0, buffer.Length); // Veriyi oku
                return Encoding.ASCII.GetString(buffer); // Byte dizisini ASCII metin formatına çevir
            }
        }

        public static List<string> Search()
        {
            List<string> results = new List<string>();

            if (IsExits("0x00000000", "0x0000017F", "81454A0701201616012924", out string software))
                results.Add(software);

            if (IsExits("0x0000017F", "0x0000019F", "22424242P0122423434P0100000", out string hardware))
                results.Add(hardware);

            if (IsExits("0x00003AE0", "0x00003AEF", "VOLVOECU", out string brand))
                results.Add(brand);

            if (IsExits("0x00000FFF", "0x0000101F", "2010-JUL-14", out string date))
                results.Add(date);

            return results;
        }

        public List<string> aramaListe()
        {
            List<string> searchResults = Search();

            return searchResults;
        }


        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Information,SoftwareNumber,HardwareNumber,Date,FilePath,Username,Password")] FileConvert fileConvert)
        {
            if (id != fileConvert.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fileConvert);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileConvertExists(fileConvert.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fileConvert);
        }

        // GET: FileConverts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.fileConverts == null)
            {
                return NotFound();
            }

            var fileConvert = await _context.fileConverts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileConvert == null)
            {
                return NotFound();
            }

            return View(fileConvert);
        }

        // POST: FileConverts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.fileConverts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.fileConverts'  is null.");
            }
            var fileConvert = await _context.fileConverts.FindAsync(id);
            if (fileConvert != null)
            {
                _context.fileConverts.Remove(fileConvert);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileConvertExists(int id)
        {
            return (_context.fileConverts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}