using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace JSONTest
{
    public class HTMLGenerator
    {
        private FJController fjController;
        private readonly String presetDir;
        private readonly String generateDir;
        private bool CDN;

        public HTMLGenerator(FJController fjController, bool CDN, string presetDir, string generateDir)
        {
            this.fjController = fjController;
            this.presetDir = presetDir;
            this.generateDir = generateDir;
            this.CDN = CDN;
        }

        private void writeHTML(string htmlCode, string pageTitle)
        {
            string path = string.Concat(generateDir, @"/" + pageTitle.ToLower().Replace(" ", "") + ".html");
            File.WriteAllText(path, htmlCode);
        }

        public void generateWebsite()
        {
            if (!Directory.Exists(generateDir))
            {
                Directory.CreateDirectory(generateDir);
            }
            else
            {
                //Clean out directory
                DirectoryInfo di = new DirectoryInfo(generateDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            foreach (string pageTitle in fjController.getPageTitles())
            {
                Console.WriteLine(pageTitle);
                Console.WriteLine(generateHTML(pageTitle));
                writeHTML(generateHTML(pageTitle), pageTitle);
            }
        }

        public string generateHTML(string pageTitle)
        {
            List<string> htmlCL = new List<string>(); //HTMLContentList

            List<Section> page = fjController.readPage(pageTitle);
            List<Snippet> pageSnippets = fjController.snippetsOnly(page);

            htmlCL.Add(generateHead(fjController.getTitle()));
            htmlCL.Add(generateBody(page, fjController.getTitle(), pageTitle));
            htmlCL.Add(generateAceScript(pageSnippets, fjController.getAceTheme()));
            htmlCL.Add(generateFooter());

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private string generateHead(string title)
        {
            List<string> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("<!DOCTYPE html>");
            htmlCL.Add("<html lang=\"en\">");
            htmlCL.Add("<html>");
            htmlCL.Add("<head>");
            htmlCL.Add("    <meta charset=\"utf-8\">");
            htmlCL.Add("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            htmlCL.Add("    <title>" + title + "</title>");
            htmlCL.Add("    <link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css\" type=\"text/css\"/>");
            htmlCL.Add("    <link href=\"style.css\" rel=\"stylesheet\">");
            htmlCL.Add("</head>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private string generateNavBar(string title, string pageTitle)
        {
            List<string> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("    <nav class=\"navbar navbar-inverse navbar-default navbar-static-top\" role=\"navigation\">");
            htmlCL.Add("        <div class=\"container\">");
            htmlCL.Add("            <div class=\"navbar-header\">");
            htmlCL.Add("                <button type=\"button\" class=\"navbar-toggle collapsed\" data-toggle=\"collapse\" data-target=\"#navbar\" aria-expanded=\"false\" aria-controls=\"navbar\">");
            htmlCL.Add("                    <span class=\"sr-only\">Toggle navigation</span>");
            htmlCL.Add("                    <span class=\"icon-bar\"></span>");
            htmlCL.Add("                    <span class=\"icon-bar\"></span>");
            htmlCL.Add("                    <span class=\"icon-bar\"></span>");
            htmlCL.Add("                </button>");
            htmlCL.Add("                <a class=\"navbar-brand\" href=\"" + fjController.getPageTitles()[0].ToLower().Replace(" ", "") + ".html\">" + title + "</a>");
            htmlCL.Add("            </div>");
            htmlCL.Add("            <div id=\"navbar\" class=\"collapse navbar-collapse\">");
            htmlCL.Add("                <ul class=\"nav navbar-nav navbar-right\">");
            for (int x = 1; x < fjController.getPageTitles().Count; x++)
            {
                htmlCL.Add("                    <li>");
                htmlCL.Add("                        <a href=\"" + fjController.getPageTitles()[x].ToLower().Replace(" ","") + ".html\">" + fjController.getPageTitles()[x] + "</a>");
                htmlCL.Add("                    </li>");
            }
            htmlCL.Add("                </ul>");
            htmlCL.Add("            </div>");
            htmlCL.Add("        </div>");
            htmlCL.Add("    </nav>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private string generateBody(List<Section> page, string title, string pageTitle)
        {
            List<string> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("<body>");
            htmlCL.Add(generateNavBar(title, pageTitle));
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
                    htmlCL.Add("            <div id = \"editor" + ++x + "\">" + code[0]);
                    for (int y = 1; y < code.Count - 1; y++)
                    {
                        htmlCL.Add(code[y]);
                    }
                    htmlCL.Add(code[code.Count - 1] + "</div>");
                    htmlCL.Add("            <br>");
                }
            }
            htmlCL.Add("        </div>");
            htmlCL.Add("    </div>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private String generateAceScript(List<Snippet> snippets, string aceTheme)
        {
            List<String> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/ace/1.2.6/ace.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
            htmlCL.Add("    <script>");
            htmlCL.Add("        for (x = 1 ; x <= " + snippets.Count + " ; x++){");
            htmlCL.Add("        var editor = ace.edit(\"editor\" + x);");
            htmlCL.Add("            editor.getSession().setUseWorker(false);");
            htmlCL.Add("            editor.setTheme(\"ace/theme/" + aceTheme + "\");");
            htmlCL.Add("            editor.renderer.setScrollMargin(10, 10, 0, 0);");
            htmlCL.Add("            editor.renderer.$cursorLayer.element.style.opacity = 0;");
            htmlCL.Add("            editor.setOptions({");
            htmlCL.Add("                maxLines: Infinity,");
            htmlCL.Add("                readOnly: true,");
            htmlCL.Add("                highlightActiveLine: false,");
            htmlCL.Add("                highlightGutterLine: false");
            htmlCL.Add("            });");
            htmlCL.Add("        }");
            for (int x = 1; x <= snippets.Count; x++)
            {
                htmlCL.Add("        ace.edit(\"editor" + x + "\").getSession().setMode(\"ace/mode/" + snippets[x - 1].language + "\");");
            }
            htmlCL.Add("    </script>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private String generateFooter()
        {
            List<String> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("</body>");
            htmlCL.Add("</html>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }
    }
}
