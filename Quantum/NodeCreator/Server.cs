using System.Data;

namespace Quantum
{
    /// <summary>
    /// Класс для хранения информации о сервере выполнения и хранения данных
    /// </summary>
    public class Server
    {
        /// <summary>
        /// ID номер в БД
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Название сервера
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Путь к хранилищу данных
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Путь к домашней директории
        /// </summary>
        public string LCTN { get; set; }

        /// <summary>
        /// Путь из домашней директории к хранилищу данных
        /// </summary>
        public string Task { get; set; }

        /// <summary>
        /// Бинарные файлы Orca
        /// </summary>
        public string OrcaBin { get; set; }


        /// <summary>
        /// Создание «пустого» сервера
        /// </summary>
        public Server()
        {

        }

        /// <summary>
        /// Создание сервера с данными
        /// </summary>
        /// <param name="id">ID номер в БД</param>
        /// <param name="name">Название сервера</param>
        /// <param name="path">Путь к хранилищу данных</param>
        /// <param name="lctn">Путь к домашней директории</param>
        /// <param name="task">Путь из домашней директории к исполнимым бинарным файлам</param>
        public Server(long id, string name, string path, string lctn, string task, string orcabin)
        {
            ID = id;
            Name = name;
            Path = path;
            LCTN = lctn;
            Task = task;
            OrcaBin = orcabin;
        }

        /// <summary>
        /// Создание сервера по строке из БД
        /// </summary>
        /// <param name="rd">Строка в БД</param>
        public Server (DataRow dr)
        {
            ID = dr.Field<long>("id");
            Name = dr.Field<string>("name");
            Path = dr.Field<string>("path");
            LCTN = dr.Field<string>("lctndir");
            Task = dr.Field<string>("taskdir");
            OrcaBin = dr.Field<string>("orca_bin");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
