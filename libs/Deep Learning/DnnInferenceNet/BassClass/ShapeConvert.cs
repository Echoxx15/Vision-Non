using System;
using HalconDotNet;

namespace DnnInferenceNet.BassClass;

public class ShapeConvert
{
    /// <summary>
    /// 将点集转换为Halcon多边形
    /// </summary>
    /// <param name="xArray"></param>
    /// <param name="yArray"></param>
    /// <param name="h_polygon"></param>
    public static void PolygonConvert(double[,] Array2D,out HObject h_polygon)
    {
        try
        {
            int num = Array2D.GetLength(0);
            double[] xArray = new double[num];
            double[] yArray = new double[num];
            for (int j = 0; j < num; j++)
            {
                //halcon的行对应图像的Y，列对应图像的X
                xArray[j] = Array2D[j, 1];
                yArray[j] = Array2D[j, 0];
            }
            //创建Halcon多边形
            HTuple hvRow = new HTuple(xArray);
            HTuple hvCol = new HTuple(yArray);
            HOperatorSet.GenContourPolygonXld(out h_polygon, hvRow, hvCol);
        }
        catch (Exception e)
        {
            h_polygon = null;
            Console.WriteLine(e);
        }
    }
}

