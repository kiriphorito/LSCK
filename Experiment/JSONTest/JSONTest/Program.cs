﻿using System;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace JSONTest
{
    class MainClass
    {

        public class InvalidInputException : Exception
        {
            public InvalidInputException(string message)
                : base(message) { }
        }

        public static void Main(string[] args)
        {

            //JSON json = new JSON(Environment.CurrentDirectory , "website");
            //json.newPage("hello kitty");
            //json.insertSection("Hello vi!");

            //json.setPage("Hello vi!", "home");

            //json.deleteSection("Hello world!");

            //json.insertSnippet("Hello World!" , 2 , "html" , "Programming is cool");

            //json.deleteSnippet("Hello World!" , 2);

            //json.swapSnippet("Hello World!", 1, 2);

            //json.deleteSection("Goodbye vi!");

            //json.swapSection("Hello World!" , "Goodbye vi!");

            //StreamReader reader = new StreamReader("0.txt");
            //string fileJson = reader.ReadToEnd();
            //Console.Write(fileJson);

            FJController fjController = FJController.GetInstance;



            //fjController.ReadAceThemeIndex();
            //fjController.SetAceTheme("twilight");
            //foreach (string fileName in fjController.readFileNames("Hello World!"))
            //{
            //    Console.WriteLine(fileName);
            //}
            //try
            //{
            //    fjController.InsertSection("Hello World!");
            //}
            //catch (InvalidInputException e)
            //{
            //    Console.WriteLine("You have entered this section name already!");
            //}
            //FJController fjController2 = FJController.GetInstance;

            //if (fjController == fjController2)
            //{
            //    Console.WriteLine("SAME INSTANCE");
            //}

            //fjController.InsertSnippet("Hello World!", "file" ,"A random txt file.", @"E:\Steps.docx");
            //fjController.InsertSnippet("Hello World!", "file" ,"Gin no Saji", @"E:\Hachiken.Yugo.full.1612302.jpg");
            //fjController.InsertSnippet("Hello World!", "file", "1080p", @"E:\1080p-hd-wallpaper-1080p-backgrounds-hd.jpg");
            //fjController.InsertSnippet("Hello World!", "file", "6th Biweekly report", @"E:\6th Bi-Weekly Report.pdf");

            //fjController.deleteSnippet("Hello World!", 5);

            //fjController.deleteSection("Hello World!");

            //fjController.InsertPageTitle("Kiriphorito");
            //fjController.InsertSection("Goodbye World!");
            //fjController.SetPage("Hello World!" , "index");
            //fjController.SetPage("Goodbye World!" , "index");
            //fjController.nullPage("Hello World!");

            //string test1 = "for (int i=0;i < total;i++){\n    printf(\"blah\");\n}";            //String test2 = "<?php\n  echo 'Hello World' \n?>";
            //string test2 = "<html>\n<body>\n    Hello World\n</body>\n</html>";
            //string test3 = "<?php\n    echo \"Hello World\"\n?>";
            //string test4 = "<?php\n    echo \"Goodbye Wrold\"\n?>";

            //fjController.InsertSection("Hello World!");
            //fjController.InsertSection("Goodbye World!");
            //fjController.swapSection("Hello World!", "Goodbye World!");



            //fjController.InsertSnippet("Hello World!", "c", "we invented the for loop",test1);
            //fjController.InsertSnippet("Hello World!", "html", "the world's first html!", test2);
            //fjController.InsertSnippet("Hello World!", "php", "php yay!", test3);
            //fjController.InsertSnippet("Hello World!", 2, "fortan", "hello", test4);

            //fjController.InsertSnippet("Goodbye World!", "c", "we invented the for loop", test1);
            //fjController.InsertSnippet("Goodbye World!", "html", "the world's first html!", test2);
            //fjController.InsertSnippet("Goodbye World!", "php", "php yay!", test3);
            //fjController.InsertSnippet("Goodbye World!", 2, "fortan", "hello", test4);

            //fjController.swapSnippet(2, 4, "Hello World!");

            //fjController.insert("For Loop" , "c" , "We invented the for loop" , test);
            //fjController.insert("Hello World Test" , "FSharp" , "The world's first PHP page" , test2);
            //fjController.delete(2);
            //fjController.swap(2,4);
            //Snippet test = new Snippet();
            //foreach (Snippet x in fjController.readAll())
            //{
            //    //test = fjController.readElement(1);
            //    Console.WriteLine("Position: " + x.position);
            //    Console.WriteLine("Header: " + x.header);
            //    Console.WriteLine("Language: " + x.language);
            //    Console.WriteLine("Code\n" + x.code);
            //    Console.WriteLine("Comment: " + x.comment);
            //    Console.WriteLine();
            //}

            //test = test.Replace("<" , "blah");
            //Console.WriteLine(test);

            //fjController.setAceTheme("monokai");

            //WebsiteGenerator html = new WebsiteGenerator(fjController, false, Environment.CurrentDirectory, @"C:\generatedWebsite");
            //html.GenerateWebsite();

            //SSH ssh = new SSH(@"C:\generatedWebsite");
            //ssh.UploadWebsite();

            //AcceptableList gen = new AcceptableList();
            //gen.modeList("theme-*.js", Environment.CurrentDirectory + @"/presets/ace", "acceptable_ace_themes");

            //Console.WriteLine(@"C:\generatedWebsite");
            //Console.ReadLine();
            //html.writeHTML();
            //Console.WriteLine(html.generateHTML());

            //FileHandler fh = new FileHandler();
            //fh.swap(1 , 2 , "Hello World" , Environment.CurrentDirectory + @"/data");
            //fh.insertSnippet(test1, 1, "Hello World", Environment.CurrentDirectory + @"/data");
            //fh.insertSnippet(test2, 2, "Hello World", Environment.CurrentDirectory + @"/data");
            //fh.insertSnippet(test3, 3, "Hello World", Environment.CurrentDirectory + @"/data");
            //fh.delete(2, "Hello World", Environment.CurrentDirectory + @"/data");

            String[] allfiles = System.IO.Directory.GetFiles(Environment.CurrentDirectory + @"\presets\", "*.*", System.IO.SearchOption.AllDirectories);
            for (int x = 0; x < allfiles.Count(); x++)
            {
                allfiles[x] = allfiles[x].Substring(69);
                Console.WriteLine(allfiles[x]);
            }
            File.WriteAllLines("C:/generatedWebsite/offlineFiles.txt", allfiles);
            Console.ReadLine();
        }
    }
}
