using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public class Job
    {
        private int id;
        private string name;
        private string comment;
        private int method;
        private int dft;
        private int basis;
        
        
        
        /// <summary>
        /// Номер в БД
        /// </summary>
        public int ID { get => id; private set => id = value; }

        /// <summary>
        /// Имя задания
        /// </summary>
        public string Name { get => name; set { name = value; Update(); } }

        /// <summary>
        /// Комментарий к заданию
        /// </summary>
        public string Comment { get => comment; set { comment = value; Update(); } }

        /// <summary>
        /// ID метода
        /// </summary>
        public int Method { get => method; set { method = value; Update(); } }

        /// <summary>
        /// ID DFT
        /// </summary>
        public int DFT { get => dft; set { dft = value; Update(); } }

        /// <summary>
        /// ID базиса
        /// </summary>
        public int Basis { get => basis; set { basis = value; Update(); } }

        public bool Update()
        {

            return true;
        }
    }
}
