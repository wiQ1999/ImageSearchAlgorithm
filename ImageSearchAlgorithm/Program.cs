using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

/*
https://codereview.stackexchange.com/questions/138011/find-a-bitmap-within-another-bitmap
*/

namespace ImageSearchAlgorithm
{
	class Program
	{
        const string MainImagePath1 = @"img\image01.png";
        const string SearchImagePath1 = @"img\image01.1.png";
        const string MainImagePath2 = @"img\image02.png";
        const string SearchImagePath2 = @"img\image02.1.png";
        const string MainImagePath3 = @"img\image03.png";
        const string SearchImagePath3 = @"img\image03.1.png";

        const string Podstawa = @"img\Przypadki\Podstawa.png";
        const string PodstawaOptymistyczny = @"img\Przypadki\PodstawaOptymistyczny.png";
        const string Optymistyczny = @"img\Przypadki\Optymistyczny.png";
        const string PodstawaPesymistyczny = @"img\Przypadki\PodstawaPesymistyczny.png";
        const string Pesymistyczny = @"img\Przypadki\Pesymistyczny.png";
        const string Calosc = @"img\Przypadki\Calosc.png";

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            Bitmap MainImage;
            Bitmap SearchImage;

            for (int image = 0; image < 8; image++)
            {

            }

            Console.ReadKey();
        }

