//------------------------------------------------------------------------------
// <copyright file="StructureControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace LSCK
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for StructureControl.
    /// </summary>
    public partial class StructureControl : UserControl
    {
        public ObservableCollection<BoolStringClass> TheList { get; set; }
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

        private void updateUI(int index)
        {
            switch (index) {
                case 0:
                    List<string> pageNames = fjController.GetPageTitles();
                    comboPages.Items.Clear();
                    foreach (string pageName in pageNames)
                    {
                        comboPages.Items.Add(pageName);
                    }
                    break;
                case 1:
                    string page = comboPages.SelectedValue.ToString();
                    List<string> sectionNames = fjController.GetPageSections(page);
                    listSections.Items.Clear();
                    foreach (string section in sectionNames)
                    {
                        listSections.Items.Add(section);
                    }
                    CreateCheckBoxList();
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
        }

        private void comboPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateUI(1);
        }

        private void addSectionsButton_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;
            foreach (var p in TheList.Where(p => p.check == true)){
                fjController.SetPage(p.name, comboPages.SelectedValue.ToString());
                x++;
            }
            for (int i =0; i<TheList.Count; i++)
            {
                if ((TheList[i].check))
                {
                    TheList.RemoveAt(i);
                }
            }
            updateUI(1);
            MessageBox.Show(x+ " sections added to "+ comboPages.SelectedValue.ToString());
        }

        private void deleteSectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (listSections.SelectedValue!=null)
            {
                string sectionName=listSections.SelectedValue.ToString();
                fjController.NullPage(sectionName);
                updateUI(1);
            }else
            {
                MessageBox.Show("Please selectthe section you wish to delete.");
            }
        }
    }
}