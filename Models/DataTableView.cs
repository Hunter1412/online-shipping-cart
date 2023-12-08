namespace OnlineShoppingCart.Models
{
    public class DataTableView
    {
        public string IDEditor { get; set; }

        public bool LoadLibrary { get; set; }

        public DataTableView(string iDEditor, bool loadLibrary = true)
        {
            LoadLibrary = loadLibrary;
            IDEditor = iDEditor;
        }

    }
}