        static Point? InsideMemoryAllPoint(Bitmap a_MainImage, Bitmap a_SearchImage)
        {
            BitmapData MainImageData = a_MainImage.LockBits(new Rectangle(0, 0, a_MainImage.Width, a_MainImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData SearchImageData = a_SearchImage.LockBits(new Rectangle(0, 0, a_SearchImage.Width, a_SearchImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                for (int yMain = 0, yLength = a_MainImage.Height - a_SearchImage.Height + 1; yMain < yLength; ++yMain)
                {
                    int[] MainImageLine = new int[a_MainImage.Width];
                    Marshal.Copy(MainImageData.Scan0 + yMain * MainImageData.Stride, MainImageLine, 0, a_MainImage.Width);
                    for (int xMain = 0, xLength = a_MainImage.Width - a_SearchImage.Width + 1; xMain < xLength; xMain++)
                    {
                        bool isMatch = true;
                        for (int ySearch = 0; ySearch < a_SearchImage.Height; ySearch++)
                        {
                            int[] SearchImageLine = new int[a_SearchImage.Width];
                            Marshal.Copy(SearchImageData.Scan0 + ySearch * SearchImageData.Stride, SearchImageLine, 0, a_SearchImage.Width);
                            for (int xSearch = 0; xSearch < a_SearchImage.Width; xSearch++)
                            {
                                if (MainImageLine[xMain + xSearch] != SearchImageLine[xSearch])
                                {
                                    isMatch = false;
                                    break;
                                }
                            }
                            if (!isMatch)
                                break;
                        }
                        if (isMatch)
                            return new Point(xMain, yMain);
                    }
                }
                return null;
            }
            finally
            {
                a_MainImage.UnlockBits(MainImageData);
                a_SearchImage.UnlockBits(SearchImageData);
            }
        }

        static Point? MemoryAllPoint(Bitmap a_MainImage, Bitmap a_SearchImage)
        {
            int[][] MainImageArray = GetPixelArray(a_MainImage);
            int[][] SearchImageArray = GetPixelArray(a_SearchImage);

            for (int yMain = 0, yLength = MainImageArray.Length - SearchImageArray.Length + 1; yMain < yLength; yMain++)
            {
                for (int xMain = 0, xLength = MainImageArray[0].Length - SearchImageArray[0].Length + 1; xMain < xLength; xMain++)
                {
                    bool isMatch = true;
                    for (int ySearch = 0; ySearch < SearchImageArray.Length; ySearch++)
                    {
                        for (int xSearch = 0; xSearch < SearchImageArray[0].Length; xSearch++)
                        {
                            if (MainImageArray[yMain + ySearch][xMain + xSearch] != SearchImageArray[ySearch][xSearch])
                            {
                                isMatch = false;
                                break;
                            }
                        }
                        if (!isMatch)
                            break;
                    }
                    if (isMatch)
                        return new Point(xMain, yMain);
                }
            }
            return null;
        }

        static int[][] GetPixelArray(Bitmap bitmap)
        {
            var result = new int[bitmap.Height][];
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            for (int y = 0; y < bitmap.Height; ++y)
            {
                result[y] = new int[bitmap.Width];
                Marshal.Copy(bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length);
            }

            bitmap.UnlockBits(bitmapData);

            return result;
        }

        static bool FirstPixelBorder(Bitmap a_MainImage, Bitmap a_SearchImage)
        {
            int RedusedWidth = a_MainImage.Width - a_SearchImage.Width;
            int RedusedHeight = a_MainImage.Height - a_SearchImage.Height;

            for (int MainX = 0; MainX < RedusedWidth; MainX++)
            {
                for (int MainY = 0; MainY < RedusedHeight; MainY++)
                {
                    if (a_MainImage.GetPixel(MainX, MainY) == a_SearchImage.GetPixel(0, 0))
                    {
                        bool IsBorderCorrect = true;
                        for (int BorderX = 0; BorderX < a_SearchImage.Width; BorderX++)
                        {
                            //sprawdzenie GÓRA
                            if (a_MainImage.GetPixel(MainX + BorderX, MainY) != a_SearchImage.GetPixel(BorderX, 0))
                            {
                                IsBorderCorrect = false;
                                break;
                            }
                        }
                        if (IsBorderCorrect)
                        {
                            for (int BorderX = 0; BorderX < a_SearchImage.Width; BorderX++)
                            {
                                //sprawdzenie DÓŁ
                                if (a_MainImage.GetPixel(MainX + BorderX, MainY + a_SearchImage.Height - 1) != a_SearchImage.GetPixel(BorderX, a_SearchImage.Height - 1))
                                {
                                    IsBorderCorrect = false;
                                    break;
                                }
                            }
                            if (IsBorderCorrect)
                            {
                                for (int BorderY = 0; BorderY < a_SearchImage.Height; BorderY++)
                                {
                                    //sprawdzenie LEWY
                                    if (a_MainImage.GetPixel(MainX, MainY + BorderY) != a_SearchImage.GetPixel(0, BorderY))
                                    {
                                        IsBorderCorrect = false;
                                        break;
                                    }
                                }
                                if (IsBorderCorrect)
                                {
                                    for (int BorderY = 0; BorderY < a_SearchImage.Height; BorderY++)
                                    {
                                        //sprawdzenie PRAWY
                                        if (a_MainImage.GetPixel(MainX + a_SearchImage.Width - 1, MainY + BorderY) != a_SearchImage.GetPixel(a_SearchImage.Width - 1, BorderY))
                                        {
                                            IsBorderCorrect = false;
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                        if (IsBorderCorrect)
                        {
                            bool IsCorrect = true;
                            int SearchX = 1;
                            while (SearchX < a_SearchImage.Width && IsCorrect)
                            {
                                for (int SearchY = 1; SearchY < a_SearchImage.Height; SearchY++)
                                {
                                    if (a_MainImage.GetPixel(SearchX + MainX, SearchY + MainY) != a_SearchImage.GetPixel(SearchX, SearchY))
                                    {
                                        IsCorrect = false;
                                        break;
                                    }
                                }
                                SearchX++;
                            }
                            if (IsCorrect)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        static bool FirstPixel(Bitmap a_MainImage, Bitmap a_SearchImage)
        {
            int RedusedWidth = a_MainImage.Width - a_SearchImage.Width;
            int RedusedHeight = a_MainImage.Height - a_SearchImage.Height;

            for (int MainX = 0; MainX < RedusedWidth; MainX++)
            {
                for (int MainY = 0; MainY < RedusedHeight; MainY++)
                {
                    if (a_MainImage.GetPixel(MainX, MainY) == a_SearchImage.GetPixel(0, 0))
                    {
                        bool IsCorrect = true;
                        int SearchX = 0;
                        while (SearchX < a_SearchImage.Width && IsCorrect)
                        {
                            for (int SearchY = 0; SearchY < a_SearchImage.Height; SearchY++)
                            {
                                if (a_MainImage.GetPixel(SearchX + MainX, SearchY + MainY) != a_SearchImage.GetPixel(SearchX, SearchY))
                                {
                                    IsCorrect = false;
                                    break;
                                }
                            }
                            SearchX++;
                        }
                        if (IsCorrect)
                            return true;
                    }
                }
            }

            return false;
        }

        static bool TheSlownest(Bitmap a_MainImage, Bitmap a_SearchImage)
        {
            for (int yMain = 0, yLength = a_MainImage.Width - a_SearchImage.Width + 1; yMain < yLength; yMain++)
            {
                for (int xMain = 0, xLength = a_MainImage.Height - a_SearchImage.Height + 1; xMain < xLength; xMain++)
                {
                    bool IsCorrect = true;
                    int ySearch = 0;
                    while (ySearch < a_SearchImage.Width && IsCorrect)
                    {
                        int xSearch = 0;
                        while (xSearch < a_SearchImage.Height && IsCorrect)
                        {
                            if (a_MainImage.GetPixel(yMain + ySearch, xMain + xSearch) != a_SearchImage.GetPixel(ySearch, xSearch))
                                IsCorrect = false;
                            xSearch++;
                        }
                        ySearch++;
                    }
                    if (IsCorrect)
                        return true;
                }
            }
            return false;
        }
    }
}
