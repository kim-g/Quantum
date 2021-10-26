using System.Data;

namespace Quantum
{
    public class Job
    {
        private long id = 0;
        private string name = "";
        private string comment = "";
        private int method = 0;
        private int dft = 0;
        private int basis = 0;
        private int other = 0;
        private int task = 0;
        private int ram = 0;
        private int hessian = 0;
        private bool charges = false;
        private bool tddft = false;
        private int solvent = 0;
        private int output = 0;

        private SQLiteConfig DB;
        
        public Job(SQLiteConfig db)
        {
            DB = db;
        }
        
        /// <summary>
        /// Номер в БД
        /// </summary>
        public long ID { get => id; private set => id = value; }

        /// <summary>
        /// Имя задания
        /// </summary>
        public string Name 
        { 
            get => name; 
            set 
            { 
                name = value;
                string SafeName = name.Replace('\"', ' ').Replace('\'', ' ');
                DB.Execute($"UPDATE `jobs` SET `name`='{SafeName}' WHERE `id`={ID};");
            } 
        }

        /// <summary>
        /// Комментарий к заданию
        /// </summary>
        public string Comment 
        { 
            get => comment; 
            set 
            { 
                comment = value;
                string SafeComment = comment.Replace('\"', ' ').Replace('\'', ' ');
                DB.Execute($"UPDATE `jobs` SET `comment`='{SafeComment}' WHERE `id`={ID};");
            } 
        }

        /// <summary>
        /// ID метода
        /// </summary>
        public int Method { get => method; set { method = value; DB.Execute($"UPDATE `jobs` SET  `method`={Method} WHERE `id`={ID};");
            } }

        /// <summary>
        /// ID DFT
        /// </summary>
        public int DFT { get => dft; set { dft = value; DB.Execute($"UPDATE `jobs` SET  `dft`={DFT} WHERE `id`={ID};");
            } }

        /// <summary>
        /// ID базиса
        /// </summary>
        public int Basis { get => basis; set { basis = value; DB.Execute($"UPDATE `jobs` SET `basis`={Basis} WHERE `id`={ID};");
            } }

        /// <summary>
        /// ID дополнительных параметров
        /// </summary>
        public int Other { get => other; set { other = value; DB.Execute($"UPDATE `jobs` SET `other`={Other} WHERE `id`={ID};");
            } }
        
        /// <summary>
        /// ID задачи
        /// </summary>
        public int Task { get => task; set { task = value; DB.Execute($"UPDATE `jobs` SET `task`={Task} WHERE `id`={ID};");
            } }

        /// <summary>
        /// Количество памяти на ядро
        /// </summary>
        public int RAM { get => ram; set { ram = value; DB.Execute($"UPDATE `jobs` SET `ram`={RAM} WHERE `id`={ID};");
            } }

        /// <summary>
        /// ID гессиана
        /// </summary>
        public int Hessian { get => hessian; set { hessian = value; DB.Execute($"UPDATE `jobs` SET `hessian`={Hessian} WHERE `id`={ID};");
            } }

        /// <summary>
        /// Считать ли зарядовые плотности
        /// </summary>
        public bool Charges { get => charges; set { charges = value; DB.Execute($"UPDATE `jobs` SET `charges`={(Charges ? 1 : 0)} WHERE `id`={ID};");
            } }

        /// <summary>
        /// Считать ли TD-DFT
        /// </summary>
        public bool TDDFT { get => tddft; set { tddft = value; DB.Execute($"UPDATE `jobs` SET `tddft`={(TDDFT ? 1 : 0)} WHERE `id`={ID};");
            } }

        /// <summary>
        /// ID растворителя
        /// </summary>
        public int Solvent { get => solvent; set { solvent = value; DB.Execute($"UPDATE `jobs` SET `solvent`={Solvent} WHERE `id`={ID};");
            } }

        /// <summary>
        /// ID Вывода (малый, большой)
        /// </summary>
        public int Output { get => output; set { output = value; DB.Execute($"UPDATE `jobs` SET `output`={Output} WHERE `id`={ID};");
            } }

        public bool Update()
        {
            string SafeName = Name.Replace('\"', ' ').Replace('\'', ' ');
            string SafeComment = Comment.Replace('\"', ' ').Replace('\'', ' ');
            
            if (ID == 0)
            {
                if (DB.Execute($"INSERT INTO `jobs` (`name`, `comment`, `method`, `dft`, `basis`, `other`, `task`, `ram`, `hessian`, `charges`, `tddft`, `solvent`, `output`) " +
                    $"VALUES ('{SafeName}', '{SafeComment}', {Method}, {DFT}, {Basis}, {Other}, {Task}, {RAM}, {Hessian}, {(Charges ? 1: 0)}, {(TDDFT ? 1 : 0)}, {Solvent}, {Output});"))
                {
                    id = DB.LastID;
                    return true;
                }
                return false;

            }
            return DB.Execute($"UPDATE `jobs` SET `name`='{SafeName}', `comment`='{SafeComment}', `method`={Method}, `dft`={DFT}, `basis`={Basis}, `other`={Other}, " +
                $"`task`={Task}, `ram`={RAM}, `hessian`={Hessian}, `charges`={(Charges ? 1 : 0)}, `tddft`={(TDDFT ? 1 : 0)}, `solvent`={Solvent}, `output`={Output} " +
                $"WHERE `id`={ID};");
        }

        /// <summary>
        /// Загрузка задания из БД по номеру
        /// </summary>
        /// <param name="_id">Номер задания в БД</param>
        /// <returns></returns>
        public static Job Load(SQLiteConfig DB, long id)
        {
            Job NewJob;
            using (DataTable dt = DB.ReadTable($"SELECT * FROM `jobs` WHERE `id`={id}"))
            {
                if (dt.Rows.Count == 0) return null;

                DataRow dr = dt.Rows[0];
                NewJob = new Job(DB)
                {
                    id = id,
                    name = dr["name"].ToString(),
                    comment = dr["comment"].ToString(),
                    method = (int)dr["method"],
                    dft = (int)dr["dft"],
                    basis = (int)dr["basis"],
                    other = (int)dr["other"],
                    task = (int)dr["task"],
                    ram = (int)dr["ram"],
                    hessian = (int)dr["hessian"],
                    charges = (int)dr["charges"] == 1,
                    tddft = (int)dr["tddft"] == 1,
                    solvent = (int)dr["solvent"],
                    output = (int)dr["output"]
                };
            }

            return NewJob;
        }
    }
}
