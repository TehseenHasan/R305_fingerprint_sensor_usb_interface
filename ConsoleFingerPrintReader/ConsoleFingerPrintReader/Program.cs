using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using test;

namespace ConsoleFingerPrintReader
{
    class Program
    {
        static void Main(string[] args)
        {   
             // This code captures and downloads scanned fingerprint bitmap image to current directory ... Sensor is attached to USB port. this code is using direct USB protocol (not Serial/UART)...
             var v = new SynoAPIExHelper();

                for (; ; )
                {
                    v.OpenDevice();
                    string path = Path.Combine(Directory.GetCurrentDirectory() , @"\fingerprint.bmp");
                    Console.WriteLine(v.SaveFigerBmp(path).ToString());

                    //int milliseconds = 100;
                    //Thread.Sleep(milliseconds);
                }         

            //Console.ReadKey();

        }
    }
}
