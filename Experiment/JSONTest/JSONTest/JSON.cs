//Class for reading and writing to JSON files.

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace JSONTest
{
    public class JSON
    {
        private RootJSONObject JSONFile = new RootJSONObject();
        private string fileDir;

        //For each snippet of code that the user wishes to add
        private class Snippet
        {
            public int position { get; set; }
            public string language { get; set; }
            public string comment { get; set; }
        }

        private class Section
        {
            public string sectionName { get; set; }
            public string page { get; set; }
            public int position { get; set; }
            public List<Snippet> snippets { get; set; }
        }

        //To hold the main objct for JSON
        private class RootJSONObject
        {
            public string title { get; set; }
            public string ace_theme { get; set; }
            public List<string> page_titles { get; set; }
            public List<Section> sections { get; set; }
        }

        //Constructor
        public JSON(string fileDir , string jsonName)
        {
            this.fileDir = fileDir + @"/" + jsonName + @".json";
            if (!File.Exists(this.fileDir))
            {
                //Default settings
                JSONFile.title = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
                JSONFile.ace_theme = "monokai";
                JSONFile.page_titles = new List<string>();
                JSONFile.page_titles.Add("index");
                JSONFile.sections = new List<Section>();
                WriteJSON();
            }
            var reader = new StreamReader(this.fileDir);
            string fileJson = reader.ReadToEnd();
            reader.Close();
            JSONFile = JsonConvert.DeserializeObject<RootJSONObject>(fileJson);
        }

        //Writes to/Updates the JSON file
        private void WriteJSON()
        {
            string jsonString = JsonConvert.SerializeObject(JSONFile, Formatting.Indented);
            File.WriteAllText(fileDir, jsonString);
        }

        public void SetTitle(string title)
        {
            JSONFile.title = title;
            WriteJSON();
        }

        public void SetAceTheme(string newTheme)
        {
            JSONFile.ace_theme = newTheme;
            WriteJSON();
        }

        public void UpdatePageName(string oldName, string newName)
        {
            for (int x = 0; x < JSONFile.page_titles.Count; x++)
            {
                if (JSONFile.page_titles[x] == oldName)
                {
                    JSONFile.page_titles[x] = newName;
                    WriteJSON();
                    return;
                }
            }
        }

        public int CountPage(string pageTitle)
        {
            int count = 0;
            foreach (Section section in JSONFile.sections)
            {
                Console.WriteLine(section.page);
                if (section.page == pageTitle)
                {
                    count++;
                }
            }
            Console.WriteLine(count);
            return count;
        }

        public void SetPage(string sectionName, string pageTitle)
        {
            JSONFile.sections[GetSectionIndex(sectionName)].position = CountPage(pageTitle) + 1;
            JSONFile.sections[GetSectionIndex(sectionName)].page = pageTitle;
            WriteJSON();
        }

        public void NullPage(string sectionName)
        {
            int sectionIndex = GetSectionIndex(sectionName);
            for (int x = 0; x < JSONFile.sections.Count; x++)
            {
                if (JSONFile.sections[x].page == JSONFile.sections[sectionIndex].page && JSONFile.sections[x].position > JSONFile.sections[sectionIndex].position)
                {
                    JSONFile.sections[x].position--;
                }
            }
            JSONFile.sections[sectionIndex].page = null;
            JSONFile.sections[sectionIndex].position = 0;
            WriteJSON();
        }

        public void SwapPageName(string firstName, string secondName)
        {
            for (int x = 0; x < JSONFile.page_titles.Count; x++)
            {
                if (JSONFile.page_titles[x] == firstName)
                {
                    JSONFile.page_titles[x] = secondName;
                }
                else if (JSONFile.page_titles[x] == secondName)
                {
                    JSONFile.page_titles[x] = firstName;
                }
            }
        }

        public void InsertPageName(string sectionName)
        {
            JSONFile.page_titles.Add(sectionName);
            WriteJSON();
        }

        //Adds the new section to the end
        public void InsertSection(string sectionName)
        {
            var section = new Section();
            section.sectionName = sectionName;
            section.snippets = new List<Snippet>();
            JSONFile.sections.Add(section);
            WriteJSON();
        }

        public void DeleteSection(string sectionName)
        {
            int sectionIndex = GetSectionIndex(sectionName);
            if (JSONFile.sections[sectionIndex].page != null) //If the section is tied to a page
            {
                NullPage(sectionName);
            }
            JSONFile.sections.RemoveAt(sectionIndex);
            WriteJSON();
        }

        public void SwapSection(string firstElement , string secondElement)
        {
            int firstElementIndex = GetSectionIndex(firstElement);
            int secondElementIndex = GetSectionIndex(secondElement);
            Console.WriteLine(firstElementIndex);
            Console.WriteLine(secondElementIndex);
            int tempPosition = JSONFile.sections[firstElementIndex].position;
            string tempPage = JSONFile.sections[firstElementIndex].page;
            JSONFile.sections[firstElementIndex].position = JSONFile.sections[secondElementIndex].position;
            JSONFile.sections[firstElementIndex].page = JSONFile.sections[secondElementIndex].page;
            JSONFile.sections[secondElementIndex].page = tempPage;
            WriteJSON();
        }

        private int GetSectionIndex(string sectionName)
        {
            for (int x = 0; x < JSONFile.sections.Count; x++)
            {
                if (JSONFile.sections[x].sectionName == sectionName)
                {
                    return x;
                }
            }
            return 0; //Shouldn't return this, non-valid search term
        }

        //Insert an entry at any position value in a particular section
        public void InsertSnippet(string sectionName , int index , string language , string comment)
        {
            var snippet = new Snippet();
            snippet.language = language;
            snippet.comment = comment;
            snippet.position = index;

            int sectionIndex = GetSectionIndex(sectionName);
            JSONFile.sections[sectionIndex].snippets.Insert(index - 1, snippet);
            for (int x = index; x < JSONFile.sections[sectionIndex].snippets.Count ; x++)
            {
                JSONFile.sections[sectionIndex].snippets[x].position++;
            }
            WriteJSON();
        }

        //Swaps two entries in a specific section
        public void swapSnippet(string sectionName , int first, int second)
        {
            int sectionIndex = GetSectionIndex(sectionName);
            JSONFile.sections[sectionIndex].snippets[first - 1].position = second;
            JSONFile.sections[sectionIndex].snippets[second - 1].position = first;
            JSONFile.sections[sectionIndex].snippets.Insert(first - 1, JSONFile.sections[sectionIndex].snippets[second - 1]);
            JSONFile.sections[sectionIndex].snippets.Insert(second, JSONFile.sections[sectionIndex].snippets[first]);
            JSONFile.sections[sectionIndex].snippets.RemoveAt(first);
            JSONFile.sections[sectionIndex].snippets.RemoveAt(second);
            WriteJSON();
        }

        //Deletes an entry
        public void deleteSnippet(string sectionName , int index)
        {
            int sectionIndex = GetSectionIndex(sectionName);
            JSONFile.sections[sectionIndex].snippets.RemoveAt(index - 1);
            for (int x = index - 1 ; x < JSONFile.sections[sectionIndex].snippets.Count ; x++)
            {
                JSONFile.sections[sectionIndex].snippets[x].position--;
            }
            WriteJSON();
        }

        public int getNumberOfSnippets(string sectionName)
        {
            return JSONFile.sections[GetSectionIndex(sectionName)].snippets.Count;
        }

        public string getTitle()
        {
            return JSONFile.title;
        }

        public string getAceTheme()
        {
            return JSONFile.ace_theme;
        }

        public List<string> getPageTitles()
        {
            return JSONFile.page_titles;
        }

        public List<string> getPageSections(string page)
        {
            var sections = new List<string>();
            foreach (Section section in JSONFile.sections)
            {
                if (section.page == page)
                {
                    sections.Add(section.sectionName);    
                }
            }
            return sections;
        }

        public int getSectionPosition(string sectionName)
        {
            return JSONFile.sections[GetSectionIndex(sectionName)].position;
        }

        public List<string> getSectionNames()
        {
            var result = new List<string>();
            foreach (Section section in JSONFile.sections)
            {
                result.Add(section.sectionName);
            }
            return result;
        }

        public string getLanguage(string sectionName, int index)
        {
            int sectionIndex = GetSectionIndex(sectionName);
            return JSONFile.sections[sectionIndex].snippets[index - 1].language;
        }

        public string getComment(string sectionName, int index)
        {
            int sectionIndex = GetSectionIndex(sectionName);
            return JSONFile.sections[sectionIndex].snippets[index - 1].comment;
        }
    }
}
