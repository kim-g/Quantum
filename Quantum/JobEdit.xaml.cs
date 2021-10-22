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

        public JobEdit(SQLiteConfig ConfigDataBase)
        {
            InitializeComponent();

            Config = ConfigDataBase;
            Methods = LoadFromDB("methods");
            DFT = LoadFromDB("dft");
            Basises = LoadFromDB("basises");
            Other = LoadFromDB("other");
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
        /// Загрузка таблицы из БД в словарь
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <returns></returns>
        public Dictionary<string, int> LoadFromDB(string table)
        {
            Dictionary<string, int> Dic = new Dictionary<string, int>();

            using (DataTable dt = Config.ReadTable($"SELECT * FROM `{table}` ORDER BY `id`;"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Dic.Add(dr["name"].ToString(), (int)dr["id"]);
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
    }
}
