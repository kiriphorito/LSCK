using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LSCK
{
    public class WebsiteGenerator
    {
        private FJController fjController;
        private readonly string fileDir;
        private readonly string generateDir;
        private bool CDN;
        private List<string> custStrings;

        public WebsiteGenerator(FJController fjController, bool CDN, string fileDir, string generateDir,List<string> custStrings)
        {
            this.fjController = fjController;
            this.fileDir = fileDir;
            this.generateDir = generateDir;
            this.CDN = CDN;
            this.custStrings = custStrings;
        }

        private void WriteHTML(string htmlCode, string pageTitle)
        {
            string path = string.Concat(generateDir, @"/" + pageTitle.ToLower().Replace(" ", "") + ".html");
            File.WriteAllText(path, htmlCode);
        }

        private void DeleteFolder(string dir)
        {
            var di = new DirectoryInfo(dir);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo folder in di.GetDirectories())
            {
                DeleteFolder(folder.FullName);
                folder.Delete(true);
            }
        }

        public void GenerateWebsite()
        {
            if (Directory.Exists(generateDir))
                //Clean out directory
                DeleteFolder(generateDir);
            Directory.CreateDirectory(generateDir);
            if (CDN == false)
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LSCK.Resources.Assets.offlineFiles.txt");
                TextReader tr = new StreamReader(stream);
                string offlineFiles = tr.ReadToEnd();
                List<string> listOfflineFiles = offlineFiles.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                ExtractEmbeddedResource(generateDir + @"\styles\", "LSCK.Resources.Assets.presets", listOfflineFiles);
            }
            foreach (string pageTitle in fjController.GetPageTitles())
            {
                WriteHTML(GenerateHTML(pageTitle), pageTitle);
            }
            GenerateCustomCSS();
        }

        private static void ExtractEmbeddedResource(string outputDir, string resourceLocation, List<string> files)
        {
            foreach (string file in files)
            {
                string resourceLoc = resourceLocation + @"." + file;
                resourceLoc = resourceLoc.Replace('\\', '.');
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLoc))
                {
                    //Some string in file may have path in them
                    string path = Path.GetDirectoryName(Path.Combine(outputDir, file)) + @"\";
                    Directory.CreateDirectory(path);
                    using (FileStream fileStream = new FileStream(Path.Combine(outputDir, file), FileMode.Create))
                    {
                        for (int i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }
                        fileStream.Close();
                    }
                }
            }
        }

        public string GenerateHTML(string pageTitle)
        {
            var htmlCL = new List<string>(); //HTMLContentList

            List<Section> page = fjController.GetPage(pageTitle);
            List<Snippet> pageSnippets = fjController.PageSnippetsOnly(page);

            htmlCL.Add(GenerateHead(fjController.GetTitle()));
            htmlCL.Add(GenerateBody(page, fjController.GetTitle(), pageTitle));
            htmlCL.Add(GenerateAceScript(pageSnippets, fjController.GetAceTheme()));
            htmlCL.Add(GenerateFooter());

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private string GenerateHead(string title)
        {
            var htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("<!DOCTYPE html>");
            htmlCL.Add("<html lang=\"en\">");
            htmlCL.Add("<html>");
            htmlCL.Add("<head>");
            htmlCL.Add("    <meta charset=\"utf-8\">");
            htmlCL.Add("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            htmlCL.Add("    <title>" + title + "</title>");
            if (CDN == true)
            {
                htmlCL.Add("    <link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css\"/>");
            }
            else
            {
                htmlCL.Add("    <link rel=\"stylesheet\" href=\"styles/bootstrap.min.css\"/>");
            }
            htmlCL.Add("    <link href=\"styles/custom.css\" rel=\"stylesheet\">");
            htmlCL.Add("    <link href=\"style.css\" rel=\"stylesheet\">");
            htmlCL.Add("</head>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private void GenerateCustomCSS()
        {
            var cssCL = new List<string>(); //HTMLContentList
            System.IO.Directory.CreateDirectory(generateDir+"/styles");
            string path = generateDir + "/styles/custom.css";

            cssCL.Add(".navbar-default {\n  background-color:"+custStrings[0]+";\n   border-color: "+custStrings[5]+"\n}");//NavBar bg color and border color
            cssCL.Add(".navbar-default .navbar-brand {\n    color:"+custStrings[1]+";\n}");//NavBar Title color
            cssCL.Add(".navbar-default .navbar-brand:hover,\n.navbar-default .navbar-brand:focus {\ncolor:"+custStrings[2]+";\n}");//NavBar title hover color
            cssCL.Add(".navbar-default .navbar-nav > li > a {\n\tcolor: "+custStrings[3]+";\n}");//NavBar Page color
            cssCL.Add(".navbar-default .navbar-nav > li > a:hover,\n.navbar-default .navbar-nav > li > a:focus {\n  color: "+custStrings[4]+";\n}");//NavBar page hover color
            cssCL.Add(".navbar-default .navbar-nav > .active > a, .navbar-default .navbar-nav > .active >   a:hover, .navbar-default .navbar-nav > .active > a:focus {color: "+custStrings[9]+";}");
            cssCL.Add("body {\nbackground-color: "+custStrings[6]+";\n}");
            cssCL.Add("h3 {\nfont-family: \""+custStrings[10]+"\";\nfont-size: "+custStrings[12]+"px;\ncolor: "+custStrings[7]+";\n}");
            cssCL.Add("p{\nfont-family: "+custStrings[11]+";\nfont-size: "+custStrings[13]+"px;\ncolor: "+custStrings[8]+";\n}");

            string cssContent = string.Join("\n", cssCL.ToArray());
            File.WriteAllText(path, cssContent);

        }

        private string GenerateNavBar(string title, string pageTitle)
        {
            var htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("    <nav class=\"navbar navbar-inverse navbar-default navbar-static-top\" role=\"navigation\">");
            htmlCL.Add("        <div class=\"container\">");
            htmlCL.Add("            <div class=\"navbar-header\">");
            htmlCL.Add("                <button type=\"button\" class=\"navbar-toggle collapsed\" data-toggle=\"collapse\" data-target=\"#navbar\" aria-expanded=\"false\" aria-controls=\"navbar\">");
            htmlCL.Add("                    <span class=\"sr-only\">Toggle navigation</span>");
            htmlCL.Add("                    <span class=\"icon-bar\"></span>");
            htmlCL.Add("                    <span class=\"icon-bar\"></span>");
            htmlCL.Add("                    <span class=\"icon-bar\"></span>");
            htmlCL.Add("                </button>");
            htmlCL.Add("                <a class=\"navbar-brand\" href=\"" + fjController.GetPageTitles()[0].ToLower().Replace(" ", "") + ".html\">" + title + "</a>");
            htmlCL.Add("            </div>");
            htmlCL.Add("            <div id=\"navbar\" class=\"collapse navbar-collapse\">");
            htmlCL.Add("                <ul class=\"nav navbar-nav navbar-left\">");
            for (int x = 1; x < fjController.GetPageTitles().Count; x++)
            {
                htmlCL.Add("                    <li" + ((fjController.GetPageTitles()[x] == pageTitle) ? " class=\"active\"" : "") + ">");
                htmlCL.Add("                        <a href=\"" + fjController.GetPageTitles()[x].ToLower().Replace(" ", "") + ".html\">" + fjController.GetPageTitles()[x] + "</a>");
                htmlCL.Add("                    </li>");
            }
            htmlCL.Add("                </ul>");
            htmlCL.Add("            </div>");
            htmlCL.Add("        </div>");
            htmlCL.Add("    </nav>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private string GenerateBody(List<Section> page, string title, string pageTitle)
        {
            var htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("<body>");
            htmlCL.Add(GenerateNavBar(title, pageTitle));
            htmlCL.Add("    <div class=\"container\">");
            htmlCL.Add("        <div class=\"container-fluid text-left\">");
            int x = 0;
            foreach (Section section in page)
            {
                htmlCL.Add("            <h3><strong>" + section.sectionName + "</strong></h3>");
                foreach (Snippet snippet in section.snippets)
                {
                    List<string> code = snippet.code.Split('\n').ToList();
                    for (int y = 0; y < code.Count; y++)
                    {
                        //Change character to special character equivalent
                        code[y] = code[y].Replace("<", "&lt;");
                    }
                    if (snippet.language != "file")
                    {
                        htmlCL.Add("            <p>" + snippet.comment + "</p>");
                        htmlCL.Add("            <div id = \"editor" + ++x + "\">" + code[0]);
                        for (int y = 1; y < code.Count - 1; y++)
                        {
                            htmlCL.Add(code[y]);
                        }
                        htmlCL.Add(code[code.Count - 1] + "</div>");
                    }
                    else
                    {
                        htmlCL.Add("            <p><center>" + snippet.comment + "</center></p>");
                        if (!Directory.Exists(generateDir + @"/userfiles"))
                        {
                            Directory.CreateDirectory(generateDir + @"/userfiles");
                        }
                        string originalPath = fileDir + @"/LSCK Data/data/userfiles/" + code[0];
                        File.Copy(originalPath, generateDir + @"/userfiles/" + code[0]);
                        string[] allowedFormats = { ".jpg", ".png", ".gif" };
                        switch (Path.GetExtension(originalPath))
                        {
                            case ".jpg":
                            case ".png":
                            case ".gif":
                                htmlCL.Add("            <center><img src=\"userfiles/" + code[0] + "\" style=\"max-width:50em\"/></center>");
                                break;
                            case ".pdf":
                                htmlCL.Add("            <center><embed  src=\"userfiles/" + code[0] + "\" width=\"800px\" height=\"1000px\"/></center>");
                                break;
                            default:
                                htmlCL.Add("            <center><a href=\"userfiles/" + code[0] + "\"/>" + code[0] + "</a></center>");
                                break;
                        }
                    }
                    htmlCL.Add("            <br>");
                }
            }
            htmlCL.Add("        </div>");
            htmlCL.Add("    </div>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private string GenerateAceScript(List<Snippet> snippets, string aceTheme)
        {
            var htmlCL = new List<string>(); //HTMLContentList

            if (CDN == true)
            {
                htmlCL.Add("    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/ace/1.2.6/ace.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
            }
            else
            {
                htmlCL.Add("    <script src=\"styles/ace/ace.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
            }
            htmlCL.Add("    <script>");
            htmlCL.Add("        for (x = 1 ; x <= " + snippets.Count(snippet => snippet.language != "file") + " ; x++){");
            htmlCL.Add("        var editor = ace.edit(\"editor\" + x);");
            htmlCL.Add("            editor.getSession().setUseWorker(false);");
            htmlCL.Add("            editor.setTheme(\"ace/theme/" + aceTheme + "\");");
            htmlCL.Add("            editor.renderer.setScrollMargin(10, 10, 0, 0);");
            htmlCL.Add("            editor.renderer.$cursorLayer.element.style.opacity = 0;");
            htmlCL.Add("            editor.setShowPrintMargin(false);");
            htmlCL.Add("            editor.setOptions({");
            htmlCL.Add("                maxLines: Infinity,");
            htmlCL.Add("                readOnly: true,");
            htmlCL.Add("                highlightActiveLine: false,");
            htmlCL.Add("                highlightGutterLine: false,");
            htmlCL.Add("            });");
            htmlCL.Add("        }");
            int y = 0;
            for (int x = 1; x <= snippets.Count; x++)
            {
                if (snippets[x - 1].language == "file")
                    continue;
                htmlCL.Add("        ace.edit(\"editor" + ++y + "\").getSession().setMode(\"ace/mode/" + snippets[x - 1].language + "\");");
            }
            htmlCL.Add("    </script>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private string GenerateFooter()
        {
            var htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("</body>");
            htmlCL.Add("</html>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }
    }
}
