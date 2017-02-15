using System;
using System.Windows.Forms;
using System.Linq;
using jspank.clipboard.Db;
using System.IO;
using System.Collections.Generic;
using jspank.clipboard.Service;
using System.Configuration;

namespace jspank.clipboard
{

    class Program
    {
        static bool ViewAbout
        {
            get
            {
                var value = ConfigurationManager.AppSettings["ViewAbout"] as string;
                return !string.IsNullOrEmpty(value) && value.Equals("1");
            }
        }

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("\t\t#### keys ####");
            Console.WriteLine("\t cpf");
            Console.WriteLine("\t mail");
            Console.WriteLine("\t dec Ex: \"Caller\" dec MessageBase64ToDecode");
            Console.WriteLine("\t enc Ex: \"Caller\" dec MessageToEncodeBase64");
            Console.WriteLine("\t ??? - other itens in repository folder /Resource/Repository/ Ex: Lorem.txt");


            var value = string.Empty;
            var onsuccess = false;
            if (args.Any())
            {
                value = args[0];
                switch (value)
                {
                    case "cpf":
                        Clipboard.SetText(CPFGenerate.Get.ToString());
                        onsuccess = true;
                        break;
                    case "mail":
                        Clipboard.SetText(string.Concat(CPFGenerate.Get, "@fake.com"));
                        onsuccess = true;
                        break;
                    case "dec":
                        Clipboard.SetText(Base64Service.Decode(args[1]));
                        onsuccess = true;
                        break;
                    case "enc":
                        Clipboard.SetText(Base64Service.Encode(args[1]));
                        onsuccess = true;
                        break;
                    default:
                        var d_repository = new DirectoryInfo(string.Concat(Environment.CurrentDirectory, @"\Resource\Repository\"));

                        var lines = new List<string>();
                        foreach (var file in d_repository.GetFiles(value + ".*", SearchOption.AllDirectories))
                            lines.AddRange(File.ReadAllLines(file.FullName).Where(a => !string.IsNullOrEmpty(a)));

                        if (lines.Any())
                        {
                            Clipboard.SetText(lines.OrderBy(a => Guid.NewGuid()).FirstOrDefault());
                            onsuccess = true;
                        }

                        break;
                }
            }

            if (!onsuccess)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\n\t >> key \"{0}\" not found", value);
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n\n\t >> key \"{0}\" to clipboard with success", value);
            }

            if (ViewAbout)
                Console.ReadKey();
        }
    }
}
