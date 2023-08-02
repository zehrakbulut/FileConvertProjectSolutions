using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FileConvertProject.Data;
using FileConvertProject.Models;
using System.Drawing.Printing;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Runtime.ConstrainedExecution;
using Microsoft.Extensions.Hosting;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Linq;

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



            _context.fileConverts.Update(fileC);
            _context.SaveChanges();
            var carList = _context.fileConverts.ToList();
            return View("Index", carList);
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
