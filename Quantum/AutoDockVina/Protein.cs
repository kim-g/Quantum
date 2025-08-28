using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;

namespace Quantum.AutoDockVina
{
    /// <summary>
    /// Представляет белок с его свойствами и методами для загрузки из базы данных.
    /// </summary>
    public class Protein : INotifyPropertyChanged
    {
        private string name;
        private string description;
        private string fileName;

        /// <summary>
        /// Получает или задает имя белка.
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Получает или задает описание белка.
        /// </summary>
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        /// <summary>
        /// Получает или задает имя файла белка.
        /// </summary>
        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        /// <summary>
        /// Получает или задает координаты центра белка.
        /// </summary>
        public string[] Center { get; set; } = new string[3];

        /// <summary>
        /// Получает или задает размеры пространства докинга.
        /// </summary>
        public string[] Size { get; set; } = new string[3];

        /// <summary>
        /// Получает или задает поток файла белка.
        /// </summary>
        public MemoryStream File { get; set; }

        public string Info => $"Координаты: {Center[0]}, {Center[1]}, {Center[2]}; Область: {Size[0]}, {Size[1]}, {Size[2]}";

        /// <summary>
        /// Загружает белок из базы данных по его идентификатору.
        /// </summary>
        /// <param name="db">Экземпляр базы данных.</param>
        /// <param name="id">Идентификатор белка.</param>
        /// <returns>Загруженный белок или null, если не найден.</returns>
        public static Protein LoadFromDB(SQLiteDataBase db, int id)
        {
            DataTable dt = db.ReadTable($"SELECT * FROM `proteins` WHERE `id`={id}");
            if (dt.Rows.Count == 0)
                return null;

            return LoadFromData(dt.Rows[0]);
        }

        /// <summary>
        /// Загружает белок из строки данных.
        /// </summary>
        /// <param name="dataRow">Строка данных, содержащая информацию о белке.</param>
        /// <returns>Загруженный белок.</returns>
        public static Protein LoadFromData(DataRow dataRow)
        {
            Protein protein = new Protein();
            protein.Name = dataRow["name"].ToString();
            protein.Description = dataRow["description"].ToString();
            protein.FileName = dataRow["file_name"].ToString();
            protein.Center[0] = dataRow["center_x"].ToString();
            protein.Center[1] = dataRow["center_y"].ToString();
            protein.Center[2] = dataRow["center_z"].ToString();
            protein.Size[0] = dataRow["size_x"].ToString();
            protein.Size[1] = dataRow["size_y"].ToString();
            protein.Size[2] = dataRow["size_z"].ToString();
            protein.File = new MemoryStream((byte[])dataRow["file"]);

            return protein;
        }

        /// <summary>
        /// Загружает все белки из базы данных.
        /// </summary>
        /// <returns>Список загруженных белков.</returns>
        public static List<Protein> LoadProteins()
        {
            List<Protein> PS = new List<Protein>();

            DataTable dt = MainMenu.Config.ReadTable("SELECT * FROM `proteins` ORDER BY `name`");
            foreach (DataRow dr in dt.Rows)
                PS.Add(LoadFromData(dr));

            return PS;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Переопределяет метод ToString для отображения информации о белке.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} ({Description})";
        }

        /// <summary>
        /// Записывает файл белка в указанный путь.
        /// </summary>
        /// <param name="fileName">Путь к файлу белка</param>
        /// <returns></returns>
        public bool WriteToFile(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    File.WriteTo(fs);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Записывает файл белка в указанную директорию. Имя файла сохраняется стандартное
        /// </summary>
        /// <param name="directory">Директория, куда записывается файл белка</param>
        /// <returns></returns>
        public bool WriteToDirectory(string directory)
        {
            string fileName = Path.Combine(directory, FileName);
            return WriteToFile(fileName);
        }
    }
}
