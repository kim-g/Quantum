using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Quantum
{
    public class EnergiesDB : SQLiteDataBase
    {
        const string ElementsTable = "CREATE TABLE \"elements\" (\"id\"	INTEGER NOT NULL, \"name\"	TEXT, \"homo\"	INTEGER, \"lumo\"	INTEGER, \"order\"	INTEGER, PRIMARY KEY(\"id\" AUTOINCREMENT)); ";
        const string MOTable = "CREATE TABLE \"mo\" (\"id\"	INTEGER NOT NULL, \"energy\"	REAL, \"image\"	BLOB, PRIMARY KEY(\"id\" AUTOINCREMENT)); ";
        
        public EnergiesDB(string FileName) : base(FileName)
        {
            dbFileName = FileName;
            Connection = new SQLiteConnection();
            Command = new SQLiteCommand();
        }


        public new static EnergiesDB Open(string FileName)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    EnergiesDB NewConf = new EnergiesDB(FileName);

                    if (NewConf.OpenDB())
                        return NewConf;
                    else
                        return null;
                }
                else
                {
                    EnergiesDB NewConf = new EnergiesDB(FileName);

                    if (NewConf.CreateDB(ElementsTable + MOTable))
                        return NewConf;
                    else
                        return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Создаёт новую молекулярную орбиталь или перезаписывает старую
        /// </summary>
        /// <param name="MO"></param>
        public void SaveMO(EnergyLevel MO)
        {
            string Energy = MO.EnergyString(5).Replace(',', '.');


            string TextQuery = "";
            if (MO.ID > 0)
                TextQuery = $"UPDATE `mo` SET `energy`={Energy}, `image`=@BLOB WHERE `id`={MO.ID};";
            else
                TextQuery = $"INSERT INTO `mo` (`energy`, `image`) VALUES ({Energy}, @BLOB);";


            BitmapSource bmp = (BitmapSource)(MO.Picture);

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bmp));
                enc.Save(outStream);
                if (ExecuteBLOB(TextQuery, outStream))
                {
                    if (MO.ID == 0) MO.ID = LastID;
                }
            }
        }

        /// <summary>
        /// Создаёт новую запись об электронной орбитали
        /// </summary>
        /// <param name="EE"></param>
        /// <returns></returns>
        public void SaveMolecule(EnergyElement EE)
        {
            SaveMO(EE.HOMO);
            SaveMO(EE.LUMO);

            string TextQuery = "";
            if (EE.ID > 0)
                TextQuery = $"UPDATE `elements` SET `name`='{EE.Name}', `homo`={EE.HOMO.ID}, `lumo`={EE.LUMO.ID} WHERE `id`={EE.ID};";
            else
                TextQuery = $"INSERT INTO `elements` (`name`, `homo`, `lumo`) VALUES ('{EE.Name}', {EE.HOMO.ID}, {EE.LUMO.ID});";

            if (Execute(TextQuery))
                EE.ID = EE.ID > 0 ? EE.ID : LastID;
        }


        public EnergyLevel LoadMO(long ID)
        {
            EnergyLevel EL;

            using (DataTable dt = ReadTable($"SELECT * FROM `mo` WHERE `id`={ID}"))
            {
                if (dt.Rows.Count == 0) return null;

                EL = new EnergyLevel()
                {
                    ID = ID,
                    Energy = Convert.ToDouble(dt.Rows[0]["energy"])
                };
                using (MemoryStream inStream = new MemoryStream(dt.Rows[0]["image"] as byte[]))
                {
                    Image Img = Image.FromStream(inStream);
                    System.Drawing.Bitmap bitmap = new Bitmap(Img);
                    EL.Picture = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, 
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }

            return EL;
        }

        /// <summary>
        /// Загрузка EnergyElement из базы данных по ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public EnergyElement LoadMolecule(long ID)
        {
            EnergyElement EE;
            using (DataTable dt = ReadTable($"SELECT * FROM `elements` WHERE `id`={ID}"))
            {
                if (dt.Rows.Count == 0) return null;

                EE = new EnergyElement()
                {
                    ID = ID,
                    Name = dt.Rows[0]["name"].ToString()
                };

                EE.HOMO = LoadMO(Convert.ToInt64(dt.Rows[0]["homo"]));
                EE.LUMO = LoadMO(Convert.ToInt64(dt.Rows[0]["lumo"]));
            }

            EE.ID = ID;
            EE.Stable();
            return EE;
        }
    }
}
