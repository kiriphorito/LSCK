//Class for reading and writing to JSON files.

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace LSCK
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
        public JSON(string fileDir, string jsonName)
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
                writeJSON();
            }
            var reader = new StreamReader(this.fileDir);
            string fileJson = reader.ReadToEnd();
            reader.Close();
            JSONFile = JsonConvert.DeserializeObject<RootJSONObject>(fileJson);
        }

        //Writes to/Updates the JSON file
        private void writeJSON()
        {
            string jsonString = JsonConvert.SerializeObject(JSONFile, Formatting.Indented);
            File.WriteAllText(fileDir, jsonString);
        }

        public void setTitle(string title)
        {
            JSONFile.title = title;
            writeJSON();
        }

        public void setAceTheme(string newTheme)
        {
            JSONFile.ace_theme = newTheme;
            writeJSON();
        }

        public void updatePageName(string oldName, string newName)
        {
            for (int x = 0; x < JSONFile.page_titles.Count; x++)
            {
                if (JSONFile.page_titles[x] == oldName)
                {
                    JSONFile.page_titles[x] = newName;
                    writeJSON();
                    return;
                }
            }
        }

        public int countPage(string pageTitle)
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

        public void setPage(string sectionName, string pageTitle)
        {
            JSONFile.sections[getSectionIndex(sectionName)].position = countPage(pageTitle) + 1;
            JSONFile.sections[getSectionIndex(sectionName)].page = pageTitle;
            writeJSON();
        }

        public void nullPage(string sectionName)
        {
            int sectionIndex = getSectionIndex(sectionName);
            for (int x = 0; x < JSONFile.sections.Count; x++)
            {
                if (JSONFile.sections[x].page == JSONFile.sections[sectionIndex].page && JSONFile.sections[x].position > JSONFile.sections[sectionIndex].position)
                {
                    JSONFile.sections[x].position--;
                }
            }
            JSONFile.sections[sectionIndex].page = null;
            JSONFile.sections[sectionIndex].position = 0;
            writeJSON();
        }

        public void swapPageName(string firstName, string secondName)
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

        public void insertPageName(string sectionName)
        {
            JSONFile.page_titles.Add(sectionName);
            writeJSON();
        }

        //Adds the new section to the end
        public void insertSection(string sectionName)
        {
            var section = new Section();
            section.sectionName = sectionName;
            section.snippets = new List<Snippet>();
            JSONFile.sections.Add(section);
            writeJSON();
        }

        public void deleteSection(string sectionName)
        {
            int sectionIndex = getSectionIndex(sectionName);
            if (JSONFile.sections[sectionIndex].page != null) //If the section is tied to a page
            {
                nullPage(sectionName);
            }
            JSONFile.sections.RemoveAt(sectionIndex);
            writeJSON();
        }

        public void swapSection(string firstElement, string secondElement)
        {
            int firstElementIndex = getSectionIndex(firstElement);
            int secondElementIndex = getSectionIndex(secondElement);
            Console.WriteLine(firstElementIndex);
            Console.WriteLine(secondElementIndex);
            int tempPosition = JSONFile.sections[firstElementIndex].position;
            string tempPage = JSONFile.sections[firstElementIndex].page;
            JSONFile.sections[firstElementIndex].position = JSONFile.sections[secondElementIndex].position;
            JSONFile.sections[firstElementIndex].page = JSONFile.sections[secondElementIndex].page;
            JSONFile.sections[secondElementIndex].page = tempPage;
            writeJSON();
        }

        private int getSectionIndex(string sectionName)
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
        public void insertSnippet(string sectionName, int index, string language, string comment)
        {
            var snippet = new Snippet();
            snippet.language = language;
            snippet.comment = comment;
            snippet.position = index;

            int sectionIndex = getSectionIndex(sectionName);
            JSONFile.sections[sectionIndex].snippets.Insert(index - 1, snippet);
            for (int x = index; x < JSONFile.sections[sectionIndex].snippets.Count; x++)
            {
                JSONFile.sections[sectionIndex].snippets[x].position++;
            }
            writeJSON();
        }

        //Swaps two entries in a specific section
        public void swapSnippet(string sectionName, int first, int second)
        {
            int sectionIndex = getSectionIndex(sectionName);
            JSONFile.sections[sectionIndex].snippets[first - 1].position = second;
            JSONFile.sections[sectionIndex].snippets[second - 1].position = first;
            JSONFile.sections[sectionIndex].snippets.Insert(first - 1, JSONFile.sections[sectionIndex].snippets[second - 1]);
            JSONFile.sections[sectionIndex].snippets.Insert(second, JSONFile.sections[sectionIndex].snippets[first]);
            JSONFile.sections[sectionIndex].snippets.RemoveAt(first);
            JSONFile.sections[sectionIndex].snippets.RemoveAt(second);
            writeJSON();
        }

        //Deletes an entry
        public void deleteSnippet(string sectionName, int index)
        {
            int sectionIndex = getSectionIndex(sectionName);
            JSONFile.sections[sectionIndex].snippets.RemoveAt(index - 1);
            for (int x = index - 1; x < JSONFile.sections[sectionIndex].snippets.Count; x++)
            {
                JSONFile.sections[sectionIndex].snippets[x].position--;
            }
            writeJSON();
        }

        public int getNumberOfSnippets(string sectionName)
        {
            return JSONFile.sections[getSectionIndex(sectionName)].snippets.Count;
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
            return JSONFile.sections[getSectionIndex(sectionName)].position;
        }

        public List<string> getSectionNames()
        {
            List<string> result = new List<string>();
            foreach (Section section in JSONFile.sections)
            {
                result.Add(section.sectionName);
            }
            return result;
        }

        public string getLanguage(string sectionName, int index)
        {
            int sectionIndex = getSectionIndex(sectionName);
            return JSONFile.sections[sectionIndex].snippets[index - 1].language;
        }

        public string getComment(string sectionName, int index)
        {
            int sectionIndex = getSectionIndex(sectionName);
            return JSONFile.sections[sectionIndex].snippets[index - 1].comment;
        }
    }
}
