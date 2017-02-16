using System.Collections.Generic;

namespace JSONTest
{
    public class Snippet
    {
        public string language { get; set; }
        public string comment { get; set; }
        public string code { get; set; }
    }

    public class Section
    {
        public string sectionName { get; set; }
        public List<Snippet> snippets { get; set; }
    }
}
