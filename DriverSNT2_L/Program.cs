using DriverSNT2_L.ComPort;
using DriverSNT2_L.Context;
using DriverSNT2_L.DataControl;
using DriverSNT2_L.IniData;
using DriverSNT2_L.IniData.Logger;
using DriverSNT2_L.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DriverSNT2_L
{
    internal class Program
    {
        static int timeoutSend = 250;
        static int timeoutRead = 1000;
        static int okResultProcedure = 0;
        static int errorCount = 0;
        static int limitErrorCom = 0;
        //--------------------------
        static string? temp_PortName;
        static string? temp_BaudRate;
        static string? temp_Parity;
        static string? temp_StopBits;
        static string? temp_DataBits;
        //--------------------------
        static bool checkSumCRC = false;
        static string tempStr = "";
        //--------------------------
        static CommunicationManager comm = new CommunicationManager();
        static SendMessage sendMsg = new SendMessage();
        static LogWriter logWriter = new LogWriter();
        static ProjectObject? projectObject = new ProjectObject();
        static Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
        

        static void Main(string[] args)
        {
            
            InitializationDB(sendMsg.CountNumberCounter);

            #region Цикл опроса счетчика.
            for (int i = 0; i < sendMsg.CountNumberCounter; i++)
            {
                Console.WriteLine("Отправляемые пакеты данных для счетчика #{0}:", sendMsg.NumbersCounters[i]);
                Console.WriteLine(sendMsg.SendStartSessionHex[i] + " - сообщение инициализации обмена.");
                Console.WriteLine(sendMsg.SendWritePage128Hex[i] + " - сообщение записи страницы 128 байт данных.");
                Console.WriteLine(sendMsg.SendWritePage256Hex[i] + " - сообщение записи страницы 256 байт данных.");
                Console.WriteLine(sendMsg.SendReadDataHex[i] + " - сообщение чтения данных со страницы.");
                Console.WriteLine("\n");
            }

            int countReadData = 0;
            ParamFromConfiguration_Load();
            OpenComPort();

            while (true)
            {
                for (int i = 0; i < sendMsg.CountNumberCounter; i++)
                {
                    countReadData = 0;
                    do
                    {
                        try
                        {
                            comm.WriteData(sendMsg.SendStartSessionHex[i]);      //Инициализации обмена данными со счетчиком.
                            Thread.Sleep(timeoutSend);
                            WriteDataRTC(sendMsg.SendWritePage128Hex[i], sendMsg.SendReadDataHex[i]);

                            Console.WriteLine($"\nЧтение данных RTC со счетчика №{i+1} - Зав.№ {sendMsg.NumbersCounters[i]}".ToUpper());

                            if (comm.DataByteList.Count != 0)
                                checkSumCRC = CheckSumCRC(comm.DataByteList);

                            if (!checkSumCRC)
                            {
                                countReadData++;
                                OutputConsole_Error($"Повторный запрос на чтение данных со счетчика №{i+1}");
                                //logWriter.WriteError($"Повторный запрос на чтение данных со счетчика №{i}");
                                //Console.WriteLine($"Повторный запрос на чтение данных со счетчика №{i}");
                            }
                            else
                            {   //Начало опроса.
                                countReadData = 0;
                                FillingAnObject_RTC(i+1);                                
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            errorCount++;
                            continue;
                        }
                    }
                    while (!checkSumCRC && countReadData < 3);
                    
                    countReadData = 0;
                    do
                    {
                        try
                        {
                            comm.WriteData(sendMsg.SendStartSessionHex[i]);      //Инициализации обмена данными со счетчиком.
                            Thread.Sleep(timeoutSend);
                            WriteDataNV(sendMsg.SendWritePage256Hex[i], sendMsg.SendReadDataHex[i]);

                            Console.WriteLine($"\nЧтение данных NV со счетчика №{i+1} - Зав.№ {sendMsg.NumbersCounters[i]}".ToUpper());

                            if (comm.DataByteList.Count != 0)
                                checkSumCRC = CheckSumCRC(comm.DataByteList);

                            if (!checkSumCRC)
                            {
                                countReadData++;
                                OutputConsole_Error($"Повторный запрос на чтение данных со счетчика №{i+1}");
                                //logWriter.WriteError($"Повторный запрос на чтение данных со счетчика №{i}");
                                //Console.WriteLine($"Повторный запрос на чтение данных со счетчика №{i}");
                            }
                            else
                            {   //Начало опроса.
                                countReadData = 0;
                                FillingAnObject_NV(i+1);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            continue;
                        }
                    }
                    while (!checkSumCRC && countReadData < 3);
                }
            }
            #endregion
        }

        //Метод чтения параметров COM-порт из ini. файла
        static void ParamFromConfiguration_Load()
        {
            try
            {
                IniFile INI = new IniFile(@ConfigurationManager.AppSettings["pathConfig"]);
                temp_PortName = INI.ReadINI("COMportSettings", "PortName");
                temp_BaudRate = INI.ReadINI("COMportSettings", "BaudRate");
                temp_Parity = INI.ReadINI("COMportSettings", "Parity");
                temp_StopBits = INI.ReadINI("COMportSettings", "StopBits");
                temp_DataBits = INI.ReadINI("COMportSettings", "DataBits");
                timeoutRead = Convert.ToInt32(INI.ReadINI("COMportSettings", "Timeout"));
                limitErrorCom = Convert.ToInt32(INI.ReadINI("SNTConfig", "LimitErrorCom"));

                logWriter.LoadFlagLog();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка чтения config.ini файла!\n" + ex);
            }
        }

        //Метод открытия COM-порта.
        static public void OpenComPort()
        {
            comm.Parity = temp_Parity;
            comm.StopBits = temp_StopBits;
            comm.DataBits = temp_DataBits;
            comm.BaudRate = temp_BaudRate;
            comm.PortName = temp_PortName;
            comm.OpenPort();
            if (comm.ComPortIsOpen())
            {
                string info = "Параметры COM-порта:\n" +
                     "Четность: " + temp_Parity + "\n" +
                     "Стоповые биты: " + temp_StopBits + "\n" +
                     "Биты данных: " + temp_DataBits + "\n" +
                     "Бит в секунду: " + temp_BaudRate + "\n" +
                     "Таймаут: " + timeoutRead + "\n";

                Console.WriteLine(info);
                logWriter.WriteInformation(info);
            }
        }

        //Метод опроса счетчика - пакет данных 128 байт - RTC
        static void WriteDataRTC(string writePageMsg, string readDataMsg)
        {
            comm.WriteData(writePageMsg);  //Записи данных на страницу '0' в счетчике (128 байт).

            Thread.Sleep(timeoutSend);

            comm.Count = 0;
            comm.DataByteList.Clear();
            comm.WriteData(readDataMsg);   //Чтения данных со счетчика.            
            Console.WriteLine("---------------------------------------------");

            Thread.Sleep(timeoutRead);
        }

        //Метод опроса счетчика - пакет данных 256 байт - RTC
        static void WriteDataNV(string writePageMsg, string readDataMsg)
        {
            comm.WriteData(writePageMsg);  //Записи данных на страницу '0' в счетчике (128 байт).

            Thread.Sleep(timeoutSend);

            comm.Count = 0;
            comm.DataByteList.Clear();
            comm.WriteData(readDataMsg);   //Чтения данных со счетчика.            
            Console.WriteLine("---------------------------------------------");

            Thread.Sleep(timeoutRead);
        }

        //Метод подсчета CRC - пакета данных 128 байт.
        static bool CheckSumCRC(List<string> dataList)
        {
            int resultCRC = 0x00;
            for (int i = 0; i < dataList.Count - 1; i++)
            {
                resultCRC ^= Convert.ToInt32(dataList[i], 16);
            }
            resultCRC ^= 0xA5;

            //Console.WriteLine("Контрольная сумма результат:\t" + resultCRC);
            //Console.WriteLine("Контрольная сумма пакета данных:" + Convert.ToInt32(dataList.Last(), 16));

            if (resultCRC == Convert.ToInt32(dataList.Last(), 16))
            {
                OutputConsole_OK("Чтение данных - OK.");
                //Console.BackgroundColor = ConsoleColor.Green;
                //Console.ForegroundColor = ConsoleColor.Black;
                //Console.Write("Чтение данных - OK.");
            }
            else
            {
                string info = "Контрольная сумма не совпадает!";
                OutputConsole_Error(info);
                //logWriter.WriteError(info);
                //Console.BackgroundColor = ConsoleColor.Red;
                //Console.ForegroundColor = ConsoleColor.Black;
                //Console.Write("Контрольная сумма не совпадает!");
            }

            Console.ResetColor();
            return resultCRC == Convert.ToInt32(dataList.Last(), 16) ? true : false;
        }

        //Метод выборки байт из массива и преобразования их в строку по индексам.
        static string FormatData(Range range)
        {
            tempStr = string.Empty; ;
            string[] buffArray = new string[comm.DataByteList.Count];
            int count = 0;
            foreach (var item in comm.DataByteList)
            {
                buffArray[count++] = item.ToString();
            }
            foreach (var item in buffArray[range])
            {
                tempStr += item;
            }
            return tempStr;
        }

        //Метод заполнения объекта данных 128 байт.
        static void FillingAnObject_RTC(int indexCount)
        {
            Data_RTC data_RTC = new Data_RTC();

            try
            {
                data_RTC.DateTimes = DateTime.Now;
                data_RTC.TimeAndDate = FormatData(2..12);
                data_RTC.SerialNumber = FormatData(17..20);
                data_RTC.AccessCode = FormatData(20..23);
                data_RTC.Configurator = FormatData(23..25);
                data_RTC.Interface = FormatData(25..28);
                data_RTC.LevelSignalU1Channel1 = FormatData(45..46);
                data_RTC.LevelSignalU2Channel1 = FormatData(44..45);
                data_RTC.LevelSignalU3Channel1 = FormatData(43..44);
                data_RTC.LevelSignalU1Channel2 = FormatData(48..49);
                data_RTC.LevelSignalU2Channel2 = FormatData(47..48);
                data_RTC.LevelSignalU3Channel2 = FormatData(46..47);
                data_RTC.ErrorConection = "0"; //Переопределен как ошибка связи со счетчиком.
                data_RTC.ErrorChannel1 = FormatData(63..64);
                data_RTC.ErrorChannel2 = FormatData(64..65);
                data_RTC.ErrorSystem = FormatData(65..66);
                data_RTC.TemperatureChanel1 = FormatData(66..69);
                data_RTC.TemperatureChanel2 = FormatData(69..72);
                data_RTC.Temperature_T3 = FormatData(72..75);
                data_RTC.Temperature_T4_T5 = FormatData(75..78);
                data_RTC.Commissioning = FormatData(78..82);
                data_RTC.LowDifferenceTemp = FormatData(89..92);
                data_RTC.Pressure_P3 = FormatData(92..94);
                data_RTC.Pressure_P4_P5 = FormatData(94..96);
                data_RTC.Downtime = FormatData(99..102);
                data_RTC.RunningTime = FormatData(103..106);
                data_RTC.IrregularFlowTime = FormatData(107..110);
                data_RTC.ExcessFlowTime = FormatData(111..114);
                data_RTC.NoFlowTime = FormatData(115..118);
                data_RTC.NegativeFlowTime = FormatData(119..122);
                data_RTC.DefectTime = FormatData(123..126);
                data_RTC.NoPowerTime = FormatData(126..129);
                data_RTC.MaxSensorsPressure = FormatData(129..130);

                Console.WriteLine($"Время:{data_RTC.DateTimes}");
                //OutputConsole_RTC(data_RTC);
                SetValueDB_RTC(data_RTC, indexCount);
            }
            catch (Exception ex)
            {
                if (errorCount >= limitErrorCom)
                {
                    errorCount = 0;
                    comm.ClosePort();
                    comm.OpenPort();
                }
                else
                {
                    errorCount++;
                    string error = $"Количество накопительных ошибок: {errorCount} из {limitErrorCom}";
                    OutputConsole_Error("\n\nНе получены данные со счетчика!\n\n");
                    Console.WriteLine(error);
                    logWriter.WriteError($"Не получены данные со счетчика!\t" + error);

                    using (var context = new DataContext())
                    {
                        GetSqlProcedure(context, dictionary[indexCount][11], "1", DateTime.Now);
                    }
                }
            }            
        }
        //Метод заполнения объекта данных 256 байт.
        static void FillingAnObject_NV(int indexCount)
        {
            Data_NV data_NV = new Data_NV();
            try
            {
                data_NV.DateTimes = DateTime.Now;
                //-ОБЩИЕ--------------------------------------------------------------------------------------------
                data_NV.FlowVolume_Sum_Diff = FormatData(78..81);
                data_NV.FlowVolume_Sum_Diff_JunTetrad = FormatData(81..82);
                data_NV.Volume_Sum_Diff = FormatData(206..210);
                //-КАНАЛ №1------------------------------------------------------------------------------------------
                data_NV.DiameterRU = FormatData(2..5);
                data_NV.BaseL = FormatData(5..8);
                data_NV.CutoffThreshold = FormatData(8..10);
                data_NV.ParameterU = FormatData(13..14);
                data_NV.ParameterZ = FormatData(11..13);
                data_NV.ParameterI = FormatData(10..11);
                data_NV.ZeroOffset = FormatData(14..18);
                data_NV.DelayTime = FormatData(20..24);
                data_NV.FlowMax = FormatData(24..27);
                data_NV.FlowBoundary = FormatData(27..30);
                data_NV.TotalTime = FormatData(30..34);
                data_NV.DifferenceTime = FormatData(34..38);
                data_NV.FlowVolume = FormatData(38..42);
                data_NV.ThermalPowerMW = FormatData(42..45);
                data_NV.ThermalPowerGcall = FormatData(45..48);
                data_NV.FlowMass = FormatData(56..60);
                data_NV.Volume = FormatData(60..64);
                data_NV.Mass = FormatData(64..68);
                data_NV.ThermalEnergyGJ = FormatData(68..72);
                data_NV.Pressure = FormatData(82..84);
                data_NV.WeightPulseInput = FormatData(95..97);
                data_NV.LengthRU = FormatData(97..100);
                data_NV.ThermalPowerGJ = FormatData(107..110);
                data_NV.FlowPulseInput = FormatData(114..117);
                data_NV.VolumePulseInput = FormatData(117..121);
                data_NV.Viscosity = FormatData(125..127);
                data_NV.ThermalEnergyGCall = FormatData(196..200);
                //-КАНАЛ №2------------------------------------------------------------------------------------------
                data_NV.DiameterRU_ch2 = FormatData(130..133);
                data_NV.BaseL_ch2 = FormatData(133..136);
                data_NV.CutoffThreshold_ch2 = FormatData(136..138);
                data_NV.ParameterU_ch2 = FormatData(141..142);
                data_NV.ParameterZ_ch2 = FormatData(139..141);
                data_NV.ParameterI_ch2 = FormatData(138..139);
                data_NV.ZeroOffset_ch2 = FormatData(142..146);
                data_NV.DelayTime_ch2 = FormatData(148..152);
                data_NV.FlowMax_ch2 = FormatData(152..155);
                data_NV.FlowBoundary_ch2 = FormatData(155..158);
                data_NV.TotalTime_ch2 = FormatData(158..162);
                data_NV.DifferenceTime_ch2 = FormatData(162..166);
                data_NV.FlowVolume_ch2 = FormatData(166..170);
                data_NV.ThermalPowerMW_ch2 = FormatData(170..173);
                data_NV.ThermalPowerGcall_ch2 = FormatData(173..176);
                data_NV.FlowMass_ch2 = FormatData(184..188);
                data_NV.Volume_ch2 = FormatData(188..192);
                data_NV.Mass_ch2 = FormatData(192..196);
                data_NV.ThermalEnergyGJ_ch2 = FormatData(72..76);
                data_NV.Pressure_ch2 = FormatData(210..212);
                data_NV.WeightPulseInput_ch2 = FormatData(223..225);
                data_NV.LengthRU_ch2 = FormatData(225..228);
                data_NV.ThermalPowerGJ_ch2 = FormatData(235..238);
                data_NV.FlowPulseInput_ch2 = FormatData(242..245);
                data_NV.VolumePulseInput_ch2 = FormatData(245..249);
                data_NV.Viscosity_ch2 = FormatData(253..255);
                data_NV.ThermalEnergyGCall_ch2 = FormatData(200..204);

                Console.WriteLine($"Время:{data_NV.DateTimes}");
                //OutputConsole_NV(data_NV);
                SetValueDB_NV(data_NV, indexCount);
            }
            catch (Exception ex)
            {
                if (errorCount >= limitErrorCom)
                {
                    errorCount = 0;
                    comm.ClosePort();
                    comm.OpenPort();
                }
                else
                {
                    errorCount++;
                    string error = $"Количество накопительных ошибок: {errorCount} из {limitErrorCom}";
                    OutputConsole_Error("\n\nНе получены данные со счетчика!\n\n");
                    Console.WriteLine(error);
                    logWriter.WriteError($"Не получены данные со счетчика!\t\n {error}" + ex);
                    
                    using (var context = new DataContext())
                    {
                        GetSqlProcedure(context, dictionary[indexCount][11], "1", DateTime.Now);
                    }
                }
            }            
        }
                
        // Метод передачи данных RTC-памяти в БД
        static private void SetValueDB_RTC(Data_RTC data_RTC, int indexCount)
        {
            try
            {
                using (var context = new DataContext())
                {
                    // [indexCount] = -1 от id (ListValueId)
                    GetSqlProcedure(context, dictionary[indexCount][0], data_RTC.TimeAndDate, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][1], data_RTC.SerialNumber, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][2], data_RTC.AccessCode, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][3], data_RTC.Configurator, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][4], data_RTC.Interface, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][5], data_RTC.LevelSignalU1Channel1, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][6], data_RTC.LevelSignalU2Channel1, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][7], data_RTC.LevelSignalU3Channel1, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][8], data_RTC.LevelSignalU1Channel2, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][9], data_RTC.LevelSignalU2Channel2, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][10], data_RTC.LevelSignalU3Channel2, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][11], data_RTC.ErrorConection, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][12], data_RTC.ErrorChannel1, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][13], data_RTC.ErrorChannel2, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][14], data_RTC.ErrorSystem, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][15], data_RTC.TemperatureChanel1, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][16], data_RTC.TemperatureChanel2, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][17], data_RTC.Temperature_T3, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][18], data_RTC.Temperature_T4_T5, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][19], data_RTC.Commissioning, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][20], data_RTC.LowDifferenceTemp, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][21], data_RTC.Pressure_P3, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][22], data_RTC.Pressure_P4_P5, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][23], data_RTC.Downtime, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][24], data_RTC.RunningTime, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][25], data_RTC.IrregularFlowTime, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][26], data_RTC.ExcessFlowTime, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][27], data_RTC.NoFlowTime, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][28], data_RTC.NegativeFlowTime, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][29], data_RTC.DefectTime, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][30], data_RTC.NoPowerTime, data_RTC.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][31], data_RTC.MaxSensorsPressure, data_RTC.DateTimes);
                }

                if (okResultProcedure > 0)
                {
                    Console.WriteLine($"Выполнено процедур: {okResultProcedure}");
                    okResultProcedure = 0;
                }
                else
                {
                    Console.WriteLine($"Процедуры не выполнены.!!!");
                    okResultProcedure = 0;
                }
            }
            catch (Exception ex)
            {
                string error = $"Косяк в отправке выполнения процедур с данными памяти RTC!\n" + ex;
                Console.WriteLine(error);
                logWriter.WriteError(error);
            }            
        }
        // Метод передачи данных NV-памяти в БД
        static private void SetValueDB_NV(Data_NV data_NV, int indexCount)
        {
            try
            {
                using (var context = new DataContext())
                {
                    // [indexCount] = -1 от id (ListValueId)
                    GetSqlProcedure(context, dictionary[indexCount][32], data_NV.FlowVolume_Sum_Diff, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][33], data_NV.FlowVolume_Sum_Diff_JunTetrad, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][34], data_NV.Volume_Sum_Diff, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][35], data_NV.DiameterRU, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][36], data_NV.BaseL, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][37], data_NV.CutoffThreshold, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][38], data_NV.ParameterU, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][39], data_NV.ParameterZ, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][40], data_NV.ParameterI, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][41], data_NV.ZeroOffset, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][42], data_NV.DelayTime, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][43], data_NV.FlowMax, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][44], data_NV.FlowBoundary, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][45], data_NV.TotalTime, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][46], data_NV.DifferenceTime, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][47], data_NV.FlowVolume, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][48], data_NV.ThermalPowerMW, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][49], data_NV.ThermalPowerGcall, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][50], data_NV.FlowMass, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][51], data_NV.Volume, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][52], data_NV.Mass, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][53], data_NV.ThermalEnergyGJ, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][54], data_NV.Pressure, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][55], data_NV.WeightPulseInput, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][56], data_NV.LengthRU, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][57], data_NV.ThermalPowerGJ, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][58], data_NV.FlowPulseInput, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][59], data_NV.VolumePulseInput, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][60], data_NV.Viscosity, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][61], data_NV.ThermalEnergyGCall, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][62], data_NV.DiameterRU_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][63], data_NV.BaseL_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][64], data_NV.CutoffThreshold_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][65], data_NV.ParameterU_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][66], data_NV.ParameterZ_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][67], data_NV.ParameterI_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][68], data_NV.ZeroOffset_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][69], data_NV.DelayTime_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][70], data_NV.FlowMax_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][71], data_NV.FlowBoundary_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][72], data_NV.TotalTime_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][73], data_NV.DifferenceTime_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][74], data_NV.FlowVolume_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][75], data_NV.ThermalPowerMW_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][76], data_NV.ThermalPowerGcall_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][77], data_NV.FlowMass_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][78], data_NV.Volume_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][79], data_NV.Mass_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][80], data_NV.ThermalEnergyGJ_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][81], data_NV.Pressure_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][82], data_NV.WeightPulseInput_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][83], data_NV.LengthRU_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][84], data_NV.ThermalPowerGJ_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][85], data_NV.FlowPulseInput_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][86], data_NV.VolumePulseInput_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][87], data_NV.Viscosity_ch2, data_NV.DateTimes);
                    GetSqlProcedure(context, dictionary[indexCount][88], data_NV.ThermalEnergyGCall_ch2, data_NV.DateTimes);
                }

                if (okResultProcedure > 0)
                {
                    Console.WriteLine($"Выполнено процедур: {okResultProcedure}");
                    okResultProcedure = 0;
                }
                else
                {
                    Console.WriteLine($"Процедуры не выполнены.!!!");
                    okResultProcedure = 0;
                }
            }
            catch (Exception ex)
            {
                string error = $"Косяк в отправке выполнения процедур с данными памяти RTC!\n" + ex;
                Console.WriteLine(error);
                logWriter.WriteError(error);
            }
        }
        // Вызов процедуры с параметрами 'id', 'value', 'date'.
        static private void GetSqlProcedure(DataContext context, int idParam, string valueParam, DateTime dateParam)
        {
            SqlParameter[] param =
                {
                    new ()
                    {
                        ParameterName = "@p0",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = idParam,
                    },
                    new ()
                    {
                        ParameterName = "@p1",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Size = 50,
                        Value = valueParam
                    },
                    new ()
                    {
                        ParameterName = "@p2",
                        SqlDbType = System.Data.SqlDbType.DateTime,
                        Value = dateParam
                    },
                    new ()
                    {
                        ParameterName = "@p3",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Direction = System.Data.ParameterDirection.Output
                    }
                };
            context.Database.ExecuteSqlRaw("update_cell @p0, @p1, @p2, @p3 output", param);
            if (Convert.ToInt32(param[3].Value) == 1)
                okResultProcedure++;

            //Console.WriteLine($"Результат выполнения процедуры: - {param[3].Value}"); //Если возвращается "1" - значит процедура полностью выполнена.
        }
        // Метод инициализации основных значений и БД.
        static private void InitializationDB(int countNumberCounter)
        {
            try
            {
                using (FileStream fs = new FileStream(@"Resources\\db_List_Data.json", FileMode.OpenOrCreate))
                {
                    projectObject = JsonSerializer.Deserialize<ProjectObject>(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Этап 1 - Ошибка чтения json-файла.");
                logWriter.WriteError($"{ex}");
                Console.ReadKey();
            }

            try
            {
                using (var context = new DataContext())
                {
                    #region Пример инициализации объекта "projectObject".
                    //var projectObject = new ProjectObject()
                    //{
                    //    NameObject = "РК Харьковская",
                    //    Counters = new List<Counter>
                    //    {
                    //        new Counter()
                    //        {
                    //            CounterId = "СНТ-2 №1",
                    //            NameCounter = "СНТ №1 Магистраль 22",
                    //            ListValues = new List<ListValue>
                    //            {
                    //                new ListValue()
                    //                {
                    //                    Hash = 10001,
                    //                    Type = "string",
                    //                    Value = "1102",
                    //                    Description = "Тест коммент.",
                    //                    Csd_Changed = false,
                    //                    Has_History = true
                    //                }
                    //            }
                    //        },
                    //        new Counter()
                    //        {
                    //            CounterId = "СНТ-2 №2",
                    //            NameCounter = "СНТ №1 Магистраль 55",
                    //            ListValues = new List<ListValue>
                    //            {
                    //                new ListValue()
                    //                {
                    //                    Hash = 22001,
                    //                    Type = "int",
                    //                    Value = "11",
                    //                    DateTimeUpdate = DateTime.Now,
                    //                    Csd_Changed = false,
                    //                    Has_History = false
                    //                }
                    //            }
                    //        }
                    //    }
                    //};
                    #endregion
                    var checkCreated = context.Database.EnsureCreated();

                    if (checkCreated)
                    {
                        context.Database.ExecuteSqlRaw("-- =============================================\r\n-- Author:\t\t<Радкевич Игорь>\r\n-- Create date: <05.05.2023>\r\n-- Description:\t<Процедура обновления данных в оперативной таблице,\r\n-- и запись этих данных в таблицу истории.>\r\n-- =============================================\r\nCREATE PROCEDURE [dbo].[update_cell] \r\n\r\n\t@cell_id_P int,\r\n\t@cell_value_P varchar(50),\r\n\t@cell_date_P datetime,\r\n\t@cell_out int output\r\n\t\r\nAS\r\n\r\n\tSET NOCOUNT ON;\r\n\tSET DATEFORMAT ymd;\r\n\r\n\tDECLARE @datetime_D datetime\r\n\tDECLARE @value_D VARCHAR(255)\r\n\tDECLARE @hasHistory_D bit\r\n\tDECLARE @idHash_D int\r\n\tDECLARE @changed_D bit\r\n\r\n\t--Выборка конкретной щаписи оп ID.\r\n\tSELECT TOP 1 @datetime_D = DateTimeUpdate, @value_D = Value, @hasHistory_D = Has_History, @idHash_D = Hash, @changed_D = Csd_Changed\r\n\tFROM ListValues\r\n\tWHERE ListValueId = @cell_id_P;\r\n\r\n\t--Обновление значение\r\n\tif @cell_value_P < > @value_D\r\n\t\tBEGIN\r\n\t\t\tUPDATE ListValues\r\n\t\t\tSET Value = @cell_value_P, DateTimeUpdate = @cell_date_P, Csd_Changed = 1\r\n\t\t\tWHERE Hash = @idHash_D;\t\t\t\r\n\t\tEND\r\n\telse\r\n\t\tBEGIN\r\n\t\t\tUPDATE ListValues\r\n\t\t\tSET Value = @cell_value_P, DateTimeUpdate = @cell_date_P, Csd_Changed = 0\r\n\t\t\tWHERE Hash = @idHash_D;\r\n\t\tEND\r\n\t\t\r\n\t--Запись в архивную таблицу\r\n\tif ((@hasHistory_D > 0) AND (@changed_D = 1))\r\n\t\tBEGIN\r\n\t\t\tINSERT INTO History (HashId, Value, DateTime)\r\n\t\t\tVALUES (@idHash_D, @cell_value_P, @cell_date_P)\r\n\t\tEND\r\n\r\n\t--Возврат значения \"1\" - процедура полностью выполнена.\r\n\tSET @cell_out = 1\r\n");
                        if (projectObject == null)
                            Console.WriteLine("Объект десириализации пуст!");
                        else
                            context.ProjectObjects.Add(projectObject);
                    }
                    else
                        Console.WriteLine("База данных уже существует!"); //Технический указатель. Требует удаления перед релизом.

                    context.SaveChanges();

                    for (int i = 1; i <= countNumberCounter; i++)
                    {
                        dictionary[i] = context.ListValues
                            .Where(countId => countId.CounterId == $"СНТ-2 №{i}")
                            .Select(id => id.ListValueId).ToList();
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Этап 2 - Ошибка на этапе подключения к БД.");
                logWriter.WriteError($"{ex}");
                Console.ReadKey();
            }
        }

        #region Методы фоматированного вывода данных в консоль. 
        //Метод вывода данных в консоль (красивый).
        static void OutputConsole_RTC(Data_RTC data_RTC)
        {
            #region Время и Дата
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"\nВремя и Дата:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.TimeAndDate}");
            #endregion

            #region Номер прибора
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Номер прибора:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.SerialNumber}");
            #endregion

            #region Код доступа
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Код доступа:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.AccessCode}");
            #endregion

            #region Конфигуратор
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Конфигуратор:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.Configurator}");
            #endregion

            #region Интерфейс
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Интерфейс:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.Interface}");
            #endregion

            #region Уровень сингнала U1 - Канала №1
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Уровень сингнала U1 - Канала №1:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.LevelSignalU1Channel1}");
            #endregion

            #region Уровень сингнала U1 - Канала №1
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Уровень сингнала U1 - Канала №1:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.LevelSignalU2Channel1}");
            #endregion

            #region Уровень сингнала U1 - Канала №1
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Уровень сингнала U1 - Канала №1:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.LevelSignalU3Channel1}");
            #endregion

            #region Уровень сингнала U1 - Канала №2
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Уровень сингнала U1 - Канала №2:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.LevelSignalU1Channel2}");
            #endregion

            #region Уровень сингнала U1 - Канала №2
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Уровень сингнала U1 - Канала №2:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.LevelSignalU2Channel2}");
            #endregion

            #region Уровень сингнала U1 - Канала №2
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Уровень сингнала U1 - Канала №2:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.LevelSignalU3Channel2}");
            #endregion

            #region Инструкция
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Инструкция:\t\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.ErrorConection}");
            #endregion

            #region Ошибки канала №1
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Ошибки канала №1:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.ErrorChannel1}");
            #endregion                       

            #region Ошибки канала №2
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Ошибки канала №2:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.ErrorChannel2}");
            #endregion

            #region Ошибки системы
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Ошибки системы:\t\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.ErrorSystem}");
            #endregion

            #region Температура канал №1
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Температура канал №1:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.TemperatureChanel1} °C");
            #endregion

            #region Температура канал №2
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Температура канал №2:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.TemperatureChanel2} °C");
            #endregion

            #region Температура Т3
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Температура Т3:\t\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.Temperature_T3} °C");
            #endregion

            #region Температура Т4/T5
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Температура Т4/T5:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.Temperature_T4_T5} °C");
            #endregion

            #region Ввод в эксплуатацию
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Ввод в эксплуатацию:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.Commissioning}");
            #endregion

            #region Малая разность температур
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Малая разность температур:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.LowDifferenceTemp} мин.");
            #endregion

            #region Давление Р3
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Давление Р3:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.Pressure_P3} МПа");
            #endregion

            #region Давление Р4/P5
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Давление Р3:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.Pressure_P4_P5} МПа");
            #endregion

            #region Время простоя
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Время простоя:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.Downtime} мин.");
            #endregion

            #region Время наработки
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Время наработки:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.RunningTime} мин.");
            #endregion

            #region Ненармированный расход
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Ненармированный расход:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.IrregularFlowTime} мин.");
            #endregion

            #region Превышение расхода
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Превышение расхода:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.ExcessFlowTime} мин.");
            #endregion

            #region Отсутствие расхода
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Отсутствие расхода:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.NoFlowTime} мин.");
            #endregion

            #region Отрицательный расход
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Отрицательный расход:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.NegativeFlowTime} мин.");
            #endregion

            #region Неисправность
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Неисправность:\t\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.DefectTime} мин.");
            #endregion

            #region Отсутствие питания
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Отсутствие питания:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.NoPowerTime} мин.");
            #endregion

            #region Максимум датчиков давления
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Максимум датчиков давления:\t");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{data_RTC.MaxSensorsPressure} МПа");
            #endregion

            Console.ResetColor();
        }
        //Метод вывода данных в консоль (красивый).
        static void OutputConsole_NV(Data_NV data_NV)
        {
            //-ОБЩИЕ--------------------------------------------------------------------------------------------
            Console.Write($"\n\n------------------\tОБЩИЕ\t---------------------");
            #region Расход объемный сумма/разность.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"\nРасход объемный сумма/разность.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowVolume_Sum_Diff} м3/ч");
            #endregion
            #region Младшая тетрада сумма/разность.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Младшая тетрада сумма/разность.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowVolume_Sum_Diff_JunTetrad}");
            #endregion
            #region Объем сумма/разность.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Объем сумма/разность.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.Volume_Sum_Diff} м3");
            #endregion

            //-КАНАЛ №1------------------------------------------------------------------------------------------
            Console.Write($"\n\n------------------\tКАНАЛ №1\t------------------");
            #region Диаметр мерного участка (Диаметр РУ)
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"\nДиаметр мерного участка (Диаметр РУ):\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.DiameterRU} мм.");
            #endregion
            #region Расстояние между датчикаами (База).
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расстояние между датчикаами (База):\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.BaseL} мм.");
            #endregion
            #region Порог отсечки.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Порог отсечки:\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.CutoffThreshold} %Qmax.");
            #endregion
            #region Параметры 'Порог', 'Строб', 'Импульс зондирования'.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Параметры 'Порог'\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ParameterU} B");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Параметры 'Строб'\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ParameterZ} мкс");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Параметры 'Импульс зонд.'\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ParameterI} мкс");
            #endregion
            #region Смещение "0".
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Смещение '0'\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ZeroOffset} мкс");
            #endregion
            #region Задержка. (Постоянная задержка в цепях).
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Задержка. (Постоянная задержка в цепях).\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.DelayTime} мкс");
            #endregion
            #region Расход максимальный.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход максимальный.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowMax} м3/ч");
            #endregion
            #region Расход граничный.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход граничный.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowBoundary} м3/ч");
            #endregion
            #region Сумарное время.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Сумарное время\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.TotalTime} мкс");
            #endregion
            #region Время разностное.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Время разностное.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.DifferenceTime} мкс");
            #endregion
            #region Расход объемный.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход объемный.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowVolume} м3/ч");
            #endregion
            #region Тепловая мощность Мвт.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая мощность.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalPowerMW} Мвт");
            #endregion
            #region Тепловая мощность ГКалл/ч.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая мощность.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalPowerGcall} ГКалл/ч");
            #endregion
            #region Расход массовый.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход массовый.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowMass} ГКалл/ч");
            #endregion
            #region Объем.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Объем.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.Volume} м3");
            #endregion
            #region Масса.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Масса.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.Mass} т");
            #endregion
            #region Тепловая энергия
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая энергия.\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalEnergyGJ} ГДж");
            #endregion
            #region Давление.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Давление.\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.Pressure} МПа");
            #endregion
            #region Вес импульсного входа.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Вес импульсного входа.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.WeightPulseInput}");
            #endregion
            #region Длина РУ.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Длина РУ.\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.LengthRU} мм");
            #endregion
            #region Тепловая мощность ГДж/ч.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая мощность.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalPowerGJ} ГДж/ч");
            #endregion
            #region Расход импульсного входа №1.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход импульсного входа №1.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowPulseInput} м3/ч");
            #endregion
            #region Объем импульсного входа №1.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Объем импульсного входа №1.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.VolumePulseInput} м3");
            #endregion
            #region Вязкость.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Вязкость.\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.Viscosity} сСт");
            #endregion
            #region Тепловая энергия Гкал.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая энергия.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalEnergyGCall} ГКалл");
            #endregion

            //-КАНАЛ №2------------------------------------------------------------------------------------------
            Console.Write($"\n\n------------------\tКАНАЛ №2\t------------------");
            #region Диаметр мерного участка (Диаметр РУ)
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"\nДиаметр мерного участка (Диаметр РУ):\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.DiameterRU_ch2} мм.");
            #endregion
            #region Расстояние между датчикаами (База).
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расстояние между датчикаами (База):\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.BaseL_ch2} мм.");
            #endregion
            #region Порог отсечки.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Порог отсечки:\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.CutoffThreshold_ch2} %Qmax.");
            #endregion
            #region Параметры 'Порог', 'Строб', 'Импульс зондирования'.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Параметры 'Порог'\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ParameterU_ch2} B");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Параметры 'Строб'\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ParameterZ} мкс");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Параметры 'Импульс зонд.'\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ParameterI} мкс");
            #endregion
            #region Смещение "0".
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Смещение '0'\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ZeroOffset_ch2} мкс");
            #endregion
            #region Задержка. (Постоянная задержка в цепях).
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Задержка. (Постоянная задержка в цепях).\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.DelayTime_ch2} мкс");
            #endregion
            #region Расход максимальный.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход максимальный.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowMax_ch2} м3/ч");
            #endregion
            #region Расход граничный.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход граничный.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowBoundary_ch2} м3/ч");
            #endregion
            #region Сумарное время.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Сумарное время\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.TotalTime_ch2} мкс");
            #endregion
            #region Время разностное.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Время разностное.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.DifferenceTime_ch2} мкс");
            #endregion
            #region Расход объемный.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход объемный.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowVolume_ch2} м3/ч");
            #endregion
            #region Тепловая мощность Мвт.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая мощность.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalPowerMW_ch2} Мвт");
            #endregion
            #region Тепловая мощность ГКалл/ч.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая мощность.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalPowerGcall_ch2} ГКалл/ч");
            #endregion
            #region Расход массовый.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход массовый.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowMass_ch2} ГКалл/ч");
            #endregion
            #region Объем.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Объем.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.Volume_ch2} м3");
            #endregion
            #region Масса.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Масса.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.Mass_ch2} т");
            #endregion
            #region Тепловая энергия
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая энергия.\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalEnergyGJ_ch2} ГДж");
            #endregion
            #region Давление.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Давление.\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.Pressure_ch2} МПа");
            #endregion
            #region Вес импульсного входа.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Вес импульсного входа.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.WeightPulseInput_ch2}");
            #endregion
            #region Длина РУ.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Длина РУ.\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.LengthRU_ch2} мм");
            #endregion
            #region Тепловая мощность ГДж/ч.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая мощность.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalPowerGJ_ch2} ГДж/ч");
            #endregion
            #region Расход импульсного входа №1.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Расход импульсного входа №1.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.FlowPulseInput_ch2} м3/ч");
            #endregion
            #region Объем импульсного входа №1.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Объем импульсного входа №1.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.VolumePulseInput_ch2} м3");
            #endregion
            #region Вязкость.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Вязкость.\t\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.Viscosity_ch2} сСт");
            #endregion
            #region Тепловая энергия Гкал.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Тепловая энергия.\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{data_NV.ThermalEnergyGCall_ch2} ГКалл");
            #endregion

            Console.ResetColor();
        }
        //Метод вывода формаритованный ошибки (красный)
        static void OutputConsole_Error(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(msg);
            Console.ResetColor();
            Console.WriteLine();
        }
        //Метод вывода формаритованный ошибки (красный)
        static void OutputConsole_OK(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Green; 
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(msg);
            Console.ResetColor();
            Console.WriteLine();
        }
        #endregion
    }
}