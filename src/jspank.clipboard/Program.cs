using jspank.clipboard.Db;
using jspank.core.services;
using jspank.core.Services;
using jspank.crosscutting.ioc;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;

namespace jspank.clipboard
{
    class Program
    {
        private static string GoogleTranslatePatternLanguageTarget
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleTranslatePatternLanguageTarget"] as string;
            }
        }

        private static bool ViewResult
        {
            get
            {
                var value = ConfigurationManager.AppSettings["ViewResult"] as string;
                return !string.IsNullOrEmpty(value) && value.Equals("1");
            }
        }

        private static string MailDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["MailDomain"] ?? "@mock.com";
            }
        }
        private const string FolderRepositories = @"\Resources\Repositories\";
        private static string CurrentDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

        private static Container serviceResolve = null;
        private static Container ServiceResolve
        {
            get
            {
                if (serviceResolve == null)
                    IocContainer.RegisterAll(ref serviceResolve);

                return serviceResolve;
            }
            set
            {
                serviceResolve = value;
            }
        }

        [STAThreadAttribute]
        private static void Main(string[] args)
        {
            var currentDirectoryRepositories = string.Concat(CurrentDirectory, FolderRepositories);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("\t\t#### keys ####");
            Console.WriteLine("\t t (value) #Tranlate(default english)");
            Console.WriteLine("\t t ar (value) #Tranlate to ar-arabic > https://cloud.google.com/translate/docs/languages");
            Console.WriteLine("\t cpf #Random CPF");
            Console.WriteLine("\t cnpj #Random CNPJ");
            Console.WriteLine("\t mail #Random E-mail");
            Console.WriteLine("\t dec Ex: \"Caller\" dec (value)");
            Console.WriteLine("\t enc Ex: \"Caller\" enc (value)");
            Console.WriteLine("\t pwd #Random Password");
            Console.WriteLine("\t start key from file start.* in repository folder ex: jspank.clipboard;c:\\jspank.clipboard;arguments");
            Console.WriteLine("\t ??? - other itens in repository folder Ex: Lorem.txt");
            Console.WriteLine("\n\t repository: {0}", currentDirectoryRepositories);

            var value = string.Empty;
            var onsuccess = false;
            if (args.Any())
            {
                value = args[0];
                switch (value)
                {
                    case "t":
                        {
                            var indice = 0;
                            var languagetarget = "en";
                            if (args[1].Length.Equals(2))
                            {
                                indice = 1;
                                languagetarget = args[1];
                            }
                            else
                                languagetarget = GoogleTranslatePatternLanguageTarget;

                            var translateService = ServiceResolve.GetInstance<ITranslateService>();
                            var value_args = args.Select((v, i) => new { i, v }).Where(a => a.i > indice);
                            var value_result = string.Join(" ", value_args.Select(a => a.v));

                            var translate = translateService.Translate(value_result, languagetarget: languagetarget);
                            Clipboard.SetText(string.Join(Environment.NewLine, translate));

                            onsuccess = true;
                        }
                        break;
                    case "cpf":
                        Clipboard.SetText(CPFGenerate.Get.ToString());
                        onsuccess = true;
                        break;
                    case "cnpj":
                        Clipboard.SetText(CNPJGenerate.Get.ToString());
                        onsuccess = true;
                        break;
                    case "mail":
                        Clipboard.SetText(string.Concat(CPFGenerate.Get, MailDomain));
                        onsuccess = true;
                        break;
                    case "dec":
                    case "enc":
                        {
                            var value_args = args.Select((v, i) => new { i, v }).Where(a => a.i > 0);
                            var value_result = string.Join(" ", value_args.Select(a => a.v));

                            if (value.Equals("dec"))
                                Clipboard.SetText(Base64Service.Decode(value_result));
                            else
                                Clipboard.SetText(Base64Service.Encode(value_result));

                            onsuccess = true;
                        }
                        break;
                    case "pwd":
                        int length = 0, numberOfNonAlphanumericCharacters = 0;

                        if (args.Length > 1)
                        {
                            int.TryParse(args[1], out length);

                            if (args.Length > 2)
                                int.TryParse(args[2], out numberOfNonAlphanumericCharacters);

                        }
                        length = length <= 0 ? 10 : length;
                        numberOfNonAlphanumericCharacters = numberOfNonAlphanumericCharacters <= 0 || numberOfNonAlphanumericCharacters > length ? 2 : numberOfNonAlphanumericCharacters;

                        Clipboard.SetText(Membership.GeneratePassword(length, numberOfNonAlphanumericCharacters));
                        break;
                    case "start":
                        {
                            var d_repository = new DirectoryInfo(currentDirectoryRepositories);
                            foreach (var file in d_repository.GetFiles(value + ".*", SearchOption.AllDirectories))
                            {
                                foreach (var line in File.ReadAllLines(file.FullName))
                                {
                                    if (line.StartsWith(args[1]))
                                    {
                                        var start_ = line.Split(';');
                                        var start_target = start_[1];
                                        var start_target_arguments = start_.Length > 2 ? start_[2] : null;

                                        Process.Start(start_target, start_target_arguments);

                                        onsuccess = true;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        {
                            try
                            {
                                var d_repository = new DirectoryInfo(currentDirectoryRepositories);
                                if (!d_repository.Exists) d_repository.Create();

                                var lines = new List<string>();
                                foreach (var file in d_repository.GetFiles(value + ".*", SearchOption.AllDirectories))
                                    lines.AddRange(File.ReadAllLines(file.FullName).Where(a => !string.IsNullOrEmpty(a)));

                                if (lines.Any())
                                {
                                    Clipboard.SetText(lines.OrderBy(a => Guid.NewGuid()).FirstOrDefault());
                                    onsuccess = true;
                                }
                            }
                            catch (Exception exp)
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.WriteLine(exp);

                            }
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

            if (ViewResult)
                Console.ReadKey();
        }
    }
}
