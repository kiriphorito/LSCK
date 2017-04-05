// <copyright file="StructureControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>

namespace LSCK
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using static LSCK.Bridge;

    /// <summary>
    /// Interaction logic for StructureControl.
    /// </summary>
    public partial class StructureControl : UserControl
    {
        public ObservableCollection<BoolStringClass> TheList { get; set; }
        string sectionName;
        string currentComment=null;
        int currentSnippetIndex=0;
        FJController fjController;
        /// <summary>
        /// Initializes a new instance of the <see cref="StructureControl"/> class.
        /// </summary>
        public StructureControl()
        {
            this.InitializeComponent();
            fjController = FJController.GetInstance;
        }

        public class BoolStringClass
        {
            public string name { get; set; }
            public bool check { get; set; }
        }

        public void CreateCheckBoxList()
        {
            TheList = new ObservableCollection<BoolStringClass>();
            List<string> freeSectionNames = fjController.GetPageSections(null);
            foreach (string freeSection in freeSectionNames)
            {
                TheList.Add(new BoolStringClass { name = freeSection, check = false });
            }
            listFreeSections.ItemsSource = TheList;
            this.DataContext = this;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "Structure");
        }

        private void updateUI(int key)
        {
            switch (key)
            {
                case 0:
                    List<string> pageNames = fjController.GetPageTitles();
                    comboPages.Items.Clear();
                    foreach (string pageName in pageNames)
                    {
                        comboPages.Items.Add(pageName);
                    }
                    break;
                case 1:
                    if (comboPages.SelectedIndex != -1)
                    {
                        string page = comboPages.SelectedValue.ToString();
                        List<string> sectionNames = fjController.GetPageSections(page);
                        listSections.Items.Clear();
                        listSnippets.Items.Clear();
                        comboSections.Items.Clear();
                        commentBox.Clear();
                        codeBox.Clear();
                        foreach (string section in sectionNames)
                        {
                            comboSections.Items.Add(section);
                            listSections.Items.Add(section);
                        }
                        CreateCheckBoxList();
                    }
                    if (comboSections.Items.Count > 0)
                    {
                        comboSections.SelectedValue = comboSections.Items.GetItemAt(0);
                        updateUI(2);
                    }
                    break;
                case 2:
                    sectionName = comboSections.SelectedValue.ToString();
                    List<Snippet> snippets = fjController.GetSectionSnippets(sectionName);
                    listSnippets.Items.Clear();
                    commentBox.Text = "";
                    codeBox.Text = "";
                    foreach (Snippet snippet in snippets)
                    {
                        string shortComment;
                        if (snippet.comment.Length > 20)
                        {
                             shortComment= snippet.comment.Substring(0, 19) + "...";
                        }
                        else
                        {
                            shortComment = snippet.comment;
                        }
                        listSnippets.Items.Add(shortComment);
                        commentBox.Text = snippet.comment;
                        codeBox.Text = snippet.code;
                    }
                    if (listSnippets.Items.Count > 0)
                    {
                        listSnippets.SelectedItem = listSnippets.Items.GetItemAt(0);
                    }
                    currentSnippetIndex = 0;
                    if (snippets.Count > 0)
                    {
                        currentComment = snippets[0].comment;
                    }
                    break;
                case 3:
                    int index = listSnippets.SelectedIndex;
                    if (index != -1)
                    {
                        currentSnippetIndex = index;
                    }
                    Snippet selectedSnippet = fjController.GetSectionSnippets(sectionName)[index];
                    commentBox.Text= selectedSnippet.comment;
                    currentComment = selectedSnippet.comment;
                    codeBox.Text = selectedSnippet.code;
                    break;
            }
        }

        public string GetImageFullPath(string filename)
        {
            return Path.Combine(
                    //Get the location of your package dll
                    Assembly.GetExecutingAssembly().Location,
                    //reference your 'images' folder
                    "/Resources/",
                    filename
                 );
        }

        private void StructureWindow_Loaded(object sender, RoutedEventArgs e)
        {
            updateUI(0);
            comboPages.SelectedIndex = 0;
            if (comboSections.HasItems)
                comboSections.SelectedIndex = 0;
        }

        private void comboPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateUI(1);
        }

        private void addSectionsButton_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;
            foreach (var p in TheList.Where(p => p.check == true))
            {
                fjController.SetPage(p.name, comboPages.SelectedValue.ToString());
                x++;
            }
            for (int i = 0; i < TheList.Count; i++)
            {
                if ((TheList[i].check))
                {
                    TheList.RemoveAt(i);
                }
            }
            updateUI(1);
            MessageBox.Show(x + " sections added to " + comboPages.SelectedValue.ToString());
        }

        private void deleteSectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (listSections.SelectedValue != null)
            {
                string sectionName = listSections.SelectedValue.ToString();
                fjController.NullPage(sectionName);
                updateUI(1);
            }
            else
            {
                MessageBox.Show("Please select the section you wish to delete.");
            }
        }

        private void comboSections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboSections.SelectedValue != null)
            {
                updateUI(2);
            }
        }

        private void listSnippets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSnippets.Items.Count>0)
            {
                updateUI(3);
            }
        }

        private void deleteSnippetButton_Click(object sender, RoutedEventArgs e)
        {
            int index = listSnippets.SelectedIndex;
            if (index >= 0) {
                fjController.DeleteSnippet(comboSections.SelectedValue.ToString(),index+1);
                updateUI(2);
            }
        }

        private void modifyCommentButton_Click(object sender, RoutedEventArgs e)
        {
            fjController.SetComment(comboSections.Text, currentSnippetIndex + 1, commentBox.Text);
            currentComment = commentBox.Text;
            modifyCommentButton.Visibility = Visibility.Hidden;
        }

        private void commentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show(commentBox.Text+","+ currentComment);
            if (commentBox.Text != currentComment && currentComment != null)
            {
                if (!modifyCommentButton.IsVisible)
                {
                    modifyCommentButton.Visibility = Visibility.Visible;
                }
            }else
            {
                modifyCommentButton.Visibility = Visibility.Hidden;
            }
        }

        private void downArrow_Click(object sender, RoutedEventArgs e)
        {
            if (listSections.SelectedIndex - 1 >= 0)
            {
                string selectedSection = listSections.SelectedItem.ToString();
                string prevSection = listSections.Items.GetItemAt(listSections.SelectedIndex - 1).ToString();
                System.Windows.MessageBox.Show(selectedSection + "," + prevSection);
                fjController.SwapSection(selectedSection, prevSection);
                updateUI(1);
            }
        }

        private void upArrow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
