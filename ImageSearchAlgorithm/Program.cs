using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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


            stopwatch.Start();
            FirstPixel(MainImage, SearchImage);
            stopwatch.Stop();

			Console.WriteLine(stopwatch.ElapsedMilliseconds);

            Console.ReadKey();
            /*
            BitmapData MainImageData;
            MainImageData = MainImage.LockBits(new Rectangle(0, 0, MainImage.Width, MainImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            */
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
