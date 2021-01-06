using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ZXing;
using System.IO;
using XtuScan;
using Microsoft.AspNetCore.Hosting;
using ZXing.Common;

namespace QrCodeScanAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("DIYCors")]
    public class ScanController : ControllerBase
    {

        [HttpPost]
        public string AnalysisQrCode()
        {
            if (HttpContext.Request.Form.Files.Count <= 0) return "没有选择文件";

            var imgFile = HttpContext.Request.Form.Files[0];

            try
            {
                Image imagePic = Image.FromStream(imgFile.OpenReadStream());
                return QrCodeUtils.AnalysisQrCode(imagePic);
            }
            catch (Exception)
            {
                return "识别失败，请重新调整";
            }
        }
    }
}

