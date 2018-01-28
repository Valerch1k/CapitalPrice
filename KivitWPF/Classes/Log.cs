using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceListCash.Classes
{
    static class Log
    {
        private static object sync = new object();
        /// <summary>
        /// Записует логи в папку Log с понятным описанием исключения( ошибки)
        /// </summary>
        /// <param name="ex">исключение</param>
        /// <param name="NameLog"> дополнительная информация  для Log</param>
        public static void Write(Exception ex, string NameLog)
        {
            try
            {
                // Путь .\\Log
                string pathToLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PriceListCashLog");
                if (!Directory.Exists(pathToLog))
                    Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
                string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log", AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                string fullText = string.Format("{0} {1} {2} {3} {4} \r\n\n", DateTime.Now.ToString(), NameLog, ex.TargetSite.DeclaringType, ex.TargetSite.Name, ex.Message);
                lock (sync)
                {
                    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                }
            }
            catch
            {
                // Перехватываем все и ничего не делаем
            }
        }

        /// <summary>
        /// Записует логи в папку Log  
        /// </summary>
        /// <param name="NameLog"> Записываемая информация для логов</param>
        public static void Write(string NameLog)
        {
            try
            {
                // Путь .\\Log
                string pathToLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PriceListCashLog");
                if (!Directory.Exists(pathToLog))
                    Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
                string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log", AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                string fullText = string.Format("{0} {1} \r\n\n", DateTime.Now.ToString(), NameLog);
                lock (sync)
                {
                    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                }
            }
            catch
            {
                // Перехватываем все и ничего не делаем
            }
        }

    }
}
