using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Cognex.VisionPro;
using HalconDotNet;

namespace HalconOperate;

public class Operate
{
    public static void ImageConvertVisionPro2HObject(object image, out HObject hoImage)
    {
        hoImage = null;
        try
        {
            ICogImage Image = (ICogImage)image;
            int Width = Image.Width;
            int Height = Image.Height;
            if (Image.GetType().Name == "CogImage24PlanarColor")
            {
                //用RGB格式变量接收图像
                CogImage24PlanarColor RGBImage = (CogImage24PlanarColor)Image;

                //获取R G B 三通道像素指针信息
                ICogImage8PixelMemory Rptr, Gptr, Bptr;
                RGBImage.Get24PlanarColorPixelMemory(CogImageDataModeConstants.Read, 0, 0, Width, Height, out Rptr,
                    out Gptr, out Bptr);

                //创建Halcon图像变量
                if (Gptr.Stride == Gptr.Width)
                {
                    //halcon合成彩色图像
                    HOperatorSet.GenImage3(out hoImage, "byte", Width, Height, Rptr.Scan0, Gptr.Scan0, Bptr.Scan0);
                }
                else //图像不被4整除（直接合成图像会乱）
                {
                    //创建3个bitmap格式变量，接收R G B 三通道信息
                    Bitmap bmpR = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
                    BitmapData bmpDataR = bmpR.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly,
                        PixelFormat.Format8bppIndexed); //获取图像参数

                    Bitmap bmpG = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
                    BitmapData bmpDataG = bmpG.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly,
                        PixelFormat.Format8bppIndexed); //获取图像参数

                    Bitmap bmpB = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
                    BitmapData bmpDataB = bmpB.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly,
                        PixelFormat.Format8bppIndexed); //获取图像参数

                    //接收R G B 三通道信息
                    unsafe
                    {
                        byte* bptrR = (byte*)bmpDataR.Scan0;
                        byte* bptrG = (byte*)bmpDataG.Scan0;
                        byte* bptrB = (byte*)bmpDataB.Scan0;

                        byte* bptrRP = (byte*)Rptr.Scan0;
                        byte* bptrGP = (byte*)Gptr.Scan0;
                        byte* bptrBP = (byte*)Bptr.Scan0;

                        for (int i = 0; i < Height; i++)
                        {
                            for (int j = 0; j < Width; j++)
                            {
                                bptrR[i * bmpDataR.Width + j] = bptrRP[i * Rptr.Stride + j];
                                bptrG[i * bmpDataR.Width + j] = bptrGP[i * Rptr.Stride + j];
                                bptrB[i * bmpDataR.Width + j] = bptrBP[i * Rptr.Stride + j];
                            }
                        }
                    }

                    //利用3个bitmap 合成彩图
                    HOperatorSet.GenImage3(out hoImage, "byte", Width, Height, bmpDataR.Scan0, bmpDataG.Scan0,
                        bmpDataB.Scan0);
                }
            }
            else
            {
                IntPtr pointer;
                CogImage8Grey outVproImage = CogImageConvert.GetIntensityImage(Image, 0, 0, Width, Height);
                ICogImage8PixelMemory ptr = outVproImage.Get8GreyPixelMemory(CogImageDataModeConstants.Read, 0, 0,
                    outVproImage.Width, outVproImage.Height);
                pointer = ptr.Scan0;
                if (ptr.Stride == outVproImage.Width)
                {
                    HOperatorSet.GenImage1(out hoImage, "byte", Width, Height, pointer);
                }
                else
                {
                    //图像列数不被4整除
                    Bitmap bmp = new Bitmap(outVproImage.Width, outVproImage.Height, PixelFormat.Format8bppIndexed);
                    BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, Width, Height)
                        , ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                    int bytes = Width * Height;
                    byte[] data = new byte[bytes];
                    unsafe
                    {
                        byte* bptr = (byte*)pointer;
                        for (int i = 0; i < Height; i++)
                        {
                            for (int j = 0; j < Width; j++)
                            {
                                data[i * Width + j] = bptr[i * ptr.Stride + j];
                            }
                        }
                    }

                    IntPtr iptr = bmpData.Scan0;
                    System.Runtime.InteropServices.Marshal.Copy(data, 0, iptr,
                        Width * Height);
                    HOperatorSet.GenImage1(out hoImage, "byte", Width, Height,
                        bmpData.Scan0);

                }
            }
        }
        catch
        {
            throw;
        }

    }

    public static void ImageConvertHalconToVisionPro(HObject ho_Image, out ICogImage iCogImage)
    {
        iCogImage = new CogImage8Grey();
        try
        {
            HTuple hRed, hGreen, hBlue, type, Pointer, Width, Height, channels;
            HOperatorSet.CountChannels(ho_Image, out channels);
            if (channels == 3)
            {
                //获取halcon图像rgb像素指针
                HOperatorSet.GetImagePointer3(ho_Image, out hRed, out hGreen, out hBlue, out type, out Width,
                    out Height);
                //创建visionpro 彩色图像变量
                CogImage24PlanarColor RGBImage = new CogImage24PlanarColor();
                //获取新建彩色图像三通道
                ICogImage8Root rImg, gImg, bImg;
                RGBImage.GetRoots(out rImg, out gImg, out bImg);

                //创建3个灰度图存储R G B三通道信息
                CogImage8Root Rimg = new CogImage8Root(); //创建vp灰度图
                CogImage8Root Gimg = new CogImage8Root(); //创建vp灰度图
                CogImage8Root Bimg = new CogImage8Root(); //创建vp灰度图 
                Rimg.Initialize(Width, Height, (IntPtr)hRed, Width, null); //初始化vp灰度图                 
                Gimg.Initialize(Width, Height, (IntPtr)hGreen, Width, null); //初始化vp灰度图                
                Bimg.Initialize(Width, Height, (IntPtr)hBlue, Width, null); //初始化vp灰度图

                //将R G B三通道信息复制到新建彩色图像的三通道
                rImg = Rimg;
                gImg = Gimg;
                bImg = Bimg;
                RGBImage.SetRoots(rImg, gImg, bImg);

                iCogImage = RGBImage;
            }
            else
            {
                HOperatorSet.GetImagePointer1(ho_Image, out Pointer, out type, out Width, out Height);
                CogImage8Root tmpImage8Root = new CogImage8Root();
                tmpImage8Root.Initialize(Width, Height, (IntPtr)Pointer, Width, null);

                CogImage8Grey outImage8Grey = new CogImage8Grey();
                outImage8Grey.SetRoot(tmpImage8Root);
                iCogImage = outImage8Grey;
            }

        }
        catch
        {
            throw;
        }
    }

    void HonjectToBitmap(HObject ho_img, out Bitmap bmp)
    {
        //获取图像尺寸
        HOperatorSet.GetImageSize(ho_img, out HTuple width0, out HTuple height0);
        //创建交错格式图像
        HOperatorSet.InterleaveChannels(ho_img, out HObject InterImage, "rgb", 4 * width0, 0);
        //获取交错格式图像指针
        HOperatorSet.GetImagePointer1(InterImage, out HTuple Pointer, out HTuple type, out HTuple width,
            out HTuple height);
        IntPtr ptr = Pointer;
        //构建新Bitmap图像
        bmp = new Bitmap(width / 4, height, width, PixelFormat.Format24bppRgb, ptr);

    }


    /// <summary>  
    /// 将一个字节数组转换为8bit灰度位图  
    /// </summary>  
    /// <param name="rawValues">显示字节数组</param>  
    /// <param name="width">图像宽度</param>  
    /// <param name="height">图像高度</param>  
    /// <returns>位图</returns>  
    private static Bitmap ToGrayBitmap(byte[] rawValues, int width, int height)
    {
        //// 申请目标位图的变量，并将其内存区域锁定  
        Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
        BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        //// 获取图像参数  
        int stride = bmpData.Stride; // 扫描线的宽度  
        int offset = stride - width; // 显示宽度与扫描线宽度的间隙  
        IntPtr iptr = bmpData.Scan0; // 获取bmpData的内存起始位置  
        int scanBytes = stride * height; // 用stride宽度，表示这是内存区域的大小  

        //// 下面把原始的显示大小字节数组转换为内存中实际存放的字节数组  
        int posScan = 0, posReal = 0; // 分别设置两个位置指针，指向源数组和目标数组  
        byte[] pixelValues = new byte[scanBytes]; //为目标数组分配内存  

        for (int x = 0; x < height; x++)
        {
            //// 下面的循环节是模拟行扫描  
            for (int y = 0; y < width; y++)
            {
                pixelValues[posScan++] = rawValues[posReal++];
            }

            posScan += offset; //行扫描结束，要将目标位置指针移过那段“间隙”  
        }

        //// 用Marshal的Copy方法，将刚才得到的内存字节数组复制到BitmapData中  
        System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, iptr, scanBytes);
        bmp.UnlockBits(bmpData); // 解锁内存区域  

        //// 下面的代码是为了修改生成位图的索引表，从伪彩修改为灰度  
        ColorPalette tempPalette;
        using (Bitmap tempBmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
        {
            tempPalette = tempBmp.Palette;
        }

        for (int i = 0; i < 256; i++)
        {
            tempPalette.Entries[i] = Color.FromArgb(i, i, i);
        }

        bmp.Palette = tempPalette;

        //// 算法到此结束，返回结果  
        return bmp;
    }

    public static Bitmap HobjectTobBitmap(HObject ho_Image)
    {
        HOperatorSet.GetImagePointer1(ho_Image, out HTuple pointer, out HTuple type, out var width, out var height);
        byte[] by = new byte[width * height];
        Marshal.Copy(pointer, by, 0, width * height);

        return ToGrayBitmap(by, width, height);
    }

    public static void HobjectToBitmap(ICogImage image, int[] PixelXArray, int[] PixelYArray, int catWidth,
        int catHeight, out HObject hv_reImage)
    {
        ImageConvertVisionPro2HObject(image, out var hv_Image);

        int[] Rows = PixelYArray;
        int[] Cols = PixelXArray;
        int cnt = Rows.Length;
        int[] hv_rCat = new int[cnt];
        HObject h_ImageArray = new HObject();
        for (int i = 0; i < Rows.Length; i++)
        {
            hv_rCat[i] = -1;
            HOperatorSet.GenRectangle1(out var rectangle, Rows[i], Cols[i], Rows[i] + catHeight - 1,
                Cols[i] + catWidth - 1);
            HOperatorSet.ReduceDomain(hv_Image, rectangle, out var imageReduced);
            HOperatorSet.CropDomain(imageReduced, out var imagePart);
            h_ImageArray = i == 0 ? imagePart : h_ImageArray.ConcatObj(imagePart);
        }

        var h_rsImgArray = h_ImageArray[0];
        for (int i = 1; i < h_ImageArray.CountObj(); i++)
        {
            h_rsImgArray = h_rsImgArray.ConcatObj(h_ImageArray[i]);
        }

        HOperatorSet.TileImagesOffset(h_rsImgArray, out var tiledImage, Rows, Cols, hv_rCat, hv_rCat, hv_rCat, hv_rCat,
            image.Width, image.Height);
        hv_reImage = tiledImage;
    }

    /// <summary>
    /// 点集转halcon轮廓
    /// </summary>
    /// <param name="Array2D"></param>
    /// <param name="Area"></param>
    public static HObject ArrayPointToXld(double[,] Array2D)
    {
        int Num = Array2D.GetLength(0);
        double[] xArray = new double[Num];
        double[] yArray = new double[Num];
        for (int j = 0; j < Num; j++)
        {
            xArray[j] = Array2D[j, 0];
            yArray[j] = Array2D[j, 1];
        }
        // 创建Halcon多边形
        HTuple hvRow = new HTuple(yArray);
        HTuple hvCol = new HTuple(xArray);
        HOperatorSet.GenContourPolygonXld(out var xld, hvRow, hvCol);
        return xld;
    }

    /// <summary>
    /// 获取多边形区域的面积
    /// </summary>
    /// <param name="Array2D"></param>  多边形点集
    /// <param name="area"></param>
    public static void GetXldRegionArea(double[,] Array2D, ref double area)
    {
        HOperatorSet.GenRegionContourXld(ArrayPointToXld(Array2D), out var hv_Region, "filled");
        HOperatorSet.AreaCenter(hv_Region, out var hv_Area, out _, out _);
        area = hv_Area.D;
    }
}
