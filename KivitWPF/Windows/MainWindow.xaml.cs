using CrystalDecisions.CrystalReports.Engine;
using PriceListCash;
using PriceListCash.Classes;
using PriceListCash.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CrystalDecisions.Shared;
using KeepAutomation.Barcode.Crystal;
using System.Collections.ObjectModel;
using PriceListCash.Windows;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace KivitWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PriceListCash.Properties.Settings App = new PriceListCash.Properties.Settings();
        ConfigTxt config = new ConfigTxt();
        public MainWindow()
        {
            InitializeComponent(); 
            Items = Item.GetItems();
            Loaded += MainWindow_Loaded;
            DeleteCommand = new MyCommand() { Collection = Items };

            // Таймер для автопечати 
            System.Windows.Threading.DispatcherTimer timerAutoPrint = new System.Windows.Threading.DispatcherTimer();
            timerAutoPrint.Tick += new EventHandler(dispatcherTimer_Tick);
            timerAutoPrint.Interval = new TimeSpan(0, 0, config.TimeAutoPrint);
            timerAutoPrint.Start();
            // Таймер для обновления статуса принтера
            System.Windows.Threading.DispatcherTimer timerStatusPrinter = new System.Windows.Threading.DispatcherTimer();
            timerStatusPrinter.Tick += new EventHandler(dispatcherTimerStatusPrinter);
            timerStatusPrinter.Interval = new TimeSpan(0, 0, 1);
            timerStatusPrinter.Start();
        }

        #region событие на удаление строки из datagrid 

        class MyCommand : ICommand
        {
            public ObservableCollection<Item> Collection { get; set; }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                Collection.Remove(parameter as Item);
            }
        }

        public ObservableCollection<Item> Items
        {
            get { return (ObservableCollection<Item>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<Item>), typeof(MainWindow), new PropertyMetadata(null));

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(MainWindow), new PropertyMetadata(null));
        #endregion

        #region события/ button/ focus/ timer/ KeyUp

        /// <summary>
        /// Обновляет статус принтера
        /// </summary>
        private void dispatcherTimerStatusPrinter(object sender, EventArgs e)
        {
            if (StatusApp.PrinterStatusBool())
            {
                lblStatus.Content = "Online";
                EllipseStatus.Fill = Brushes.GreenYellow;
            }
            else
            {
                lblStatus.Content = "Offline";
                 EllipseStatus.Fill = Brushes.OrangeRed;
            }
        }
        /// <summary>
        ///  Изменяем подключения к бд
        /// </summary>
        private void MenuChangeConnection_Click(object sender, RoutedEventArgs e)
        {
            ConnectionStringDataBase.ChangeConnect();
            // перезагружаем приложение
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Метод запускаемый в таймере 
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if ((bool)chcAutoPrint.IsChecked)
            {
                SelectAllPriceList();
                PrintAllPriceList();
            }
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SelectAllPriceList();
            lblConnectionString.Content = ConnectionStringDataBase.Read();
        }

        /// <summary>
        ///  Удаляем запись из  бд
        /// </summary>
        private void btnDataGridDeletRow_Click(object sender, RoutedEventArgs e)
        {
            Item item = dgMain.SelectedItem as Item;
            if (item != null)
            {
                DeleteRowFromDB(item.GuiId.ToString());
                txtRegistrInput.Focus();
            }
        }

        /// <summary>
        ///  Вводит DataGridTextColumn   сразу для редактирования 
        /// </summary>
        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count == 0) return;
            var currentCell = e.AddedCells[0];
            if (currentCell.Column ==  dgMain.Columns[2])
            {
                dgMain.BeginEdit();
            }
        }

        /// <summary>
        /// Перемещение по DateGrid  с  помощью клавиш
        /// </summary>
        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var uiElement = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && uiElement != null)
            {
                e.Handled = true;
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
            if (e.Key == Key.Down && uiElement != null)
            {
                e.Handled = true;
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
            if (e.Key == Key.Up && uiElement != null)
            {
                e.Handled = true;
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
            }
        }

        /// <summary>
        /// Закрыть программу 
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Потеря фокуса textbox 
        /// </summary>
        private void txtRegistrInput_LostFocus(object sender, RoutedEventArgs e)
        {
            txtRegistrInput.Background = Brushes.Coral;
        }

        /// <summary>
        /// Фокус на textbox 
        /// </summary>
        private void txtRegistrInput_GotFocus(object sender, RoutedEventArgs e)
        {
            txtRegistrInput.Background = Brushes.White;
        }
 
        /// <summary>
        /// О программе
        /// </summary>
        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutProgram about = new AboutProgram();
            about.ShowDialog();
        }
         
        /// <summary>
        ///  Инструкция
        /// </summary>
        private void MenuInstructions_Click(object sender, RoutedEventArgs e)
        {
            Instructions instruct = new Instructions();
            instruct.ShowDialog();
        }

        /// <summary>
        /// При вводе кода регистра в textbox   дабавляем артикул в  DataGrid
        /// </summary>
        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtRegistrInput.Text != string.Empty) // валидация textboxa
                {
                    if (IsNum(txtRegistrInput.Text)) // валидация textboxa
                    {
                        if ((bool)chcFromDocument.IsChecked) // если  check загрузка из документа
                        {
                            // AddNewRowDataGrid(sender, e);
                        }
                        else
                        {
                            if ((bool)chcAutoPrint.IsChecked) // если check автопечать
                            {
                                AddNewRowsInDB();
                                SelectAllPriceList();
                                UpdateCountInDB();
                                PrintAllPriceList();
                            }
                            else
                            {
                                AddNewRowsInDB();
                                SelectAllPriceList();
                            }

                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show(" Не правильно введен код регистра или штрихкод !!! ", "Ошибка ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show(" Введите код регистра или штрихкод !!! ", "Ошибка ", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                txtRegistrInput.Clear();
            }
            if (e.Key == Key.Delete)
            {
                Item item = dgMain.SelectedItem as Item;
                if (item != null)
                {
                    DeleteRowFromDB(item.GuiId.ToString());
                }
            }
            if (e.Key == Key.F1) // инструкция 
            {
                Instructions instr = new Instructions();
                instr.ShowDialog();
            }
            if (e.Key == Key.F2 && e.KeyboardDevice.Modifiers == ModifierKeys.Control) // Ctr + F2  фокус на textbox
            {
                txtRegistrInput.Focus();
            }
            if (e.Key == Key.F4) // фокус на редактирование в  datagrid
            {
                if (Items.Count > 0)
                {
                    dgMain.CurrentCell = new DataGridCellInfo(dgMain.Items[0], dgMain.Columns[2]);
                    dgMain.BeginEdit();
                }   
            }
            if(e.Key == Key.P && e.KeyboardDevice.Modifiers == ModifierKeys.Control ) // Ctr + P печатаем
            {
                txtRegistrInput.Focus();
                btnPrint_Click(sender, e);
            }
            if (e.Key == Key.A && e.KeyboardDevice.Modifiers == ModifierKeys.Control) // Ctr + A снять/ поставить флажек  автопечать
            {
                if ((bool)chcAutoPrint.IsChecked)
                    chcAutoPrint.IsChecked = false;
                else
                    chcAutoPrint.IsChecked = true;
            }
            if (e.Key == Key.F6) // снять/ поставить флажек загрузить из документа
            {
                if ((bool)chcFromDocument.IsChecked)
                    chcFromDocument.IsChecked = false;
                else
                    chcFromDocument.IsChecked = true;
            }
            if (e.Key == Key.F7) // фокус на  главное меню
            {
                MenuMain.Focus();
            } 
        }

        /// <summary>
        /// Кнопка печатать ценники
        /// </summary>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {   
            // обновляем количество
            UpdateCountInDB();
            // печатаем все , что добавили в таблицу
            PrintAllPriceList();
            // выводим не расспечатанные на экран , если вдруг остались о_О
            SelectAllPriceList();
            // фокус на texbox
            txtRegistrInput.Focus();

        }

        #endregion

        #region методы удаления/выборки/ обновления/печати  из бд

        /// <summary>
        /// Добавление новой строки в dataGrid и бд  по одному артикулу
        /// </summary>
        private void AddNewRowsInDB() 
        {

            DataTable NewRow = CRUD.SelectToDateTable(" exec [dbo].[cash_Ценники Добавление по одному] " + txtRegistrInput.Text);
            foreach (DataRow row in NewRow.Rows)
            {
                if (Convert.ToInt32(row["Result"]) == 0)
                {
                    MessageBoxResult result = MessageBox.Show(" Код регистра или штрихкод не найден  ", " ", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            } 

        }
 
        /// <summary>
        /// Вывод всех нераспечатанных ценников на экран 
        /// </summary>
        private void SelectAllPriceList()
        {
            Items.Clear();
            DataTable AllRow = CRUD.SelectToDateTable(" exec [cash_Ценники Вывод всех ценников]");
            if (AllRow.Rows.Count > 0)
            {
                foreach ( DataRow row in AllRow.Rows)
                {
                    Items.Add(new Item()
                    {
                        Name = row["Наименование"].ToString(),
                        Price = row["Цена"].ToString(),
                        Count = row["Количество"].ToString(),
                        KodRegistr = row["КодРегистра"].ToString(),
                        Barcode = row["Barcode"].ToString(),
                        GuiId = row["rowguid"].ToString()

                    });
                }
            }
        }

        /// <summary>
        /// Опрашивает бд и печатает  ценники и устанавливает flag 1 
        /// </summary>
        private void PrintAllPriceList() 
        {
            PriceListCash.Properties.Settings App1 = new PriceListCash.Properties.Settings();
            if (StatusApp.PrinterStatusBool()) //  если подключен принтер , то печатаем  
                {
                    DataTable dtQuery = CRUD.SelectToDateTable(" exec [cash_Ценники Вывод всех ценников]");
                    if (dtQuery.Rows.Count > 0)
                    {
                   
                    string defaultPrinterName = MyPrintersOptions.GetDefaultPrinterName();
                    foreach (DataRow myDataRow in dtQuery.Rows)
                        {
                            for (int i = 0; i < Convert.ToInt32(myDataRow["Количество"]); i++)
                            {
                                PriceList barcodeDetails = new PriceList();
                                DataTable dtPriceList = barcodeDetails._PriceList;
                                DataRow drow = dtPriceList.NewRow();
                                drow["Name"] = myDataRow["Наименование"].ToString();
                                drow["EI"] = myDataRow["ЕИ"].ToString();
                                drow["Barcode"] = myDataRow["BarCode"].ToString();
                                drow["Barcode_"] = "*";
                                drow["Barcode_"] += myDataRow["КодРегистра"].ToString();
                                drow["Barcode_"] += "*";
                                var Price = Convert.ToDouble(myDataRow["Цена"]);
                                var IntegerPartPrice = Convert.ToInt32(Math.Truncate(Price)); //  Целая часть цены
                                var FractionalPart = Convert.ToInt32((Price - IntegerPartPrice) * 100); // Дробная часть 
                                drow["IntegerPartPrice"] = IntegerPartPrice.ToString();
                                drow["FractionalPartPrice"] = ((FractionalPart < 10) ? ("0" + FractionalPart.ToString()) : FractionalPart.ToString());
                                drow["Code"] = myDataRow["КодРегистра"].ToString();
                                drow["TimeStamp"] = myDataRow["timestamp"].ToString();
                                drow["HostName"] = myDataRow["HostName"].ToString();
                                dtPriceList.Rows.Add(drow);
                                if (Convert.ToInt32(App1.ВидЦенника) == 0)
                                {
                                    using (CrystalReportMida Report = new CrystalReportMida())
                                    {
                                        Report.Database.Tables["PriceList"].SetDataSource((DataTable)dtPriceList);
                                        MyPrintersOptions.SetDefaultPrinter(config.PrintName);
                                        Report.PrintOptions.PrinterName = config.PrintName;
                                        Report.PrintToPrinter(1, false, 0, 0); // печать без предосмотра
                                    }  
                                }
                                else
                                {
                                    using (CrystalReportGippo Report = new CrystalReportGippo())
                                    {
                                        Report.Database.Tables["PriceList"].SetDataSource((DataTable)dtPriceList);
                                        MyPrintersOptions.SetDefaultPrinter(config.PrintName);
                                        Report.PrintOptions.PrinterName = config.PrintName;
                                        Report.PrintToPrinter(1, false, 0, 0); // печать без предосмотра
                                    }
                                }
                               
                            }
                            CRUD.QuerySQL("UPDATE [cash_Ценники] SET [fPrint] = 1 WHERE  rowguid = '" + myDataRow["rowguid"].ToString() + "'");
                        }
                         MyPrintersOptions.SetDefaultPrinter(defaultPrinterName);
                    }
                }
                else
                {
                    Log.Write("Status printer " + config.PrintName + " : Offline\n");
                    MessageBox.Show("Нет подключения к принтеру " + config.PrintName + " или нет драйвера . Установите по умолчанию принтер " + config.PrintName + " !!!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }                     
        }

        /// <summary>
        /// Обновляем  в бд количество в ценнике из DataGrid 
        /// </summary>
        private void UpdateCountInDB()
        {
            if (Items.Count > 0)
            {
                foreach ( var row in Items)
                {
                    CRUD.QuerySQL("EXEC [cash_Ценники обновления Count ценника ] '" + row.GuiId + "'," + row.Count);
                }
            }
        }

        /// <summary>
        /// Удаляет строку из бд
        /// </summary>
        /// <param name="gui"></param>
        private void DeleteRowFromDB(string gui)
        {
            CRUD.QuerySQL("Delete From [dbo].[cash_Ценники] Where rowguid = '" + gui + "'");
        }

        /// <summary>
        /// Проверяет содержит ли переменная текст 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsNum(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }

        #endregion

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateCountInDB();
            this.SelectAllPriceList();
            this.txtRegistrInput.Focus();
        }

        private void MenuSetings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsForm = new Settings();
            settingsForm.ShowDialog();
        }
    }

}
