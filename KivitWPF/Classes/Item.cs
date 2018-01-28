using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceListCash.Classes
{
    /// <summary>
    /// Содержит коллекцию , для хранения информации ценников и вывода в DataGrid
    /// </summary>
    public class Item
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Count { get; set; }
        public string KodRegistr { get; set; }
        public string Barcode { get; set; }
        public string GuiId { get; set; } 

        public static ObservableCollection<Item> GetItems()
        {
            ObservableCollection<Item> result = new ObservableCollection<Item>();
            return result;
        }

    }
}
