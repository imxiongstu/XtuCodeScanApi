using System;
using System.Drawing;
using ZXing;
using ZXing.Common;

namespace XtuScan
{
    public static class QrCodeUtils
    {
        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="barcodeBitmap">二维码图片</param>
        /// <returns></returns>
        public static string AnalysisQrCode(Image qrcodeImg)
        {
            Bitmap bmp = qrcodeImg as Bitmap;

            //将图片进行简单定位，并将像素变成512px
            if (qrcodeImg.Width > 512)
            {
                int zoomMultiple = qrcodeImg.Width / 512;
                int toWidth = 512;
                int toHeight = qrcodeImg.Height / zoomMultiple;
                bmp = new Bitmap(toWidth, toHeight);
                //重新绘制二维码图
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    //设置高平滑度
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.Clear(Color.Transparent);
                    g.DrawImage(qrcodeImg,
                                new Rectangle(0, 0, toWidth, toHeight),
                                new Rectangle(0, 0, qrcodeImg.Width, qrcodeImg.Height),
                                GraphicsUnit.Pixel);
                    qrcodeImg.Dispose();
                }

            }
            //创建二维码元阅读器，设置简单自动调整
            var codes = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions
                {
                    TryHarder = true
                }
            }.DecodeMultiple(bmp);


            return (codes[0].Text == null) ? "识别失败，请重新调整" : codes[0].Text;
        }
    }
}
