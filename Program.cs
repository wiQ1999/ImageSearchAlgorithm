﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

/*
https://codereview.stackexchange.com/questions/138011/find-a-bitmap-within-another-bitmap
*/

namespace ImageSearchAlgorithm
{
    class Program
	{
        const string Path = @"E:\Programowanie\C#\ImageSearchAlgorithm\ImageSearchAlgorithm\img\";
        //const string Path = @"C:\Users\Dell_3620\Source\Repos\ImageSearchAlgorithm\ImageSearchAlgorithm\img\";

        static void Main()
        {
            Stopwatch stopwatch = new Stopwatch();

            int loops = 1;

            Console.WriteLine("GetPixel\tGetPixelBorder\tMemoryArray\tInsideMemory");

            Bitmap MainImage = null;
            Bitmap SearchImage = null;
            
            for (int image = 0; image < 6; image++)
            {
                switch (image)
                {
                    case 0:
                        MainImage = new Bitmap(Path + "Optymistyczny1_Main.png");
                        SearchImage = new Bitmap(Path + "Optymistyczny1_Search.png");
                        break;
                    case 1:
                        MainImage = new Bitmap(Path + "Optymistyczny2_Main.png");
                        SearchImage = new Bitmap(Path + "Optymistyczny2_Search.png");
                        break;
                    case 2:
                        MainImage = new Bitmap(Path + "Pesymistyczny1_Main.png");
                        SearchImage = new Bitmap(Path + "Pesymistyczny1_Search.png");
                        break;
                    case 3:
                        MainImage = new Bitmap(Path + "Pesymistyczny2_Main.png");
                        SearchImage = new Bitmap(Path + "Pesymistyczny2_Search.png");
                        break;
                    case 4:
                        MainImage = new Bitmap(Path + "Male_Main.png");
                        SearchImage = new Bitmap(Path + "Male_Search.png");
                        break;
                    case 5:
                        MainImage = new Bitmap(Path + "Duze_Main.png");
                        SearchImage = new Bitmap(Path + "Duze_Search.png");
                        break;
                }

                //GetPixel
                //for (int i = 0; i < loops; i++)
                //{
                //    stopwatch.Start();
                //    Console.Write(GetPixel(MainImage, SearchImage) + " - ");
                //    stopwatch.Stop();
                //}
                //Console.Write(stopwatch.ElapsedMilliseconds / loops + "\n");
                //stopwatch.Reset();

                ////GetPixelBorder
                //for (int i = 0; i < loops; i++)
                //{
                //    stopwatch.Start();
                //    Console.Write(GetPixelBorder(MainImage, SearchImage) + " - ");
                //    stopwatch.Stop();
                //}
                //Console.Write(stopwatch.ElapsedMilliseconds / loops + "\n");
                //stopwatch.Reset();

                //MemoryArray
                for (int i = 0; i < loops; i++)
                {
                    stopwatch.Start();
                    Console.Write(MemoryArray(MainImage, SearchImage) + " - ");
                    stopwatch.Stop();
                }
                Console.Write(stopwatch.ElapsedMilliseconds / loops + "\n");
                stopwatch.Reset();

                //InsideMemory
                //for (int i = 0; i < loops; i++)
                //{
                //    stopwatch.Start();
                //    Console.Write(InsideMemory(MainImage, SearchImage) + " - ");
                //    stopwatch.Stop();
                //}
                //Console.Write(stopwatch.ElapsedMilliseconds / loops + "\n");
                //stopwatch.Reset();

                //InsideMemory2
                for (int i = 0; i < loops; i++)
                {
                    stopwatch.Start();
                    Console.Write(InsideMemory2(MainImage, SearchImage) + " - ");
                    stopwatch.Stop();
                }
                Console.Write(stopwatch.ElapsedMilliseconds / loops + "\n");
                stopwatch.Reset();

                //InsideMemory3
                for (int i = 0; i < loops; i++)
                {
                    stopwatch.Start();
                    Console.Write(InsideMemory3(MainImage, SearchImage) + " - ");
                    stopwatch.Stop();
                }
                Console.Write(stopwatch.ElapsedMilliseconds / loops + "\n");
                stopwatch.Reset();

                Console.WriteLine();
            }
        }

