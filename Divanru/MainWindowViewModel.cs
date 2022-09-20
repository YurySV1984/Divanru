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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Divanru
{
    internal class MainWindowViewModel : ViewModel
    {
        private Furniture _furniture = new Furniture();
        private Categories _categories = new Categories();
        private Products _products = new Products();
        private SFurniture[] sFurnitureTable;
        private readonly DB db = new DB();

        private const string productUrl = "https://www.divan.ru/ekaterinburg/product/";
        private const string categoryUrl = "https://www.divan.ru/ekaterinburg/category/";

        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

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
        /// <summary>
        /// Текст на строке состояния
        /// </summary>
        public string Status
        {
            get { return _status; }
            set => Set(ref _status, value);
        }

        private SolidColorBrush _statusColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 244, 243, 241));
        /// <summary>
        /// Цвет строки состояния.
        /// </summary>
        public SolidColorBrush StatusColor
        {
            get { return _statusColor; }
            set => Set(ref _statusColor, value);
        }
        #endregion

        #region Categories
        private ObservableCollection<string> _categoriesListBox = new ObservableCollection<string>();
        /// <summary>
        /// Коллекция категорий товаров для отображения.
        /// </summary>
        public ObservableCollection<string> CategoriesListBox
        {
            get { return _categoriesListBox; }
            set { Set(ref _categoriesListBox, value); }
        }
        private int _selectedCat = -1;
        /// <summary>
        /// Выбранная категория.
        /// </summary>
        public int SelectedCat
        {
            get { return _selectedCat; }
            set { Set(ref _selectedCat, value); }
        }
        #endregion

        #region Products
        private ObservableCollection<string> _productsListBox = new ObservableCollection<string>();
        /// <summary>
        /// Коллекция мебели для отображения
        /// </summary>
        public ObservableCollection<string> ProductsListBox
        {
            get { return _productsListBox; }
            set { Set(ref _productsListBox, value); }
        }
        private int _selectedProduct = -1;
        /// <summary>
        /// Выбранная мебель.
        /// </summary>
        public int SelectedProduct
        {
            get { return _selectedProduct; }
            set { Set(ref _selectedProduct, value); }
        }
        private string _productsCount;
        /// <summary>
        /// Количество мебели в коллекции.
        /// </summary>
        public string ProductsCount
        {
            get { return "Products: " + _productsCount; }
            set { Set(ref _productsCount, value); }
        }
        /// <summary>
        /// Заполняет лист с мебелью при парсинге всех категорий товаров.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetProducts(object sender, AllCategoriesParsingArgs e)
        {
            _products = e.Products;
            ProductsListBox = _products.GetList();
            ProductsCount = ProductsListBox.Count.ToString();
        }
        /// <summary>
        /// Заполняет лист с мебелью при копировании категории в БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetProducts(object sender, CopyCatToDBArgs e)
        {
            _furniture = e.Furniture;
            ProductsListBox = _products.GetList();
            ProductsCount = ProductsListBox.Count.ToString();
            WriteLabels();
        }
        #endregion

        #region DBlist
        private ObservableCollection<string> _dbProductsList = new ObservableCollection<string>();
        /// <summary>
        /// Коллекция найденных в БД названий мебели.
        /// </summary>
        public ObservableCollection<string> DBProductsList
        {
            get { return _dbProductsList; }
            set { Set(ref _dbProductsList, value); }
        }
        private int _selectedDBProduct = -1;
        /// <summary>
        /// Выбранное название мебели в коллекции найденных в БД.
        /// </summary>
        public int SelectedDBProduct
        {
            get { return _selectedDBProduct; }
            set { Set(ref _selectedDBProduct, value); }
        }
        #endregion

        #region SearchTextBox
        private string _searchText = "";
        /// <summary>
        /// Поле поиска мебели в БД.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set { Set(ref _searchText, value); }
        }
        #endregion

        #region Notifications
        private ObservableCollection<string> _notifications = new ObservableCollection<string>();
        /// <summary>
        /// Коллекция уведомлений для отображения.
        /// </summary>
        public ObservableCollection<string> Notifications
        {
            get { return _notifications; }
            set { Set(ref _notifications, value, "Nots"); }
        }
        /// <summary>
        /// Добавляет уведомление в коллекцию.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotificationWrite(object sender, NotificationEventArgs e)
        {
            Notifications.Add(e.Text);
        }
        #endregion

        #region Controls Enabled/Disabled
        private bool _controlsEnabled = true;
        /// <summary>
        /// Включение/выключение элементов упавления.
        /// </summary>
        public bool ControlsEnabled
        {
            get { return _controlsEnabled; }
            set { Set(ref _controlsEnabled, value); }
        }
        private bool _cancelEnabled = false;
        /// <summary>
        /// Включает/выключает доступность кнопки прерывания операции.
        /// </summary>
        public bool CancelEnabled
        {
            get { return _cancelEnabled; }
            set { Set(ref _cancelEnabled, value); }
        }
        #endregion

        #region Cursor
        private Cursor _cursor = Cursors.Arrow;
        /// <summary>
        /// Вид курсора мыши.
        /// </summary>
        public Cursor WinCursor
        {
            get { return _cursor; }
            set { Set(ref _cursor, value); }
        }
        #endregion

        #region Description labels
        private string _cat0 = "";
        /// <summary>
        /// Категория мебели.
        /// </summary>
        public string Cat0
        {
            get { return _cat0; }
            set { Set(ref _cat0, value); }
        }

        private string _model = "";
        /// <summary>
        /// Название мебели.
        /// </summary>
        public string Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
        }

        private string _price = "";
        /// <summary>
        /// Цена мебели.
        /// </summary>
        public string Price
        {
            get { return _price; }
            set { Set(ref _price, value); }
        }

        private string _oldPrice = "";
        /// <summary>
        /// Старая цена мебели.
        /// </summary>
        public string OldPrice
        {
            get { return _oldPrice; }
            set { Set(ref _oldPrice, value); }
        }

        private string _description = "";
        /// <summary>
        /// Описание мебели.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private string _size = "";
        /// <summary>
        /// Размеры мебели.
        /// </summary>
        public string Size
        {
            get { return _size; }
            set { Set(ref _size, value); }
        }

        private string _ch0;
        /// <summary>
        /// Характеристики 0
        /// </summary>
        public string Ch0
        {
            get { return _ch0; }
            set { Set(ref _ch0, value); }
        }

        private string _ch1;
        /// <summary>
        /// Характеристики 1
        /// </summary>
        public string Ch1
        {
            get { return _ch1; }
            set { Set(ref _ch1, value); }
        }

        private string _ch2;
        /// <summary>
        /// Характеристики 2
        /// </summary>
        public string Ch2
        {
            get { return _ch2; }
            set { Set(ref _ch2, value); }
        }

        private string _ch3;
        /// <summary>
        /// Характеристики 3
        /// </summary>
        public string Ch3
        {
            get { return _ch3; }
            set { Set(ref _ch3, value); }
        }

        private string _ch4;
        /// <summary>
        /// Характеристики 4
        /// </summary>
        public string Ch4
        {
            get { return _ch4; }
            set { Set(ref _ch4, value); }
        }

        private string _ch5;
        /// <summary>
        /// Характеристики 5
        /// </summary>
        public string Ch5
        {
            get { return _ch5; }
            set { Set(ref _ch5, value); }
        }

        private string _ch6;
        /// <summary>
        /// Характеристики 6
        /// </summary>
        public string Ch6
        {
            get { return _ch6; }
            set { Set(ref _ch6, value); }
        }

        private string _ch7;
        /// <summary>
        /// Характеристики 7
        /// </summary>
        public string Ch7
        {
            get { return _ch7; }
            set { Set(ref _ch7, value); }
        }

        private string _ch8;
        /// <summary>
        /// Характеристики 8
        /// </summary>
        public string Ch8
        {
            get { return _ch8; }
            set { Set(ref _ch8, value); }
        }

        private string _ch9;
        /// <summary>
        /// Характеристики 9
        /// </summary>
        public string Ch9
        {
            get { return _ch9; }
            set { Set(ref _ch9, value); }
        }

        private string _ch10;
        /// <summary>
        /// Характеристики 10
        /// </summary>
        public string Ch10
        {
            get { return _ch10; }
            set { Set(ref _ch10, value); }
        }

        private string _ch11;
        /// <summary>
        /// Характеристики 11
        /// </summary>
        public string Ch11
        {
            get { return _ch11; }
            set { Set(ref _ch11, value); }
        }

        private string _ch12;
        /// <summary>
        /// Характеристики 12
        /// </summary>
        public string Ch12
        {
            get { return _ch12; }
            set { Set(ref _ch12, value); }
        }

        private string _ch13;
        /// <summary>
        /// Характеристики 13
        /// </summary>
        public string Ch13
        {
            get { return _ch13; }
            set { Set(ref _ch13, value); }
        }

        private string _link;
        /// <summary>
        /// Ссылка на сайт.
        /// </summary>
        public string Link
        {
            get { return _link; }
            set { Set(ref _link, value); }
        }

        private BitmapImage _image;
        /// <summary>
        /// Изображение мебели
        /// </summary>
        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }
        #endregion

        #region CloseApplicationCommand
        /// <summary>
        /// Команда закрытия приложения
        /// </summary>
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommandExecute(object p) => true;
        private void OnCloseApplicationCommandExecuted(object p) => Application.Current.Shutdown();
        #endregion

        #region ProgressBar
        private int _progressBarMax = 0;
        /// <summary>
        /// Максимальное значение прогрессбара.
        /// </summary>
        public int ProgressBarMax
        {
            get { return _progressBarMax; }
            set { Set(ref _progressBarMax, value); }
        }
        private int _progressBarValue = 0;
        /// <summary>
        /// Текущее значение прогрессбара.
        /// </summary>
        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set { Set(ref _progressBarValue, value); }
        }
        /// <summary>
        /// Устанавливает значения прогрессбара.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetProgressBar(object sender, IProgressBarEventArgs e)
        {
            ProgressBarMax = e.MaxVal;
            ProgressBarValue = e.Val;
        }
        ///// <summary>
        ///// Устанавливает значения прогрессбара при парсинге всех категорий мебели.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void SetProgressBar(object sender, AllCategoriesParsingArgs e)
        //{
        //    ProgressBarMax = e.MaxVal;
        //    ProgressBarValue = e.Val;
        //}
        ///// <summary>
        ///// Устанавливает значения прогрессбара при копировании категории мебели в БД.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void SetProgressBar(object sender, CopyCatToDBArgs e)
        //{
        //    ProgressBarMax = e.MaxVal;
        //    ProgressBarValue = e.Val;
        //}
        #endregion

        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            FindCategoriesCommand = new LambdaCommand(OnFindCategoriesCommandExecuted, CanFindCategoriesCommand);
            ParseSelectedCategoryCommand = new LambdaCommand(OnParseSelectedCategoryCommandExecuted, CanParseSelectedCategoryCommand);
            OpenCategorySiteCommand = new LambdaCommand(OnOpenCategorySiteExecuted, CanOpenCategorySite);
            OpenProductSiteCommand = new LambdaCommand(OnOpenProductSiteExecuted, CanOpenProductSite);
            //OpenDescriptonProductSiteCommand = new LambdaCommand(OnOpenDescriptonProductSiteExecuted, CanOpenDescriptonProductSite);
            ParseAllCategoriesCommand = new LambdaCommand(OnParseAllCategoriesCommandExecuted, CanParseAllCategoriesCommand);
            CopyCatToDbCommand = new LambdaCommand(OnCopyCatToDbCommandExecuted, CanCopyCatToDbCommand);
            LoadSelectedProductCommand = new LambdaCommand(OnLoadSelectedProductCommandExecuted, CanLoadSelectedProductCommand);
            CopyProductToDbCommand = new LambdaCommand(OnCopyProductToDbExecuted, CanCopyProductToDb);
            SearchProductsInDBCommand = new LambdaCommand(OnSearchProductsInDBExecuted, CanSearchProductsInDB);
            OpenProductFromDBCommand = new LambdaCommand(OnOpenProductFromDBExecuted, CanOpenProductFromDB);
            DeleteProductFromDBCommand = new LambdaCommand(OnDeleteProductFromDB, CanDeleteProductFromDB);
            CancelOperationCommand = new LambdaCommand(OnCancelOperationExecuted, CanCancelOperation);
        }

        #region FindCategoriesCommand
        /// <summary>
        /// Команда "Find cateogries".
        /// </summary>
        public ICommand FindCategoriesCommand { get; }
        private bool CanFindCategoriesCommand(object p) => true;
        private async void OnFindCategoriesCommandExecuted(object p)
        {
            DisableControls();
            _categories.OnError += new EventHandler<NotificationEventArgs>(NotificationWrite);
            await _categories.FindCategoriesAsync();
            _categories.OnError -= new EventHandler<NotificationEventArgs>(NotificationWrite);
            CategoriesListBox = _categories.GetTitleList();
            EnableControls();
        }
        #endregion

        #region ParseSelectedCategoryCommand
        /// <summary>
        /// Команда "Parse selected category".
        /// </summary>
        public ICommand ParseSelectedCategoryCommand { get; }
        private bool CanParseSelectedCategoryCommand(object p) => true;
        private async void OnParseSelectedCategoryCommandExecuted(object p)
        {
            if (SelectedCat == -1) return;
            DisableControls();
            CancelEnabled = true;
            _products.Clear();
            ProgressBarValue = 0;
            _categories.OnError += new EventHandler<NotificationEventArgs>(NotificationWrite);
            _categories.OnParsing += new EventHandler<CatParsingEventArgs>(SetProgressBar);
            CancellationToken cancellationToken = cancelTokenSource.Token;
            _products.AddRange(await _categories.ParseProductsOneCatAsync(categoryUrl + _categories[SelectedCat].Link, cancellationToken));
            cancelTokenSource.Dispose();
            cancelTokenSource = new CancellationTokenSource();
            _categories.OnError -= new EventHandler<NotificationEventArgs>(NotificationWrite);
            _categories.OnParsing -= new EventHandler<CatParsingEventArgs>(SetProgressBar);
            ProductsListBox = _products.GetList();
            ProductsCount = ProductsListBox.Count.ToString();
            EnableControls();
            CancelEnabled = false;
        }
        #endregion

        #region ParseAllCategoriesCommand
        /// <summary>
        /// Команда "Parse ALL the cateogries".
        /// </summary>
        public ICommand ParseAllCategoriesCommand { get; }
        private bool CanParseAllCategoriesCommand(object p) => true;
        private async void OnParseAllCategoriesCommandExecuted(object p)
        {
            if (_categories.Count == 0) return;
            DisableControls();
            CancelEnabled = true;
            _categories.OnError += new EventHandler<NotificationEventArgs>(NotificationWrite);
            _categories.OnAllCategoriesParsing += new EventHandler<AllCategoriesParsingArgs>(SetProgressBar);
            _categories.OnAllCategoriesParsing += new EventHandler<AllCategoriesParsingArgs>(SetProducts);
            CancellationToken cancellationToken = cancelTokenSource.Token;
            await _categories.ParseAllCategories(cancellationToken);
            cancelTokenSource.Dispose();
            cancelTokenSource = new CancellationTokenSource();
            _categories.OnError -= new EventHandler<NotificationEventArgs>(NotificationWrite);
            _categories.OnAllCategoriesParsing -= new EventHandler<AllCategoriesParsingArgs>(SetProgressBar);
            _categories.OnAllCategoriesParsing -= new EventHandler<AllCategoriesParsingArgs>(SetProducts);
            ProductsListBox = _products.GetList();
            ProductsCount = ProductsListBox.Count.ToString();
            EnableControls();
            CancelEnabled = false;
        }
        #endregion

        #region CopyCatToDbCommand
        /// <summary>
        /// Команда "Copy selected category to the Database".
        /// </summary>
        public ICommand CopyCatToDbCommand { get; }
        private bool CanCopyCatToDbCommand(object p) => true;
        private async void OnCopyCatToDbCommandExecuted(object p)
        {
            if (SelectedCat == -1) return;
            ProductsListBox.Clear();
            ProductsCount = "";
            DisableControls();
            CancelEnabled = true;

            ProgressBarValue = 0;
            _products.Clear();
            _categories.OnError += new EventHandler<NotificationEventArgs>(NotificationWrite);
            _categories.OnParsing += new EventHandler<CatParsingEventArgs>(SetProgressBar);
            CancellationToken cancellationToken = cancelTokenSource.Token;
            _products.AddRange(await _categories.ParseProductsOneCatAsync(categoryUrl + _categories[SelectedCat].Link, cancellationToken));
            _categories.OnError -= new EventHandler<NotificationEventArgs>(NotificationWrite);
            _categories.OnParsing -= new EventHandler<CatParsingEventArgs>(SetProgressBar);
            _products.OnError += new EventHandler<NotificationEventArgs>(NotificationWrite);

            if (!cancellationToken.IsCancellationRequested)
            {
                db.OnEvent += new EventHandler<NotificationEventArgs>(NotificationWrite);
                db.OnCopyCategoryToDB += new EventHandler<CopyCatToDBArgs>(SetProgressBar);
                db.OnCopyCategoryToDB += new EventHandler<CopyCatToDBArgs>(SetProducts);
                await db.CopyCategoryToDbAsync(_products, cancellationToken);
                db.OnEvent -= new EventHandler<NotificationEventArgs>(NotificationWrite);
                db.OnCopyCategoryToDB -= new EventHandler<CopyCatToDBArgs>(SetProgressBar);
                db.OnCopyCategoryToDB -= new EventHandler<CopyCatToDBArgs>(SetProducts);
            }

            _products.OnError -= new EventHandler<NotificationEventArgs>(NotificationWrite);

            cancelTokenSource.Dispose();
            cancelTokenSource = new CancellationTokenSource();

            CancelEnabled = false;
            EnableControls();
        }
        #endregion

        #region Cancel Operation
        /// <summary>
        /// Команда "Cancel current operation".
        /// </summary>
        public ICommand CancelOperationCommand { get; }
        private bool CanCancelOperation(object p) => true;
        private void OnCancelOperationExecuted(object p)
        {
            cancelTokenSource.Cancel();
        }
        #endregion

        #region OpenCategorySiteCommand
        /// <summary>
        /// Команда "Open category site".
        /// </summary>
        public ICommand OpenCategorySiteCommand { get; }
        private bool CanOpenCategorySite(object p) => true;
        private void OnOpenCategorySiteExecuted(object p)
        {
            if (SelectedCat == -1) return;
            System.Diagnostics.Process.Start("explorer.exe", categoryUrl + _categories[SelectedCat].Link);
        }
        #endregion

        #region OpenProductSiteCommand
        /// <summary>
        /// Команда "Open product site".
        /// </summary>
        public ICommand OpenProductSiteCommand { get; }
        private bool CanOpenProductSite(object p) => true;
        private void OnOpenProductSiteExecuted(object p)
        {
            if (SelectedProduct == -1) return;
            System.Diagnostics.Process.Start("explorer.exe", productUrl + _products[SelectedProduct].Link);
        }
        #endregion

        //#region OpenDescriptonProductSiteCommand
        ///// <summary>
        ///// Команда "".
        ///// </summary>
        //public ICommand OpenDescriptonProductSiteCommand { get; }
        //private bool CanOpenDescriptonProductSite(object p) => true;
        //private void OnOpenDescriptonProductSiteExecuted(object p)
        //{
        //    if (_furniture.Link == null) return;
        //    System.Diagnostics.Process.Start("explorer.exe", productUrl + _furniture.Link);
        //}
        //#endregion

        #region LoadSelectedProductCommand
        /// <summary>
        /// Команда "Load selected product".
        /// </summary>
        public ICommand LoadSelectedProductCommand { get; }
        private bool CanLoadSelectedProductCommand(object p) => true;
        private async void OnLoadSelectedProductCommandExecuted(object p)
        {
            if (SelectedProduct == -1) return;
            DisableControls();
            _products.OnError += new EventHandler<NotificationEventArgs>(NotificationWrite);
            await _products.GetOneProduct(productUrl + _products[SelectedProduct].Link, _furniture);
            _products.OnError -= new EventHandler<NotificationEventArgs>(NotificationWrite);
            WriteLabels();
            EnableControls();
        }
        #endregion

        #region CopyProductToDbCommand
        /// <summary>
        /// Команда "Copy selected product to the Database".
        /// </summary>
        public ICommand CopyProductToDbCommand { get; }
        private bool CanCopyProductToDb(object p) => true;
        private async void OnCopyProductToDbExecuted(object p)
        {
            if (SelectedProduct == -1) return;
            DisableControls();
            _products.OnError += new EventHandler<NotificationEventArgs>(NotificationWrite);
            await _products.GetOneProduct(productUrl + _products[SelectedProduct].Link, _furniture);
            _products.OnError -= new EventHandler<NotificationEventArgs>(NotificationWrite);
            WriteLabels();
            db.OnEvent += new EventHandler<NotificationEventArgs>(NotificationWrite);
            db.CopyProductToDB(_furniture);
            db.OnEvent -= new EventHandler<NotificationEventArgs>(NotificationWrite);
            EnableControls();
        }

        #endregion

        #region SearchProductsInDBCommand
        /// <summary>
        /// Команда "Search products in the database:".
        /// </summary>
        public ICommand SearchProductsInDBCommand { get; }
        private bool CanSearchProductsInDB(object p) => true;
        private void OnSearchProductsInDBExecuted(object p)
        {
            DBProductsList.Clear();
            db.OnEvent += new EventHandler<NotificationEventArgs>(NotificationWrite);
            sFurnitureTable = db.SearchInDb(SearchText);
            db.OnEvent -= new EventHandler<NotificationEventArgs>(NotificationWrite);
            if (sFurnitureTable != null)
                DBProductsList = SFurniture.GetModels(sFurnitureTable);
        }
        #endregion

        #region OpenProductFromDBCommand
        /// <summary>
        /// Команда "Open selected product from DB".
        /// </summary>
        public ICommand OpenProductFromDBCommand { get; }
        private bool CanOpenProductFromDB(object p) => true;
        private void OnOpenProductFromDBExecuted(object p)
        {
            if (SelectedDBProduct == -1) return;
            DisableControls();
            db.OnEvent += new EventHandler<NotificationEventArgs>(NotificationWrite);
            _furniture = db.OpenProductFromDB(sFurnitureTable[SelectedDBProduct].Id);
            db.OnEvent -= new EventHandler<NotificationEventArgs>(NotificationWrite);
            WriteLabels();
            EnableControls();
        }
        #endregion

        #region DeleteProductFromDBCommand
        /// <summary>
        /// Команда "Delete selected product from DB".
        /// </summary>
        public ICommand DeleteProductFromDBCommand { get; }
        private bool CanDeleteProductFromDB(object p) => true;
        private void OnDeleteProductFromDB(object p)
        {
            if (SelectedDBProduct == -1) return;
            db.OnEvent += new EventHandler<NotificationEventArgs>(NotificationWrite);
            db.DeleteProductFromDB(sFurnitureTable[SelectedDBProduct].Id, sFurnitureTable[SelectedDBProduct].Model);
            OnSearchProductsInDBExecuted(p);
            db.OnEvent -= new EventHandler<NotificationEventArgs>(NotificationWrite);
        }
        #endregion

        #region Write labels
        /// <summary>
        /// Меняет поля с описаниями и изображние мебели.
        /// </summary>
        private void WriteLabels()
        {
            Cat0 = (_furniture.Categories?.Length > 0 ? _furniture.Categories[0] : "") + (_furniture.Categories?.Length > 1 ? " / " + _furniture.Categories[1] : "") + (_furniture.Categories?.Length > 2 ? " / " + _furniture.Categories[2] : "").Trim('/');
            Model = _furniture.Model;
            Price = _furniture.Price ?? "";
            OldPrice = _furniture.OldPrice ?? "";
            Description = (_furniture.Description) ?? "";
            Size = (_furniture.Size?.Length > 0 ? _furniture.Size[0] : "") + (_furniture.Size?.Length > 1 ? " x " + _furniture.Size[1] : "") + (_furniture.Size?.Length > 2 ? " x " + _furniture.Size[2] : "");
            Ch0 = _furniture.Characteristics?.Length > 0 ? _furniture.Characteristics[0] : "";
            Ch1 = _furniture.Characteristics?.Length > 1 ? _furniture.Characteristics[1] : "";
            Ch2 = _furniture.Characteristics?.Length > 2 ? _furniture.Characteristics[2] : "";
            Ch3 = _furniture.Characteristics?.Length > 3 ? _furniture.Characteristics[3] : "";
            Ch4 = _furniture.Characteristics?.Length > 4 ? _furniture.Characteristics[4] : "";
            Ch5 = _furniture.Characteristics?.Length > 5 ? _furniture.Characteristics[5] : "";
            Ch6 = _furniture.Characteristics?.Length > 6 ? _furniture.Characteristics[6] : "";
            Ch7 = _furniture.Characteristics?.Length > 7 ? _furniture.Characteristics[7] : "";
            Ch8 = _furniture.Characteristics?.Length > 8 ? _furniture.Characteristics[8] : "";
            Ch9 = _furniture.Characteristics?.Length > 9 ? _furniture.Characteristics[9] : "";
            Ch10 = _furniture.Characteristics?.Length > 10 ? _furniture.Characteristics[10] : "";
            Ch11 = _furniture.Characteristics?.Length > 11 ? _furniture.Characteristics[11] : "";
            Ch12 = _furniture.Characteristics?.Length > 12 ? _furniture.Characteristics[12] : "";
            Ch13 = _furniture.Characteristics?.Length > 13 ? _furniture.Characteristics[13] : "";
            Link = _furniture.Link ?? "";
            Image = ToBitmapImage(_furniture.Image);
        }
        #endregion

        #region convert byte[] to bitmap
        /// <summary>
        /// Конвертирует массив байт в битмап.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Отключает элементы управления, меняет курсор.
        /// </summary>
        private void DisableControls()
        {
            ControlsEnabled = false;
            WinCursor = Cursors.Wait;
            Status = "BUSY";
            StatusColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 200, 215, 100));
        }
        /// <summary>
        /// Включает элементы управления, меняет курсор по умолчанию.
        /// </summary>
        private void EnableControls()
        {
            ControlsEnabled = true;
            WinCursor = Cursors.Arrow;
            Status = "Ready";
            StatusColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 244, 243, 241));
        }
    }
}
