/*
 * The following class is a controller to manaage a JSON class and a File Handler class - MVC pattern
 * Controller - FJController
 * Model - FileHander & JSON
 * View - Extension UI
 * 
 * FJController class follows the Singleton design pattern where only one instance can be instantiated
 * 
*/

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using EnvDTE80;
using System.Runtime.InteropServices;
using System.Reflection;

namespace LSCK
{
    public sealed class FJController
    {
        private readonly JSON json;
        private readonly FileHandler fileHandler = new FileHandler();
        private readonly string fileDir;

        private string jsonSettingsName = "website";

        private static FJController instance = null;
        private static readonly object Instancelock = new object();

        private FJController()
        {
            DTE2 dte = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE");
            string solutionDir = Path.GetDirectoryName(dte.Solution.FullName);
            fileDir = solutionDir + @"\LSCK Data";
            Directory.CreateDirectory(fileDir);
            json = new JSON(fileDir, jsonSettingsName);
            if (!Directory.Exists(string.Concat(fileDir, @"\data")))
            {
                Directory.CreateDirectory(string.Concat(fileDir, @"\data"));
            }
        }

        public static FJController GetInstance
        {
            get
            {
                lock (Instancelock)
                {
                    if (instance == null)
                    {
                        instance = new FJController();
                    }
                    return instance;
                }
            }
        }

        public class InvalidInputException : Exception
        {
            public InvalidInputException(string message) : base(message) { }
        }

        ///<summary>
        ///<para>Retreive the Title of the website fron JSON</para>
        ///</summary>
        public string GetTitle()
        {
            return json.GetTitle();
        }

        ///<summary>
        ///<para>Set the Title of the website in JSON</para>
        ///</summary>
        public void SetTitle(string newTitle)
        {
            json.SetTitle(newTitle);
        }

        ///<summary>
        ///<para>Set the title of a page of the website in JSON</para>
        ///</summary>
        public void SetPageTitle(string oldName, string newName)
        {
            json.UpdatePageName(oldName, newName);
        }

        public void DeletePage(string pageName)
        {
            json.DeletePage(pageName);
        }

        public void SwapPage(string firstName, string secondName)
        {
            json.SwapPage(firstName, secondName);
        }

        private List<string> getListofThemes()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LSCK.Resources.Assets.acceptable_ace_themes.txt");
            TextReader tr = new StreamReader(stream);
            string fileContents = tr.ReadToEnd();
            return fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
        }

        public int GetAceThemeIndex()
        {
            List<string> themes = getListofThemes();
            int x = 0;
            foreach (string theme in themes)
            {
                if (json.GetAceTheme().Equals(theme))
                {
                    return x;
                }
                x++;
            }
            return -1; //To make compiler happy
        }

        ///<summary>
        ///<para>Retreive the theme for Ace Editor of the website fron JSON</para>
        ///</summary>
        public string GetAceTheme()
        {
            return json.GetAceTheme();
        }

        ///<summary>
        ///<para>Set the theme for Ace Editor of the website fron JSON</para>
        ///</summary>
        public void SetAceTheme(string newAceTheme)
        {
            List<string> themes = getListofThemes();
            if (!themes.Contains(newAceTheme))
                throw new InvalidInputException("You have entered an invalid theme for the Ace Editor!");
            json.SetAceTheme(newAceTheme);
        }

        ///<summary>
        ///<para>Retreive the names of all pages in the website</para>
        ///</summary>
        public List<string> GetPageTitles()
        {
            return json.GetPageTitles();
        }

        ///<summary>
        ///<para>Add a new page to the website</para>
        ///</summary>
        public void InsertPageTitle(string newPageTitle)
        {
            if (json.GetPageTitles().Contains(newPageTitle))
                throw (new InvalidInputException("You have already entered this title for a page!"));
            else
                json.InsertPage(newPageTitle);
        }

