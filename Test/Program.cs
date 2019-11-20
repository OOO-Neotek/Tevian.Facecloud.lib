using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var tevian = new Tevian.Tevian("", "");
            Console.WriteLine("JWT: " + tevian.jwt);
            byte[] image = new byte[] {}; // image.jpg
            //tevian.Detect(image);
        }
    }
}
