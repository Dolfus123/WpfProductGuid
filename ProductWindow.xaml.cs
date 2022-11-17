using ProductGuide1._0.Model;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ProductGuide1._0
{
    public partial class ProductWindow : Window
    {
        public Product Product { get; private set; }
        public String? price { get; private set; }
        public ProductWindow(Product product)
        {
            InitializeComponent();
            Product = product;
            DataContext = product;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TextBoxName.Text) && !String.IsNullOrEmpty(TextBoxPrice.Text)
                && !String.IsNullOrEmpty(TextBoxBarCode.Text) && !String.IsNullOrEmpty(TextBoxModel.Text)
                && !String.IsNullOrEmpty(TextBoxSort.Text) && !String.IsNullOrEmpty(TextBoxColor.Text)
                && !String.IsNullOrEmpty(TextBoxSize.Text) && !String.IsNullOrEmpty(TextBoxWight.Text))
            {
                DialogResult = true;
                price = TextBoxPrice.Text;
            }
            else
            {
                warning.Content = "Все поля обязательны к заполнению!";
            }
        }
    }
}
