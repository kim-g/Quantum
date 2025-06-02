using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text;

namespace Quantum.General
{
    /// <summary>
    /// Класс для работы с Open Babel, предоставляющий методы для конвертации файлов химических форматов.
    /// </summary>
    public class OpenBabel
    {
        /// <summary>
        /// Запускает внешнее приложение с указанными параметрами.
        /// </summary>
        /// <param name="appPath">Путь к исполняемому файлу приложения.</param>
        /// <param name="arguments">Аргументы командной строки для приложения.</param>
        /// <param name="workingDirectory">Рабочая директория для приложения.</param>
        /// <returns>Код завершения процесса. Возвращает -1 в случае ошибки.</returns>
        protected static int RunExternalApp(string appPath, string arguments = "", string workingDirectory = "")
        {
            try
            {
                // Создаем настройки запуска
                var startInfo = new ProcessStartInfo
                {
                    FileName = appPath,          // Путь к исполняемому файлу
                    Arguments = arguments,        // Аргументы командной строки
                    WorkingDirectory = workingDirectory, // Рабочая директория
                    UseShellExecute = false,      // Не использовать системную оболочку
                    CreateNoWindow = true,        // Не создавать окно для консольного приложения
                    RedirectStandardOutput = true, // Не перенаправлять вывод (можно включить при необходимости)
                    RedirectStandardError = true
                };

                // Создаем и запускаем процесс
                using (var process = Process.Start(startInfo))
                {
                    // Ожидаем завершения процесса
                    process.WaitForExit();
                    Console.WriteLine(process.StandardError.ReadToEnd());
                    Console.WriteLine(process.StandardOutput.ReadToEnd());
                    // Возвращаем код завершения
                    return process.ExitCode;
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок запуска
                Console.WriteLine($"Ошибка: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Конвертирует файл из одного формата в другой с использованием Open Babel.
        /// </summary>
        /// <param name="inputFile">Путь к входному файлу.</param>
        /// <param name="outputFile">Путь к выходному файлу.</param>
        /// <param name="Format">Формат выходного файла.</param>
        /// <returns>Код завершения процесса. Возвращает -1 в случае ошибки.</returns>
        public static int Convert(string inputFile, string outputFile, string Format, bool Gen3D=false)
        {
            // Путь к исполняемому файлу Open Babel
            string openBabelPath = @"obabel.exe";
            // Формируем аргументы для Open Babel
            string arguments = $"\"{inputFile}\" -o {Format} -O \"{outputFile}\"";
            if (Gen3D) arguments += " --gen3d best -h";
            // Запускаем Open Babel с указанными аргументами

            /*Encoding wrongEncoding = Encoding.GetEncoding(1251);
            Encoding realEncoding = Encoding.GetEncoding(866);
            byte[] originalBytes = wrongEncoding.GetBytes(arguments);
            arguments = realEncoding.GetString(originalBytes);*/
            return RunExternalApp(openBabelPath, arguments);
        }

        /// <summary>
        /// Конвертирует файл в формат PDBQT с использованием Open Babel.
        /// </summary>
        /// <param name="inputFile">Путь к входному файлу.</param>
        /// <param name="outputFile">Путь к выходному файлу.</param>
        /// <returns>Код завершения процесса. Возвращает -1 в случае ошибки.</returns>
        public static int ConvertToPDBQT(string inputFile, string outputFile)
        {
            return Convert(inputFile, outputFile, "pdbqt", true);
        }
    }
}
