using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Quantum.AutoDockVina
{
    /// <summary>
    /// Модель представления для управления списком белков и выбранным белком.
    /// </summary>
    internal class CreatorModel : INotifyPropertyChanged
    {
        private Protein selectedProtein;

        /// <summary>
        /// Коллекция белков, доступных для выбора.
        /// </summary>
        public ObservableCollection<Protein> proteinList { get; set; }

        /// <summary>
        /// Выбранный белок.
        /// </summary>
        public Protein SelectedProtein
        {
            get => selectedProtein;
            set
            {
                selectedProtein = value;
                OnPropertyChanged(nameof(SelectedProtein));
            }
        }

        /// <summary>
        /// Конструктор по умолчанию. Инициализирует коллекцию белков.
        /// </summary>
        public CreatorModel()
        {
            proteinList = new ObservableCollection<Protein>();
        }

        /// <summary>
        /// Событие, возникающее при изменении свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывает событие <see cref="PropertyChanged"/> для указанного свойства.
        /// </summary>
        /// <param name="prop">Имя измененного свойства. Указывается автоматически.</param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

