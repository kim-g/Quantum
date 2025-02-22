﻿using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;

namespace Quantum
{
    public class SQLiteDataBase : IDisposable
    {
        protected string dbFileName;
        protected SQLiteConnection Connection;
        protected SQLiteCommand Command;

        public string ErrorMsg;

        public SQLiteDataBase(string FileName = "")
        {
            dbFileName = FileName;
            Connection = new SQLiteConnection();
            Command = new SQLiteCommand();
        }

        static public SQLiteDataBase Create(string FileName, string Query)
        {
            SQLiteDataBase NewBase = new SQLiteDataBase(FileName);

            if (NewBase.CreateDB(Query))
                return NewBase;
            else
                return null;
        }

        static public SQLiteDataBase Open(string FileName)
        {
            if (File.Exists(FileName))
            {
                SQLiteDataBase NewBase = new SQLiteDataBase(FileName);

                if (NewBase.OpenDB())
                    return NewBase;
                else
                    return null;
            }
            return null;
        }

        protected bool OpenDB()
        {
            if (!File.Exists(dbFileName))
            {
                ErrorMsg = "Database file not found";
                return false;
            }

            try
            {
                Connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                Connection.Open();
                Command.Connection = Connection;
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
            return true;
        }

        protected bool CreateDB(string Query)
        {
            string Dir = Path.GetDirectoryName(Path.GetFullPath(dbFileName));
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);
            if (!File.Exists(dbFileName))
                SQLiteConnection.CreateFile(dbFileName);

            try
            {
                Connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                Connection.Open();
                Command.Connection = Connection;

                Command.CommandText = Query;
                Command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Выполняет запрос. Возвращает таблицу.
        /// </summary>
        /// <param name="Query">Запрос</param>
        /// <returns></returns>
        public DataTable ReadTable(string Query)
        {
            DataTable dTable = new DataTable();

            if (Connection.State != ConnectionState.Open)
            {
                ErrorMsg = "Open Database";
                return null;
            }

            try
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(Query, Connection))
                    adapter.Fill(dTable);
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                return null;
            }

            return dTable;
        }

        /// <summary>
        /// Выдаёт количество записей в таблице с условиями
        /// </summary>
        /// <param name="Table">Имя таблицы</param>
        /// <param name="Where">Условия</param>
        /// <returns></returns>
        public int GetCount(string Table, string Where = null)
        {
            DataTable dTable = new DataTable();

            if (Connection.State != ConnectionState.Open)
            {
                ErrorMsg = "Open Database";
                return 0;
            }

            try
            {
                string Query = Where == null ? "SELECT COUNT() AS 'C' FROM `" + Table + ";" : "SELECT COUNT() AS 'C' FROM `" + Table + "` WHERE " + Where + ";";
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(Query, Connection))
                    adapter.Fill(dTable);
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                return 0;
            }

            return Convert.ToInt32(dTable.Rows[0].ItemArray[0].ToString());
        }

