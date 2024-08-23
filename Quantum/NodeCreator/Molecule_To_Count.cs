using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenBabel;

namespace Quantum
{
    /// <summary>
    /// Молекула для расчётов. Храним название для поддиректории и саму молекулу в формате OpenBabel
    /// </summary>
    public class Molecule_To_Count
    {
        protected string _name = "";
        protected OBMol _molecule;
        protected ImageSource _image = null;
        protected static Random _random = new Random();
        
        /// <summary>
        /// Название поддиректории молекулы
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                // Проверим корректность названия файла
                string NewName = value;
                char[] banned = Path.GetInvalidFileNameChars();
                foreach (char c in banned)
                    NewName = NewName.Replace(c, '_');
                _name = NewName;
            }
        }

        /// <summary>
        /// Молекула в формате OpenBabel
        /// </summary>
        public OBMol Molecule
        { 
            get => _molecule;
            set
            {
                _molecule = value;
                _image = null;
            }
            
        }

        /// <summary>
        /// Получить изображение молекулы
        /// </summary>
        public ImageSource Image
        {
            get
            {
                if (_image == null)
                {
                    OBConversion convert = new OBConversion();
                    convert.SetOutFormat("_png2");
                    string FileName = Path.Combine(Path.GetTempPath(),"quantum_molecule_" + (0xFF000000 + _random.Next(0xFFFFFF)).ToString() + ".png");
                    convert.WriteFile(Molecule, FileName);
                    convert.CloseOutFile();
                    _image = new BitmapImage(new Uri(FileName));
                    try
                    {
                        File.Delete(FileName);
                    }
                    catch { }
                }
                return _image;
            }
        }

        /// <summary>
        /// Загрузка молекулы из текста формата SMILES
        /// </summary>
        /// <param name="text">Данные</param>
        /// <returns></returns>
        public bool LoadFromSMILES(string text)
        {
            return LoadFromText("smiles", text);
        }

        /// <summary>
        /// Загрузка молекулы из текста
        /// </summary>
        /// <param name="format">Формат</param>
        /// <param name="text">Данные</param>
        /// <returns></returns>
        public bool LoadFromText(string format, string text)
        {
            OBConversion convert = new OBConversion();
            convert.SetInFormat(format);
            if (Molecule == null) Molecule = new OBMol();
            if (convert.ReadString(Molecule, text))
            {
                Molecule.AddHydrogens();
                OBBuilder builder = new OBBuilder();
                return builder.Build(Molecule);
            }
            return false;
        }

        /// <summary>
        /// Загрузка молекулы из файла
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public bool LoadFromFile(string FileName)
        {
            OBConversion convert = new OBConversion();
            OBFormat OBF = OBConversion.FormatFromExt(FileName);

            if (OBF == null)
            {
                MessageBox.Show("Неподдерживаемый формат данных", "Ошибка открытия файла");
                return false;
            }
            convert.SetInFormat(OBF);
            if (convert.ReadFile(Molecule, FileName))
            {
                Name = Path.GetFileNameWithoutExtension(FileName);
                Molecule.AddHydrogens();
                OBBuilder builder = new OBBuilder();
                return builder.Build(Molecule);
            }
            
            return false;
        }

        /// <summary>
        /// Сохранить молекулу в файл XYZ
        /// </summary>
        /// <param name="FileName">Полное имя файла</param>
        /// <returns></returns>
        public bool SaveXYZ(string FileName)
        {
            return SaveFile("xyz", FileName);
        }

        /// <summary>
        /// Сохранить молекулу в файл XYZ
        /// </summary>
        /// <param name="FileName">Полное имя файла</param>
        /// <returns></returns>
        public bool SaveMOL(string FileName)
        {
            return SaveFile("mol", FileName);
        }

        /// <summary>
        /// Сохранить молекулу в указанном формате
        /// </summary>
        /// <param name="format">Формат</param>
        /// <param name="FileName">Полное имя файла</param>
        /// <returns></returns>
        public bool SaveFile(string format, string FileName)
        {
            OBConversion convert = new OBConversion();
            convert.SetOutFormat(format);
            return convert.WriteFile(Molecule, FileName);
        }
        
    }
}
