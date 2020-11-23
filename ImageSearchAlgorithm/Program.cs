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

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            Bitmap MainImage = new Bitmap(MainImagePath1);
            Bitmap SearchImage = new Bitmap(SearchImagePath1);

            
            int loops = 50;

            for (int i = 0; i < loops; i++)
            {
                stopwatch.Start();
                MemoryFirstPixel(MainImage, SearchImage);
                stopwatch.Stop();
            }
            Console.WriteLine(stopwatch.ElapsedMilliseconds/loops);
            stopwatch.Reset();
            
            for (int i = 0; i < loops; i++)
            {
                stopwatch.Start();
                Find(MainImage, SearchImage);
                stopwatch.Stop();
            }
            Console.WriteLine(stopwatch.ElapsedMilliseconds / loops);
            stopwatch.Reset();
            
            
            Console.ReadKey();
        }

        static bool MemoryFirstPixel(Bitmap a_MainImage, Bitmap a_SearchImage)
        {
            int[][] MainImageArray = GetPixelArray(a_MainImage);
            int[][] SearchImageArray = GetPixelArray(a_SearchImage);

            int RedusedWidth = a_MainImage.Width - a_SearchImage.Width;
            int RedusedHeight = a_MainImage.Height - a_SearchImage.Height;

            foreach (int[] mainLine in MainImageArray)
            {
                for (int x = 0, n = mainLine.Length - SearchImageArray.GetLength(0); x < n; x++)
                {

                }
            }


            for (int MainX = 0; MainX < RedusedWidth; MainX++)
            {
                for (int MainY = 0; MainY < RedusedHeight; MainY++)
                {
                    if (MainImageArray[MainY][MainX] == SearchImageArray[0][0])
                    {
                        bool IsCorrect = true;
                        int SearchX = 0;
                        while (SearchX < a_SearchImage.Width && IsCorrect)
                        {
                            for (int SearchY = 0; SearchY < a_SearchImage.Height; SearchY++)
                            {
                                if (MainImageArray[MainY + SearchY][MainX + SearchX] != SearchImageArray[SearchY][SearchX])
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

        #region internet

        public static Point? Find(Bitmap haystack, Bitmap needle)
        {
            if (null == haystack || null == needle)
            {
                return null;
            }
            if (haystack.Width < needle.Width || haystack.Height < needle.Height)
            {
                return null;
            }

            var haystackArray = GetPixelArray(haystack);
            var needleArray = GetPixelArray(needle);

            foreach (var firstLineMatchPoint in FindMatch(haystackArray.Take(haystack.Height - needle.Height), needleArray[0]))
            {
                if (IsNeedlePresentAtLocation(haystackArray, needleArray, firstLineMatchPoint, 1))
                {
                    return firstLineMatchPoint;
                }
            }

            return null;
        }

        private static int[][] GetPixelArray(Bitmap bitmap)
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

        private static IEnumerable<Point> FindMatch(IEnumerable<int[]> haystackLines, int[] needleLine)
        {
            var y = 0;
            foreach (var haystackLine in haystackLines)
            {
                for (int x = 0, n = haystackLine.Length - needleLine.Length; x < n; ++x)
                {
                    if (ContainSameElements(haystackLine, x, needleLine, 0, needleLine.Length))
                    {
                        yield return new Point(x, y);//yield zapobiega powtarzaniu return'ów
                    }
                }
                y += 1;
            }
        }

        private static bool ContainSameElements(int[] first, int firstStart, int[] second, int secondStart, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                if (first[i + firstStart] != second[i + secondStart])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsNeedlePresentAtLocation(int[][] haystack, int[][] needle, Point point, int alreadyVerified)
        {
            //we already know that "alreadyVerified" lines already match, so skip them
            for (int y = alreadyVerified; y < needle.Length; ++y)
            {
                if (!ContainSameElements(haystack[y + point.Y], point.X, needle[y], 0, needle[y].Length))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

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
            int RedusedWidth = a_MainImage.Width - a_SearchImage.Width;
            int RedusedHeight = a_MainImage.Height - a_SearchImage.Height;

            for (int MainX = 0; MainX < RedusedWidth; MainX++)
            {
                for (int MainY = 0; MainY < RedusedHeight; MainY++)
                {
                    bool IsCorrect = true;
                    int SearchX = 0;
                    while (SearchX < a_SearchImage.Width && IsCorrect)
                    {
                        int SearchY = 0;
                        while (SearchY < a_SearchImage.Height && IsCorrect)
                        {
                            if (a_MainImage.GetPixel(MainX + SearchX, MainY + SearchY) != a_SearchImage.GetPixel(SearchX, SearchY))
                                IsCorrect = false;
                            SearchY++;
                        }
                        SearchX++;
                    }
                    if (IsCorrect)
                        return true;
                }
            }
            return false;
        }

        static int GetObjectSize(object a_Object)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, a_Object);
            return ms.ToArray().Length;
        }
    }
}
