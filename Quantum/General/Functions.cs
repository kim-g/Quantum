using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Quantum
{
    /// <summary>
    /// Содержит вспомогательные функции для работы с элементами WPF, файлами и системой.
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// Рекурсивно находит все элементы <see cref="TextBox"/> в указанном <see cref="Grid"/> и его дочерних гридах.
        /// </summary>
        /// <param name="RootGrid">Корневой <see cref="Grid"/>, в котором производится поиск.</param>
        /// <returns>Список всех найденных <see cref="TextBox"/>.</returns>
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

        /// <summary>
        /// Парсит строку в число с плавающей точкой с учетом локального разделителя.
        /// </summary>
        /// <param name="Num">Строка для преобразования.</param>
        /// <returns>Число типа <see cref="float"/>.</returns>
        public static float Parse(string Num)
        {
            char separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            Num = Num.Trim().Replace('.', separator).Replace(',', separator);
            return float.Parse(Num);
        }

        /// <summary>
        /// Открывает диалоговое окно выбора файла.
        /// </summary>
        /// <param name="FileType">Фильтр расширений файлов (по умолчанию "Все файлы (*.*)|*.*").</param>
        /// <returns>Путь к выбранному файлу или <c>null</c>, если выбор отменён.</returns>
        public static string OpenFile(string FileType = "Все файлы (*.*)|*.*")
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = FileType;
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;

            return myDialog.ShowDialog()==true ? myDialog.FileName : null;
        }

        /// <summary>
        /// Открывает проводник Windows в указанной директории.
        /// </summary>
        /// <param name="directoryPath">Путь к целевой директории.</param>
        /// <param name="throwExceptions">Бросать исключения при ошибках (по умолчанию <c>false</c>).</param>
        /// <returns><c>true</c>, если операция выполнена успешно; иначе <c>false</c>.</returns>
        public static bool OpenExplorerWindow(string directoryPath, bool throwExceptions = false)
        {
            try
            {
                // Нормализация пути
                var path = System.IO.Path.GetFullPath(directoryPath);

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