        ///<summary>
        ///<para>Associates section to the bottom of a selected page</para>
        ///</summary>
        public void SetPage(string sectionName, string pageTitle)
        {
            if (!json.GetPageTitles().Contains(pageTitle))
                throw new InvalidInputException("You have selected a page name that hasn't been added!");
            json.SetPage(sectionName, pageTitle);
        }

        ///<summary>
        ///<para>dissociates section from its selected page</para>
        ///</summary>
        public void NullPage(string sectionName)
        {
            json.NullPage(sectionName);
        }

        ///<summary>
        ///<para>Retreive all elements of a snippet</para>
        ///</summary>
        public Snippet GetSnippet(string sectionName, int index)
        {
            var snippet = new Snippet();
            snippet.language = json.GetLanguage(sectionName, index);
            snippet.comment = json.GetComment(sectionName, index);
            snippet.code = fileHandler.Read(fileDir + @"/data/" + sectionName.ToLower().Replace(" ", "") + "-" + index + ".txt");
            return snippet;
        }

        ///<summary>
        ///<para>Retreive all sections on a specific page</para>
        ///</summary>
        public List<Section> GetPage(string pageTitle)
        {
            var result = new List<Section>();

            //Retrieve list of sections with the associated pageTitle
            List<string> sectionNames = json.GetPageSections(pageTitle);

            //For each section
            for (int x = 1; x <= sectionNames.Count; x++)
            {
                var section = new Section();
                foreach (string sectionName in sectionNames)
                {
                    if (json.GetSectionPosition(sectionName) == x)
                    {
                        section.sectionName = sectionName;
                        break;
                    }
                }
                var listOfSnippets = new List<Snippet>();
                for (int y = 1; y <= json.GetNumberOfSnippets(section.sectionName); y++)
                {
                    listOfSnippets.Add(GetSnippet(section.sectionName, y));
                }
                section.snippets = listOfSnippets;
                result.Add(section);
            }
            return result;
        }

        public List<string> GetPageSections(string pageName)
        {
            return json.GetPageSections(pageName);
        }

        ///<summary>
        ///<para>Reteive all snippets from a specific page, removing section information</para>
        ///</summary>
        public List<Snippet> GetPageSnippetOnly(string pageTitle)
        {
            return PageSnippetsOnly(GetPage(pageTitle));
        }

        ///<summary>
        ///<para>Retreive snippets from sections, i.e removing section information</para>
        ///</summary>
        public List<Snippet> PageSnippetsOnly(List<Section> page)
        {
            var result = new List<Snippet>();

            foreach (Section section in page)
            {
                foreach (Snippet snippet in section.snippets)
                {
                    result.Add(snippet);
                }
            }

            return result;
        }

        ///<summary>
        ///<para>Retreive all sections names stored</para>
        ///</summary>
        public List<string> GetSectionNames()
        {
            return json.GetSectionNames();
        }

        public Boolean SectionExists(string sectionName)
        {
            if (json.GetSectionNames().Contains(sectionName))
                return true;
            else
                return false;
        }

        ///<summary>
        ///<para>Retreive all file names in a particular snippet</para>
        ///</summary>
        public List<string> GetFileNames(string sectionName)
        {
            var result = new List<string>();
            for (int x = 1; x <= json.GetNumberOfSnippets(sectionName); x++)
            {
                if (json.GetLanguage(sectionName, x) == "file")
                {
                    result.Add(fileHandler.Read(fileDir + @"/data/" + sectionName.ToLower().Replace(" ", "") + "-" + x + ".txt"));
                }
            }
            return result;
        }

        ///<summary>
        ///<para>Add a new section</para>
        ///</summary>
        public void InsertSection(string section)
        {
            if (json.GetSectionNames().Contains(section))
            {
                throw new InvalidInputException("The name you have entered already exists for a section!");
            }
            else
            {
                json.InsertSection(section);
            }
        }

