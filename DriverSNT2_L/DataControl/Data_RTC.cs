using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSNT2_L.DataControl
{
    class Data_RTC
    {
        delegate string InvertedString(string input);

        private DateTime _dateTime;
        private string? _timeAndDate;                    //Текущее время и дата.
        private string? _serialNumber;                   //Заводской номер.
        private string? _accessCode;                     //Код доступа.
        private string? _configurator;                   //Конфигуратор.
        private string? _interface;                      //Интерфейс.
        private string? _levelSignalU1Channel1;          //Уровень сингнала U1 - Канала №1.          
        private string? _levelSignalU2Channel1;          //Уровень сингнала U2 - Канала №1.          
        private string? _levelSignalU3Channel1;          //Уровень сингнала U3 - Канала №1.
        private string? _levelSignalU1Channel2;          //Уровень сингнала U1 - Канала №2.          
        private string? _levelSignalU2Channel2;          //Уровень сингнала U2 - Канала №2.          
        private string? _levelSignalU3Channel2;          //Уровень сингнала U3 - Канала №2.
        private string? _instraction;                    //Инструкция.
        private string? _errorChannel1;                  //Ошибки канала №1.
        private string? _errorChannel2;                  //Ошибки канала №2.
        private string? _errorSystem;                    //Ошибки системы.
        private string? _temperatureChanel1;             //Температура канал №1.
        private string? _temperatureChanel2;             //Температура канал №2.
        private string? _temperature_T3;                 //Температура Т3.
        private string? _temperature_T4_T5;              //Температура Т4/T5.
        private string? _commissioning;                  //Ввод в эксплуатацию.
        private string? _lowDifferenceTemp;              //Малая разность температур.
        private string? _pressure_P3;                    //Давление Р3.
        private string? _pressure_P4_P5;                 //Давление Р4/P5.
        private string? _downtime;                       //Время простоя.
        private string? _runningTime;                    //Время наработки.
        private string? _irregularFlowTime;              //Ненармированный расход (время).
        private string? _excessFlowTime;                 //Превышение расхода (время).
        private string? _noFlowTime;                     //Отсутствие расхода (время).
        private string? _negativeFlowTime;               //Отрицательный расход (время).
        private string? _defectTime;                     //Неисправность (время).
        private string? _noPowerTime;                    //Отсутствие питания (время).
        private string? _maxSensorsPressure;             //Максимум датчиков давления.

        public int Id { get; set; }
        public DateTime DateTimes
        {
            get => _dateTime;
            set => _dateTime = value;
        }
        [MaxLength(20)]
        public string? TimeAndDate
        {
            get => _timeAndDate;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append("-");
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append("-");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(" ");
                    strBuilder.Append(tempStr[10]);
                    strBuilder.Append(tempStr[11]);
                    strBuilder.Append(":");
                    strBuilder.Append(tempStr[14]);
                    strBuilder.Append(tempStr[15]);
                    strBuilder.Append(":");
                    strBuilder.Append(tempStr[18]);
                    strBuilder.Append(tempStr[19]);

                    _timeAndDate = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? SerialNumber
        {
            get => _serialNumber;
            set => _serialNumber = invertedString(value);
        }
        [MaxLength(20)]
        public string? AccessCode
        {
            get => _accessCode;
            set => _accessCode = invertedString(value);
        }
        [MaxLength(20)]
        public string? Configurator
        {
            get => _configurator;
            set => _configurator = invertedString(value);
        }
        [MaxLength(20)]
        public string? Interface
        {
            get => _interface;
            set => _interface = invertedString(value);
        }
        [MaxLength(20)]
        public string? LevelSignalU1Channel1
        {
            get => _levelSignalU1Channel1;
            set => _levelSignalU1Channel1 = value;
        }
        [MaxLength(20)]
        public string? LevelSignalU2Channel1
        {
            get => _levelSignalU2Channel1;
            set => _levelSignalU2Channel1 = value;
        }
        [MaxLength(20)]
        public string? LevelSignalU3Channel1
        {
            get => _levelSignalU3Channel1;
            set => _levelSignalU3Channel1 = value;
        }
        [MaxLength(20)]
        public string? LevelSignalU1Channel2
        {
            get => _levelSignalU1Channel2;
            set => _levelSignalU1Channel2 = value;
        }
        [MaxLength(20)]
        public string? LevelSignalU2Channel2
        {
            get => _levelSignalU2Channel2;
            set => _levelSignalU2Channel2 = value;
        }
        [MaxLength(20)]
        public string? LevelSignalU3Channel2
        {
            get => _levelSignalU3Channel2;
            set => _levelSignalU3Channel2 = value;
        }
        [MaxLength(20)]
        public string? Instraction
        {
            get => _instraction;
            set => _instraction = value;
        }
        [MaxLength(20)]
        public string? ErrorChannel1
        {
            get => _errorChannel1;
            set => _errorChannel1 = value;
        }
        [MaxLength(20)]
        public string? ErrorChannel2
        {
            get => _errorChannel2;
            set => _errorChannel2 = value;
        }
        [MaxLength(20)]
        public string? ErrorSystem
        {
            get => _errorSystem;
            set => _errorSystem = value;
        }
        [MaxLength(20)]
        public string? TemperatureChanel1
        {
            get => _temperatureChanel1;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    _temperatureChanel1 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? TemperatureChanel2
        {
            get => _temperatureChanel2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    _temperatureChanel2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? Temperature_T3
        {
            get => _temperature_T3;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    _temperature_T3 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? Temperature_T4_T5
        {
            get => _temperature_T4_T5;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    _temperature_T4_T5 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? Commissioning
        {
            get => _commissioning;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append("-");
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append("-");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(":");
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _commissioning = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? LowDifferenceTemp
        {
            get => _lowDifferenceTemp;
            set => _lowDifferenceTemp = invertedString(value);
        }
        [MaxLength(20)]
        public string? Pressure_P3
        {
            get => _pressure_P3;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);

                    _pressure_P3 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? Pressure_P4_P5
        {
            get => _pressure_P4_P5;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);

                    _pressure_P4_P5 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? Downtime
        {
            get => _downtime;
            set => _downtime = invertedString(value);
        }
        [MaxLength(20)]
        public string? RunningTime
        {
            get => _runningTime;
            set => _runningTime = invertedString(value);
        }
        [MaxLength(20)]
        public string? IrregularFlowTime
        {
            get => _irregularFlowTime;
            set => _irregularFlowTime = invertedString(value);
        }
        [MaxLength(20)]
        public string? ExcessFlowTime
        {
            get => _excessFlowTime;
            set => _excessFlowTime = invertedString(value);
        }
        [MaxLength(20)]
        public string? NoFlowTime
        {
            get => _noFlowTime;
            set => _noFlowTime = invertedString(value);
        }
        [MaxLength(20)]
        public string? NegativeFlowTime
        {
            get => _negativeFlowTime;
            set => _negativeFlowTime = invertedString(value);
        }
        [MaxLength(20)]
        public string? DefectTime
        {
            get => _defectTime;
            set => _defectTime = invertedString(value);
        }
        [MaxLength(20)]
        public string? NoPowerTime
        {
            get => _noPowerTime;
            set => _noPowerTime = invertedString(value);
        }
        [MaxLength(20)]
        public string? MaxSensorsPressure
        {
            get => _maxSensorsPressure;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[1]);
                    _maxSensorsPressure = strBuilder.ToString();
                }
            }
        }

        InvertedString invertedString = inputStr =>
        {
            string tempstr = "";
            for (int i = 0; i < inputStr.Length; i += 2)
            {
                tempstr = tempstr.Insert(0, inputStr.Substring(i, 2));
            }
            return tempstr;
        };
    }
}
