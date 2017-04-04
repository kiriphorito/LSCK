//Class for reading and writing to JSON files.

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using EnvDTE80;

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
            public string CSSNavBgC { get; set; }
            public string CSSNavTitleC { get; set; }
            public string CSSNavTitleHoverC { get; set; }
            public string CSSNavMenuC { get; set; }
            public string CSSNavMenuHoverC { get; set; }
            public string CSSNavMenuSelectedC { get; set; }
            public string CSSNavBorderC { get; set; }
            public string CSSPageBgC { get; set; }
            public string CSSSectionTitleC { get; set; }
            public string CSSCommentC { get; set; }
            public string CSSSectionTitleF { get; set; }
            public string CSSCommentF { get; set; }
            public string CSSSectionTitleFS { get; set; }
            public string CSSCommentFS { get; set; }
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
                DTE2 dte = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE");
                JSONFile.title = Path.GetFileName(Path.GetFileNameWithoutExtension(dte.Solution.FullName));
                JSONFile.ace_theme = "monokai";
                JSONFile.CSSNavBgC = "Black";
                JSONFile.CSSNavTitleC = "White";
                JSONFile.CSSNavTitleHoverC = "Aquamarine";
                JSONFile.CSSNavMenuC = "White";
                JSONFile.CSSNavMenuHoverC = "Aquamarine";
                JSONFile.CSSNavMenuSelectedC = "Aquamarine";
                JSONFile.CSSNavBorderC = "Transparent";
                JSONFile.CSSPageBgC = "White";
                JSONFile.CSSSectionTitleC = "Black";
                JSONFile.CSSCommentC = "Black";
                JSONFile.CSSSectionTitleF = "Helvetica";
                JSONFile.CSSCommentF = "Helvetica";
                JSONFile.CSSSectionTitleFS = "32";
                JSONFile.CSSCommentFS = "14";
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

        public void SetCSS(List<string> CSSSettings)
        {
            JSONFile.CSSNavBgC = CSSSettings[0];
            JSONFile.CSSNavTitleC = CSSSettings[1];
            JSONFile.CSSNavTitleHoverC = CSSSettings[2];
            JSONFile.CSSNavMenuC = CSSSettings[3];
            JSONFile.CSSNavMenuHoverC = CSSSettings[4];
            JSONFile.CSSNavMenuSelectedC = CSSSettings[5];
            JSONFile.CSSNavBorderC = CSSSettings[6];
            JSONFile.CSSPageBgC = CSSSettings[7];
            JSONFile.CSSSectionTitleC = CSSSettings[8];
            JSONFile.CSSCommentC = CSSSettings[9];
            JSONFile.CSSSectionTitleF = CSSSettings[10];
            JSONFile.CSSCommentF = CSSSettings[11];
            JSONFile.CSSSectionTitleFS = CSSSettings[12];
            JSONFile.CSSCommentFS = CSSSettings[13];
            WriteJSON();
        }

        public List<string> GetCSS()
        {
            List<string> CSSSettings = new List<string>();
            CSSSettings.Add(JSONFile.CSSNavBgC);
            CSSSettings.Add(JSONFile.CSSNavTitleC);
            CSSSettings.Add(JSONFile.CSSNavTitleHoverC);
            CSSSettings.Add(JSONFile.CSSNavMenuC);
            CSSSettings.Add(JSONFile.CSSNavMenuHoverC);
            CSSSettings.Add(JSONFile.CSSNavMenuSelectedC);
            CSSSettings.Add(JSONFile.CSSNavBorderC);
            CSSSettings.Add(JSONFile.CSSPageBgC);
            CSSSettings.Add(JSONFile.CSSSectionTitleC);
            CSSSettings.Add(JSONFile.CSSCommentC);
            CSSSettings.Add(JSONFile.CSSSectionTitleF);
            CSSSettings.Add(JSONFile.CSSCommentF);
            CSSSettings.Add(JSONFile.CSSSectionTitleFS);
            CSSSettings.Add(JSONFile.CSSCommentFS);
            return CSSSettings;
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
            JSONFile.sections[getSectionIndex(sectionName)].position = CountPage(pageTitle) + 1;
            JSONFile.sections[getSectionIndex(sectionName)].page = pageTitle;
            WriteJSON();
        }

        public void NullPage(string sectionName)
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
            WriteJSON();
        }

        public void SwapPage(string firstName, string secondName)
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

        public void InsertPage(string pageName)
        {
            JSONFile.page_titles.Add(pageName);
            WriteJSON();
        }

        public void DeletePage(string pageName)
        {
            JSONFile.page_titles.Remove(pageName);
            foreach (Section section in JSONFile.sections)
            {
                if (section.page == pageName)
                {
                    NullPage(section.sectionName);
                }
            }
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
            int sectionIndex = getSectionIndex(sectionName);
            if (JSONFile.sections[sectionIndex].page != null) //If the section is tied to a page
            {
                NullPage(sectionName);
            }
            JSONFile.sections.RemoveAt(sectionIndex);
            WriteJSON();
        }

        public void SwapSection(string firstElement, string secondElement)
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
            WriteJSON();
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
        public void InsertSnippet(string sectionName, int index, string language, string comment)
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
            WriteJSON();
        }

        public void SetComment(string sectionName, int index, string newComment)
        {
            for (int x = 0; x < JSONFile.sections.Count; x++)
            {
                if (JSONFile.sections[x].sectionName == sectionName)
                {
                    JSONFile.sections[x].snippets[index - 1].comment = newComment;
                    WriteJSON();
                }
            }
        }

        //Swaps two entries in a specific section
        public void SwapSnippet(string sectionName, int first, int second)
        {
            int sectionIndex = getSectionIndex(sectionName);
            JSONFile.sections[sectionIndex].snippets[first - 1].position = second;
            JSONFile.sections[sectionIndex].snippets[second - 1].position = first;
            JSONFile.sections[sectionIndex].snippets.Insert(first - 1, JSONFile.sections[sectionIndex].snippets[second - 1]);
            JSONFile.sections[sectionIndex].snippets.Insert(second, JSONFile.sections[sectionIndex].snippets[first]);
            JSONFile.sections[sectionIndex].snippets.RemoveAt(first);
            JSONFile.sections[sectionIndex].snippets.RemoveAt(second);
            WriteJSON();
        }

        //Deletes an entry
        public void DeleteSnippet(string sectionName, int index)
        {
            int sectionIndex = getSectionIndex(sectionName);
            JSONFile.sections[sectionIndex].snippets.RemoveAt(index - 1);
            for (int x = index - 1; x < JSONFile.sections[sectionIndex].snippets.Count; x++)
            {
                JSONFile.sections[sectionIndex].snippets[x].position--;
            }
            WriteJSON();
        }

        public int GetNumberOfSnippets(string sectionName)
        {
            return JSONFile.sections[getSectionIndex(sectionName)].snippets.Count;
        }

        public string GetTitle()
        {
            return JSONFile.title;
        }

        public string GetAceTheme()
        {
            return JSONFile.ace_theme;
        }

        public List<string> GetPageTitles()
        {
            return JSONFile.page_titles;
        }

        public List<string> GetPageSections(string page)
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

        public int GetSectionPosition(string sectionName)
        {
            return JSONFile.sections[getSectionIndex(sectionName)].position;
        }

        public List<string> GetSectionNames()
        {
            var result = new List<string>();
            foreach (Section section in JSONFile.sections)
            {
                result.Add(section.sectionName);
            }
            return result;
        }

        public string GetLanguage(string sectionName, int index)
        {
            int sectionIndex = getSectionIndex(sectionName);
            return JSONFile.sections[sectionIndex].snippets[index - 1].language;
        }

        public string GetComment(string sectionName, int index)
        {
            int sectionIndex = getSectionIndex(sectionName);
            return JSONFile.sections[sectionIndex].snippets[index - 1].comment;
        }
    }
}
