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
            string filePath = "C:\\Users\\ZEHRA\\OneDrive\\Masaüstü\\FH612 - 00217.bin";
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

            if (IsExits("0x000212A7", "0x000212C7", "A23650777.000002422933629.0000029", out string A))
                results.Add(A);

            if (IsExits("0x00023804", "0x00023813", "ATO1", out string B))
                results.Add(B);

            if (IsExits("0x00023824", "0x00023833", "ATI1", out string C))
                results.Add(C);

            if (IsExits("0x0002A616", "0x0002A624", "1g1Z1", out string D))
                results.Add(D);

            if (IsExits("0x001063A0", "0x001063AB", "VLE90FLBC030", out string E))
                results.Add(E);

            if (IsExits("0x00106698", "0x001066A3", "VLE90FLFE043", out string F))
                results.Add(F);

            if (IsExits("0x0010683C", "0x00106847", "VLE90FLFI030", out string G))
                results.Add(G);

            if (IsExits("0x00106DC0", "0x00106DCB", "VLE90FLFP043", out string H))
                results.Add(H);
            if (IsExits("0x00106F40", "0x001006F4B", "VLE90FLPV030", out string I))
                results.Add(I);
            if (IsExits("0x001071EE", "0x001071F9", "VLE90FLSL030", out string J))
                results.Add(J);
            if (IsExits("0x00109100", "0x00109142", "drivers_ems23", out string K))
                results.Add(K);
            if (IsExits("0x0010C9E8", "0x0010C9EF", "21364469", out string L))
                results.Add(L);
            if (IsExits("0x00060451", "0x00060455", "TR25B", out string M))
                results.Add(M);
            if (IsExits("0x00060000", "0x00060081", "v=1;a=24227123P01;p=P3163;d=2021-07-29;t=01:17:00;u=A322696;f=P3163_14B_2107_07.dst2.s3;b=EMS2:BSW.P.14B.2107.07;o=60000;;m=vision", out string N))
                results.Add(N);
            if (IsExits("0x000100000", "0x000100078", "v=1;a=24192452P01;p=P3163;d=2021-07-29;t=01:16:57;u=A322696;f=P3163_14B_2107_07.msw.s3;b=EMS2:BSW.P.14B.2107.07;o=100000", out string O))
                results.Add(O);
            if (IsExits("0x00104324", "0x001043CC", "../../../software/platform_framework/src/memory.c", out string P))
                results.Add(P);
            if (IsExits("0x001048D4", "0x00104ADE", "PRECONDITION_PMR_PendingChanges", out string R))
                results.Add(R);
            if (IsExits("0x001051E0", "0x0010535C", "PRECONDITION_SSC_getPropertyString", out string S))
                results.Add(S);
            if (IsExits("0x001056BC", "0x00105708", "eraseTicket", out string U))
                results.Add(U);
            if (IsExits("0x00108C9C", "0x00108D42", "BOOT_ECC_ERROR!!!!!!!!!", out string V))
                results.Add(V);
            if (IsExits("0x0010B6F8", "0x0010B7F0", "DDD../../../software/drivers_ems23/src/hwi_pit.c", out string W))
                results.Add(W);
            if (IsExits("0x0010CB98", "0x0010CBCE", "../../../software/nodemanager/src/nmgr_postmortem.c", out string X))
                results.Add(X);
            if (IsExits("0x0010D1D4", "0x0010D262", "../../../software/nodemanager/src/nodemanager.c", out string Y))
                results.Add(Y);
            if (IsExits("0x00134DF4", "0x00134E2A", "DDDsignal/src/signal", out string Z))
                results.Add(Z);
            if (IsExits("0x0014E1B2", "0x0014E216", "DD../../../software/aps/src/aps.c", out string AA))
                results.Add(AA);
            if (IsExits("0x0014F6A4", "0x0014F727", "DDD../../../software/aps/src/aps_common.c", out string BB))
                results.Add(BB);
            if (IsExits("0x001506E3", "0x00150719", "D../../../software/asi/src/ASIObject_type.c", out string CC))
                results.Add(CC);
            if (IsExits("0x0015A49C", "0x0015A4FF", "DDD../../../software/cswc/cswc-r75-d0A-ems23/src/vpt_romtest_callouts.c", out string DD))
                results.Add(DD);
            if (IsExits("0x0015B2E4", "0x0015B5BE", "D../../../software/diagcore/src/diagcore.c", out string EE))
                results.Add(EE);
            if (IsExits("0x0015BDB8", "0x0015BE63", "DD../../../software/diagcore/src/timestamp.c", out string FF))
                results.Add(FF);
            if (IsExits("0x0015DED0", "0x0015DF9E", "../../../software/drivers_ems23/src/hwi_eeprom.c", out string GG))
                results.Add(GG);
            if (IsExits("0x00161D3A", "0x00161D98", "software/injection/src/ValveHandler.c", out string HH))
                results.Add(HH);
            if (IsExits("0x0016EBCA", "0x0016EC24", "DDD../../../software/iso_diag/src/dtc_data.c", out string II))
                results.Add(II);
            if (IsExits("0x000602C4", "0x00060308", "6333NW0PH", out string JJ))
                results.Add(JJ);
            if (IsExits("0x00102F38", "0x00102FA3", "DDD../../../software/cswc/cswc-r75-d0A-ems23/src/vpt_security_access_callouts.c", out string KK))
                results.Add(KK);
            if (IsExits("0x00103CC4", "0x00103D5F", "DDD../../../software/platform_framework/src/parameter_manager.c", out string LL))
                results.Add(LL);
            if (IsExits("0x0010F23C", "0x0010F2CF", "../../../software/platform_framework/src/platform.c", out string MM))
                results.Add(MM);
            if (IsExits("0x0010F9A8", "0x0010FABA", "D../../../software/rhapsody_framework/src/mxf/RiCOSRtaOsek.c", out string NN))
                results.Add(NN);
            if (IsExits("0x001106BC", "0x001106FF", "DD../../../software/rhapsody_framework/src/mxf/RiCTypes.c", out string OO))
                results.Add(OO);
            if (IsExits("0x00113C54", "0x00113CA3", "DDD../../../software/signal/src/LookUpTable.c", out string PP))
                results.Add(PP);
            if (IsExits("0x0011FF34", "0x0011FFF0", "DDsignal/src/signal_d.c", out string RR))
                results.Add(RR);
            if (IsExits("0x001A66E9", "0x001A6760", "diagcore/src/dem_com.c", out string SS))
                results.Add(SS);
            if (IsExits("0x001A9074", "0x001A90F7", "DDD../../../software/diagcore/src/dem_idtcstate.c", out string TT))
                results.Add(TT);
            if (IsExits("0x001AA058", "0x001AA109", "DDD../../../software/diagcore/src/dem_inhibit.c", out string UU))
                results.Add(UU);
            if (IsExits("0x001AA260", "0x001AA2B4", "DDD../../../software/diagcore/src/dem_lamp.c", out string VV))
                results.Add(VV);
            if (IsExits("0x001AAB74", "0x001AAC46", "DD../../../software/diagcore/src/dem_monitors.c", out string WW))
                results.Add(WW);
            if (IsExits("0x001F70E8", "0x001F714B", "DDrhap-eecu/src/EngineOperationAnalysis_type.c", out string XX))
                results.Add(XX);
            if (IsExits("0x001F98A8", "0x001F98CD", "rhap-eecu/src/EngineOperationPkg.c", out string YY))
                results.Add(YY);
            if (IsExits("0x001F9DF4", "0x001F9E88", "Drhap-eecu/src/EngineProtectionSignal_type.c", out string ZZ))
                results.Add(ZZ);
            if (IsExits("0x001FAEEC", "0x001FAF40", "DDrhap-eecu/src/EngineProtection_type.c", out string AAA))
                results.Add(AAA);
            if (IsExits("0x001FB874", "0x001FB997", "rhap-eecu/src/EngineRecorder_type.c", out string BBB))
                results.Add(BBB);
            if (IsExits("0x001FC368", "0x001FC3CD", "Drhap-eecu/src/EngineSampleFrame_type.c", out string CCC))
                results.Add(CCC);
            if (IsExits("0x001FCBB4", "0x001FCBED", "DDrhap-eecu/src/EngineSampler_type.c", out string DDD))
                results.Add(DDD);
            if (IsExits("0x001FECD0", "0x001FED4D", "DDDrhap-eecu/src/FootprintDemander_type.c", out string EEE))
                results.Add(EEE);
            if (IsExits("0x000C0000", "0x000C0007", "VOLVOECU", out string a))
                results.Add(a);
            if (IsExits("0x000C04F0", "0x000C04FA", "22424240P01", out string b))
                results.Add(b);
            if (IsExits("0x000CC8AC", "0x000CC986", "ISO_DIAG_INFO_FOR_APPL", out string c))
                results.Add(c);
            if (IsExits("0x000D1C98", "0x000D1CA3", "VLE90FLFE044", out string d))
                results.Add(d);
            if (IsExits("0x000D1E34", "0x000D1E3F", "VLE90FLFI044", out string e))
                results.Add(e);
            if (IsExits("0x000D23A4", "0x000D23AF", "VLE90FLFP044", out string f))
                results.Add(f);
            if (IsExits("0x00040000", "0x00040076", "v=1;a=24192454P01;p=P3163;d=2021-07-29;t=01:17:01;u=A322696;", out string g))
                results.Add(g);
            if (IsExits("0x00060100", "0x0006010F", "761266VOLVO", out string h))
                results.Add(h);
            if (IsExits("0x00060120", "0x00060130", "YV2RT6", out string ı))
                results.Add(ı);
            if (IsExits("0x00060180", "0x0006019F", "23650777.000002422933629.0000029", out string i))
                results.Add(i);
            if (IsExits("0x000D26B8", "0x000D26C3", "VLE90FLGL045", out string j))
                results.Add(j);
            if (IsExits("0x000D295E", "0x000D2969", "VLE90FLSL044", out string k))
                results.Add(k);
            if (IsExits("0x0001B3138", "0x0001B3174", "FALSE", out string l))
                results.Add(l);
            if (IsExits("0x0001EA780", "0x0001EA7B6", "DDrhap-eecu", out string m))
                results.Add(m);
            if (IsExits("0x0001F3402", "0x0001F3439", "DDrhap-eecu/src", out string n))
                results.Add(n);
            if (IsExits("0x0001F38EA", "0x0001F3915", "DDrhap-eecu/src/EgrMass", out string o))
                results.Add(o);
            if (IsExits("0x0001F4A4A", "0x0001F4A86", "DDDrhap-eecu/src/EngineConfig", out string p))
                results.Add(p);
            if (IsExits("0x0001716B4", "0x0001716EB", "software/nodemanager/src/nmgr", out string r))
                results.Add(r);
            if (IsExits("0x000178550", "0x000178590", "software/rhapsody", out string s))
                results.Add(s);
            if (IsExits("0x000178BDC", "0x000178C19", "/src/mxf/RiCList", out string t))
                results.Add(t);
            if (IsExits("0x000178CEC", "0x000178D15", "Loop", out string v))
                results.Add(v);
            if (IsExits("0x00019712A", "0x00019714C", "DDrhap-eecu/src/VehicleSpeed", out string y))
                results.Add(y);
            if (IsExits("0x000197840", "0x00019786E", "DDrhap-eecu/src/WiggleTest", out string z))
                results.Add(z);
            if (IsExits("0x0001AB32", "0x0001AB50C", "HealConfirmedDuration", out string aa))
                results.Add(aa);
            if (IsExits("0x001ABE34", "0x001ABECF", "GetReadinessSet", out string bb))
                results.Add(bb);
            if (IsExits("0x001B21F4", "0x001B230A", "CYLINDERS", out string cc))
                results.Add(cc);
            if (IsExits("0x00380000", "0x00380082", "v=1;a=24227136P01", out string dd))
                results.Add(dd);
            if (IsExits("0x0033FEDC", "0x0033FEE6", "21976516P01", out string ee))
                results.Add(ee);
            if (IsExits("0x002123D6", "0x002123F9", "DDrhap-eecu/src/UreaSysSuper", out string ff))
                results.Add(ff);
            if (IsExits("0x0020597C", "0x002059B8", "ApplicationPtr", out string gg))
                results.Add(gg);
            if (IsExits("0x00203550", "0x0020358F", "GearboxApplication", out string jj))
                results.Add(jj);
            if (IsExits("0x00200510", "0x002005BE", "itsApplication", out string ii))
                results.Add(ii);

            return results;
        }

        public string aramaListe()
        {
            List<string> searchResults = Search();
            return FormatResults(searchResults);
        }

        public static string FormatResults(List<string> results)
        {
            return string.Join(Environment.NewLine, results);
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