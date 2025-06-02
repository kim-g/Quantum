using Quantum.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.AutoDockVina
{
    /// <summary>
    /// Модель представления для управления списком белков, лигандов и связанных данных.
    /// </summary>
    internal class CreatorModel : INotifyPropertyChanged
    {
        private Protein selectedProtein;
        private Ligand ligandFileSelected;
        private string userSelected;
        private string projectName;
        private bool enableRun = false;
        private bool enableGen = true;
        private bool enableOther = true;
        private string statusText = "Проект не существует. Добавьте белки и лиганды для генерации.";
        private bool isRunning = false;
        private StreamWriter Analysing;
        private float[,,] energies;
        private Dictionary<string, int> LigandsForAnalysis;
        private Dictionary<string, int> ProteinsForAnalysis;

        /// <summary>
        /// Коллекция белков, доступных для выбора.
        /// </summary>
        public ObservableCollection<Protein> ProteinList { get; } = new ObservableCollection<Protein>();

        /// <summary>
        /// Коллекция лигандов, доступных для выбора.
        /// </summary>
        public ObservableCollection<Ligand> LigandFileList { get; } = new ObservableCollection<Ligand>();

        /// <summary>
        /// Коллекция пользовательских директорий, доступных для выбора.
        /// </summary>
        public ObservableCollection<string> UserList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Выбранный белок.
        /// </summary>
        public Protein SelectedProtein
        {
            get => selectedProtein;
            set => SetProperty(ref selectedProtein, value);
        }

        /// <summary>
        /// Выбранный лиганд.
        /// </summary>
        public Ligand LigandFileSelected
        {
            get => ligandFileSelected;
            set => SetProperty(ref ligandFileSelected, value);
        }

        /// <summary>
        /// Выбранная пользовательская директория.
        /// </summary>
        public string UserSelected
        {
            get => userSelected;
            set
            {
                SetProperty(ref userSelected, value);
                ButtonsSetEnable();
            }
        }

        /// <summary>
        /// Строка статуса
        /// </summary>
        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        /// <summary>
        /// Название директории проекта.
        /// </summary>
        public string ProjectName
        {
            get => projectName;
            set
            {
                SetProperty(ref projectName, value);
                ButtonsSetEnable();
            }
        }

        /// <summary>
        /// Возможность запуска задачи.
        /// </summary>
        public bool EnableRun
        {
            get => enableRun;
            set => SetProperty(ref enableRun, value);
        }

        /// <summary>
        /// Возможность генерации задачи.
        /// </summary>
        public bool EnableGen
        {
            get => enableGen;
            set => SetProperty(ref enableGen, value);
        }

        /// <summary>
        /// Возможность генерации задачи.
        /// </summary>
        public bool EnableOther
        {
            get => enableOther;
            set => SetProperty(ref enableOther, value);
        }

        /// <summary>
        /// Статус выполнения задачи.
        /// </summary>
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                SetProperty(ref isRunning, value);
                ButtonsSetEnable();
            }
        }

        /// <summary>
        /// Полный путь к директории проекта.
        /// </summary>
        public string ProjectPath => Path.Combine(MainMenu.Config.GetConfigValue("autodock_dir"), UserSelected, ProjectName);

        /// <summary>
        /// Конструктор по умолчанию. Инициализирует коллекции и заполняет список пользователей.
        /// </summary>
        public CreatorModel()
        {
            ProjectName = DateTime.Now.ToString("yyyy-MM-dd");
            FillUserList();
            ProteinList.CollectionChanged += (s, e) => ButtonsSetEnable();
            LigandFileList.CollectionChanged += (s, e) => ButtonsSetEnable();
        }

        /// <summary>
        /// Событие, возникающее при изменении свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывает событие <see cref="PropertyChanged"/> для указанного свойства.
        /// </summary>
        /// <typeparam name="T">Тип свойства.</typeparam>
        /// <param name="field">Ссылка на поле свойства.</param>
        /// <param name="value">Новое значение свойства.</param>
        /// <param name="propertyName">Имя свойства. Указывается автоматически.</param>
        /// <returns>Возвращает true, если значение изменилось.</returns>
        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        /// <summary>
        /// Заполняет список директорий пользователей.
        /// </summary>
        private void FillUserList()
        {
            UserList.Clear();

            string autoDockDir = MainMenu.Config.GetConfigValue("autodock_dir");
            if (!Directory.Exists(autoDockDir))
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        autoDockDir = dialog.SelectedPath;
                        MainMenu.Config.SetConfigValue("autodock_dir", autoDockDir);
                    }
                    else return;
                }
            }

            foreach (string directory in Directory.EnumerateDirectories(autoDockDir))
            {
                string userName = Path.GetFileName(directory);
                UserList.Add(userName);
                if (userName == MainMenu.Config.GetConfigValue("autodock_orderer"))
                {
                    UserSelected = userName;
                }
            }
        }

        /// <summary>
        /// Создаёт файлы для проекта докинга
        /// </summary>
        /// <returns></returns>
        public bool CreateProject()
        {
            string projectPath = Path.Combine(MainMenu.Config.GetConfigValue("autodock_dir"), UserSelected, ProjectName);
            if (!CheckUserData(projectPath)) return false;

            // Создание директорий для проекта
            Directory.CreateDirectory(projectPath);
            string LigandDirectory = Path.Combine(projectPath, "Ligands");
            string ProteinDirectory = Path.Combine(projectPath, "Proteins");
            string ResultDirectory = Path.Combine(projectPath, "Results");
            string TaskDirectory = Path.Combine(projectPath, "Tasks");
            Directory.CreateDirectory(LigandDirectory);
            Directory.CreateDirectory(ProteinDirectory);
            Directory.CreateDirectory(ResultDirectory);
            Directory.CreateDirectory(TaskDirectory);

            // Создание файлов белков
            foreach (Protein peptide in ProteinList)
                peptide.WriteToDirectory(ProteinDirectory);

            // Создание файлов лигандов
            EnableGen = false;
            Log("Генерация файлов лигандов");

            // Создаем прогресс-отчет для обновления UI
            var progress = new Progress<string>(status =>
            {
                Log(status);
            });

            foreach (Ligand ligand in LigandFileList)
                OpenBabel.ConvertToPDBQT(ligand.FileName, Path.Combine(LigandDirectory, ligand.NamePDBQT));
            EnableGen = true;
            Log("Генерация остальных файлов");



            // Создание файлов задания
            using (StreamWriter sw = new StreamWriter(Path.Combine(projectPath, "Run.bat"), false, Encoding.GetEncoding(1251)))
            {
                sw.WriteLine($"@ECHO OFF");
                sw.WriteLine($"chcp 1251 > nul");

                // Найдём все пути
                string Vina = MainMenu.Config.GetConfigValue("vina_path");
                string WorkingDir = MainMenu.Config.GetConfigValue("vina_working_dir");
                if (!Directory.Exists(WorkingDir))
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog() { Description = "Рабочая директория для расчётов" })
                    {
                        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            WorkingDir = dialog.SelectedPath;
                            MainMenu.Config.SetConfigValue("vina_working_dir", WorkingDir);
                        }
                        else return false;
                    }
                }

                // Найдём путь до исполняемого файла AutoDock Vina
                if (!File.Exists(Vina))
                {
                    Microsoft.Win32.OpenFileDialog OD = new Microsoft.Win32.OpenFileDialog
                    {
                        Multiselect = false,
                        Filter = "AutoDock Vina EXE|*.exe",
                        Title = "Выберите файл приложения Autodock Vina"
                    };
                    if (OD.ShowDialog() == true)
                    {
                        Vina = OD.FileName;
                        MainMenu.Config.SetConfigValue("vina_path", Vina);
                    }
                    else return false;
                }

                // В Batch переходим в рабочую директорию
                string Drive = Path.GetPathRoot(WorkingDir).Replace("\\", "");
                sw.WriteLine("");
                sw.WriteLine($":: Расчёт докинга для {UserSelected} от {DateTime.Now:yyyy-MM-dd}");
                sw.WriteLine(":: Расчёт производится в программе AutoDock Vina");
                sw.WriteLine(":: Задание подготовлено программой Quantum");
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine($":: Подготовка рабочей директории");
                sw.WriteLine($"ECHO Копирование файлов в рабочую директорию {WorkingDir}");
                sw.WriteLine(Drive);
                sw.WriteLine($"cd {WorkingDir}");

                // В Batch удалим все предыдущие расчёты
                sw.WriteLine($"del {Path.Combine(WorkingDir, "*.pdbqt")} > nul");
                sw.WriteLine($"del {Path.Combine(WorkingDir, "*.txt")} > nul 2> nul");

                // В Batch скопируем все файлы в рабочую директорию
                sw.WriteLine("");
                sw.WriteLine(":: Копирование белков");
                foreach (Protein peptide in ProteinList)
                {
                    sw.WriteLine($"copy {Path.Combine(ProteinDirectory, peptide.FileName)} {Path.Combine(WorkingDir, peptide.FileName)} > nul");
                }

                sw.WriteLine("");
                sw.WriteLine(":: Копирование лигандов");
                foreach (Ligand ligand in LigandFileList)
                {
                    sw.WriteLine($"copy {Path.Combine(LigandDirectory, ligand.NamePDBQT)} {Path.Combine(WorkingDir, ligand.NamePDBQT)} > nul");
                }

                sw.WriteLine("");
                sw.WriteLine(":: Копирование заданий");
                foreach (Ligand ligand in LigandFileList)
                    foreach (Protein peptide in ProteinList)
                    {
                        string Task = $"Task-{ligand.NameWithoutExtension}-{peptide.Name}.txt".Replace(" ", "_");
                        if (!CreateTaskFile(Path.Combine(TaskDirectory, Task), ligand, peptide))
                            return false;
                        sw.WriteLine($"copy {Path.Combine(TaskDirectory, Task)} {Path.Combine(WorkingDir, Task)} > nul");
                    }

                // В Batch создадим последовательность запусков
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine(":: Запуск расчёта");
                sw.WriteLine($"ECHO Запуска расчётов:");
                foreach (Ligand ligand in LigandFileList)
                {
                    string OutFile = $"{ligand.NameWithoutExtension}_Clear_Energies.txt";

                    sw.WriteLine($":: Расчёт лиганда {ligand.NameWithoutExtension}");
                    sw.WriteLine($"ECHO Расчёт {ligand.NameWithoutExtension}");
                    sw.WriteLine($"ECHO Расчёт {ligand.NameWithoutExtension} > {OutFile}");
                    foreach (Protein peptide in ProteinList)
                    {
                        string Task = $"Task-{ligand.NameWithoutExtension}-{peptide.Name}.txt".Replace(" ", "_");

                        sw.WriteLine($"ECHO Расчёт {ligand.NameWithoutExtension} в {peptide.Name} >> {OutFile}");
                        sw.WriteLine($"{Vina} --config {Task} >> {OutFile} 2> nul");
                    }
                }

                // В Batch скопируем обратно результаты
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine(":: Копирование результатов");
                foreach (Ligand ligand in LigandFileList)
                {
                    sw.WriteLine($"copy {Path.Combine(WorkingDir, ligand.NameWithoutExtension + "_Clear_Energies.txt")} {Path.Combine(ResultDirectory, ligand.NameWithoutExtension + "_Clear_Energies.txt")} > nul");
                    foreach (Protein peptide in ProteinList)
                    {
                        string ResultFile = $"{ligand.NameWithoutExtension}_{peptide.Name.Replace(" ", "_")}_out.pdbqt";
                        sw.WriteLine($"copy {Path.Combine(WorkingDir, ResultFile)} {Path.Combine(ResultDirectory, ResultFile)} > nul");
                    }
                }
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine("ECHO Расчёты закончены");
            }

            ButtonsSetEnable();
            return true;
        }

        /// <summary>
        /// Проверка правильности ввода данных пользователем
        /// </summary>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        private bool CheckUserData(string projectPath)
        {
            if (string.IsNullOrEmpty(ProjectName))
            {
                System.Windows.MessageBox.Show("Введите название проекта.");
                return false;
            }
            if (string.IsNullOrEmpty(UserSelected))
            {
                System.Windows.MessageBox.Show("Выберите пользователя.");
                return false;
            }
            if (Directory.Exists(projectPath))
            {
                System.Windows.MessageBox.Show("Проект с таким именем уже существует.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Создаёт файл задания для AutoDock Vina.
        /// </summary>
        /// <param name="fileName">Имя файла задания</param>
        /// <param name="ligand">Лиганд для расчёта</param>
        /// <param name="peptide">Белок для расчёта</param>
        /// <returns>Возвращает true, если сождание удалось.</returns>
        protected bool CreateTaskFile(string fileName, Ligand ligand, Protein peptide)
        {
            string ResultFile = $"{ligand.NameWithoutExtension}_{peptide.Name}_out.pdbqt";

            try
            {
                using (StreamWriter TaskFile = new StreamWriter(fileName, false, Encoding.GetEncoding(1251)))
                {
                    TaskFile.WriteLine($"receptor = {peptide.FileName}");
                    TaskFile.WriteLine($"ligand = {ligand.NamePDBQT}");
                    TaskFile.WriteLine($"center_x = {peptide.Center[0]}");
                    TaskFile.WriteLine($"center_y = {peptide.Center[1]}");
                    TaskFile.WriteLine($"center_z = {peptide.Center[2]}");
                    TaskFile.WriteLine($"size_x = {peptide.Size[0]}");
                    TaskFile.WriteLine($"size_y = {peptide.Size[1]}");
                    TaskFile.WriteLine($"size_z = {peptide.Size[2]}");
                    TaskFile.WriteLine($"out = {ResultFile.Replace(" ", "_")}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при создании файла задания: {ex.Message}");
                return false;
            }
        }

        /// <summary>   
        /// Проверяет возможность запуска задачи.
        /// </summary>
        protected bool CheckRunEnable()
        {
            if (string.IsNullOrEmpty(ProjectName))
                return false;

            if (string.IsNullOrEmpty(UserSelected))
                return false;

            if (!Directory.Exists(ProjectPath))
                return false;

            if (!File.Exists(Path.Combine(ProjectPath, "Run.bat")))
                return false;

            return true;
        }

        /// <summary>
        /// Запускает задачу AutoDock Vina.
        /// </summary>
        /// <returns></returns>
        public async Task RunTask()
        {
            if (!CheckRunEnable())
            {
                System.Windows.MessageBox.Show("Не удалось запустить задачу. Проверьте настройки.");
                return;
            }

            IsRunning = true;

            string batFilePath = Path.Combine(ProjectPath, "Run.bat");
            StringBuilder outputBuilder = new StringBuilder();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe", // Запускаем через командную строку
                Arguments = $"/C \"{batFilePath}\"", // /C - выполнить команду и закрыть
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(batFilePath),
                StandardOutputEncoding = Encoding.GetEncoding(1251), // Кодировка консоли Windows
                StandardErrorEncoding = Encoding.GetEncoding(1251)
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;

                // Вывод выдаваемой информации
                process.OutputDataReceived += async (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        outputBuilder.AppendLine(e.Data);
                        // Если процесс выдал финальную фразу, то завершаем
                        if (e.Data.StartsWith("Расчёты закончены"))
                        {
                            IsRunning = false;
                            Log("Расчёты закончены. ");
                            // Создаем прогресс-отчет для обновления UI
                            var progress = new Progress<string>(status =>
                            {
                                Log(status);
                            });
                            await Analyse(progress);
                            Functions.OpenExplorerWindow(ProjectPath);
                        }
                        Log(e.Data);
                        
                    }
                };

                // Вывод выдаваемых ошибок
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        outputBuilder.AppendLine($"[ERROR] {e.Data}");
                        System.Windows.Application.Current.Dispatcher.Invoke(() => Log($"[ERROR] {e.Data}"));
                    }
                };

                // Обработка завершения процесса
                process.Exited += (sender, e) =>
                {
                    IsRunning = false;
                    Functions.OpenExplorerWindow(ProjectPath);
                    Log("Процесс завершился. ");
                };

                process.Start();

                // Начинаем асинхронное чтение вывода
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                return;
            }
        }

        /// <summary>
        /// Управляет активностью кнопок и статусом
        /// </summary>
        private void ButtonsSetEnable()
        {
            // Если идёт расчёт, отключим всё
            if (isRunning)
            {
                EnableRun = false;
                EnableGen = false;
                EnableOther = false;
                return;
            }

            // Включим все настроечные компоненты и настроим кнопки
            EnableOther = true;
            EnableRun = CheckRunEnable();
            EnableGen = !EnableRun && ProteinList.Count > 0 && LigandFileList.Count > 0;

            // Настроим строку статуса
            if (EnableGen)
            {
                StatusText = "Проект готов к генерации.";
                return;
            }
            if (EnableRun)
            {
                StatusText = "Проект готов к запуску.";
                return;
            }

            StatusText = "Проект не может быть сгенерирован. ";
            if (ProteinList.Count == 0)
                StatusText += "Добавьте хотя бы один белок. ";
            if (LigandFileList.Count == 0)
                StatusText += "Добавьте хотя бы один лиганд. ";

        }

        /// <summary>
        /// Выдаёт статус и записывает его в консоль для отладки
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            // Логирование в консоль или файл
            StatusText = message;
            Console.WriteLine(message);
        }

        /// <summary>
        /// Анализ результатов докинга.
        /// </summary>
        public async Task Analyse(IProgress<string> progress)
        {
            progress?.Report("Анализ результатов... Обработка файлов.");
            
            string ResultPath = Path.Combine(ProjectPath, "Results");
            using (Analysing = new StreamWriter(Path.Combine(ResultPath, "Results.csv"), false, Encoding.GetEncoding(1251)))
            {
                Analysing.WriteLine($"Проект; {ProjectName}");
                Analysing.WriteLine($"Заказчик; {UserSelected}");
                Analysing.WriteLine($"Дата расчёта; {DateTime.Now.ToString("yyyy-MM-dd")}");
                Analysing.WriteLine("");

                energies = new float[ProteinList.Count, LigandFileList.Count, 9];
                ProteinsForAnalysis = new Dictionary<string, int>();
                LigandsForAnalysis = new Dictionary<string, int>();
                int i = 0;
                foreach (Protein protein in ProteinList) ProteinsForAnalysis.Add(protein.Name, i++);
                i = 0;
                foreach (Ligand ligand in LigandFileList) LigandsForAnalysis.Add(ligand.NameWithoutExtension, i++);

                foreach (Ligand ligand in LigandFileList)
                {
                    string FileName = ligand.NameWithoutExtension.Replace(' ', '_') + "_Clear_Energies.txt";
                    AnalyseFile(Path.Combine(ResultPath, FileName));
                }

                progress?.Report("Анализ результатов... Формирование таблицы максимальных значений.");
                Analysing.WriteLine($"Максимальные значения");
                string LineOut = "Лиганд;";
                foreach (Protein protein in ProteinList)
                    LineOut += $"{protein.Name};";

                Analysing.WriteLine(LineOut);
                for (i = 0; i < LigandFileList.Count; i++)
                {
                    LineOut = LigandFileList[i].NameWithoutExtension + ";";
                    for (int j = 0; j < ProteinList.Count; j++)
                        LineOut += $"{energies[j, i, 0]:F3};";
                    Analysing.WriteLine(LineOut);
                }

                progress?.Report("Анализ результатов... Вывод всех энергий лигандов.");
                for (i = 0; i < LigandFileList.Count; i++)
                {
                    Analysing.WriteLine("");
                    Analysing.WriteLine($"Энергии лиганда {LigandFileList[i].NameWithoutExtension}");
                    LineOut = "Белок;1;2;3;4;5;6;7;8;9";
                    Analysing.WriteLine(LineOut);
                    for (int j = 0; j < ProteinList.Count; j++)
                    {
                        LineOut = ProteinList[j].Name + ";";
                        for (int k = 0; k < 9; k++)
                            LineOut += $"{energies[j, i, k]:F3};";
                        Analysing.WriteLine(LineOut);
                    }
                }
                
            }

            progress?.Report("Анализ результатов выполнен");
            return;
        }

        /// <summary>
        /// Анализирует один файл
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        protected void AnalyseFile(string FileName)
        {
            if (Analysing == null) return;
            
            using (StreamReader Reader = new StreamReader(FileName, Encoding.GetEncoding(1251)))
            {
                string line = Reader.ReadLine();
                string Ligand = line.Replace("Расчёт ", "").Trim();
                while (AnalyseTask(Reader, Ligand));
            }

        }

        protected bool AnalyseTask(StreamReader Reader, string ligand)
        {
            if (Analysing == null) return false;

            if (Reader.EndOfStream) return false;
            string line = Reader.ReadLine();
            if (line == "") return false;
            if (!line.Contains($"Расчёт {ligand}")) return false;

            string protein = line.Replace($"Расчёт {ligand} в ", "").Trim();
            int LigandNum = LigandsForAnalysis[ligand];
            int ProteinNum = ProteinsForAnalysis[protein];
            
            while (!line.Contains("-----+------------+----------+----------"))
                line = Reader.ReadLine();

            for (int i = 0; i < 9; i++)
            {
                line = Reader.ReadLine();
                float Energy = 0;
                try
                {
                    string TextToParce = line.Substring(6, 12);
                    Energy = Functions.Parse(line.Substring(6, 12));
                }
                catch
                {
                    Log($"Ошибка парсинга. Не могу распарсить строку \"{line}\"");
                }
                energies[ProteinNum, LigandNum, i] = Energy;
            }

            return true;
        }
    }
}

