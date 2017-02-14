/*
 * The following class is a controller to manaage a JSON class and a File Handler class - MVC pattern
 * Controller - FJController
 * Model - FileHander & JSON
 * View - Extension UI
 * 
 * FJController class follows the Singleton design pattern where only one instance can be instantiated
 * 
 * Public Functions
 * Website Information
 * - getTitle()
 * - getAceTheme()
 * - getPageTitles()
 * 
 * - setTitle()
 * - setAceTheme()
 * 
 * - insertPageTitle()
 * 
 * Page Infomation 
 * - readPage()
 * - readPageSnippetOnly()
 * 
 * Section Information
 * - insertSection()
 * - deleteSection()
 * - swapSection()
 * - setPage()
 * - nullPage()
 * 
 * - snippetsOnly()
 * 
 * Snippet Information
 * - insertSnippet()
 * - deleteSnippet()
 * - swapSnippet()
 * - readSnippet()
 * 
 * 
*/

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace JSONTest
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
            fileDir = Environment.CurrentDirectory;
            json = new JSON(fileDir, jsonSettingsName);
            if (!Directory.Exists(string.Concat(fileDir, @"/data")))
            {
                Directory.CreateDirectory(string.Concat(fileDir, @"/data"));
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
            public InvalidInputException(string message)
                : base(message) { }
        }

        ///<summary>
        ///<para>Retreive the Title of the website fron JSON</para>
        ///</summary>
        public string getTitle()
        {
            return json.getTitle();
        }

        ///<summary>
        ///<para>Set the Title of the website fron JSON</para>
        ///</summary>
        public void setTitle(string newTitle)
        {
            json.setTitle(newTitle);
        }

        ///<summary>
        ///<para>Retreive the theme for Ace Editor of the website fron JSON</para>
        ///</summary>
        public string getAceTheme()
        {
            return json.getAceTheme();
        }

        ///<summary>
        ///<para>Set the theme for Ace Editor of the website fron JSON</para>
        ///</summary>
        public void setAceTheme(string newAceTheme)
        {
            var reader = new StreamReader(fileDir + @"/presets/acceptable_ace_themes.txt");
            string stringThemes = reader.ReadToEnd();
            reader.Close();
            List<string> themes = stringThemes.Split('\n').ToList();
            if (!themes.Contains(newAceTheme))
                throw new InvalidInputException("You have entered an invalid theme for the Ace Editor!");
            json.setAceTheme(newAceTheme);
        }

        ///<summary>
        ///<para>Retreive the names of all pages in the website</para>
        ///</summary>
        public List<string> getPageTitles()
        {
            return json.getPageTitles();
        }

        ///<summary>
        ///<para>Add a new page to the website</para>
        ///</summary>
        public void insertPageTitle(string newPageTitle)
        {
            if (json.getPageTitles().Contains(newPageTitle))
                throw new InvalidInputException("You have already entered this title for a page!");
            json.insertPageName(newPageTitle);
        }

        ///<summary>
        ///<para>Associates section to the bottom of a selected page</para>
        ///</summary>
        public void setPage(string sectionName, string pageTitle)
        {
            if (!json.getPageTitles().Contains(pageTitle))
                throw new InvalidInputException("You have selected a page name that hasn't been added!");
            json.setPage(sectionName , pageTitle);
        }

        ///<summary>
        ///<para>dissociates section from its selected page</para>
        ///</summary>
        public void nullPage(string sectionName)
        {
            json.nullPage(sectionName);
        }

        ///<summary>
        ///<para>Retreive all elements of a snippet</para>
        ///</summary>
        public Snippet readSnippet(string sectionName, int index)
        {
            var snippet = new Snippet();
            snippet.language = json.getLanguage(sectionName, index);
            snippet.comment = json.getComment(sectionName, index);
            snippet.code = fileHandler.read(fileDir + @"/data/" + sectionName.ToLower().Replace(" ", "") + "-" + index + ".txt");
            return snippet;
        }

        ///<summary>
        ///<para>Retreive all sections on a specific page</para>
        ///</summary>
        public List<Section> readPage(string pageTitle)
        {
            var result = new List<Section>();

            //Retrieve list of sections with the associated pageTitle
            List<string> sectionNames = json.getPageSections(pageTitle);

            //For each section
            for (int x = 1; x <= sectionNames.Count; x++)
            {
                var section = new Section();
                foreach (string sectionName in sectionNames)
                {
                    if (json.getSectionPosition(sectionName) == x)
                    {
                        section.sectionName = sectionName;
                        break;
                    }
                }
                var listOfSnippets = new List<Snippet>();
                for (int y = 1; y <= json.getNumberOfSnippets(section.sectionName); y++)
                {
                    listOfSnippets.Add(readSnippet(section.sectionName, y));
                }
                section.snippets = listOfSnippets;
                result.Add(section);
            }
            return result;
        }

        ///<summary>
        ///<para>Reteive all snippets from a specific page, removing section information</para>
        ///</summary>
        public List<Snippet> readPageSnippetOnly(string pageTitle)
        {
            return pageSnippetsOnly(readPage(pageTitle));
        }

        ///<summary>
        ///<para>Retreive snippets from sections, i.e removing section information</para>
        ///</summary>
        public List<Snippet> pageSnippetsOnly(List<Section> page)
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

        public List<string> readSectionNames()
        {
            return json.getSectionNames();
        }

        ///<summary>
        ///<para>Add a new section</para>
        ///</summary>
        public void insertSection(string section)
        {
            if (json.getSectionNames().Contains(section))
            {
                throw new InvalidInputException("You have already entered this title for a section!");
            }
            json.insertSection(section);
        }

        ///<summary>
        ///<para>Delete a section</para>
        ///</summary>
        public void deleteSection(string section)
        {
            for (int x = json.getNumberOfSnippets(section); x >= 1; x--)
            {
                deleteSnippet(section, x);
            }
            json.deleteSection(section);
        }

        ///<summary>
        ///<para>Swap two sections</para>
        ///</summary>
        public void swapSection(string first, string second)
        {
            json.swapSection(first, second);
        }

        ///<summary>
        ///<para>Add a snippet to a specific position in the section</para>
        ///</summary>
        public void insertSnippet(string section , int index, string language , string comment , List<string> code)
        {
            string codeString = string.Join("\n", code.ToArray());
            insertSnippet(section, index, language, comment, codeString);
        }

        public void insertSnippet(string section, int index, string language, string comment, string content)
        {
            switch (language)
            {
                case "file":
                    fileHandler.insertFile(content, index, section, fileDir + @"/data/");
                    json.insertSnippet(section, index, "file", comment);
                    break;
                default:
                    fileHandler.insertSnippet(content, index, section, fileDir + @"/data/");
                    json.insertSnippet(section, index, language, comment);
                    break;
            }
        }

        ///<summary>
        ///<para>Add a snippet to the end of a section</para>
        ///</summary>
        public void insertSnippet(string section, string language, string comment, List<string> code)
        {
            string codeString = string.Join("\n", code.ToArray());
            insertSnippet(section, language, comment, codeString);
        }

        public void insertSnippet(string section, string language, string comment, string content)
        {
            switch (language)
            {
                case "file":
                    fileHandler.insertFile(content, json.getNumberOfSnippets(section) + 1, section, fileDir + @"/data/");
                    json.insertSnippet(section, json.getNumberOfSnippets(section) + 1, "file", comment);
                    break;
                default:
                    fileHandler.insertSnippet(content, json.getNumberOfSnippets(section) + 1, section, fileDir + @"/data/");
                    json.insertSnippet(section, json.getNumberOfSnippets(section) + 1, language, comment);
                    break;
            }
        }

        ///<summary>
        ///<para>Swaps two different snippets</para>
        ///</summary>
        public void swapSnippet(int first, int second, string section)
        {
            fileHandler.swap(first, second, section, fileDir + @"/data");
            json.swapSnippet(section, first, second);
        }

        ///<summary>
        ///<para>Delete a snippet at a specific position</para>
        ///</summary>
        public void deleteSnippet(string section, int index)
        {
            fileHandler.delete(index, section, fileDir + @"/data");
            json.deleteSnippet(section, index);
        }
    }
}
