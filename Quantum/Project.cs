using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public class Project
    {
        #region Внутренние переменные
        private long _id = 0;
        private string _name = "";
        private string _description = "";
        private long _input_node = 0;
        private NodePanel _project_panel;
        private SQLiteDataBase DB;
        #endregion

        #region Свойства
        /// <summary>
        /// Номер проекта в базе данных
        /// </summary>
        public long ID
        {
            get => _id;
            private set
            {
                if (_id != 0)
                    DB.Execute($"UPDATE `projects` SET `id`={value} WHERE `id`={_id}");
                _id = value;
            }
        }
        
        /// <summary>
        /// Название проекта
        /// </summary>
        public string Title
        {
            get => _name;
            set
            {
                _name = value;
                if (ID != 0)
                    DB.Execute($"UPDATE `projects` SET `name`='{_name}' WHERE `id`={ID}");
            }
        }

        /// <summary>
        /// Описание проекта
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                if (ID != 0)
                    DB.Execute($"UPDATE `projects` SET `comment`='{_description}' WHERE `id`={ID}");
            }
        }

        /// <summary>
        /// Номер начального узла в базе данных
        /// </summary>
        public long Input
        {
            get => _input_node;
            set
            {
                if (DB.GetCount("nodes", $"`id`='{value}'") == 1)
                {
                    _input_node = value;
                    DB.Execute($"UPDATE `projects` SET `input`='{_input_node}' WHERE `id`={ID}");
                }
            }
        }

        #endregion

        #region Конструкторы
        public Project (SQLiteDataBase db)
        {
            DB = db;
        }

        /// <summary>
        /// Создание нового проекта и добавление его в БД
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static Project New(SQLiteDataBase db)
        {
            db.Execute("INSERT INTO `projects`(`name`) VALUES('')");
            return Load(db, db.LastID);
        }

        /// <summary>
        /// Загрузка проекта из БД
        /// </summary>
        /// <param name="db">База данных</param>
        /// <param name="id">Номер проекта в базе данных</param>
        /// <returns></returns>
        public static Project Load(SQLiteDataBase db, long id)
        {
            using (DataTable dt = db.ReadTable($"SELECT * FROM `projects` WHERE `id`={id}"))
            {
                if (dt.Rows.Count == 0) return null;

                return Load(db, dt.Rows[0]);
            }
        }


        /// <summary>
        /// Загрузка проекта из БД
        /// </summary>
        /// <param name="db">База данных</param>
        /// <param name="dr">Строка таблицы</param>
        /// <returns></returns>
        private static Project Load(SQLiteDataBase db, DataRow dr)
        {
            Project NP = new Project(db);
            NP._id = dr.Field<long>("id");
            NP._name = dr.Field<string>("name");
            NP._description = dr.Field<string>("comment");
            NP._input_node = dr.Field<long>("input");
            return NP;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Получение списка проектов
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<Project> GetProjects(SQLiteDataBase db)
        {
            List<Project> PL = new List<Project>();
            using (DataTable dt = db.ReadTable("SELECT * FROM `projects` ORDER BY `name`"))
            {
                foreach (DataRow dr in dt.Rows)
                    PL.Add(Load(db, dr));
            }
            return PL;
        }


        /// <summary>
        /// Выдаёт заголовок класса при вызове стандартной функции ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Title;
        }

        /// <summary>
        /// Обновление проекта
        /// </summary>
        public void Update()
        {
            using (DataTable dt = DB.ReadTable($"SELECT * FROM `projects` WHERE `id`={ID}"))
            {
                if (dt.Rows.Count == 0) return;
                DataRow dr = dt.Rows[0];

                _name = dr.Field<string>("name");
                _description = dr.Field<string>("comment");
                _input_node = dr.Field<long>("input");
            }
        }


        public void DeleteProject()
        {
            NodePanel NP = new NodePanel();
            Node InputNode = Node.LoadFromDB(DB, Input);
            NP.Children.Add(InputNode);
            InputNode.LoadChildren();
            InputNode.DeleteAll();
            DB.Execute($"DELETE FROM `projects` WHERE `id`={ID}");
        }
        #endregion

        #region События

        #endregion
    }
}
