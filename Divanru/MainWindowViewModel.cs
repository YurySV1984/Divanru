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
        private Categories categories = new Categories();
        private Products products = new Products();
        private SFurniture[] sfurTable;
        private DB db = new DB();

        const string productUrl = "https://www.divan.ru/ekaterinburg/product/";
        const string categoryUrl = "https://www.divan.ru/ekaterinburg/category/";

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
        private void NotificationWrite(object sender, ErrEventArgs e)
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
        //private int _progressBarMin = 0;
        //public int ProgressBarMin
        //{
        //    get { return _progressBarMin; }
        //    set { Set(ref _progressBarMin, value); }
        //}
        private int _progressBarValue = 0;
        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set { Set(ref _progressBarValue, value); }
        }
        private bool _barEnabled = false;
        public bool BarEnabled
        { get { return _barEnabled; } set { Set(ref _barEnabled, value);} }
        
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
            categories.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
            await categories.FindCategories();
            categories.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
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
            categories.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
            categories.OnParsing += new EventHandler<CatParsingEventArgs>(SetProgressBar);
            products.AddRange(await categories.ParseProductsOneCat(categoryUrl + categories[SelectedCat].link));
            categories.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
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
            //for (int i = 0; i < categories.Count; i++)
            //foreach (var cat in categories)
            {
                categories.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
                products.AddRange(await categories.ParseProductsOneCat(categoryUrl + categories[i].link));
                categories.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
                for (; counter < products.Count; counter++)
                {
                    ProductsListBox.Add(products[counter].title);
                    ProductsCount = ProductsListBox.Count.ToString();
                }
                ProgressBarValue = i + 1;
            }
            products.OrdBy();

            ProductsListBox.Clear();
            for (int i = 0; i < products.Count - 1; i++)
            {
                if (products[i + 1].title == products[i].title)
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
            categories.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
            products.AddRange(await categories.ParseProductsOneCat(categoryUrl + categories[SelectedCat].link));
            categories.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
            for (int i = 0; i < products.Count; i++)
            {
                ProductsListBox.Add(products[i].title);
                ProductsCount = ProductsListBox.Count + " of " + products.Count;
                products.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
                await products.GetOneProduct(productUrl + products[i].link);
                products.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
                WriteLabels();
                db.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
                db.CopyProductToDB();
                db.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
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
            System.Diagnostics.Process.Start("explorer.exe", categoryUrl + categories[SelectedCat].link);
        }
        #endregion

        #region OpenProductSiteCommand
        public ICommand OpenProductSiteCommand { get; }
        private bool CanOpenProductSite(object p) => true;
        private void OnOpenProductSiteExecuted(object p)
        {
            if (SelectedProduct == -1) return;
            System.Diagnostics.Process.Start("explorer.exe", productUrl + products[SelectedProduct].link);
        }
        #endregion

        #region OpenDescriptonProductSiteCommand
        public ICommand OpenDescriptonProductSiteCommand { get; }
        private bool CanOpenDescriptonProductSite(object p) => true;
        private void OnOpenDescriptonProductSiteExecuted(object p)
        {
            if (Furniture.Link == null) return;
            System.Diagnostics.Process.Start("explorer.exe", productUrl + Furniture.Link);
        }
        #endregion

        #region LoadSelectedProductCommand
        public ICommand LoadSelectedProductCommand { get; }
        private bool CanLoadSelectedProductCommand(object p) => true;
        private async void OnLoadSelectedProductCommandExecuted(object p)
        {
            if (SelectedProduct == -1) return;
            DisableControls();
            products.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
            await products.GetOneProduct(productUrl + products[SelectedProduct].link);
            products.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
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
            products.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
            await products.GetOneProduct(productUrl + products[SelectedProduct].link);
            products.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
            WriteLabels();
            EnableControls();
            db.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
            db.CopyProductToDB();
            db.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
        }

        
        #endregion

        #region SearchProductsInDBCommand
        public ICommand SearchProductsInDBCommand { get; }
        private bool CanSearchProductsInDB(object p) => true;
        private void OnSearchProductsInDBExecuted(object p)
        {
            DBProductsList.Clear();
            db.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
            sfurTable = db.SearchInDb(SearchText);
            db.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
            if (sfurTable != null)
                DBProductsList = SFurniture.GetModels(sfurTable);
        }
        #endregion

        #region OpenProductFromDBCommand
        public ICommand OpenProductFromDBCommand { get; }
        private bool CanOpenProductFromDB(object p) => true;
        private void OnOpenProductFromDBExecuted(object p)
        {
            if (SelectedDBProduct == -1) return;
            DisableControls();
            db.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
            db.OpenProductFromDB(sfurTable[SelectedDBProduct].id);
            db.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
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
            db.OnError += new EventHandler<ErrEventArgs>(NotificationWrite);
            db.DeleteProductFromDB(sfurTable[SelectedDBProduct].id, sfurTable[SelectedDBProduct].model);
            OnSearchProductsInDBExecuted(p);
            db.OnError -= new EventHandler<ErrEventArgs>(NotificationWrite);
        }
        #endregion

        #region write labels
        private void WriteLabels()
        {
            Cat0 = (Furniture.Categories?.Length > 0 ? Furniture.Categories[0] : "") + (Furniture.Categories?.Length > 1 ? " / " + Furniture.Categories[1] : "") + (Furniture.Categories?.Length > 2 ? " / " + Furniture.Categories[2] : "");
            Model = Furniture.Model;
            Price = Furniture.Price ?? "";
            OldPrice = Furniture.OldPrice ?? "";
            Description = (Furniture.Description) ?? "";
            Size = (Furniture.Size?.Length > 0 ? Furniture.Size[0] : "") + (Furniture.Size?.Length > 1 ? " x " + Furniture.Size[1] : "") + (Furniture.Size?.Length > 2 ? " x " + Furniture.Size[2] : "");
            Ch0 = Furniture.Characteristics?.Length > 0 ? Furniture.Characteristics[0] : "";
            Ch1 = Furniture.Characteristics?.Length > 1 ? Furniture.Characteristics[1] : "";
            Ch2 = Furniture.Characteristics?.Length > 2 ? Furniture.Characteristics[2] : "";
            Ch3 = Furniture.Characteristics?.Length > 3 ? Furniture.Characteristics[3] : "";
            Ch4 = Furniture.Characteristics?.Length > 4 ? Furniture.Characteristics[4] : "";
            Ch5 = Furniture.Characteristics?.Length > 5 ? Furniture.Characteristics[5] : "";
            Ch6 = Furniture.Characteristics?.Length > 6 ? Furniture.Characteristics[6] : "";
            Ch7 = Furniture.Characteristics?.Length > 7 ? Furniture.Characteristics[7] : "";
            Ch8 = Furniture.Characteristics?.Length > 8 ? Furniture.Characteristics[8] : "";
            Ch9 = Furniture.Characteristics?.Length > 9 ? Furniture.Characteristics[9] : "";
            Ch10 = Furniture.Characteristics?.Length > 10 ? Furniture.Characteristics[10] : "";
            Ch11 = Furniture.Characteristics?.Length > 11 ? Furniture.Characteristics[11] : "";
            Ch12 = Furniture.Characteristics?.Length > 12 ? Furniture.Characteristics[12] : "";
            Ch13 = Furniture.Characteristics?.Length > 13 ? Furniture.Characteristics[13] : "";
            Link = Furniture.Link ?? "";
            Image = ToBitmapImage(Furniture.Image);
        }
        #endregion

        #region convert byte[] to bitmap
        public static BitmapImage ToBitmapImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;//CacheOption must be set after BeginInit()
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
