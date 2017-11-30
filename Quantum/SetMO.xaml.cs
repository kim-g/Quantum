using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для SetMO.xaml
    /// </summary>
    public partial class SetMO : Window
    {
        SQLiteDataBase Config;

        public SetMO(SQLiteDataBase ConfigDataBase)
        {
            InitializeComponent();
            Config = ConfigDataBase;
            foreach (TextBox TB in Functions.Find_All_TextBoxes(Root))
            {
                TB.Text = Config.GetConfigValue((string)TB.Tag);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Tag != null)
                Config.SetConfigValue((string)((TextBox)sender).Tag, ((TextBox)sender).Text);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            // Получение данных
            string FN;
            uint HOMO;
            uint Additional;
            uint LengthNum;

            if (File.Exists(FileName.Text)) FN = FileName.Text;
            else
            {
                MessageBox.Show("Файл «" + FileName.Text + "» не найден. Проверьте правильность пути");
                return;
            }

            try
            { HOMO = Convert.ToUInt32(N_Homo.Text); }
            catch
            { MessageBox.Show("Введите число в поле ВЗМО", "Ошибка"); return; }

            try
            { Additional = Convert.ToUInt32(N_MO.Text); }
            catch
            { MessageBox.Show("Введите число в поле «Количество доп.»", "Ошибка"); return; }

            try
            { LengthNum = Convert.ToUInt32(N_Length.Text); }
            catch
            { MessageBox.Show("Введите число в поле «Длина расчёта»", "Ошибка"); return; }

            string MO_File = File.ReadAllText(FN);
            string Length = @"min1 -@LENGTH@.0 # x-min value in bohr
    max1 @LENGTH@.0 # x-min value in bohr
	min2 -@LENGTH@.0 # y-min value in bohr
	max2 @LENGTH@.0 # y-max value in bohr
	min3 -@LENGTH@.0 # z-min value in bohr
	max3 @LENGTH@.0 # z-max value in bohr".Replace("@LENGTH@", LengthNum.ToString());

            // Поиск строчни MO  и создание всех нужных
            string[] MO_File_Array = File.ReadAllLines(FN);
            string MO_Line = "";
            foreach (string Line in MO_File_Array)
            {
                if (Line.Trim().StartsWith("MO")) MO_Line = Line.Trim();
            }

            string MO_List="";
            if (Additional == 0)
            {
                MO_List += GetMO(MO_Line, HOMO, "HOMO", 0);
                MO_List += GetMO(MO_Line, HOMO + 1, "LUMO", 0);
            }
            else
            {
                for (uint i = Additional; i>0; i--)
                {
                    MO_List += GetMO(MO_Line, HOMO - i, "HOMO", i);
                }
                MO_List += GetMO(MO_Line, HOMO, "HOMO", 0);
                MO_List += GetMO(MO_Line, HOMO+1, "LUMO", 0);
                for (uint i = 1; i <= Additional; i++)
                {
                    MO_List += GetMO(MO_Line, HOMO+1+i, "LUMO", i);
                }
            }
            MO_File = MO_File.Replace("@Length@", Length).Replace(MO_Line, MO_List);
            File.WriteAllText(FN, MO_File);
            MessageBox.Show("Редактирование прошло успешно");
        }

        string GetMO(string MO_Line, uint MO_N, string TypeMO, uint Delta)
        {
            string OutLine;
            string TypeNMO = TypeMO == "HOMO"
                ? TypeMO + "-" + Delta.ToString()
                : TypeMO + "+" + Delta.ToString();
            if (Delta == 0)
                TypeNMO = TypeMO;
            OutLine = MO_Line.Replace("@NNN@", MO_N.ToString("D3")).
                Replace("@MO_N@", MO_N.ToString()).Replace("@TYPE@", TypeNMO);
            OutLine += " # " + TypeNMO + " orbital to plot\n\t";

            return OutLine;
        }

        private void FileNameBrowse_Click(object sender, RoutedEventArgs e)
        {
            string FileToOpen = Functions.OpenFile("Orca INPUT файлы (*.inp)|*.inp");
            if (FileToOpen != null)
                FileName.Text = FileToOpen;
        }
    }
}
