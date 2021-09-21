using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace EmbleemsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("EmbleemsBot by DaHuK\n\n");
            Console.ResetColor();

            int Error;

            Console.WriteLine("Getting links...");
            List<string> link = new List<string>();
            Error = GetListOfLinks(ref link);
            CheckError(Error);

            Console.WriteLine("Getting Text...");
            string text = null;
            Error = GetText(ref text);
            CheckError(Error);

            string path = Directory.GetCurrentDirectory();
            int image = 0;
            Console.WriteLine("Getting Image:" + path + "\\Source\\Image.jpg");
            if (File.Exists(path + "/Source/Image.jpg"))
            {
                image = 1;
                CheckError(Error);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Working without image.\n");
                Console.ResetColor();
            }
          

            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            IWebDriver Brows = new ChromeDriver(driverService);

            Console.WriteLine("Starting Browser...");
            Error = ActivateBrowser(ref Brows);
            CheckError(Error);

            Console.WriteLine("Authorization...");
            Error = Authorization(Brows);
            CheckError(Error);

            Console.WriteLine("Starting work...");
            Error = Work(Brows, link, text, image);
            CheckError(Error);
        }

        static private void ErrorMessage(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        static private void CheckError(int error)
        {
            if (error == 1)
            {
                Console.ReadKey();
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Done.\n");
                Console.ResetColor();
            }
        }

        static private int GetListOfLinks(ref List<string> link)
        {
            string path = Directory.GetCurrentDirectory() + "\\Source\\Links.txt";

            try
            {
                link = Get.ListOfLines("Source/Links.txt");

                return 0;
            }
            catch
            {
                ErrorMessage("File with links didn't found.\n" +
                    "Need file: " + path);

                return 1;
            }
        }

        static private int GetText(ref string str)
        {
            string path = Directory.GetCurrentDirectory() + "\\Source\\Text.txt";

            try
            {
                str = Get.FileText("Source/Text.txt");

                return 0;
            }
            catch
            {
                ErrorMessage("File with text didn't found.\n" +
                    "Need file: " + path + "\n" +
                    "with UTF-8 encoding.");

                return 1;
            }
        }

        static private int ActivateBrowser(ref IWebDriver Browser)
        {
            try
            {
                Browser.Manage().Window.Minimize();
                Browser.Navigate().GoToUrl("https://vk.com");

                return 0;
            }
            catch
            {
                ErrorMessage("Activate Browser Error." +
                    "\nCheck versions of dll.");

                return 1;
            }
        }

        static private int Authorization(IWebDriver Brows)
        {
            IWebElement Find(string str)
            {
                return Brows.FindElement(By.XPath(str));
            }

            IWebElement element;
            int CheckPlace = -1;

            string temp;

            try
            {
                element = Find("//input[@id = 'index_email']");
                Console.Write("Login: ");
                temp = Console.ReadLine();
                element.SendKeys(temp);

                element = Find("//input[@id = 'index_pass']");
                Console.Write("Password: ");
                temp = Console.ReadLine();
                element.SendKeys(temp + Keys.Enter);
            }
            catch
            {
                ErrorMessage("Page error.\n" +
                    "Maybe you closed page?");

                return 1;
            }

            Brows.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            try
            {
                element = Find("//input[@id='authcheck_code']");

                CheckPlace = 0;
            }
            catch
            {
                if (Brows.Title == "Новости")
                {
                    CheckPlace = 1;
                }    
                else
                {
                    CheckPlace = -1;
                }
            }

            switch (CheckPlace)
            {
                case -1:
                    ErrorMessage("Wrong login or password.\n" +
                        "Try one more time.\n");
                    Console.WriteLine("Press any key to continue..");
                    Console.ReadKey();
                    Brows.Navigate().GoToUrl("https://vk.com");
                    Authorization(Brows);
                    break;

                case 1:
                    return 0;

                case 0:
                    string key;
                    while (true)
                    {
                        Console.Write("Write two-factor authentication key: ");
                        key = Console.ReadLine();
                        Console.WriteLine("Proccess...");
                        element.SendKeys(key + Keys.Enter);

                        Thread.Sleep(2000);

                        if (Brows.Title == "Новости")
                        {
                            return 0;
                        }
                        else
                        {
                            ErrorMessage("\nWrong key. Try again.");
                            element.Clear();
                        }
                    }
            }

            ErrorMessage("\n\nCongritulations! You found bug :)\n" +
                "Write about it on my GitHub.");

            return 1;
        }

        static private int Work(IWebDriver Brows, List<string> link, string text, int image)
        {
            IWebElement Find(string str)
            {
                return Brows.FindElement(By.XPath(str));
            }

            IWebElement element;

            Brows.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            
            foreach (var i in link)
            {
                try
                {
                    Brows.Navigate().GoToUrl(i);
                    try
                    {
                        element = Find("//div[@class='box_title']");
                        element = Find("//div[@class='box_x_button']");
                        element.Click();
                    }
                    catch 
                    {
                        //Just check for box;
                    }
                    Console.Write(Brows.Title + ": ");
                    if (image == 1)
                    {
                        element = Find("//div[@class='reply_field submit_post_field']");
                        element.Click();
                        element = Find("//a[@class='ms_item ms_item_photo _type_photo']");
                        element.Click();
                        element = Find("//input[@id='choose_photo_upload']");
                        element.SendKeys(Directory.GetCurrentDirectory() + "/Source/Image.jpg");
                        Thread.Sleep(3000);
                    }
                    element = Find("//div[@class = 'reply_field submit_post_field']");
                    string[] lines = text.Split('\n');
                    foreach (var line in lines)
                    {
                        element.SendKeys(line + (Keys.Shift + '\n'));
                    }
                    element.SendKeys(Keys.Enter);
                    Thread.Sleep(3000);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Done!\n");
                    Console.ResetColor();
                }
                catch
                {
                    ErrorMessage("Error");
                }
            }

            return 0;
        }
    }
}
