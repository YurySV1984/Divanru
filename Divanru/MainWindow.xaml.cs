using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.Collections.ObjectModel;
using Divanru;

namespace Divanru
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //private Categories categories = new Categories(); //лист с категориями
        //private Products products = new Products(); //лист с мебелью
       

        private async void bStartParsing_Click(object sender, RoutedEventArgs e)
        {
            
            //DisableControls();

            await Categories.ParseCats();
            //listBox1.Items.Clear();
            //listBox1.ItemsSource = categories.GetList();
            //for (int i = 0; i < Categories.Count; i++)
                //listBox1.Items.Add(categories[i].title);
            //EnableControls();
        }

        
    }

    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private int _number1;
        public int Number1
        {
            get { return _number1; }
            set { _number1 = value; OnPropertyChanged("Number3"); }
        }
        private int _number2;
        public int Number2
        {
            get { return _number2; }
            set { _number2 = value; OnPropertyChanged("Number3"); }
        }
        public int Number3 { get { return Number1 + Number2; } }

    }

    public class CategoriesVM : INotifyPropertyChanged
    {
        private ListElement _selectedCat;
        public List<ListElement> Categories { get; set; }
        public ListElement SelectedCat
        {
            get { return _selectedCat; }
            set { _selectedCat = value; OnPropertyChanged("SelectedCat"); }
        }

        public CategoriesVM() => Categories = new List<ListElement>();
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        
    }
}




    public class Phone : INotifyPropertyChanged
    {
        private string title;
        private string company;
        private int price;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }
        public string Company
        {
            get { return company; }
            set
            {
                company = value;
                OnPropertyChanged("Company");
            }
        }
        public int Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged("Price");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private Phone selectedPhone;

        public ObservableCollection<Phone> Phones { get; set; }
        public Phone SelectedPhone
        {
            get { return selectedPhone; }
            set
            {
                selectedPhone = value;
                OnPropertyChanged("SelectedPhone");
            }
        }

        public ApplicationViewModel()
        {
            Phones = new ObservableCollection<Phone>
            {
                new Phone { Title="iPhone 7", Company="Apple", Price=56000 },
                new Phone {Title="Galaxy S7 Edge", Company="Samsung", Price =60000 },
                new Phone {Title="Elite x3", Company="HP", Price=56000 },
                new Phone {Title="Mi5S", Company="Xiaomi", Price=35000 }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    
