using Divanru.Commands.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Divanru
{
    internal class MainWindowViewModel : ViewModel
    {
        private Furniture furniture = new Furniture();
        private Categories categories = new Categories();
        private Products products = new Products();
        private SFurniture[] sFurnitureTable;
        private DB db = new DB();

        private const string productUrl = "https://www.divan.ru/ekaterinburg/product/";
        private const string categoryUrl = "https://www.divan.ru/ekaterinburg/category/";

        #region Title
        private string _title = "Divanru parser";
        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get { return _title; }
            set => Set(ref _title, value);
        }
        #endregion

        #region Status
        private string _status = "Ready";
        public string Status
        { 
            get { return _status; }
            set => Set(ref _status, value);
        }
        #endregion

        #region Categories
        private ObservableCollection<string> _categoriesListBox = new ObservableCollection<string>();
        public ObservableCollection<string> CategoriesListBox
        {
            get {return _categoriesListBox; }
            set { Set(ref _categoriesListBox, value); }
        }
        private int _selectedCat = -1;
        public int SelectedCat
        { 
            get { return _selectedCat; } 
            set { Set(ref _selectedCat, value); }
        }
        #endregion

        #region Products
        private ObservableCollection<string> _productsListBox = new ObservableCollection<string>();
        public ObservableCollection<string> ProductsListBox
        {
            get {return _productsListBox; }
            set { Set(ref _productsListBox, value); }
        }
        private int _selectedProduct = -1;
        public int SelectedProduct
        {
            get { return _selectedProduct; }
            set { Set(ref _selectedProduct, value); }
        }
        private string _productsCount;
        public string ProductsCount
        {
            get { return "Products: " + _productsCount; }
            set { Set(ref _productsCount, value); }
        }
        #endregion

        #region DBlist
        private ObservableCollection<string> _dbProductsList = new ObservableCollection<string>();
        public ObservableCollection<string> DBProductsList
        {
            get { return _dbProductsList; }
            set { Set(ref _dbProductsList, value); }
        }
        private int _selectedDBProduct = -1;
        public int SelectedDBProduct
        {
            get { return _selectedDBProduct; }
            set { Set(ref _selectedDBProduct, value); }
        }
        #endregion

        #region SearchTextBox
        private string _searchText = "";
        public string SearchText
        {
            get { return _searchText; }
            set { Set(ref _searchText, value); }
        }
        #endregion

        #region Notifications
        private ObservableCollection<string> _notifications = new ObservableCollection<string>();
        public ObservableCollection<string> Notifications
        {
            get { return _notifications; }
            set { Set(ref _notifications, value, "Nots"); }
        }
        //private int _selectedLog;
        //public int SelectedLog
        //{
        //    get { return _selectedLog; }
        //    set { Set(ref _selectedLog, value); }
        //}
        private void NotificationWrite(object sender, EventArgs e)
        {
            Notifications.Add(e.ErrorText);            
        }
        #endregion

        #region Controls Enabled/Disabled
        private bool _controlsEnabled = true;
        public bool ControlsEnabled
        { 
            get { return _controlsEnabled; }
            set { Set(ref _controlsEnabled, value); }
        }
        #endregion

        #region Cursor
        private Cursor _cursor = Cursors.Arrow;
        public Cursor WinCursor 
        {
            get { return _cursor; }
            set { Set(ref _cursor, value); }
        }
        #endregion

        #region Description labels
        private string _cat0 = "";
        public string Cat0
        {
            get { return _cat0; }
            set { Set(ref _cat0, value); }
        }

        private string _model = "";
        public string Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
        }

        private string _price = "";
        public string Price
        {
            get { return _price; }
            set { Set(ref _price, value); }
        }

        private string _oldPrice = "";
        public string OldPrice
        {
            get { return _oldPrice; }
            set { Set(ref _oldPrice, value); }
        }

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private string _size = "";
        public string Size
        {
            get { return _size; }
            set { Set(ref _size, value); }
        }

        private string _ch0;
        public string Ch0
        {
            get { return _ch0; }
            set { Set(ref _ch0, value); }
        }

        private string _ch1;
        public string Ch1
        {
            get { return _ch1; }
            set { Set(ref _ch1, value); }
        }

        private string _ch2;
        public string Ch2
        {
            get { return _ch2; }
            set { Set(ref _ch2, value); }
        }

        private string _ch3;
        public string Ch3
        {
            get { return _ch3; }
            set { Set(ref _ch3, value); }
        }

        private string _ch4;
        public string Ch4
        {
            get { return _ch4; }
            set { Set(ref _ch4, value); }
        }

        private string _ch5;
        public string Ch5
        {
            get { return _ch5; }
            set { Set(ref _ch5, value); }
        }

        private string _ch6;
        public string Ch6
        {
            get { return _ch6; }
            set { Set(ref _ch6, value); }
        }

        private string _ch7;
        public string Ch7
        {
            get { return _ch7; }
            set { Set(ref _ch7, value); }
        }

        private string _ch8;
        public string Ch8
        {
            get { return _ch8; }
            set { Set(ref _ch8, value); }
        }

        private string _ch9;
        public string Ch9
        {
            get { return _ch9; }
            set { Set(ref _ch9, value); }
        }

        private string _ch10;
        public string Ch10
        {
            get { return _ch10; }
            set { Set(ref _ch10, value); }
        }

        private string _ch11;
        public string Ch11
        {
            get { return _ch11; }
            set { Set(ref _ch11, value); }
        }

        private string _ch12;
        public string Ch12
        {
            get { return _ch12; }
            set { Set(ref _ch12, value); }
        }

        private string _ch13;
        public string Ch13
        {
            get { return _ch13; }
            set { Set(ref _ch13, value); }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { Set(ref _link, value); }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }
        #endregion

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommandExecute(object p) => true;
        private void OnCloseApplicationCommandExecuted (object p) => Application.Current.Shutdown();
        #endregion

        #region ProgressBar
        private int _progressBarMax = 0;
        public int ProgressBarMax
        {
            get { return _progressBarMax; }
            set { Set(ref _progressBarMax, value); }
        }
        private int _progressBarValue = 0;
        public int ProgressBarValue
        { 
            get { return _progressBarValue; } 
            set { Set(ref _progressBarValue, value); } 
        }
        private bool _barEnabled = false;
        public bool BarEnabled
        { 
            get { return _barEnabled; } 
            set { Set(ref _barEnabled, value);} 
        }
        private void SetProgressBar(object sender, CatParsingEventArgs e)
        {
            ProgressBarMax = e.MaxVal;
            ProgressBarValue = e.Val;
        }
        #endregion

        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            FindCategoriesCommand = new LambdaCommand(OnFindCategoriesCommandExecuted, CanFindCategoriesCommand);
            ParseSelectedCategoryCommand = new LambdaCommand (OnParseSelectedCategoryCommandExecuted, CanParseSelectedCategoryCommand);
            OpenCategorySiteCommand = new LambdaCommand(OnOpenCategorySiteExecuted, CanOpenCategorySite);
            OpenProductSiteCommand = new LambdaCommand(OnOpenProductSiteExecuted, CanOpenProductSite);
            OpenDescriptonProductSiteCommand = new LambdaCommand(OnOpenDescriptonProductSiteExecuted, CanOpenDescriptonProductSite);
            ParseAllCategoriesCommand = new LambdaCommand(OnParseAllCategoriesCommandExecuted, CanParseAllCategoriesCommand);
            CopyCatToDbCommand = new LambdaCommand(OnCopyCatToDbCommandExecuted, CanCopyCatToDbCommand);
            LoadSelectedProductCommand = new LambdaCommand(OnLoadSelectedProductCommandExecuted, CanLoadSelectedProductCommand);
            CopyProductToDbCommand = new LambdaCommand(OnCopyProductToDbExecuted, CanCopyProductToDb);
            SearchProductsInDBCommand = new LambdaCommand(OnSearchProductsInDBExecuted, CanSearchProductsInDB);
            OpenProductFromDBCommand = new LambdaCommand(OnOpenProductFromDBExecuted, CanOpenProductFromDB);
            DeleteProductFromDBCommand = new LambdaCommand(OnDeleteProductFromDB,CanDeleteProductFromDB);
        }

        #region FindCategoriesCommand
        public ICommand FindCategoriesCommand { get; }
        private bool CanFindCategoriesCommand(object p) => true;
        private async void OnFindCategoriesCommandExecuted(object p)
        {
            DisableControls();
            categories.OnError += new EventHandler<EventArgs>(NotificationWrite);
            await categories.FindCategories();
            categories.OnError -= new EventHandler<EventArgs>(NotificationWrite);
            CategoriesListBox = categories.GetList();
            EnableControls();
        }
        #endregion

        #region ParseSelectedCategoryCommand
        public ICommand ParseSelectedCategoryCommand { get; }
        private bool CanParseSelectedCategoryCommand(object p) => true;
        private async void OnParseSelectedCategoryCommandExecuted(object p)
        {
            if (SelectedCat == -1) return;
            DisableControls();
            products.Clear();
            ProgressBarValue = 0;
            categories.OnError += new EventHandler<EventArgs>(NotificationWrite);
            categories.OnParsing += new EventHandler<CatParsingEventArgs>(SetProgressBar);
            products.AddRange(await categories.ParseProductsOneCat(categoryUrl + categories[SelectedCat].Link));
            categories.OnError -= new EventHandler<EventArgs>(NotificationWrite);
            categories.OnParsing -= new EventHandler<CatParsingEventArgs>(SetProgressBar);
            ProductsListBox = products.GetList();
            ProductsCount = ProductsListBox.Count.ToString();
            EnableControls();
        }
        #endregion

        #region ParseAllCategoriesCommand
        public ICommand ParseAllCategoriesCommand { get; }
        private bool CanParseAllCategoriesCommand(object p) => true;
        private async void OnParseAllCategoriesCommandExecuted(object p)
        {
            if (categories.Count == 0) return;
            DisableControls();
            int counter = 0;
            products.Clear();
            ProductsListBox.Clear();
            ProgressBarMax = 9;
            //ProgressBarMax = categories.Count;
            for (int i = 0; i < 9; i++)                //для демонстрации стоит ограничение только на 9 катогорий
            //foreach (var cat in categories)
            {
                categories.OnError += new EventHandler<EventArgs>(NotificationWrite);
                products.AddRange(await categories.ParseProductsOneCat(categoryUrl + categories[i].Link));
                categories.OnError -= new EventHandler<EventArgs>(NotificationWrite);
                for (; counter < products.Count; counter++)
                {
                    ProductsListBox.Add(products[counter].Title);
                    ProductsCount = ProductsListBox.Count.ToString();
                }
                ProgressBarValue = i + 1;
            }
            products.OrderByTitle();

            ProductsListBox.Clear();
            for (int i = 0; i < products.Count - 1; i++)
            {
                if (products[i + 1].Title == products[i].Title)
                    products.RemoveAt(i + 1);
            }
            ProductsListBox = products.GetList();
            ProductsCount = ProductsListBox.Count.ToString();
            
            EnableControls();
        }
        #endregion

        #region CopyCatToDbCommand
        public ICommand CopyCatToDbCommand { get; }
        private bool CanCopyCatToDbCommand(object p) => true;
        private async void OnCopyCatToDbCommandExecuted(object p)
        {
            if (SelectedCat == -1) return;
            products.Clear();
            ProductsListBox.Clear();
            ProductsCount = "";
            DisableControls();
            categories.OnError += new EventHandler<EventArgs>(NotificationWrite);
            products.AddRange(await categories.ParseProductsOneCat(categoryUrl + categories[SelectedCat].Link));
            categories.OnError -= new EventHandler<EventArgs>(NotificationWrite);
            foreach (var product in products)
            {
                ProductsListBox.Add(product.Title);
                ProductsCount = ProductsListBox.Count + " of " + products.Count;
                products.OnError += new EventHandler<EventArgs>(NotificationWrite);
                await products.GetOneProduct(productUrl + product.Link, furniture);
                products.OnError -= new EventHandler<EventArgs>(NotificationWrite);
                WriteLabels();
                db.OnError += new EventHandler<EventArgs>(NotificationWrite);
                db.CopyProductToDB(furniture);
                db.OnError -= new EventHandler<EventArgs>(NotificationWrite);
            }
            EnableControls();
        }
        #endregion

        #region OpenCategorySiteCommand
        public ICommand OpenCategorySiteCommand { get; }
        private bool CanOpenCategorySite(object p) => true;
        private void OnOpenCategorySiteExecuted(object p)
        {
            if (SelectedCat == -1) return;
            System.Diagnostics.Process.Start("explorer.exe", categoryUrl + categories[SelectedCat].Link);
        }
        #endregion

        #region OpenProductSiteCommand
        public ICommand OpenProductSiteCommand { get; }
        private bool CanOpenProductSite(object p) => true;
        private void OnOpenProductSiteExecuted(object p)
        {
            if (SelectedProduct == -1) return;
            System.Diagnostics.Process.Start("explorer.exe", productUrl + products[SelectedProduct].Link);
        }
        #endregion

        #region OpenDescriptonProductSiteCommand
        public ICommand OpenDescriptonProductSiteCommand { get; }
        private bool CanOpenDescriptonProductSite(object p) => true;
        private void OnOpenDescriptonProductSiteExecuted(object p)
        {
            if (furniture.Link == null) return;
            System.Diagnostics.Process.Start("explorer.exe", productUrl + furniture.Link);
        }
        #endregion

        #region LoadSelectedProductCommand
        public ICommand LoadSelectedProductCommand { get; }
        private bool CanLoadSelectedProductCommand(object p) => true;
        private async void OnLoadSelectedProductCommandExecuted(object p)
        {
            if (SelectedProduct == -1) return;
            DisableControls();
            products.OnError += new EventHandler<EventArgs>(NotificationWrite);
            await products.GetOneProduct(productUrl + products[SelectedProduct].Link, furniture);
            products.OnError -= new EventHandler<EventArgs>(NotificationWrite);
            WriteLabels();
            EnableControls();
        }
        #endregion

        #region CopyProductToDbCommand
        public ICommand CopyProductToDbCommand { get; }
        private bool CanCopyProductToDb(object p) => true;
        private async void OnCopyProductToDbExecuted(object p)
        {
            if (SelectedProduct == -1) return;
            DisableControls();
            products.OnError += new EventHandler<EventArgs>(NotificationWrite);
            await products.GetOneProduct(productUrl + products[SelectedProduct].Link, furniture);
            products.OnError -= new EventHandler<EventArgs>(NotificationWrite);
            WriteLabels();
            EnableControls();
            db.OnError += new EventHandler<EventArgs>(NotificationWrite);
            db.CopyProductToDB(furniture);
            db.OnError -= new EventHandler<EventArgs>(NotificationWrite);
        }

        
        #endregion

        #region SearchProductsInDBCommand
        public ICommand SearchProductsInDBCommand { get; }
        private bool CanSearchProductsInDB(object p) => true;
        private void OnSearchProductsInDBExecuted(object p)
        {
            DBProductsList.Clear();
            db.OnError += new EventHandler<EventArgs>(NotificationWrite);
            sFurnitureTable = db.SearchInDb(SearchText);
            db.OnError -= new EventHandler<EventArgs>(NotificationWrite);
            if (sFurnitureTable != null)
                DBProductsList = SFurniture.GetModels(sFurnitureTable);
        }
        #endregion

        #region OpenProductFromDBCommand
        public ICommand OpenProductFromDBCommand { get; }
        private bool CanOpenProductFromDB(object p) => true;
        private void OnOpenProductFromDBExecuted(object p)
        {
            if (SelectedDBProduct == -1) return;
            DisableControls();
            db.OnError += new EventHandler<EventArgs>(NotificationWrite);
            db.OpenProductFromDB(sFurnitureTable[SelectedDBProduct].Id, furniture);
            db.OnError -= new EventHandler<EventArgs>(NotificationWrite);
            WriteLabels();
            EnableControls();
        }
        #endregion

        #region DeleteProductFromDBCommand
        public ICommand DeleteProductFromDBCommand { get; }
        private bool CanDeleteProductFromDB(object p) => true;
        private void OnDeleteProductFromDB(object p)
        {
            if(SelectedDBProduct == -1) return;
            db.OnError += new EventHandler<EventArgs>(NotificationWrite);
            db.DeleteProductFromDB(sFurnitureTable[SelectedDBProduct].Id, sFurnitureTable[SelectedDBProduct].Model);
            OnSearchProductsInDBExecuted(p);
            db.OnError -= new EventHandler<EventArgs>(NotificationWrite);
        }
        #endregion

        #region write labels
        private void WriteLabels()
        {
            Cat0 = (furniture.Categories?.Length > 0 ? furniture.Categories[0] : "") + (furniture.Categories?.Length > 1 ? " / " + furniture.Categories[1] : "") + (furniture.Categories?.Length > 2 ? " / " + furniture.Categories[2] : "").Trim('/');
            Model = furniture.Model;
            Price = furniture.Price ?? "";
            OldPrice = furniture.OldPrice ?? "";
            Description = (furniture.Description) ?? "";
            Size = (furniture.Size?.Length > 0 ? furniture.Size[0] : "") + (furniture.Size?.Length > 1 ? " x " + furniture.Size[1] : "") + (furniture.Size?.Length > 2 ? " x " + furniture.Size[2] : "");
            Ch0 = furniture.Characteristics?.Length > 0 ? furniture.Characteristics[0] : "";
            Ch1 = furniture.Characteristics?.Length > 1 ? furniture.Characteristics[1] : "";
            Ch2 = furniture.Characteristics?.Length > 2 ? furniture.Characteristics[2] : "";
            Ch3 = furniture.Characteristics?.Length > 3 ? furniture.Characteristics[3] : "";
            Ch4 = furniture.Characteristics?.Length > 4 ? furniture.Characteristics[4] : "";
            Ch5 = furniture.Characteristics?.Length > 5 ? furniture.Characteristics[5] : "";
            Ch6 = furniture.Characteristics?.Length > 6 ? furniture.Characteristics[6] : "";
            Ch7 = furniture.Characteristics?.Length > 7 ? furniture.Characteristics[7] : "";
            Ch8 = furniture.Characteristics?.Length > 8 ? furniture.Characteristics[8] : "";
            Ch9 = furniture.Characteristics?.Length > 9 ? furniture.Characteristics[9] : "";
            Ch10 = furniture.Characteristics?.Length > 10 ? furniture.Characteristics[10] : "";
            Ch11 = furniture.Characteristics?.Length > 11 ? furniture.Characteristics[11] : "";
            Ch12 = furniture.Characteristics?.Length > 12 ? furniture.Characteristics[12] : "";
            Ch13 = furniture.Characteristics?.Length > 13 ? furniture.Characteristics[13] : "";
            Link = furniture.Link ?? "";
            Image = ToBitmapImage(furniture.Image);
        }
        #endregion

        #region convert byte[] to bitmap
        public static BitmapImage ToBitmapImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
                if (img.CanFreeze)
                    img.Freeze();
                return img;
            }
        }
        #endregion

        private void DisableControls()
        {
            ControlsEnabled = false;
            WinCursor = Cursors.Wait;
            Status = "BUSY";
        }
        private void EnableControls()
        {
            ControlsEnabled = true;
            WinCursor = Cursors.Arrow;
            Status = "Ready";
        }

    }

}
