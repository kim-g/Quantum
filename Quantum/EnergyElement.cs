using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Quantum
{
    public class EnergyElement
    {
        #region Внутренние переменные
        string name;
        EnergyLevel homo = new EnergyLevel();
        EnergyLevel lumo = new EnergyLevel();
        bool modifyed = false;
        private long iD = 0;
        #endregion

        #region Свойства
        public long ID 
        { 
            get => iD;
            set
            {
                iD = value;
                modifyed = true;
            }
        }

        /// <summary>
        /// Название элемента
        /// </summary>
        public string Name 
        { 
            get => name;
            set
            {
                name = value;
                modifyed = true;
            }
        }

        /// <summary>
        /// Highest Occupied Molecule Orbital
        /// </summary>
        public EnergyLevel HOMO { get => homo; set => homo = value; }

        /// <summary>
        /// Lowest Unoccupied Molecule Orbital
        /// </summary>
        public EnergyLevel LUMO { get => lumo; set => lumo = value; }

        /// <summary>
        /// Энергетическая щель
        /// </summary>
        public double Gap { get => LUMO.Energy - HOMO.Energy; }

        public string GapString(byte Decimal)
        {
            return Gap.ToString($"F{Decimal}");
        }

        public bool Modifyed 
        {
            get => modifyed || HOMO.Modifyed || LUMO.Modifyed;
        }

        #endregion

        public void Stable()
        {
            modifyed = false;
            HOMO.Modifyed = false;
            LUMO.Modifyed = false;
        }
    }

    public class EnergyLevel
    {
        private long iD = 0;
        private double energy = 0;
        private ImageSource picture;

        public EnergyLevel()
        {
            Picture = new BitmapImage();
        }

        public long ID
        {
            get => iD;
            set
            {
                iD = value;
                Modifyed = true;
            }
        }
        public double Energy
        {
            get => energy;
            set
            {
                energy = value;
                Modifyed = true;
            }
        }
        public string EnergyString(byte Decimal)
        {
            return Energy.ToString($"F{Decimal}");
        }

        public ImageSource Picture 
        { 
            get => picture;
            set
            {
                picture = value;
                Modifyed = true;
            }
        }

        public void PictureFromFile(string FileName)
        {
            BitmapImage BI = new BitmapImage(new Uri(FileName, UriKind.Absolute));
            Picture = BI;
            Modifyed = true;
        }

        public bool Modifyed { get; set; } = false;
    }
}
