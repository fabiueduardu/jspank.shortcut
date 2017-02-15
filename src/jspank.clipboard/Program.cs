using System;
using System.Windows.Forms;
using System.Linq;
using jspank.clipboard.Db;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using jspank.clipboard.Service;

namespace jspank.clipboard
{

    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            if (!args.Any())
                return;

            var value = args[0];
            switch (value)
            {
                case "cpf":
                    Clipboard.SetText(CPFGenerate.Get.ToString());
                    break;
                case "mail":
                    Clipboard.SetText(string.Concat(CPFGenerate.Get, "@fake.com"));
                    break;
                case "dec":
                    Clipboard.SetText(Base64Service.Decode(args[1]));
                    break;
                case "enc":
                    Clipboard.SetText(Base64Service.Encode(args[1]));
                    break;
                default:
                    var d_repository = new DirectoryInfo(string.Concat(Environment.CurrentDirectory, @"\Resource\Repository\"));

                    var lines = new List<string>();
                    foreach (var file in d_repository.GetFiles(value + ".*", SearchOption.AllDirectories))
                        lines.AddRange(File.ReadAllLines(file.FullName).Where(a => !string.IsNullOrEmpty(a)));

                    if (lines.Any())
                        Clipboard.SetText(lines.OrderBy(a => Guid.NewGuid()).FirstOrDefault());

                    break;
            }
        }
    }
}
