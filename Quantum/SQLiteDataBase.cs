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

        public void Dispose()
        {
            Connection.Dispose();
            Command.Dispose();
            Connection = null;
            Command = null;
            dbFileName = null;
            ErrorMsg = null;
        }
    }

    public class SQLiteConfig : SQLiteDataBase
    {
        public SQLiteConfig(string FileName) : base(FileName)
        {
            if (File.Exists(FileName))
            {
                OpenDB();
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

        public string GetConfigValue(string name)
        {
            using (DataTable Conf = ReadTable("SELECT `value` FROM `config` WHERE `name`='" + name + "' LIMIT 1"))
            {
                if (Conf.Rows.Count == 0) return "";
                return Conf.Rows[0].ItemArray[0].ToString();
            }
        }

        public int GetConfigValueInt(string name)
        {
            using (DataTable Conf = ReadTable("SELECT `value` FROM `config` WHERE `name`='" + name + "' LIMIT 1"))
            {
                if (Conf.Rows.Count == 0) return 0;
                return Convert.ToInt32(Conf.Rows[0].ItemArray[0].ToString());
            }
        }

        public bool GetConfigValueBool(string name)
        {
            using (DataTable Conf = ReadTable("SELECT `value` FROM `config` WHERE `name`='" + name + "' LIMIT 1"))
            {
                if (Conf.Rows.Count == 0) return false;
                return Conf.Rows[0].ItemArray[0].ToString() == "1";
            }
        }


        //Работа с конфигом, установка значения

        public bool SetConfigValue(string name, string value)
        {
            if (GetCount("config", "`name`='" + name + "'") > 0)
                return Execute("UPDATE `config` SET `value`='" + value + "' WHERE `name`='" + name + "';");
            else
                return Execute("INSERT INTO `config` (`name`, `value`) VALUES ('" + name + "','" + value + "');");
        }

        public bool SetConfigValue(string name, int value)
        {
            if (GetCount("config", "`name`='" + name + "'") > 0)
                return Execute("UPDATE `config` SET `value`='" + value.ToString() + "' WHERE `name`='" + name + "'");
            else
                return Execute("INSERT INTO `config` (`name`, `value`) VALUES ('" + name + "','" + value.ToString() + "');");
        }

        public bool SetConfigValue(string name, bool value)
        {
            string val = value ? "1" : "0";
            if (GetCount("config", "`name`='" + name + "'") > 0)
                return Execute("UPDATE `config` SET `value`='" + val + "' WHERE `name`='" + name + "'");
            else
                return Execute("INSERT INTO `config` (`name`, `value`) VALUES ('" + name + "','" + val + "');");
        }
    }
}
