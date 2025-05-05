using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Quantum.AutoDockVina
{
    /// <summary>
    /// Представляет лиганд с его свойствами и уведомлениями об изменении.
    /// </summary>
    public class Ligand : INotifyPropertyChanged
    {
        private string name;
        private string fileName;

        /// <summary>
        /// Получает или задает имя лиганда.
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
        /// Получает или задает имя файла лиганда.
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
        /// Получает имя файла лиганда с расширением PDBQT.
        /// </summary>
        public string NamePDBQT
        {
            get
            {
                return $"{NameWithoutExtension}.pdbqt";
            }
        }

        /// <summary>
        /// Получает имя лиганда без разширений файла
        /// </summary>
        public string NameWithoutExtension
        {
            get
            {
                if (string.IsNullOrEmpty(fileName))
                    return string.Empty;
                return Path.GetFileNameWithoutExtension(fileName);
            }
        }

        public Ligand(string fileName)
        {
            FileName = fileName;
            Name = Path.GetFileName(fileName);
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