        ///<summary>
        ///<para>Delete a section</para>
        ///</summary>
        public void DeleteSection(string section)
        {
            if (json.GetSectionNames().Contains(section))
            {
                for (int x = json.GetNumberOfSnippets(section); x >= 1; x--)
                {
                    DeleteSnippet(section, x);
                }
                json.DeleteSection(section);
            }
            else
            {
                throw new InvalidInputException("The section you have entered doesn't exist!");
            }
        }

        ///<summary>
        ///<para>Swap two sections</para>
        ///</summary>
        public void SwapSection(string first, string second)
        {
            json.SwapSection(first, second);
        }

        ///<summary>
        ///<para>Add a snippet to a specific position in the section</para>
        ///</summary>
        public void InsertSnippet(string section, int index, string language, string comment, List<string> code)
        {
            string codeString = string.Join("\n", code.ToArray());
            InsertSnippet(section, index, language, comment, codeString);
        }

        public void InsertSnippet(string section, int index, string language, string comment, string content)
        {
            switch (language)
            {
                case "file":
                    fileHandler.InsertFile(content, index, section, fileDir + @"/data/");
                    json.InsertSnippet(section, index, "file", comment);
                    break;
                default:
                    fileHandler.InsertSnippet(content, index, section, fileDir + @"/data/");
                    json.InsertSnippet(section, index, language, comment);
                    break;
            }
        }

        ///<summary>
        ///<para>Add a snippet to the end of a section</para>
        ///</summary>
        public void InsertSnippet(string section, string language, string comment, List<string> code)
        {
            string codeString = string.Join("\n", code.ToArray());
            InsertSnippet(section, language, comment, codeString);
        }

        public void InsertSnippet(string section, string language, string comment, string content)
        {
            switch (language)
            {
                case "file":
                    fileHandler.InsertFile(content, json.GetNumberOfSnippets(section) + 1, section, fileDir + @"/data/");
                    json.InsertSnippet(section, json.GetNumberOfSnippets(section) + 1, "file", comment);
                    break;
                default:
                    fileHandler.InsertSnippet(content, json.GetNumberOfSnippets(section) + 1, section, fileDir + @"/data/");
                    json.InsertSnippet(section, json.GetNumberOfSnippets(section) + 1, language, comment);
                    break;
            }
        }

        public void SetComment(string sectionName, int index, string newComment)
        {
            json.SetComment(sectionName, index, newComment);
        }

        ///<summary>
        ///<para>Swaps two different snippets</para>
        ///</summary>
        public void SwapSnippet(int first, int second, string section)
        {
            fileHandler.Swap(first, second, section, fileDir + @"/data");
            json.SwapSnippet(section, first, second);
        }

        ///<summary>
        ///<para>Delete a snippet at a specific position</para>
        ///</summary>
        public void DeleteSnippet(string section, int index)
        {
            fileHandler.Delete(index, section, fileDir + @"/data");
            json.DeleteSnippet(section, index);
        }

        ///<summary>
        ///<para>Delete a particular file in a section</para>
        ///</summary>
        public void DeleteFileSnippet(string sectionName, string fileName)
        {
            for (int x = 1; x <= json.GetNumberOfSnippets(sectionName); x++)
            {
                if (json.GetLanguage(sectionName, x) == "file")
                {
                    if (fileHandler.Read(fileDir + @"/data/" + sectionName.ToLower().Replace(" ", "") + "-" + x + ".txt") == fileName)
                    {
                        DeleteSnippet(sectionName, x);
                    }
                }
            }
        }

        public string GetDataDir()
        {
            return fileDir;
        }

        public List<Snippet> GetSectionSnippets(string sectionName)
        {
            List<Snippet> result = new List<Snippet>();
            for (int x = 1; x <= json.GetNumberOfSnippets(sectionName); x++)
            {
                result.Add(GetSnippet(sectionName, x));
            }
            return result;
        }
    }
}
