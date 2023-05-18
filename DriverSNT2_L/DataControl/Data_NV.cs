using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSNT2_L.DataControl
{
    class Data_NV
    {
        delegate string InvertedString(string? input);

        //Поля
        private DateTime _dateTime;
        //-ОБЩИЕ--------------------------------------------------------------------------------------------
        private string? _flowVolume_Sum_Diff;           //Расход объемный сумма/разность.
        private string? _flowVolume_Sum_Diff_JunTetrad; //Младшая тетрада сумма/разность.
        private string? _volume_Sum_Diff;               //Объем сумма/разность.

        //-КАНАЛ №1------------------------------------------------------------------------------------------
        private string? _diameterRU;                    //Диаметр мерного участка (Диаметр РУ).
        private string? _baseL;                         //Расстояние между датчикаами (База).
        private string? _cutoffThreshold;               //Порог отсечки.
        private string? _parameterU;                    //Параметр "Порог".
        private string? _parameterZ;                    //Параметр "Строб".
        private string? _parameterI;                    //Параметр "Импульс зондирования".
        private string? _zeroOffset;                    //Смещение "0".
        private string? _delayTime;                     //Задержка. (Постоянная задержка в цепях).
        private string? _flowMax;                       //Расход максимальный.
        private string? _flowBoundary;                  //Расход граничный.
        private string? _totalTime;                     //Сумарное время.
        private string? _differenceTime;                //Разностное время.
        private string? _flowVolume;                    //Расход объемный.
        private string? _thermalPowerMW;                //Тепловая мощность Мвт.
        private string? _thermalPowerGcall;             //Тепловая мощность ГКалл/ч.
        private string? _flowMass;                      //Расход массовый.
        private string? _volume;                        //Объем.
        private string? _mass;                          //Масса.
        private string? _thermalEnergyGJ;               //Тепловая энергия ГДж.
        private string? _pressure;                      //Давление.
        private string? _weightPulseInput;              //Вес импульсного входа.
        private string? _lengthRU;                      //Длина РУ.
        private string? _thermalPowerGJ;                //Тепловая мощность ГДж/ч.
        private string? _flowPulseInput;                //Расход импульсного входа №1.
        private string? _volumePulseInput;              //Объем импульсного входа №1.
        private string? _viscosity;                     //Вязкость.
        private string? _thermalEnergyGCall;            //Тепловая энергия Гкал.

        //-КАНАЛ №2------------------------------------------------------------------------------------------
        private string? _thermalEnergyGJ_ch2;           //Тепловая энергия ГДж - канал №2.
        private string? _diameterRU_ch2;                //Диаметр мерного участка (Диаметр РУ) - канал №2.
        private string? _baseL_ch2;                     //Расстояние между датчикаами (База) - канал №2.
        private string? _cutoffThreshold_ch2;           //Порог отсечки - канал №2.
        private string? _parameterU_ch2;                //Параметр "Порог" - канал №2.
        private string? _parameterZ_ch2;                //Параметр "Строб" - канал №2.
        private string? _parameterI_ch2;                //Параметр "Импульс зондирования" - канал №2.
        private string? _zeroOffset_ch2;                //Смещение "0" - канал №2.
        private string? _delayTime_ch2;                 //Задержка. (Постоянная задержка в цепях) - канал №2.
        private string? _flowMax_ch2;                   //Расход максимальный - канал №2.
        private string? _flowBoundary_ch2;              //Расход граничный - канал №2.
        private string? _totalTime_ch2;                 //Сумарное время - канал №2.
        private string? _differenceTime_ch2;            //Разностное время - канал №2.
        private string? _flowVolume_ch2;                //Расход объемный - канал №2.
        private string? _thermalPowerMW_ch2;            //Тепловая мощность Мвт - канал №2.
        private string? _thermalPowerGcall_ch2;         //Тепловая мощность ГКалл/ч - канал №2.
        private string? _flowMass_ch2;                  //Расход массовый - канал №2.
        private string? _volume_ch2;                    //Объем - канал №2.
        private string? _mass_ch2;                      //Масса - канал №2.
        private string? _thermalEnergyGCall_ch2;        //Тепловая энергия Гкал - канал №2.
        private string? _pressure_ch2;                  //Давление - канал №2.
        private string? _weightPulseInput_ch2;          //Вес импульсного входа - канал №2.
        private string? _lengthRU_ch2;                  //Длина РУ - канал №2.
        private string? _thermalPowerGJ_ch2;            //Тепловая мощность ГДж/ч - канал №2.
        private string? _flowPulseInput_ch2;            //Расход импульсного входа №2 - канал №2.
        private string? _volumePulseInput_ch2;          //Объем импульсного входа №2 - канал №2.
        private string? _viscosity_ch2;                 //Вязкость - канал №2.


        //Свойста.
        public int Id { get; set; }
        public DateTime DateTimes
        {
            get => _dateTime;
            set => _dateTime = value;
        }
        //-ОБЩИЕ------------------------------------------------------------------------------------------
        [MaxLength(20)]
        public string? FlowVolume_Sum_Diff
        {
            get => _flowVolume_Sum_Diff;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    _flowVolume_Sum_Diff = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? FlowVolume_Sum_Diff_JunTetrad
        {
            get => _flowVolume_Sum_Diff_JunTetrad;
            set => _flowVolume_Sum_Diff_JunTetrad = value;
        }
        [MaxLength(20)]
        public string? Volume_Sum_Diff
        {
            get => _volume_Sum_Diff;
            set => _volume_Sum_Diff = invertedString(value);
        }

        //-КАНАЛ №1------------------------------------------------------------------------------------------
        [MaxLength(20)]
        public string? DiameterRU
        {
            get => _diameterRU;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[5]);
                    _diameterRU = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? BaseL
        {
            get => _baseL;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[5]);
                    _baseL = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? CutoffThreshold
        {
            get => _cutoffThreshold;
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
                    _cutoffThreshold = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? ParameterU
        {
            get => _parameterU;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[1]);
                    _parameterU = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? ParameterZ
        {
            get => _parameterZ;
            set => _parameterZ = invertedString(value);
        }
        [MaxLength(20)]
        public string? ParameterI
        {
            get => _parameterI;
            set => _parameterI = value;
        }
        [MaxLength(20)]
        public string? ZeroOffset
        {
            get => _zeroOffset;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append('(');
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(')');
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _zeroOffset = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? DelayTime
        {
            get => _delayTime;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _delayTime = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? FlowMax
        {
            get => _flowMax;
            set => _flowMax = invertedString(value);
        }
        [MaxLength(20)]
        public string? FlowBoundary
        {
            get => _flowBoundary;
            set => _flowBoundary = invertedString(value);
        }
        [MaxLength(20)]
        public string? TotalTime
        {
            get => _totalTime;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _totalTime = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? DifferenceTime
        {
            get => _differenceTime;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append('(');
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(')');
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _differenceTime = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? FlowVolume
        {
            get => _flowVolume;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append('(');
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(')');
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _flowVolume = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? ThermalPowerMW
        {
            get => _thermalPowerMW;
            set => _thermalPowerMW = invertedString(value);
        }
        [MaxLength(20)]
        public string? ThermalPowerGcall
        {
            get => _thermalPowerGcall;
            set => _thermalPowerGcall = invertedString(value);
        }
        [MaxLength(20)]
        public string? FlowMass
        {
            get => _flowMass;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append('(');
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(')');
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _flowMass = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? Volume
        {
            get => _volume;
            set => _volume = invertedString(value);
        }
        [MaxLength(20)]
        public string? Mass
        {
            get => _mass;
            set => _mass = invertedString(value);
        }
        [MaxLength(20)]
        public string? ThermalEnergyGJ
        {
            get => _thermalEnergyGJ;
            set => _thermalEnergyGJ = invertedString(value);
        }
        [MaxLength(20)]
        public string? ThermalEnergyGCall
        {
            get => _thermalEnergyGCall;
            set => _thermalEnergyGCall = invertedString(value);
        }
        [MaxLength(20)]
        public string? Pressure
        {
            get => _pressure;
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
                    _pressure = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? WeightPulseInput
        {
            get => _weightPulseInput;
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
                    _weightPulseInput = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? LengthRU
        {
            get => _lengthRU;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[5]);
                    _lengthRU = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? ThermalPowerGJ
        {
            get => _thermalPowerGJ;
            set => _thermalPowerGJ = invertedString(value);
        }
        [MaxLength(20)]
        public string? FlowPulseInput
        {
            get => _flowPulseInput;
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
                    _flowPulseInput = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? VolumePulseInput
        {
            get => _volumePulseInput;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _volumePulseInput = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? Viscosity
        {
            get => _viscosity;
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
                    _viscosity = strBuilder.ToString();
                }
            }
        }

        //-КАНАЛ №2------------------------------------------------------------------------------------------
        [MaxLength(20)]
        public string? DiameterRU_ch2
        {
            get => _diameterRU_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[5]);
                    _diameterRU_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? BaseL_ch2
        {
            get => _baseL_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[5]);
                    _baseL_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? CutoffThreshold_ch2
        {
            get => _cutoffThreshold_ch2;
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
                    _cutoffThreshold_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? ParameterU_ch2
        {
            get => _parameterU_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[1]);
                    _parameterU_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? ParameterZ_ch2
        {
            get => _parameterZ_ch2;
            set => _parameterZ_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? ParameterI_ch2
        {
            get => _parameterI_ch2;
            set => _parameterI_ch2 = value;
        }
        [MaxLength(20)]
        public string? ZeroOffset_ch2
        {
            get => _zeroOffset_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append('(');
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(')');
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _zeroOffset_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? DelayTime_ch2
        {
            get => _delayTime_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _delayTime_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? FlowMax_ch2
        {
            get => _flowMax_ch2;
            set => _flowMax_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? FlowBoundary_ch2
        {
            get => _flowBoundary_ch2;
            set => _flowBoundary_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? TotalTime_ch2
        {
            get => _totalTime_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _totalTime_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? DifferenceTime_ch2
        {
            get => _differenceTime_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append('(');
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(')');
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _differenceTime_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? FlowVolume_ch2
        {
            get => _flowVolume_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append('(');
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(')');
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _flowVolume_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? ThermalPowerMW_ch2
        {
            get => _thermalPowerMW_ch2;
            set => _thermalPowerMW_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? ThermalPowerGcall_ch2
        {
            get => _thermalPowerGcall_ch2;
            set => _thermalPowerGcall_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? FlowMass_ch2
        {
            get => _flowMass_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append('(');
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(')');
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _flowMass_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? Volume_ch2
        {
            get => _volume_ch2;
            set => _volume_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? Mass_ch2
        {
            get => _mass_ch2;
            set => _mass_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? ThermalEnergyGJ_ch2
        {
            get => _thermalEnergyGJ_ch2;
            set => _thermalEnergyGJ_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? ThermalEnergyGCall_ch2
        {
            get => _thermalEnergyGCall_ch2;
            set => _thermalEnergyGCall_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? Pressure_ch2
        {
            get => _pressure_ch2;
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
                    _pressure_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? WeightPulseInput_ch2
        {
            get => _weightPulseInput_ch2;
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
                    _weightPulseInput_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? LengthRU_ch2
        {
            get => _lengthRU_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[5]);
                    _lengthRU_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? ThermalPowerGJ_ch2
        {
            get => _thermalPowerGJ_ch2;
            set => _thermalPowerGJ_ch2 = invertedString(value);
        }
        [MaxLength(20)]
        public string? FlowPulseInput_ch2
        {
            get => _flowPulseInput_ch2;
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
                    _flowPulseInput_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? VolumePulseInput_ch2
        {
            get => _volumePulseInput_ch2;
            set
            {
                string tempStr = invertedString(value);
                StringBuilder strBuilder = new StringBuilder();
                if (tempStr != null)
                {
                    strBuilder.Append(tempStr[0]);
                    strBuilder.Append(tempStr[1]);
                    strBuilder.Append(tempStr[2]);
                    strBuilder.Append(tempStr[3]);
                    strBuilder.Append(tempStr[4]);
                    strBuilder.Append(",");
                    strBuilder.Append(tempStr[5]);
                    strBuilder.Append(tempStr[6]);
                    strBuilder.Append(tempStr[7]);
                    _volumePulseInput_ch2 = strBuilder.ToString();
                }
            }
        }
        [MaxLength(20)]
        public string? Viscosity_ch2
        {
            get => _viscosity_ch2;
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
                    _viscosity_ch2 = strBuilder.ToString();
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
