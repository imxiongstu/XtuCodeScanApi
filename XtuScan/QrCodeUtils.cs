using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using ZXing;
using ZXing.Common;

namespace XtuScan
{
    //注意bitmap是image的派生类型，并且都是引用地址传递！！！会影响全局
    public static class QrCodeUtils
    {
        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="codeImg">二维码图片</param>
        /// <returns></returns>
        public static string AnalysisQrCode(Image codeImg)
        {
            Bitmap bmp = CodeImageHandler(codeImg);
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


            return (codes == null) ? "识别失败，请重新调整" : codes[0].Text;
        }



        /// <summary>
        /// 码图处理器
        /// </summary>
        /// <param name="codeImg"></param>
        /// <returns></returns>
        public static Bitmap CodeImageHandler(Image codeImg)
        {
            //变成灰度图
            ToGray((Bitmap)codeImg);

            Bitmap bmp = null;

            //缩放率
            int zoom = 768;
            //强制图片为768px
            if (codeImg.Width > zoom)
            {
                int zoomMultiple = codeImg.Width / zoom;
                int toWidth = zoom;
                int toHeight = codeImg.Height / zoomMultiple;
                bmp = new Bitmap(toWidth, toHeight);
                //重新绘制二维码图，bmp引用传递，graphics相当于是基于bmp进行的绘制，会影响到bmp本身。bmp相当于画板，在画板基础上进行绘制。
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    Rectangle srcRect = new Rectangle(0, 0, codeImg.Width, codeImg.Height);//需要裁剪的原图像位置

                    Rectangle destRect = new Rectangle(0, 0, toWidth, toHeight);//处理以后呈现出来的图像

                    g.DrawImage(codeImg, destRect, srcRect, GraphicsUnit.Pixel);
                }
            }
            else
            {
                bmp = (Bitmap)codeImg;
            }
            return bmp;
        }


        /// <summary>
        /// 转为灰度图
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp)
        {
            if (bmp == null)
            {
                return null;
            }
            int w = bmp.Width;
            int h = bmp.Height;
            try
            {
                byte newColor = 0;
                BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                //unsafe为的是解除安全限制，加快for速度
                unsafe
                {
                    byte* p = (byte*)srcData.Scan0.ToPointer();
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            newColor = (byte)((float)p[0] * 0.114f + (float)p[1] * 0.587f + (float)p[2] * 0.299f);
                            p[0] = newColor;
                            p[1] = newColor;
                            p[2] = newColor;

                            p += 3;
                        }
                        p += srcData.Stride - w * 3;
                    }
                    bmp.UnlockBits(srcData);
                    return bmp;
                }
            }
            catch
            {
                return null;
            }

        }
    }
}