using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using ZXing;
using ZXing.Common;

namespace XtuScan
{
    public static class QrCodeUtils
    {
        /// <summary>
        /// 解析码图
        /// </summary>
        /// <param name="codeImg">图片</param>
        /// <returns></returns>
        public static string AnalysisCodeImage(Image codeImg)
        {
            #region 单次操作代码
            //Bitmap bmp = CodeImageHandler(codeImg,512f);
            ////创建二维码元阅读器，设置简单自动调整
            //var codes = new BarcodeReader
            //{
            //    AutoRotate = true,
            //    TryInverted = true,
            //    Options = new DecodingOptions
            //    {
            //        TryHarder = true
            //    }
            //}.DecodeMultiple(bmp);

            //return (codes == null) ? "识别失败，请重新调整" : codes[0].Text;
            #endregion


            string reslut = null;
            float pixel = 512f;

            //不同分辨率，循环5次查询
            for (int i = 0; i < 5; i++)
            {
                Bitmap bmp = CodeImageHandler(codeImg, pixel);

                var codes = new BarcodeReader
                {
                    AutoRotate = true,
                    TryInverted = true,
                    Options = new DecodingOptions
                    {
                        TryHarder = true
                    }
                }.DecodeMultiple(bmp);

                if (codes == null)
                {
                    pixel += 128f;
                    continue;
                }
                else
                {
                    reslut = codes[0].Text;
                    break;
                }
            }
            return (reslut == null) ? "识别失败，请重新调整" : reslut;
        }



        /// <summary>
        /// 图片处理器
        /// </summary>
        /// <param name="codeImg">图片</param>
        /// <param name="pixel">需要变成的像素</param>
        /// <param name="rotateAngle">旋转角度</param>
        /// <returns></returns>
        public static Bitmap CodeImageHandler(Image codeImg, float pixel)
        {
            //变成灰度图
            ToGray((Bitmap)codeImg);
            Bitmap bmp = null;

            if (codeImg.Width > pixel)
            {
                float zoomMultiple = codeImg.Width / pixel;
                int toWidth = (int)pixel;
                int toHeight = (int)(codeImg.Height / zoomMultiple);
                bmp = new Bitmap(toWidth, toHeight);
                //重新绘制二维码图，bmp引用传递，graphics相当于是基于bmp进行的绘制，会影响到bmp本身。bmp相当于画板，在画板基础上进行绘制。
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.Clear(Color.Gray);

                    ///调整旋转角度，提高识别率，有用，但耗费太多时间
                    ////移动旋转中心点
                    //g.TranslateTransform(toWidth / 2, toHeight / 2);
                    //g.RotateTransform(rotateAngle);
                    ////将中心点还原
                    //g.TranslateTransform(-toWidth / 2, -toHeight / 2);

                    Rectangle srcRect = new Rectangle(0, 0, codeImg.Width, codeImg.Height);//需要裁剪的原图像位置
                    // Rectangle destRect = new Rectangle(-300, -100, (int)(toWidth * 2f), (int)(toHeight * 2f));//处理以后呈现出来的图像
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
        /// 图片切割
        /// </summary>
        /// <param name="codeImg"></param>
        /// <param name="sliceCount"></param>
        /// <returns></returns>
        public static Bitmap[] ToSlice(Bitmap codeImg, int sliceCount)
        {
            return null;
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