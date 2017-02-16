using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace LSCK
{
    public class HTMLGenerator
    {
        private FJController fjController;
        private readonly string fileDir;
        private readonly string generateDir;
        private bool CDN;

        public HTMLGenerator(FJController fjController, bool CDN, string fileDir, string generateDir)
        {
            this.fjController = fjController;
            this.fileDir = fileDir;
            this.generateDir = generateDir;
            this.CDN = CDN;
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
            if (!Directory.Exists(generateDir))
            {
                Directory.CreateDirectory(generateDir);
            }
            else
            {
                //Clean out directory
                DeleteFolder(generateDir);
            }
            if (CDN == false)
            {
                Directory.CreateDirectory(generateDir + @"/styles");
                File.Copy(fileDir + @"/presets/bootstrap.min.css", generateDir + @"/styles/bootstrap.min.css");
                Directory.CreateDirectory(generateDir + @"/styles/ace");
                string sourcePath = fileDir + @"/presets/ace";
                string destinPath = generateDir + @"/styles/ace";
                foreach (string dirPath in Directory.GetDirectories(fileDir + @"/presets/ace", "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinPath));
                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(sourcePath, destinPath), true);
            }
            foreach (string pageTitle in fjController.GetPageTitles())
            {
                //Console.WriteLine(pageTitle);
                //Console.WriteLine(generateHTML(pageTitle));
                WriteHTML(GenerateHTML(pageTitle), pageTitle);
            }
        }

        public string GenerateHTML(string pageTitle)
        {
            var htmlCL = new List<string>(); //HTMLContentList

            List<Section> page = fjController.GetPage(pageTitle);
            List<Snippet> pageSnippets = fjController.PageSnippets(page);

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
            htmlCL.Add("    <link href=\"style.css\" rel=\"stylesheet\">");
            htmlCL.Add("</head>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
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
                    htmlCL.Add("            <p>" + snippet.comment + "</p>");
                    if (snippet.language != "file")
                    {
                        htmlCL.Add("            <div id = \"editor" + ++x + "\">" + code[0]);
                        for (int y = 1; y < code.Count - 1; y++)
                        {
                            htmlCL.Add(code[y]);
                        }
                        htmlCL.Add(code[code.Count - 1] + "</div>");
                    }
                    else
                    {
                        if (!Directory.Exists(generateDir + @"/userfiles"))
                        {
                            Directory.CreateDirectory(generateDir + @"/userfiles");
                        }
                        Console.WriteLine("Write File!");
                        File.Copy(fileDir + @"/data/userfiles/" + code[0], generateDir + @"/userfiles/" + code[0]);
                        htmlCL.Add("            <center><a href=\"userfiles/" + code[0] + "\"/>" + code[0] + "</a></center>");
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
