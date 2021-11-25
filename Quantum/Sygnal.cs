using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        #endregion

        #region Конструкторы
        public Sygnal (Node parent)
        {
            Parent = parent;
        }

        #endregion

        #region Методы

        #endregion
    }
}
