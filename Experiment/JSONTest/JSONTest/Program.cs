using System;
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
            fjController.SetAceTheme("twilight");
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

            //fjController.insertFile("Hello World!", "A random txt file.", "/Users/sampham/Documents/Test1.txt");

            //fjController.deleteSnippet("Hello World!", 5);

            //fjController.deleteSection("Hello World!");

            //fjController.insertPageName("Kiriphorito");
            //fjController.insertSection("Goodbye World!");
            //fjController.setPage("Hello World!" , "home");
            //fjController.nullPage("Hello World!");

            String test1 = "for (int i=0;i < total;i++){\n    printf(\"blah\");\n}";            //String test2 = "<?php\n  echo 'Hello World' \n?>";
            String test2 = "<html>\n<body>\n    Hello World\n</body>\n</html>";
            String test3 = "<?php\n    echo \"Hello World\"\n?>";
            String test4 = "<?php\n    echo \"Goodbye Wrold\"\n?>";

            //fjController.insertSection("Hello World!");
            //fjController.swapSection("Hello World!", "Goodbye World!");



            //fjController.insertSnippet("Hello World!", "c" , "We invented the for loop" , test1);
            //fjController.insertSnippet("Hello World!", "html", "The world's first html!", test2);
            //fjController.insertSnippet("Hello World!", "php", "PHP yay!", test3);
            //fjController.insertSnippet("Hello World!", 2, "fortan", "Hello", test4);

            //fjController.insertSnippet("Goodbye World!", "c" , "We invented the for loop" , test1);
            //fjController.insertSnippet("Goodbye World!", "html", "The world's first html!", test2);
            //fjController.insertSnippet("Goodbye World!", "php", "PHP yay!", test3);
            //fjController.insertSnippet("Goodbye World!", 2, "fortan", "Hello", test4);

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

            //HTMLGenerator html = new HTMLGenerator(fjController , false , Environment.CurrentDirectory, Environment.CurrentDirectory + @"/generatedWebsite");
            //html.generateWebsite();

            //AcceptableList gen = new AcceptableList();
            //gen.modeList("theme-*.js", Environment.CurrentDirectory + @"/presets/ace", "acceptable_ace_themes");



            //html.writeHTML();
            //Console.WriteLine(html.generateHTML());

            //FileHandler fh = new FileHandler();
            //fh.swap(1 , 2 , "Hello World" , Environment.CurrentDirectory + @"/data");
            //fh.insertSnippet(test1, 1, "Hello World", Environment.CurrentDirectory + @"/data");
            //fh.insertSnippet(test2, 2, "Hello World", Environment.CurrentDirectory + @"/data");
            //fh.insertSnippet(test3, 3, "Hello World", Environment.CurrentDirectory + @"/data");
            //fh.delete(2, "Hello World", Environment.CurrentDirectory + @"/data");
        }
    }
}
