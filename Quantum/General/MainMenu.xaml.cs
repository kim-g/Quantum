using System.Windows;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        /// <summary>
        /// Версия программы
        /// </summary>
        public static string ProgramVersion = "1.7.6";
        /// <summary>
        /// База данных конфига
        /// </summary>
        static public SQLiteConfig Config;

        /// <summary>
        /// Инициализация окна
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();
            Config = SQLiteConfig.Open("config.db");
            CheckTables();
        }

        /// <summary>
        /// Нажатие на кнопку "Проекты расчётов из шаблонов"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ShowModal(this, Config);
        }

        /// <summary>
        /// Нажатие кнопки "Закрыть"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Нажатие кнопки "О программе"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            About.ShowModal(this);
        }

        private void EnergyButton_Click(object sender, RoutedEventArgs e)
        {
            Energy_Diagram.ShowModal(this);
        }

        private void ConstructorButton_Click(object sender, RoutedEventArgs e)
        {
            ShowInTaskbar = false;
            ProjectListWindow PLW = new ProjectListWindow(Config);
            PLW.ShowDialog();

            ShowInTaskbar = true;
        }

        private void AutoDockVinaButton_Click(object sender, RoutedEventArgs e)
        {
            AutoDockVina.AutoDockCreator autoDockCreator = new AutoDockVina.AutoDockCreator();
            autoDockCreator.Owner = this;
            autoDockCreator.ShowDialog();   
        }

        /// <summary>
        /// Проверка наличия всех нужных таблиц в БД. Если их нет, они создаются.
        /// </summary>
        private void CheckTables()
        {
            // Таблица базисов
            if (!Config.TableExists("basises"))
            {
                Config.Execute(@"CREATE TABLE ""basises"" (
	                ""id""	INTEGER,
	                ""name""	TEXT,
	                ""code""	TEXT,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица методов DFT
            if (!Config.TableExists("dft"))
            {
                Config.Execute(@"CREATE TABLE ""dft"" (
	                ""id""	INTEGER,
	                ""name""	TEXT,
	                ""code""	TEXT,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица методов расчёта Гессиана
            if (!Config.TableExists("hessians"))
            {
                Config.Execute(@"CREATE TABLE ""hessians"" (
	                ""id""	INTEGER,
	                ""name""	TEXT,
	                ""code""	TEXT,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }
            
            // Таблица заданий
            if (!Config.TableExists("jobs"))
            {
                Config.Execute(@"CREATE TABLE ""jobs"" (
	                ""id""	INTEGER,
	                ""name""	TEXT,
	                ""comment""	TEXT,
	                ""method""	INTEGER,
	                ""dft""	INTEGER,
	                ""basis""	INTEGER,
	                ""other""	INTEGER,
	                ""task""	INTEGER,
	                ""ram""	INTEGER,
	                ""hessian""	INTEGER,
	                ""charges""	INTEGER,
	                ""tddft""	INTEGER,
	                ""solvent""	INTEGER,
	                ""output""	INTEGER,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица методов расчёта
            if (!Config.TableExists("methods"))
            {
                Config.Execute(@"CREATE TABLE ""methods"" (
	                ""id""	INTEGER,
	                ""name""	TEXT,
	                ""code""	TEXT,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица узлов для нодовой структуры расчётов
            if (!Config.TableExists("nodes"))
            {
                Config.Execute(@"CREATE TABLE ""nodes"" (
	                ""id""	INTEGER NOT NULL,
	                ""type""	INTEGER NOT NULL,
	                ""name""	TEXT NOT NULL,
	                ""parent""	INTEGER,
	                ""comment""	TEXT,
	                ""job""	INTEGER,
	                ""pos_x""	REAL NOT NULL,
	                ""pos_y""	REAL NOT NULL,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица дополнительных параметров
            if (!Config.TableExists("other"))
            {
                Config.Execute(@"CREATE TABLE ""other"" (
	                ""id""	INTEGER,
	                ""code""	TEXT,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица параметров вывода
            if (!Config.TableExists("output"))
            {
                Config.Execute(@"CREATE TABLE ""output"" (
	                ""id""	INTEGER,
	                ""name""	TEXT,
	                ""code""	TEXT,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица проектов нодовой структуры
            if (!Config.TableExists("projects"))
            {
                Config.Execute(@"CREATE TABLE ""projects"" (
	                ""id""	INTEGER NOT NULL UNIQUE,
	                ""name""	TEXT,
	                ""comment""	TEXT,
	                ""input""	INTEGER DEFAULT 0,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица белков
            if (!Config.TableExists("proteins"))
            {
                Config.Execute(@"CREATE TABLE ""proteins"" (
	                ""id""	INTEGER NOT NULL,
	                ""name""	TEXT NOT NULL,
                    ""description""	TEXT NOT NULL,
	                ""file_name""	TEXT NOT NULL,
	                ""center_x""	TEXT NOT NULL,
	                ""center_y""	TEXT NOT NULL,
	                ""center_z""	TEXT NOT NULL,
	                ""size_x""	TEXT NOT NULL,
	                ""size_y""	TEXT NOT NULL,
	                ""size_z""	TEXT NOT NULL,
	                ""file""	BLOB NOT NULL,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица расчётных устройств и мест хранения
            if (!Config.TableExists("roots"))
            {
                Config.Execute(@"CREATE TABLE ""roots"" (
	                ""id""	INTEGER,
	                ""name""	TEXT UNIQUE,
	                ""path""	TEXT,
	                ""lctndir""	TEXT,
	                ""taskdir""	TEXT,
	                ""orca_bin""	TEXT,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица поддерживаемых растворителей
            if (!Config.TableExists("solvents"))
            {
                Config.Execute(@"CREATE TABLE ""solvents"" (
	                ""id""	INTEGER NOT NULL,
	                ""name""	TEXT NOT NULL,
	                ""code""	TEXT,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }

            // Таблица заданий для расчёта
            if (!Config.TableExists("tasks"))
            {
                Config.Execute(@"CREATE TABLE ""tasks"" (
	                ""id""	INTEGER,
	                ""name""	TEXT,
	                ""code""	TEXT,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                );");
            }



        }
    }
}
