using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace LSCK
{
    public class HTMLGenerator
    {
        private FJController fjController;
        private readonly String presetDir;
        private readonly String generateDir;
        private String aceTheme;
        private Boolean CDN;

        public HTMLGenerator(FJController fjController , String aceTheme , Boolean CDN , String presetDir , String generateDir)
        {
            this.fjController = fjController;
            this.presetDir = presetDir;
            this.generateDir = generateDir;
            this.aceTheme = aceTheme;
            this.CDN = CDN;
        }

        public void writeHTML()
        {
            if (!Directory.Exists(generateDir))
            {
                Directory.CreateDirectory(generateDir);
            }
            else 
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(generateDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            //string path = string.Concat(generateDir, @"/index.html");
            //File.WriteAllText(path, generateHTML());
        }

        public String generateHTML()
        {
            List<String> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add(generateHead());

            htmlCL.Add(generateBody());
            htmlCL.Add(generateAceScript());
            htmlCL.Add("</body>");
            htmlCL.Add("</html>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private String generateHead()
        {
            List<String> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("<!DOCTYPE html>");
            htmlCL.Add("<html lang=\"en\">");
            htmlCL.Add("<html>");
            htmlCL.Add("<head>");
            htmlCL.Add("    <meta charset=\"utf-8\">");
            htmlCL.Add("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            htmlCL.Add("    <title>Test</title>");
            htmlCL.Add("    <link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css\" type=\"text/css\"/>");
            htmlCL.Add("    <link href=\"style.css\" rel=\"stylesheet\">");
            htmlCL.Add("</head>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private String generateNavBar()
        {
            List<String> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("    <nav class=\"navbar navbar-inverse navbar-default navbar-static-top\" role=\"navigation\">");
            htmlCL.Add("        <div class=\"container\">");
            htmlCL.Add("            <div class=\"navbar-header\">");
            htmlCL.Add("                <button type=\"button\" class=\"navbar-toggle collapsed\" data-toggle=\"collapse\" data-target=\"#navbar\" aria-expanded=\"false\" aria-controls=\"navbar\">");
            htmlCL.Add("                    <span class=\"sr-only\">Toggle navigation</span>");
            htmlCL.Add("                    <span class=\"icon-bar\"></span>");
            htmlCL.Add("                    <span class=\"icon-bar\"></span>");
            htmlCL.Add("                    <span class=\"icon-bar\"></span>");
            htmlCL.Add("                </button>");
            htmlCL.Add("                <a class=\"navbar-brand\" href=\"/\">" + fjController.getTitle() + "</a>");
            htmlCL.Add("            </div>");
            htmlCL.Add("            <div id=\"navbar\" class=\"collapse navbar-collapse\">");
            htmlCL.Add("                <ul class=\"nav navbar-nav navbar-right\">");
            htmlCL.Add("                    <li>");
            htmlCL.Add("                    </li>");
            htmlCL.Add("                </ul>");
            htmlCL.Add("            </div>");
            htmlCL.Add("        </div>");
            htmlCL.Add("    </nav>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private String generateBody()
        {
            List<String> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("<body>");
            htmlCL.Add(generateNavBar());
            htmlCL.Add("    <div class=\"container\">");
            htmlCL.Add("        <div class=\"container-fluid text-left\">");
            for (int x = 1; x <= fjController.readAll().Count; x++)
            {
                List<string> code = fjController.readElement(x).code.Split('\n').ToList();
                for (int y = 0 ; y < code.Count ; y++)
                {
                    //Change character to special character equivalent
                    code[y] = code[y].Replace("<", "&lt;");
                }
                htmlCL.Add("<h3><strong>" + fjController.readElement(x).header + "</strong></h3>");
                htmlCL.Add("<p>" + fjController.readElement(x).comment + "</p>");
                htmlCL.Add("            <div id = \"editor" + x + "\">" + code[0]);
                for (int y = 1 ; y < code.Count - 1 ; y++)
                {
                    htmlCL.Add(code[y]);
                }
                htmlCL.Add(code[code.Count - 1] + "</div>");
            }
            htmlCL.Add("        </div>");
            htmlCL.Add("    </div>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }

        private String generateAceScript()
        {
            List<String> htmlCL = new List<string>(); //HTMLContentList

            htmlCL.Add("    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/ace/1.2.6/ace.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
            htmlCL.Add("    <script>");
            htmlCL.Add("        for (x = 1 ; x <= " + fjController.readAll().Count + " ; x++){");
            htmlCL.Add("        var editor = ace.edit(\"editor\" + x);");
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
            for (int x = 1; x <= fjController.readAll().Count; x++)
            {
                htmlCL.Add("        ace.edit(\"editor" + x + "\").getSession().setMode(\"ace/mode/" + fjController.readElement(x).language + "\");");
            }
            htmlCL.Add("    </script>");

            string htmlContent = string.Join("\n", htmlCL.ToArray());
            return htmlContent;
        }
    }
}
