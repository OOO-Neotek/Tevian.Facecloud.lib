using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Test
{
    class Test
    {
        private Tevian.Tevian tevian;

        /// <summary>
        /// Just "login" and get an access token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passw"></param>
        public void Login(string email, string passw)
        {
            tevian = new Tevian.Tevian(email, passw);
            Console.WriteLine("JWT: " + tevian.jwt);
        }
        /// <summary>
        /// In case of account does not exist, you're able to create a demo account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passw"></param>
        public void CreateAccountNLogin(string email, string passw)
        {
            var user = Tevian.Tevian.CreateAccount(email, passw).Result;
            Console.WriteLine(JsonConvert.SerializeObject(user));
            Login(email, passw);
        }
        public void Detect(string filePath)
        {
            var img1 = File.ReadAllBytes(filePath);

            var resp = tevian.Detect(img1, liveness: true, fd_threshold: 0.8f).Result;
            Console.Write("\"" + Path.GetFileName(filePath) +"\": ");
            Console.WriteLine(JsonConvert.SerializeObject(resp));
        }

        public void Match(string file1, string file2)
        {
            var img1 = File.ReadAllBytes(file1);
            var img2 = File.ReadAllBytes(file2);

            var resp = tevian.Match(img1, img2, fd_threshold: 0.25f).Result;

            Console.WriteLine(JsonConvert.SerializeObject(resp));
        }
    }

    class Program
    {
        static (string, string) AskCredentials(string[] args)
        {
            bool confirm = args?.Length == 3 && args[2][0] == 'y';
            string email, passw;
            do
            {
                if (args?.Length >= 1)
                {
                    email = args[0];
                }
                else
                {
                    Console.Write("Email: ");
                    email = Console.ReadLine();
                }

                if (args?.Length >= 2)
                {
                    passw = args[1];
                }
                else
                {
                    Console.Write("Password: ");
                    passw = Console.ReadLine();
                }

                if (!confirm)
                {
                    bool ok;
                    char a;
                    do
                    {
                        Console.Write($"Continue with: Email \"{email}\" and Password \"{passw}\"?\n y/n: ");
                        ok = char.TryParse(Console.ReadLine(), out a);
                        confirm = a == 'y';
                    } while (ok && !(a == 'y' || a == 'n'));
                }

                args = null;
            } while (!confirm);

            return (email, passw);
        }


        static void Main(string[] args)
        {
            Console.Write("1. Create Account & Login\n2. Login\n3. Exit\nChoose option: ");
            bool ok = int.TryParse(Console.ReadLine(), out var selection);
            if (!ok)
                throw new Exception("Only numbers acceptable");

            var test = new Test();

            switch (selection)
            {
                case 1:
                {
                    var (email, passw) = AskCredentials(args);
                    test.CreateAccountNLogin(email, passw);
                    break;
                }
                case 2:
                {
                    var (email, passw) = AskCredentials(args);
                    test.Login(email, passw);
                    break;
                }
                default:
                    return;
            }


            string fpath = "C:\\Users\\zsv\\source\\repos\\Tevian\\testdata\\";

            var selfie = fpath + "SelfieWithId.jpg";
            var idFront = fpath + "PhotoOfIdFrontSide.jpg";

            var selfie1 = fpath + "SelfieWithId (1).jpg";
            var idFront1 = fpath + "PhotoOfIdFrontSide (1).jpg";

            var selfie2 = fpath + "SelfieWithId (2).jpg";
            var idFront2 = fpath + "PhotoOfIdFrontSide (2).jpg";


            test.Detect(selfie2);
            test.Detect(idFront2);
            //test.Match(selfie1, idFront1);

            Console.ReadKey();

            //byte[] image = new byte[] { }; // image.jpg
            //tevian.Detect(image);
        }
    }
}