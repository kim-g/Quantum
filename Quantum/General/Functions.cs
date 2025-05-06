using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Quantum
{
    public class Functions
    {
       public static List<TextBox> Find_All_TextBoxes(Grid RootGrid)
        {
            List<TextBox> List = new List<TextBox>();

            // Ищем всё в корне
            foreach (TextBox TB in RootGrid.Children.OfType<TextBox>())
            {
                List.Add(TB);
            }

            // Ищем всё в дочерних гридах
            foreach (Grid ChildrenGrid in RootGrid.Children.OfType<Grid>())
            {
                List.AddRange(Find_All_TextBoxes(ChildrenGrid));
            }

            return List;
        }

        public static string OpenFile(string FileType = "Все файлы (*.*)|*.*")
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = FileType;
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;

            return myDialog.ShowDialog()==true ? myDialog.FileName : null;
        }

        /// <summary>
        /// Открывает проводник Windows в указанной директории
        /// </summary>
        /// <param name="directoryPath">Путь к целевой директории</param>
        /// <param name="throwExceptions">Бросать исключения при ошибках (по умолчанию false)</param>
        /// <returns>True если операция выполнена успешно</returns>
        public static bool OpenExplorerWindow(string directoryPath, bool throwExceptions = false)
        {
            try
            {
                // Нормализация пути
                var path = Path.GetFullPath(directoryPath);

                // Проверка существования директории
                if (!Directory.Exists(path))
                {
                    if (throwExceptions)
                    {
                        throw new DirectoryNotFoundException($"Директория не найдена: {path}");
                    }
                    return false;
                }

                // Создание параметров запуска
                var startInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{path}\"",
                    UseShellExecute = true
                };

                // Запуск процесса
                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException ||
                                      ex is ArgumentException ||
                                      ex is IOException)
            {
                if (throwExceptions)
                {
                    throw;
                }

                Debug.WriteLine($"Ошибка открытия проводника: {ex.Message}");
                return false;
            }
        }
    }
}
