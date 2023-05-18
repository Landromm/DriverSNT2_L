using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DriverSNT2_L.IniData
{
    class IniFile
    {
        // Путь файла.
        string Path;

        // С помощью конструктора записываем путь до файла и его имя.
        public IniFile(string iniPath)
        {
            Path = new FileInfo(iniPath).FullName.ToString();
        }

        /// <summary>
        /// Метод записи в .ini файл
        /// </summary>
        /// <param name="section">sring - Наименование блока данных в квадратых скобках "[ ]"</param>
        /// <param name="key">string - Название ключа данных в конкретном блоке данных</param>
        /// <param name="value">string - Записываемое значение</param>
        public void WriteINI(string section, string key, string value)
        {
            if (NativeMethods.WritePrivateProfileString(section, key, value, Path) == false)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// Метод чтения из .ini файла
        /// </summary>
        /// <param name="section">sring - Наименование блока данных в квадратых скобках "[ ]"</param>
        /// <param name="key">string - Название ключа данных в конкретном блоке данных</param>
        /// <returns>string</returns>
        public string ReadINI(string section, string key)
        {
            try
            {
                uint maxLength = 32;
                string value = new string(' ', (int)maxLength);
                NativeMethods.GetPrivateProfileString(section, key, "", value, maxLength, Path);
                return value.Split('\0')[0];
            }
            catch (Exception ex)
            {
                ConsoleColorOutputError("Ошибка чтения INI-файла!", ex);
                return string.Empty;
            }

        }

        #region ReadSections
        /// <summary>
        /// Чтение всех блоков.
        /// </summary>
        /// <returns>string[] array</returns>
        public string[] ReadSections()
        {
            try
            {
                string value = new string(' ', 65535);
                NativeMethods.GetPrivateProfileString(null, null, "\0", value, 65535, Path);
                return SplitNullTerminatedStrings(value);
            }
            catch (Exception ex)
            {
                ConsoleColorOutputError("Ошибка чтения (или расшифровки) INI-файла!", ex);
                throw;
            }
        }
        private static string[] SplitNullTerminatedStrings(string value)
        {
            string[] raw = value.Split('\0');
            int itemCount = raw.Length - 2;
            string[] items = new string[itemCount];
            Array.Copy(raw, items, itemCount);
            return items;
        }
        #endregion

        //Метод форматировванного цветного вывода текста ошибки.
        private void ConsoleColorOutputError(string str, Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"{str}\n");
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(ex.Message + "\n");
            Console.ResetColor();
        }

        //public string[] ReadKeysInSection(string section)
        //{
        //    string value = new string(' ', 65535);
        //    NativeMethods.GetPrivateProfileString(section, null, "\0", value, 65535, Path);
        //    return SplitNullTerminatedStrings(value);
        //}
    }
}
