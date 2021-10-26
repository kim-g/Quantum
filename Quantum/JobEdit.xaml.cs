using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для JobEdit.xaml
    /// </summary>
    public partial class JobEdit : Window
    {
        SQLiteConfig Config;

        // Словари
        Dictionary<string, int> Methods;
        Dictionary<string, int> DFT;
        Dictionary<string, int> Basises;
        Dictionary<string, int> Other;
        Dictionary<string, int> Tasks;
        Dictionary<string, int> Hessians;
        Dictionary<string, int> Solutions;
        Dictionary<string, int> Outputs;

        Job CurrentJob;

        public JobEdit(SQLiteConfig ConfigDataBase)
        {
            InitializeComponent();

            Config = ConfigDataBase;
            Methods = LoadFromDB("methods");
            DFT = LoadFromDB("dft");
            Basises = LoadFromDB("basises");
            Other = LoadFromDB("other", "code");
            Tasks = LoadFromDB("tasks");
            Hessians = LoadFromDB("hessians");
            Solutions = LoadFromDB("solvents");
            Outputs = LoadFromDB("output");

            FullfillCB(JobMethodTB, Methods);
            FullfillCB(JobDFT_TypeTB, DFT);
            FullfillCB(JobBasisTB, Basises);
            FullfillCB(JobOtherTB, Other);
            FullfillCB(JobTaskTB, Tasks);
            FullfillCB(JobHesTB, Hessians);
            FullfillCB(JobSolutionTB, Solutions);
            FullfillCB(JobOutputTB, Outputs);
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
        public static Job New(Window owner, SQLiteConfig ConfigDataBase)
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
        public static Job Edit(Window owner, SQLiteConfig ConfigDataBase, long JobID)
        {
            JobEdit JobWindow = new JobEdit(ConfigDataBase);
            JobWindow.CurrentJob = Job.Load(ConfigDataBase, JobID);
            JobWindow.Owner = owner;
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
        public Dictionary<string, int> LoadFromDB(string table, string name = "name")
        {
            Dictionary<string, int> Dic = new Dictionary<string, int>();

            using (DataTable dt = Config.ReadTable($"SELECT * FROM `{table}` ORDER BY `id`;"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Dic.Add(dr[name].ToString(), Convert.ToInt32(dr["id"]));
                }
            }

            return Dic;
        }

        /// <summary>
        /// Заполнить ComboBox значениями
        /// </summary>
        /// <param name="CB">ComboBox для заполнения</param>
        /// <param name="Data">Значения</param>
        public void FullfillCB(ComboBox CB, Dictionary<string, int> Data)
        {
            foreach (KeyValuePair<string, int> Line in Data)
                CB.Items.Add(Line.Key);
        }

        private void JobMethodTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine($"«{JobMethodTB.SelectedItem.ToString()}»");
            
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
            CurrentJob.Method = Methods[JobMethodTB.SelectedItem.ToString()];
            CurrentJob.DFT = JobDFT_TypeTB.SelectedItem == null ? 0 : DFT[JobDFT_TypeTB.SelectedItem.ToString()];
            CurrentJob.Basis = JobBasisTB.SelectedItem == null ? 0 : Basises[JobBasisTB.SelectedItem.ToString()];
            CurrentJob.Other = Other[JobOtherTB.SelectedItem.ToString()];
            CurrentJob.Task = Tasks[JobTaskTB.SelectedItem.ToString()];
            CurrentJob.RAM = Convert.ToInt32(JobMemoryTB.Text);
            CurrentJob.Hessian = Hessians[JobHesTB.SelectedItem.ToString()];
            CurrentJob.Charges = ChargesCB.IsChecked == true;
            CurrentJob.TDDFT = TDDFT_CB.IsChecked == true;
            CurrentJob.Solvent = Solutions[JobSolutionTB.SelectedItem.ToString()];
            CurrentJob.Output = Outputs[JobOutputTB.SelectedItem.ToString()];

            Close();
        }

        private void JobMemoryTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void Full()
        {
            if (CurrentJob == null)
            {
                JobNameTB.Text = "Task01";
                JobCommentTB.Text = "";
                JobMethodTB.SelectedItem = null;
                JobDFT_TypeTB.SelectedItem = null;
                JobBasisTB.SelectedItem = null;
                JobOtherTB.SelectedItem = null;
                JobTaskTB.SelectedItem = null;
                JobMemoryTB.Text = "4096";
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
                JobMethodTB.SelectedItem = Methods.First(x => x.Value == CurrentJob.Method).Key;
                JobDFT_TypeTB.SelectedItem = DFT.First(x => x.Value == CurrentJob.DFT).Key;
                JobBasisTB.SelectedItem = Basises.First(x => x.Value == CurrentJob.Basis).Key;
                JobOtherTB.SelectedItem = Other.First(x => x.Value == CurrentJob.Other).Key;
                JobTaskTB.SelectedItem = Tasks.First(x => x.Value == CurrentJob.Task).Key;
                JobMemoryTB.Text = CurrentJob.RAM.ToString();
                JobHesTB.SelectedItem = Hessians.First(x => x.Value == CurrentJob.Hessian).Key;
                ChargesCB.IsChecked = CurrentJob.Charges;
                TDDFT_CB.IsChecked = CurrentJob.TDDFT;
                JobSolutionTB.SelectedItem = Solutions.First(x => x.Value == CurrentJob.Solvent).Key;
                JobOutputTB.SelectedItem = Outputs.First(x => x.Value == CurrentJob.Output).Key;
            }
        }
    }
}
