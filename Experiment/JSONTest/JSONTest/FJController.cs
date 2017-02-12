/*
 * The following class a controller to manaage a JSON class and a File Handler class
 * Onlt one
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
            this.fileDir = Environment.CurrentDirectory;
            json = new JSON(this.fileDir, jsonSettingsName);
            if (!Directory.Exists(string.Concat(this.fileDir, @"/data")))
            {
                Directory.CreateDirectory(string.Concat(this.fileDir, @"/data"));
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

        public class InvalidInputException : System.Exception
        {
            public InvalidInputException(string message)
                : base(message) { }
        }

        public string getTitle()
        {
            return json.getTitle();
        }

        public void setTitle(string newTitle)
        {
            json.setTitle(newTitle);
        }

        public string getAceTheme()
        {
            return json.getAceTheme();
        }

        public List<string> getPageTitles()
        {
            return json.getPageTitles();
        }

        public void setAceTheme(string newAceTheme)
        {
            StreamReader reader = new StreamReader(fileDir + @"/presets/acceptable_ace_themes.txt");
            string stringThemes = reader.ReadToEnd();
            reader.Close();
            List<string> themes = stringThemes.Split('\n').ToList();
            if (!themes.Contains(newAceTheme))
                throw new InvalidInputException("You have entered an invalid theme for the Ace Editor!");
            json.setAceTheme(newAceTheme);
        }

        public void insertPageName(string newPageName)
        {
            json.insertPageName(newPageName);
        }

        public void setPage(string sectionName, string pageTitle)
        {
            if (!json.getPageTitles().Contains(pageTitle))
                throw new InvalidInputException("You have selected a page name that hasn't been added!");
            json.setPage(sectionName , pageTitle);
        }

        public void nullPage(string sectionName)
        {
            json.nullPage(sectionName);
        }

        //Get all elements of a snippet
        public Snippet readSnippet(string sectionName, int index)
        {
            Snippet snippet = new Snippet();
            snippet.language = json.getLanguage(sectionName, index);
            snippet.comment = json.getComment(sectionName, index);
            snippet.code = fileHandler.read(fileDir + @"/data/" + sectionName.ToLower().Replace(" ", "") + "-" + index + ".txt");
            return snippet;
        }

        public List<Section> readPage(string pageTitle)
        {
            List<Section> result = new List<Section>();

            //Retrieve list of sections with the associated pageTitle
            List<string> sectionNames = json.getPageSections(pageTitle);

            //For each section
            for (int x = 1; x <= sectionNames.Count; x++)
            {
                Section section = new Section();
                foreach (string sectionName in sectionNames)
                {
                    if (json.getSectionPosition(sectionName) == x)
                    {
                        section.sectionName = sectionName;
                        break;
                    }
                }
                List<Snippet> listOfSnippets = new List<Snippet>();
                for (int y = 1; y <= json.getNumberOfSnippets(section.sectionName); y++)
                {
                    listOfSnippets.Add(readSnippet(section.sectionName, y));
                }
                section.snippets = listOfSnippets;
                result.Add(section);
            }
            return result;
        }

        public List<Snippet> readPageSnippetOnly(string pageTitle)
        {
            return snippetsOnly(readPage(pageTitle));
        }

        public List<Snippet> snippetsOnly(List<Section> page)
        {
            List<Snippet> result = new List<Snippet>();

            foreach (Section section in page)
            {
                foreach (Snippet snippet in section.snippets)
                {
                    result.Add(snippet);
                }
            }

            return result;
        }

        public void insertSection(string section)
        {
            json.insertSection(section);
        }

        public void deleteSection(string section)
        {
            for (int x = json.getNumberOfSnippets(section); x >= 1; x--)
            {
                deleteSnippet(section, x);
            }
            json.deleteSection(section);
        }

        public void swapSection(string first, string second)
        {
            json.swapSection(first, second);
        }

        //Insert into specific position in list
        public void insertSnippet(string section , int index, string language , string comment , List<string> code)
        {
            string codeString = string.Join("\n", code.ToArray());
            insertSnippet(section, index, language, comment, codeString);
        }

        public void insertSnippet(string section, int index, string language, string comment, string code)
        {
            fileHandler.insertSnippet(code, index, section, fileDir + @"/data/");
            json.insertSnippet(section, index, language, comment);   
        }

        public void insertFile(string section, int index, string comment, string code)
        {
            fileHandler.insertFile(code, index, section, fileDir + @"/data/");
            json.insertSnippet(section, index, "file", comment);
        }

        //Add to the end of the list
        public void insertSnippet(string section, string language, string comment, List<string> code)
        {
            string codeString = string.Join("\n", code.ToArray());
            insertSnippet(section, language, comment, codeString);
        }

        public void insertSnippet(string section, string language, string comment, string code)
        {
            fileHandler.insertSnippet(code, json.getNumberOfSnippets(section) + 1, section, fileDir + @"/data/");
            json.insertSnippet(section, json.getNumberOfSnippets(section) + 1, language, comment);
        }

        public void insertFile(string section, string comment, string userFileDir)
        {
            fileHandler.insertFile(userFileDir, json.getNumberOfSnippets(section) + 1, section, fileDir + @"/data/");
            json.insertSnippet(section, json.getNumberOfSnippets(section) + 1, "file", comment);
        }

        //Swaps two different entries
        public void swapSnippet(int first, int second, string section)
        {
            fileHandler.swap(first, second, section, fileDir + @"/data");
            json.swapSnippet(section, first, second);
        }

        //Delete an entry at a specific position
        public void deleteSnippet(string section, int index)
        {
            fileHandler.delete(index, section, fileDir + @"/data");
            json.deleteSnippet(section, index);
        }
    }
}
