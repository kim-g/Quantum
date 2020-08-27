using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Quantum
{
    public class EnergyElement
    {
        #region Внутренние переменные
        string name;
        EnergyLevel homo = new EnergyLevel();
        EnergyLevel lumo = new EnergyLevel();
        #endregion

        #region Свойства
        /// <summary>
        /// Название элемента
        /// </summary>
        public string Name { get => name; set => name = value; }

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

        public string GapString (byte Decimal)
        {
            return Gap.ToString($"F{Decimal}");
        }

        #endregion
    }

    public class EnergyLevel
    {
        
        public EnergyLevel()
        {
            Picture = new BitmapImage();
        }

        public double Energy { get; set; } = 0;

        public string EnergyString(byte Decimal)
        {
            return Energy.ToString($"F{Decimal}");
        }

        public BitmapImage Picture { get; set; }

        public void PictureFromFile (string FileName)
        {
            Picture.BeginInit();
            Picture.UriSource = new Uri(FileName, UriKind.Absolute);
            Picture.EndInit();
        }
    }
}
