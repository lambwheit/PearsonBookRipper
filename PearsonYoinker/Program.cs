using System;
using System.IO;
using System.Net;
using System.Threading;
using iTextSharp.text;
using Leaf.xNet;
using iTextSharp.text.pdf;

namespace PearsonYoinker
{
    class Program
    {
        static void Main(string[] args)
        {

            int init = 1;
            Console.WriteLine("Example of image link: https://resources.pearsonactivelearn.com/r00/r0066/r006623/r00662374/current/OPS/images/");
            Console.Write("Enter in image link: ");
            string link = Console.ReadLine();
            Console.WriteLine("Instructions for page name: ");
            Console.WriteLine("Get the file name");
            Console.WriteLine("https://resources.pearsonactivelearn.com/r00/r0066/r006623/r00662374/current/OPS/images/Pure_Maths_1-001.jpg");
            Console.WriteLine("In the above it's Pure_Maths_1-001.jpg");
            Console.WriteLine("That will be your page name");
            Console.Write("Enter in page name: ");
            string pagename = Console.ReadLine();
            Console.Write("Folder name: ");
            string foldername = Console.ReadLine();
            Console.Write("Pdf Name: ");
            string pdfname = Console.ReadLine();
            Directory.CreateDirectory(foldername);
            Console.Clear();
            Console.Title = "Downloading pages into: " + foldername;
            string[] starting = pagename.Split('-');//[pure_math_1,001]
            string stringtocount = starting[1];//001
            string extention = stringtocount.Split('.')[1];//jpg
            int digitcount = 0;
            foreach (char c in stringtocount)
            {
                if (Char.IsDigit(c))
                {
                    digitcount++;
                }
            }//value to count padding in the abv case it's 3
            while (true)
            {
                try
                {
                    String pagename2 = starting[0]+"-"+init.ToString().PadLeft(digitcount,'0')+"."+extention;//pure_maths_1-001.jpg
                    string url = link + pagename2;
                    using (HttpRequest request = new HttpRequest())
                    {
                        var resp = request.Get(url);
                        resp.ToFile(foldername + "\\" + init + "."+extention);
                    }
                    Console.WriteLine("Ripped Page Number: " + init);
                }
                catch
                {
                    Console.WriteLine("Rip Finished, " + (init - 1).ToString() + " Pages Ripped");
                    if (init - 1 == 0)
                    {
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                    break;
                }
                init++;
            }
            Console.Clear();
            Console.Title = "Adding pages into:" + pdfname + ".pdf";
            Document document = new Document();
            using (var stream = new FileStream(pdfname + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                PdfWriter.GetInstance(document, stream);
                document.Open();
                for (int i = 1; i < init; i++)
                {
                    Console.WriteLine("Adding page " + i + " into pdf");
                    using (var imageStream = new FileStream(foldername + "\\" + i + ".jpg", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var image = Image.GetInstance(imageStream);
                        float maxWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        float maxHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;
                        if (image.Height > maxHeight || image.Width > maxWidth) image.ScaleToFit(maxWidth, maxHeight);
                        document.Add(image);
                    }
                }
                document.Close();
            }
            Console.Clear();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
