﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для ProjectListWindow.xaml
    /// </summary>
    public partial class ProjectListWindow : Window
    {
        private SQLiteDataBase DB;
        private bool Loading = true;
        
        public ProjectListWindow(SQLiteDataBase db)
        {
            InitializeComponent();

            DB = db;
            List<Project> Projects = Project.GetProjects(DB);
            foreach (Project P in Projects)
                ProjectsList.Items.Add(P);
            ParamGrid.Visibility = ProjectsList.SelectedItem == null ? Visibility.Hidden : Visibility.Visible;


        }

        private void ProjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedItem == null)
            {
                ParamGrid.Visibility = Visibility.Hidden;
                return;
            }
            ParamGrid.Visibility = Visibility.Visible;
            ProjectNameTextBlock.Text = ((Project)((ListBox)sender).SelectedItem).Title;
            DesciptionTextBlock.Text = ((Project)((ListBox)sender).SelectedItem).Description;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (GenRangeSP == null) return;
            GenRangeSP.Visibility = Visibility.Collapsed;
            GenListGrid.Visibility = Visibility.Visible;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            if (GenRangeSP == null) return;
            GenRangeSP.Visibility = Visibility.Visible;
            GenListGrid.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLiteConfig Config = (SQLiteConfig)DB;

            if (!Directory.Exists(((SQLiteConfig)DB).GetConfigValue("work_dir")))
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.Cancel)
                        return;
                    ((SQLiteConfig)DB).SetConfigValue("work_dir", dialog.SelectedPath);
                }
            }
            List<string> Orderers = Directory.EnumerateDirectories(((SQLiteConfig)DB).GetConfigValue("work_dir")).ToList();
            string CurOrderer = (DB as SQLiteConfig).GetConfigValue("plw_orderer");
            foreach (string Orderer in Orderers)
            {
                string NewOrderer = System.IO.Path.GetFileName(Orderer);


                OrdererCB.Items.Add(NewOrderer);
                if (NewOrderer == CurOrderer) OrdererCB.SelectedItem = CurOrderer;
            }
            CollectionTB.Text = Config.GetConfigValue("collection_dir");

            SetComboBox(SolventCB, "SELECT * FROM `solvents` WHERE `id`>0");

            long StorageID = (DB as SQLiteConfig).GetConfigValueLong("plw_storage");
            using (DataTable dt = DB.ReadTable("SELECT * FROM `roots`"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Server CurStorage = new Server(dr);
                    StorageCB.Items.Add(CurStorage);
                    if (CurStorage.ID == StorageID)
                        StorageCB.SelectedItem = CurStorage;
                }
            }
        }

        private void SetComboBox(ComboBox CB, string Query)
        {
            using (DataTable dt = DB.ReadTable(Query))
            {
                foreach (DataRow dr in dt.Rows)
                    CB.Items.Add(new TitleCodePair(dr.Field<string>("name"), dr.Field<string>("code")) { ID = dr.Field<long>("id") });
            }
            CB.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NodePanel NP = new NodePanel();
            Node Input = Node.LoadFromDB(DB, ((Project)ProjectsList.SelectedItem).Input);
            NP.Children.Add(Input);
            Input.LoadChildren();

            Sygnal MainSygnal = Input.StartSygnal();

            if (RB_Range.IsChecked == true)
            {
                int From = 1;
                int To = 1;
                try
                {
                    From = Convert.ToInt32(GenRangeFromTB.Text);
                    To = Convert.ToInt32(GenRangeToTB.Text);
                }
                catch
                {
                    new Exception("Неправильное значение для диапазона");

                }

                for (int i = From; i <= To; i++)
                {
                    CreateNewCount(MainSygnal, $"{i:00}");
                }
            }

            if (RB_List.IsChecked == true)
            {
                foreach (object Line in CountList.Items)
                {
                    CreateNewCount(MainSygnal, Line.ToString());
                }
            }
        }

        private void CreateNewCount(Sygnal MainSygnal, string CountName)
        {
            string Dir = System.IO.Path.Combine(((SQLiteConfig)DB).GetConfigValue("work_dir"), OrdererCB.SelectedItem.ToString(), CollectionTB.Text, CountName);
            string Task = OrdererCB.SelectedItem.ToString() + "/" + CollectionTB.Text.Replace('\\', '/') + "/" + CountName.Replace('\\', '/');
            MainSygnal.MakeRun(Dir, Task, (SQLiteConfig)DB, StorageCB.SelectedItem as Server, Convert.ToInt32(ChargeTB.Text), Convert.ToInt32(MultipletTB.Text),  ((TitleCodePair)SolventCB.SelectedItem).ID);
            foreach (Sygnal S in MainSygnal.GetChildren())
                S.MakeRun(Dir, Task, (SQLiteConfig)DB, StorageCB.SelectedItem as Server, Convert.ToInt32(ChargeTB.Text), Convert.ToInt32(MultipletTB.Text), ((TitleCodePair)SolventCB.SelectedItem).ID);
        }

        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsList.SelectedItem == null) return;
            ShowInTaskbar = false;
            ProjectEdit.Edit(DB, ((Project)ProjectsList.SelectedItem).ID);
            ShowInTaskbar = true;
            ((Project)ProjectsList.SelectedItem).Update();
            ProjectsList.Items.Refresh();
            ProjectNameTextBlock.Text = ((Project)ProjectsList.SelectedItem).Title;
            DesciptionTextBlock.Text = ((Project)ProjectsList.SelectedItem).Description;
        }

        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            Project NewProject = Project.New(DB);
            NewProject.Title = "Новый проект";
            NewProject.Input = DB.Insert("INSERT INTO `nodes` (`type`, `name`, `parent`, `comment`, `job`, `pos_x`, `pos_y`) VALUES (0, 'INPUT', 0, '', 0, 100.0, 100.0)");
            ShowInTaskbar = false;
            ProjectEdit.Edit(DB, NewProject.ID);
            ShowInTaskbar = true;
            NewProject.Update();
            ProjectsList.Items.Add(NewProject);
        }

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsList.SelectedItem == null) return;
            if (MessageBox.Show($"Вы действительно хотите удалить проект {ProjectsList.SelectedItem}? Отменить это будет невозможно", "Удаление проекта", MessageBoxButton.YesNo) != MessageBoxResult.Yes) 
                return;
            ((Project)ProjectsList.SelectedItem).DeleteProject();
            ProjectsList.Items.Remove(ProjectsList.SelectedItem);
        }

        private void NumbersTB_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void AddCountBtn_Click(object sender, RoutedEventArgs e)
        {
            string Task = AddParam.AddString("Добавить расчётное задание");
            if (Task == null) return;
            CountList.Items.Add(Task);
        }

        private void DeleteCountBtn_Click(object sender, RoutedEventArgs e)
        {
            CountList.Items.Remove(CountList.SelectedItem);
        }

        private void OrdererCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((SQLiteConfig)DB).SetConfigValue("plw_orderer", OrdererCB.SelectedItem as string);
        }

        private void StorageCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ = ((SQLiteConfig)DB).SetConfigValue("plw_storage", (StorageCB.SelectedItem as Server).ID);
        }
    }
}