        /// <summary>
        /// Выполнить запрос. Возврещает true в случае удачного выполнения и false в случае ошибки
        /// </summary>
        /// <param name="Query">Запрос</param>
        /// <returns></returns>
        public bool Execute(string Query)
        {
            if (Connection.State != ConnectionState.Open)
            {
                ErrorMsg = "Open connection with database";
                return false;
            }

            try
            {
                Command.CommandText = Query;
                Command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                Console.WriteLine($"Ошибка {ErrorMsg}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Выполнить запрос. Возврещает true в случае удачного выполнения и false в случае ошибки
        /// </summary>
        /// <param name="Query">Запрос. Значение @BLOB заменяется на поток</param>
        /// <param name="BLOB">Поток для BLOB</param>
        /// <returns></returns>
        public bool ExecuteBLOB(string Query, Stream BLOB)
        {
            if (Connection.State != ConnectionState.Open)
            {
                ErrorMsg = "Open connection with database";
                return false;
            }

            try
            {
                Command.CommandText = Query;
                byte[] byteBLOB = new byte[BLOB.Length];
                BLOB.Position = 0;
                BLOB.Read(byteBLOB, 0, Convert.ToInt32(BLOB.Length));
                Command.Parameters.Add("@BLOB", DbType.Binary, byteBLOB.Length).Value = byteBLOB;
                Command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
            return true;
        }

        public long LastID
        {
            get { return Connection.LastInsertRowId; }
        }

        /// <summary>
        /// Добавляет элемент по запросу и возвращает ID последней записи
        /// </summary>
        /// <param name="Query">Запрос</param>
        /// <returns></returns>
        public long Insert(string Query)
        {
            if (Execute(Query))
                return LastID;
            return 0;
        }

        public void Dispose()
        {
            Connection.Dispose();
            Command.Dispose();
            Connection = null;
            Command = null;
            dbFileName = null;
            ErrorMsg = null;
        }

        public T GetOne<T>(string Field, string Table, string Where = "TRUE")
        {
            using (DataTable dt = ReadTable($"SELECT `{Field}` FROM `{Table}` WHERE {Where}"))
            {
                if (dt.Rows.Count == 0) return default(T);

                try
                {
                    return dt.Rows[0].Field<T>(Field);
                }
                catch (Exception e)
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Определяет, имеется ли данная таблица в БД
        /// </summary>
        /// <param name="Table">Имя таблицы</param>
        /// <returns></returns>
        public bool TableExists(string Table)
        {
            return GetCount("sqlite_master", $"type='table' AND name='{Table}'") > 0;
        }
    }

    public class SQLiteConfig : SQLiteDataBase
    {
        public SQLiteConfig(string FileName) : base(FileName)
        {
            if (File.Exists(FileName))
            {
                OpenDB();
                if (!TableExists("proteins"))
                {
                    Execute(@"CREATE TABLE ""config"" (
	                    ""id""	INTEGER,
	                    ""name""	TEXT,
	                    ""value""	TEXT,
	                    PRIMARY KEY(""id"" AUTOINCREMENT)
                    );");
                }
            }
            else
            {
                CreateDB("CREATE TABLE `config` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name`	TEXT NOT NULL, `value` TEXT); ");
            }
        }

        public static new SQLiteConfig Open(string FileName)
        {
            return new SQLiteConfig(FileName);
        }

        //Работа с конфигом, получение значения

        /// <summary>
        /// Получение конфига как объекта
        /// </summary>
        /// <param name="name">Имя параметра конфига</param>
        /// <returns></returns>
        private object GetConfigObject(string name)
        {
            using (DataTable Conf = ReadTable($"SELECT `value` FROM `config` WHERE `name`='{name}' LIMIT 1"))
            {
                if (Conf.Rows.Count == 0) return "";
                return Conf.Rows[0].ItemArray[0];
            }
        }

        /// <summary>
        /// Получение конфига как строки
        /// </summary>
        /// <param name="name">Имя параметра конфига</param>
        /// <returns></returns>
        public string GetConfigValue(string name)
        {
            return GetConfigObject(name).ToString();
        }

        /// <summary>
        /// Получение конфига как числа INT
        /// </summary>
        /// <param name="name">Имя параметра конфига</param>
        /// <returns></returns>
        public int GetConfigValueInt(string name)
        {
            try
            { return Convert.ToInt32(GetConfigObject(name));}

            catch (Exception)
            { return 0; }
        }

        /// <summary>
        /// Получение конфига как лоическое значение
        /// </summary>
        /// <param name="name">Имя параметра конфига</param>
        /// <returns></returns>
        public bool GetConfigValueBool(string name)
        {
            return GetConfigValue(name) == "1";
        }

        /// <summary>
        /// Получение конфига как числа LONG
        /// </summary>
        /// <param name="name">Имя параметра конфига</param>
        /// <returns></returns>
        public long GetConfigValueLong(string name)
        {
            try
            { return Convert.ToInt64(GetConfigObject(name)); }

            catch (Exception)
            { return 0; }

        }

        public T GetConfigValue<T>(string name)
        {
            try
            { return (T)GetConfigObject(name); }
            catch (Exception)

            { return default(T); }
            
        }


        //Работа с конфигом, установка значения

        public bool SetConfigValue(string name, string value)
        {
            if (GetCount("config", $"`name`='{name}'") > 0)
                return Execute($"UPDATE `config` SET `value`='{value}' WHERE `name`='{name}';");
            return Execute($"INSERT INTO `config` (`name`, `value`) VALUES ('{name}','{value}');");
        }

        public bool SetConfigValue(string name, int value)
        {
            if (GetCount("config", $"`name`='{name}'") > 0)
                return Execute($"UPDATE `config` SET `value`='{value}' WHERE `name`='{name}'");
            return Execute($"INSERT INTO `config` (`name`, `value`) VALUES ('{name}','{value}');");
        }

        public bool SetConfigValue(string name, long value)
        {
            if (GetCount("config", $"`name`='{name}'") > 0)
                return Execute($"UPDATE `config` SET `value`='{value}' WHERE `name`='{name}'");
            return Execute($"INSERT INTO `config` (`name`, `value`) VALUES ('{name}','{value}');");
        }

        public bool SetConfigValue(string name, bool value)
        {
            string val = value ? "1" : "0";
            if (GetCount("config", $"`name`='{name}'") > 0)
                return Execute($"UPDATE `config` SET `value`='{val}' WHERE `name`='{name}'");
            return Execute($"INSERT INTO `config` (`name`, `value`) VALUES ('{name}','{val}');");
        }
    }
}
