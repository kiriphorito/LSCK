using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace JSONTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {


            //StreamReader reader = new StreamReader("0.txt");
            //string fileJson = reader.ReadToEnd();
            //Console.Write(fileJson);

            FJController fjController = new FJController(Environment.CurrentDirectory);
            //String test = "<html>\n<body>\n  Hello World\n</body>\n</html>";
            //String test2 = "<?php\n  echo 'Hello World' \n?>";
            //fjController.insert("Hello World Test" , "HTML" , "The world's first PHP page" , test);
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

            HTMLGenerator html = new HTMLGenerator(fjController , "twilight" , true , Environment.CurrentDirectory + @"/presets" , Environment.CurrentDirectory + @"/generatedWebsite");
            html.writeHTML();
            Console.WriteLine(html.generateHTML());
        }
    }
}
