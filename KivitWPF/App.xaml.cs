using PriceListCash.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KivitWPF
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // проверяем статус принтера
            if (!StatusApp.PrinterStatusBool())
            {
                ConfigTxt config = new ConfigTxt();
                Log.Write("Status printer " + config.PrintName + " : Offline\n");
                MessageBox.Show("Нет подключения к принтеру " + config.PrintName+ " или нет драйвера .Установите в Меню/Настройки программы принтер !!!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // проверяем наличие файла шрифта штрихкода
            StatusApp.FontBarcod();
            // проверяем подключения к бд
            if (!StatusApp.CheckConnection())
            {
                MessageBox.Show("Нет соединение с базой данных , проверьте сетевое подключение или обратитесь к системному администратору ", "", MessageBoxButton.OK, MessageBoxImage.Hand);
                ConnectionStringDataBase.ChangeConnect();
            }   
        } 
    }
}
