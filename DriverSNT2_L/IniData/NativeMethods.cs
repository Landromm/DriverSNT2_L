using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DriverSNT2_L.IniData
{
    class NativeMethods
    {
        // Подключение системных библиотек для чтения\записи ini файла.
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, string lpReturnedString, uint nSize, string lpFileName);

    }
}
