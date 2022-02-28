using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для JobEdit.xaml
    /// </summary>
    public partial class JobEdit : Window
    {
        SQLiteDataBase Config;
        Job CurrentJob;

        public JobEdit(SQLiteDataBase ConfigDataBase)
        {
            InitializeComponent();

            Config = ConfigDataBase;
            LoadFromDB(JobMethodTB, "methods");
            LoadFromDB(JobDFT_TypeTB, "dft");
            LoadFromDB(JobBasisTB, "basises");
            LoadFromDB(JobOtherTB, "other", "code");
            LoadFromDB(JobTaskTB, "tasks");
            LoadFromDB(JobHesTB, "hessians");
            LoadFromDB(JobSolutionTB, "solvents");
            LoadFromDB(JobOutputTB, "output");
        }

        /// <summary>
        /// Показывает окно добавления задания расчёта
        /// </summary>
        /// <param name="owner">Окно-родитель</param>
        /// <param name="ConfigDataBase">База данных конфигурации</param>
        public static void ShowModal(Window owner, SQLiteConfig ConfigDataBase)
        {
            JobEdit Job = new JobEdit(ConfigDataBase);
            Job.Owner = owner;
            Job.ShowDialog();
            GC.Collect();
        }

        /// <summary>
        /// Показывает окно добавления задания расчёта
        /// </summary>
        /// <param name="owner">Окно-родитель</param>
        /// <param name="ConfigDataBase">База данных конфигурации</param>
        public static Job New(Window owner, SQLiteDataBase ConfigDataBase)
        {
            JobEdit Job = new JobEdit(ConfigDataBase);
            Job.Owner = owner;
            Job.Full();
            Job.CurrentJob = new Job(ConfigDataBase);
            Job.ShowDialog();
            
            GC.Collect();
            return Job.CurrentJob;
        }

        /// <summary>
        /// Показывает окно редактирования задания расчёта
        /// </summary>
        /// <param name="owner">Окно-родитель</param>
        /// <param name="ConfigDataBase">База данных конфигурации</param>
        /// <param name="JobID">Номер задачи в БД</param>
        public static Job Edit(SQLiteConfig ConfigDataBase, long JobID)
        {
            return Edit(ConfigDataBase, Job.Load(ConfigDataBase, JobID));
        }

        /// <summary>
        /// Показывает окно редактирования задания расчёта
        /// </summary>
        /// <param name="owner">Окно-родитель</param>
        /// <param name="ConfigDataBase">База данных конфигурации</param>
        /// <param name="job">Задача</param>
        public static Job Edit(SQLiteConfig ConfigDataBase, Job job)
        {
            JobEdit JobWindow = new JobEdit(ConfigDataBase);
            JobWindow.CurrentJob = job;
            JobWindow.Full();
            JobWindow.AddB.Content = "Изменить";
            JobWindow.ShowDialog();

            GC.Collect();
            return JobWindow.CurrentJob;
        }

        /// <summary>
        /// Загрузка таблицы из БД в словарь
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <returns></returns>
        public void LoadFromDB(ComboBox CB, string table, string name = "name")
        {
            CB.Items.Clear();
            using (DataTable dt = Config.ReadTable($"SELECT * FROM `{table}` ORDER BY `id`;"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TitleCodePair TCV = new TitleCodePair(dr[name].ToString(), dr.Field<long>("id"));
                    CB.Items.Add(TCV);
                }
            }
        }

        private void JobMethodTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (JobMethodTB.SelectedItem == null) return;
            
            DFT_Type_Grid.Visibility = JobMethodTB.SelectedItem.ToString() == "DFT"
                ? Visibility.Visible
                : Visibility.Collapsed;

            BasisGrid.Visibility = JobMethodTB.SelectedItem.ToString() == "AM1"
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void CancelB_Click(object sender, RoutedEventArgs e)
        {
            CurrentJob = null;
            Close();
        }

        private void AddB_Click(object sender, RoutedEventArgs e)
        {
            CurrentJob.Update();
            CurrentJob.Name = JobNameTB.Text;
            CurrentJob.Comment = JobCommentTB.Text;
            CurrentJob.Method = (long)((TitleCodePair)JobMethodTB.SelectedItem).Value;
            CurrentJob.DFT = JobDFT_TypeTB.SelectedItem == null ? 0 : (long)((TitleCodePair)JobDFT_TypeTB.SelectedItem).Value;
            CurrentJob.Basis = JobBasisTB.SelectedItem == null ? 0 : (long)((TitleCodePair)JobBasisTB.SelectedItem).Value;
            CurrentJob.Other = (long)((TitleCodePair)JobOtherTB.SelectedItem).Value;
            CurrentJob.Task = (long)((TitleCodePair)JobTaskTB.SelectedItem).Value;
            CurrentJob.RAM = Convert.ToInt32(JobMemoryTB.Text);
            CurrentJob.Hessian = (long)((TitleCodePair)JobHesTB.SelectedItem).Value;
            CurrentJob.Charges = ChargesCB.IsChecked == true;
            CurrentJob.TDDFT = TDDFT_CB.IsChecked == true;
            CurrentJob.Solvent = (long)((TitleCodePair)JobSolutionTB.SelectedItem).Value;
            CurrentJob.Output = (long)((TitleCodePair)JobOutputTB.SelectedItem).Value;

            Close();
        }

        private void JobMemoryTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Заполнение полей при открытии окна
        /// </summary>
        private void Full()
        {
            void Set(ComboBox CB, object Value)
            {
                foreach (var Val in CB.Items)
                    if ((long)((TitleCodePair)Val).Value == (long)Value)
                    {
                        CB.SelectedItem = Val;
                        return;
                    }
            }
            
            if (CurrentJob == null)
            {
                JobNameTB.Text = "Task01";
                JobCommentTB.Text = "";
                JobMethodTB.SelectedItem = null;
                JobDFT_TypeTB.SelectedItem = null;
                JobBasisTB.SelectedItem = null;
                JobOtherTB.SelectedItem = null;
                JobTaskTB.SelectedItem = null;
                JobMemoryTB.Text = "0";
                JobHesTB.SelectedItem = null;
                ChargesCB.IsChecked = false;
                TDDFT_CB.IsChecked = false;
                JobSolutionTB.SelectedItem = null;
                JobOutputTB.SelectedItem = null;
            }
            else
            {
                JobNameTB.Text = CurrentJob.Name;
                JobCommentTB.Text = CurrentJob.Comment;
                Set(JobMethodTB, CurrentJob.Method) ;
                switch (CurrentJob.Method)
                {
                    case 1:
                        JobDFT_TypeTB.SelectedIndex = -1; 
                        JobBasisTB.SelectedIndex = -1;
                        break;
                    case 3:
                        Set(JobDFT_TypeTB, CurrentJob.DFT);
                        Set(JobBasisTB,CurrentJob.Basis);
                        break;
                    default:
                        JobDFT_TypeTB.SelectedIndex = -1;
                        Set(JobBasisTB,CurrentJob.Basis);
                        break;
                }
                Set(JobOtherTB,CurrentJob.Other);
                Set(JobTaskTB,CurrentJob.Task);
                JobMemoryTB.Text = CurrentJob.RAM.ToString();
                Set(JobHesTB,CurrentJob.Hessian);
                ChargesCB.IsChecked = CurrentJob.Charges;
                TDDFT_CB.IsChecked = CurrentJob.TDDFT;
                Set(JobSolutionTB,CurrentJob.Solvent);
                Set(JobOutputTB,CurrentJob.Output);
            }
        }

        private void MethodAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddValue("methods", AddParam.Add("Добавить метод расчёта")))
                UpdateCB(JobMethodTB, "methods");
        }

        /// <summary>
        /// Добавление нового пункта в БД
        /// </summary>
        /// <param name="Table">Таблица</param>
        /// <param name="Data">Пара «Имя» - «Значение»</param>
        /// <returns></returns>
        private bool AddValue(string Table, KeyValuePair<string, string> Data)
        {
            if (Data.Key == null) return false;

            string SafeName = Data.Key.Replace('\"', ' ').Replace('\'', ' ');
            string SafeCode = Data.Value.Replace('\"', ' ').Replace('\'', ' ');
            return Config.Execute($"INSERT INTO `{Table}`(`name`, `code`) VALUES ('{SafeName}', '{SafeCode}')");
        }

        /// <summary>
        /// Добавление нового пункта в БД
        /// </summary>
        /// <param name="Table">Таблица</param>
        /// <param name="Data">Пара «Имя» - «Значение»</param>
        /// <returns></returns>
        private bool AddValue(string Table, string Data)
        {
            if (Data == null) return false;

            string SafeCode = Data.Replace('\"', ' ').Replace('\'', ' ');
            return Config.Execute($"INSERT INTO `{Table}`(`code`) VALUES ('{SafeCode}')");
        }

        /// <summary>
        /// Обновление списка ComboBox с сохранением выбранного элемента
        /// </summary>
        /// <param name="CB">ComboBox</param>
        /// <param name="Table">Таблица, из которой берутся значения</param>
        private void UpdateCB(ComboBox CB, string Table, string name="name")
        {
            object LastValue = CB.SelectedItem;
            LoadFromDB(CB, Table, name);
            CB.SelectedItem = LastValue;
        }

        private void DFT_TypeAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddValue("dft", AddParam.Add("Добавить реализацию DFT")))
                UpdateCB(JobDFT_TypeTB, "dft");
        }

        private void BasisAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddValue("basises", AddParam.Add("Добавить базис")))
                UpdateCB(JobBasisTB, "basises");
        }

        private void OtherAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddValue("other", AddParam.AddString("Добавить прочие параметры расчёта")))
                UpdateCB(JobOtherTB, "other", "code");
        }

        private void TaskAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddValue("tasks", AddParam.Add("Добавить задачу")))
                UpdateCB(JobTaskTB, "tasks");
        }

        private void HesAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddValue("hessians", AddParam.Add("Добавить вид гессиана")))
                UpdateCB(JobHesTB, "hessians");
        }

        private void SolutionAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddValue("solvents", AddParam.Add("Добавить растворитель")))
                UpdateCB(JobSolutionTB, "solvents");
        }

        private void OutputAdd_Click(object sender, RoutedEventArgs e)
        {
                UpdateCB(JobOutputTB, "outputs");
        }
    }
}