        static Point? InsideMemory3(Bitmap a_mainImage, Bitmap a_searchImage)
        {
            BitmapData mainImageData = a_mainImage.LockBits(new Rectangle(0, 0, a_mainImage.Width, a_mainImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            //BitmapData searchImageData = a_searchImage.LockBits(new Rectangle(0, 0, a_searchImage.Width, a_searchImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int[][] searchImageLine = GetPixelArray(a_searchImage);

            List<Point> maches = new List<Point>();
            
            for (int yMain = 0, yMainLength = a_mainImage.Height - a_searchImage.Height + 1; yMain < yMainLength; yMain++)
            {
                int[] mainImageLine = new int[a_mainImage.Width];
                Marshal.Copy(mainImageData.Scan0, mainImageLine, 0, a_mainImage.Width);
                for (int ySearch = 0, ySearchLength = yMain < a_searchImage.Height ? yMain + 1 : a_searchImage.Height; ySearch < ySearchLength; ySearch++)
                {
                    bool isMatch = true;
                    int xMain = 0;
                    for (; xMain < yMainLength; xMain++)
                    {
                        if (mainImageLine[xMain] != searchImageLine[ySearch][xMain])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    if (isMatch)
                    {
                        if(maches.Contains(new Point()))

                        maches.Add(new Point(xMain, yMain));
                    }
                }

            }
            a_mainImage.UnlockBits(mainImageData);
            //a_searchImage.UnlockBits(searchImageData);
            return null;
        }

        static Point? InsideMemory2(Bitmap a_mainImage, Bitmap a_searchImage)
        {
            BitmapData mainImageData = a_mainImage.LockBits(new Rectangle(0, 0, a_mainImage.Width, a_mainImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData searchImageData = a_searchImage.LockBits(new Rectangle(0, 0, a_searchImage.Width, a_searchImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int[] searchImageLine = new int[a_searchImage.Width];
            Marshal.Copy(searchImageData.Scan0, searchImageLine, 0, a_searchImage.Width);

            for (int yMain = 0, yLength = a_mainImage.Height - a_searchImage.Height + 1; yMain < yLength; yMain++)
            {
                int[] mainImageLine = new int[a_mainImage.Width];
                Marshal.Copy(mainImageData.Scan0 + yMain * mainImageData.Stride, mainImageLine, 0, a_mainImage.Width);
                for (int xMain = 0, xLength = a_mainImage.Width - a_searchImage.Width + 1; xMain < xLength; xMain++)
                {
                    bool isMatch = true;
                    for (int xSearch = 0; xSearch < a_searchImage.Width; xSearch++)
                    {
                        if (mainImageLine[xMain + xSearch] != searchImageLine[xSearch])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    if (isMatch)
                    {
                        for (int ySearch = 1; ySearch < a_searchImage.Height; ySearch++)
                        {
                            int[] tempMainImageLine = new int[a_mainImage.Width];
                            Marshal.Copy(mainImageData.Scan0 + ySearch * mainImageData.Stride, mainImageLine, 0, a_mainImage.Width);
                            int[] tempSearchImageLine = new int[a_searchImage.Width];
                            Marshal.Copy(searchImageData.Scan0 + ySearch * searchImageData.Stride, searchImageLine, 0, a_searchImage.Width);
                            for (int xSearch = 0; xSearch < a_searchImage.Width; xSearch++)
                            {
                                if (tempMainImageLine[xMain + xSearch] != tempSearchImageLine[xSearch])
                                {
                                    isMatch = false;
                                    break;
                                }
                            }
                            if (!isMatch)
                                break;
                        }
                        if (isMatch)
                        {
                            a_mainImage.UnlockBits(mainImageData);
                            a_searchImage.UnlockBits(searchImageData);
                            return new Point(xMain, yMain);
                        }
                    }
                }
            }
            a_mainImage.UnlockBits(mainImageData);
            a_searchImage.UnlockBits(searchImageData);
            return null;
        }

        static Point? InsideMemory(Bitmap a_mainImage, Bitmap a_searchImage)
        {
            BitmapData mainImageData = a_mainImage.LockBits(new Rectangle(0, 0, a_mainImage.Width, a_mainImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData searchImageData = a_searchImage.LockBits(new Rectangle(0, 0, a_searchImage.Width, a_searchImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            for (int yMain = 0, yLength = a_mainImage.Height - a_searchImage.Height + 1; yMain < yLength; yMain++)
            {
                for (int xMain = 0, xLength = a_mainImage.Width - a_searchImage.Width + 1; xMain < xLength; xMain++)
                {
                    bool isMatch = true;
                    int ySearch = 0;
                    while (isMatch && ySearch < a_searchImage.Height)
                    {
                        int[] mainImageLine = new int[a_mainImage.Width];
                        Marshal.Copy(mainImageData.Scan0 + (yMain + ySearch) * mainImageData.Stride, mainImageLine, 0, a_mainImage.Width);
                        int[] searchImageLine = new int[a_searchImage.Width];
                        Marshal.Copy(searchImageData.Scan0 + ySearch * searchImageData.Stride, searchImageLine, 0, a_searchImage.Width);
                        for (int xSearch = 0; xSearch < a_searchImage.Width; xSearch++)
                        {
                            if (mainImageLine[xMain + xSearch] != searchImageLine[xSearch])
                            {
                                isMatch = false;
                                break;
                            }
                        }
                        ySearch++;
                    }
                    if (isMatch)
                    {
                        a_mainImage.UnlockBits(mainImageData);
                        a_searchImage.UnlockBits(searchImageData);
                        return new Point(xMain, yMain);
                    }
                }
            }
            a_mainImage.UnlockBits(mainImageData);
            a_searchImage.UnlockBits(searchImageData);
            return null;
        }

        static Point? MemoryArray(Bitmap a_mainImage, Bitmap a_searchImage)
        {
            int[][] mainImageArray = GetPixelArray(a_mainImage);
            int[][] searchImageArray = GetPixelArray(a_searchImage);

            for (int yMain = 0, yLength = mainImageArray.Length - searchImageArray.Length + 1; yMain < yLength; yMain++)
            {
                for (int xMain = 0, xLength = mainImageArray[0].Length - searchImageArray[0].Length + 1; xMain < xLength; xMain++)
                {
                    bool isMatch = true;
                    for (int ySearch = 0; ySearch < searchImageArray.Length; ySearch++)
                    {
                        for (int xSearch = 0; xSearch < searchImageArray[0].Length; xSearch++)
                        {
                            if (mainImageArray[yMain + ySearch][xMain + xSearch] != searchImageArray[ySearch][xSearch])
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

        static int[][] GetPixelArray(Bitmap a_bitmap)
        {
            var result = new int[a_bitmap.Height][];
            var bitmapData = a_bitmap.LockBits(new Rectangle(0, 0, a_bitmap.Width, a_bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            for (int y = 0; y < a_bitmap.Height; ++y)
            {
                result[y] = new int[a_bitmap.Width];
                Marshal.Copy(bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length);
            }
            a_bitmap.UnlockBits(bitmapData);

            return result;
        }

        static Point? GetPixelBorder(Bitmap a_mainImage, Bitmap a_searchImage)
        {
            for (int yMain = 0, yLength = a_mainImage.Height - a_searchImage.Height + 1; yMain < yLength; yMain++)
            {
                for (int xMain = 0, xLength = a_mainImage.Width - a_searchImage.Width + 1; xMain < xLength; xMain++)
                {
                    if (a_mainImage.GetPixel(xMain, yMain) == a_searchImage.GetPixel(0, 0))
                    {
                        bool isMatch = true;
                        for (int xBorder = 0; xBorder < a_searchImage.Width; xBorder++)
                        {
                            //sprawdzenie GÓRA
                            if (a_mainImage.GetPixel(xMain + xBorder, yMain) != a_searchImage.GetPixel(xBorder, 0))
                            {
                                isMatch = false;
                                break;
                            }
                        }
                        if (isMatch)
                        {
                            for (int xBorder = 0; xBorder < a_searchImage.Width; xBorder++)
                            {
                                //sprawdzenie DÓŁ
                                if (a_mainImage.GetPixel(xMain + xBorder, yMain + a_searchImage.Height - 1) != a_searchImage.GetPixel(xBorder, a_searchImage.Height - 1))
                                {
                                    isMatch = false;
                                    break;
                                }
                            }
                            if (isMatch)
                            {
                                for (int yBorder = 0; yBorder < a_searchImage.Height; yBorder++)
                                {
                                    //sprawdzenie LEWY
                                    if (a_mainImage.GetPixel(xMain, yMain + yBorder) != a_searchImage.GetPixel(0, yBorder))
                                    {
                                        isMatch = false;
                                        break;
                                    }
                                }
                                if (isMatch)
                                {
                                    for (int yBorder = 0; yBorder < a_searchImage.Height; yBorder++)
                                    {
                                        //sprawdzenie PRAWY
                                        if (a_mainImage.GetPixel(xMain + a_searchImage.Width - 1, yMain + yBorder) != a_searchImage.GetPixel(a_searchImage.Width - 1, yBorder))
                                        {
                                            isMatch = false;
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                        if (isMatch)
                        {
                            bool IsCorrect = true;
                            int SearchX = 1;
                            while (SearchX < a_searchImage.Width && IsCorrect)
                            {
                                for (int SearchY = 1; SearchY < a_searchImage.Height; SearchY++)
                                {
                                    if (a_mainImage.GetPixel(SearchX + xMain, SearchY + yMain) != a_searchImage.GetPixel(SearchX, SearchY))
                                    {
                                        IsCorrect = false;
                                        break;
                                    }
                                }
                                SearchX++;
                            }
                            if (IsCorrect)
                                return new Point(xMain, yMain);
                        }
                    }
                }
            }
            return null;
        }

        static Point? GetPixel(Bitmap a_mainImage, Bitmap a_searchImage)
        {
            for (int yMain = 0, yLength = a_mainImage.Height - a_searchImage.Height + 1; yMain < yLength; yMain++)
            {
                for (int xMain = 0, xLength = a_mainImage.Width - a_searchImage.Width + 1; xMain < xLength; xMain++)
                {
                    bool isMatch = true;
                    int ySearch = 0;
                    while (ySearch < a_searchImage.Height && isMatch)
                    {
                        int xSearch = 0;
                        while (xSearch < a_searchImage.Width && isMatch)
                        {
                            if (a_mainImage.GetPixel(xMain + xSearch, yMain + ySearch) != a_searchImage.GetPixel(xSearch, ySearch))
                                isMatch = false;
                            xSearch++;
                        }
                        ySearch++;
                    }
                    if (isMatch)
                        return new Point(xMain, yMain);
                }
            }
            return null;
        }
    }
}