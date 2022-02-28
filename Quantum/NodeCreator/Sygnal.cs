using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    /// <summary>
    /// Класс, описывающий сигнал, передаваемый между нодами для формирования
    /// </summary>
    public class Sygnal
    {
        #region Внутренние переменные
        private Node _parent;
        private string _RunFile = "";
        private List<Sygnal> child = new List<Sygnal>();
        private ObservableCollection<Job> jobs = new ObservableCollection<Job>();

        #endregion

        #region Свойства
        /// <summary>
        /// Родительский элемент, который вызывает сигнал
        /// </summary>
        public Node Parent
        {
            get => _parent;
            protected set => _parent = value;
        }

        /// <summary>
        /// Список задач, собранных сигналом. 
        /// </summary>
        public ObservableCollection<Job> Jobs
        {
            get => jobs;
            set => jobs = value;
        }

        /// <summary>
        /// Параллельность запуска
        /// </summary>
        public bool Parallel { get; set; } = false;

        /// <summary>
        /// Параллельность запуска родительского сигнала для правильного формирования названий
        /// </summary>
        public bool ParentParallel { get; set; } = false;

        /// <summary>
        /// Название файла для старта
        /// </summary>
        public string RunFileName
        {
            get => _RunFile;
            set => _RunFile = value;
        }

        /// <summary>
        /// Список сигналов-потомков
        /// </summary>
        public List<Sygnal> Children
        {
            get => child;
            set => child = value;
        }

        /// <summary>
        /// Сигнал-родитель
        /// </summary>
        public Sygnal ParentSygnal { get; set; }
        #endregion

        #region Конструкторы
        public Sygnal (Node parent, string Run = "")
        {
            Parent = parent;
            RunFileName = Run;
        }

        #endregion

        #region Методы
        public void MakeRun(string Dir, string Task, SQLiteConfig Config, Server Storage, int Charge, int Multiplet, long ProjectSolvent = 1 )
        {
            string CurDir = Dir;
            string CurTask = Task;
            
            if (RunFileName.Contains("/"))
            {
                string[] Folders = RunFileName.Split('/');
                for (int i = 0; i < Folders.Count() - 1; i++)
                {
                    CurDir = Path.Combine(Dir, Folders[i]);
                    CurTask += $"/{Folders[i]}";
                }
            }
            
            Directory.CreateDirectory(CurDir);
            MakeRunFile(CurDir, CurTask, Config, Storage);

            for (int i = 0; i < Jobs.Count; i++)
            {
                string xyz = Storage.Path + "/" + CurTask + "/";
                if (i == 0)
                {
                    string ParentTask = Task;
                    if (ParentSygnal?.RunFileName.Contains("/") == true)
                    {
                        string[] ParentFolders = ParentSygnal.RunFileName.Split('/');
                        for (int j = 0; j < ParentFolders.Count() - 1; j++)
                            ParentTask += $"/{ParentFolders[j]}";
                    }

                    if (ParentParallel) xyz += "p_";
                    xyz = Storage.Path + "/" + ParentTask + "/";
                    xyz += Parent.Title;
                }
                else
                {
                    if (Parallel) xyz += "p_";
                    xyz += Jobs[i - 1].Name;
                }
                xyz += ".xyz";
                Jobs[i]?.MakeInputFile(CurDir, xyz, Charge, Multiplet, ProjectSolvent); 
            }
        }

        /// <summary>
        /// Создаёт Start файл
        /// </summary>
        /// <param name="Dir">Рабочая директория для записи файла</param>
        /// <param name="Task">Директория проекта</param>
        /// <param name="Config">Конфигурация</param>
        /// <returns></returns>
        private bool MakeRunFile(string Dir, string Task, SQLiteConfig Config, Server Storage)
        {
            if (RunFileName == "") return false;
            string[] SplitName = RunFileName.Split('/');
            string RunName = SplitName[SplitName.Count() - 1];
            using (StreamWriter sw = new StreamWriter(Path.Combine(Dir, RunName), false) { NewLine= "\n" })
            {
                sw.WriteLine("#!/bin/bash");
                sw.WriteLine("# Настройка каталогов для расчёта");
                sw.WriteLine("task=${1:-'" + Task + "'} #Рабочий каталог");
                sw.WriteLine($"run_orca=\"{Storage.LCTN}" + Config.GetConfigValue("orca_bin") + "\" # Каталог файлов Orca");
                sw.WriteLine($"taskdir=\"{Storage.Path}/$task\" # Каталог проекта");
                sw.WriteLine("");
                sw.WriteLine("# Настройка замены команд для работы SLURM");
                sw.WriteLine("cd=\"cd\"");
                sw.WriteLine("rm=\"/bin/rm\"");
                sw.WriteLine("cat=\"/bin/cat\"");
                sw.WriteLine($"export PATH=\"{Storage.LCTN}/bin:$PATH\"");
                sw.WriteLine("");
                sw.WriteLine("# Запуск расчёта");
                sw.WriteLine("echo \"task: $task\"");
                sw.WriteLine("$cd $taskdir");
                sw.WriteLine("");
                string Tasks = "";
                foreach (Job job in Jobs)
                    Tasks += Tasks == "" ? job.Name : " " + job.Name;

                sw.WriteLine($"for q in {Tasks} ;do");
                sw.Write("  echo \" job=$q\"\n" +
                    "  if [ \"$SLURM_NTASKS.\" == \"1.\" ]; then\n" +
                    "      pq=$q\n" +
                    "  else\n" +
                    "      pq=p_$q\n" +
                    "      echo %pal nprocs=$SLURM_NTASKS >$pq.inp\n" +
                    "      echo \"   end\"                 >>$pq.inp\n" +
                    "      $cat $q.inp                   >>$pq.inp\n" +
                    "  fi\n" +
                    $"  {Storage.LCTN}/bin/donodes.pl >$pq.nodes\n" +
                    "  $run_orca $pq.inp >$do$pq.out 2>$do$pq.err\n" +
                    "done\n");

                sw.Close();
            }
            return true;
        }

        public List<Sygnal> GetChildren()
        {
            List<Sygnal> Collection = new List<Sygnal>();
            foreach (Sygnal S in Children)
            {
                Collection.Add(S);
                Collection.AddRange(S.GetChildren());
            }
            return Collection;
        }
        #endregion
    }
}
