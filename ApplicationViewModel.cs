using Microsoft.EntityFrameworkCore;
using ProductGuide1._0.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace ProductGuide1._0
{
    internal class ApplicationViewModel : INotifyPropertyChanged
    {
        ApplicationContext db = new ApplicationContext();
       
        RelayCommand? addCommand;
        RelayCommand? editCommand;
        RelayCommand? deleteCommand;

        public ICollectionView ProductColellection;
        
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<PriceProduct> Prices { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _switchSort = string.Empty;
        private string _filterString = string.Empty;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                OnPropertyChanged("FilterString");

                switch (_switchSort)
                {
                    case "Наименование":
                        ProductColellection.Filter = FilterName;
                        break;
                    case "Цена":
                        ProductColellection.Filter = FilterPrice;
                        break;
                    case "Код":
                        ProductColellection.Filter = FilterCode;
                        break;
                    case "БарКод":
                        ProductColellection.Filter = FilterBarCode;
                        break;
                }
            }
        }
        public string SwitchSort
        {
            get { return _switchSort; }
            set
            {
                _switchSort = value;
                OnPropertyChanged("SwitchSort");
            }
        }
        public bool FilterName(object value)
        {
            var entry = value as Product;
            return entry != null && (string.IsNullOrEmpty(_filterString) || entry.Name.ToLower().Contains(_filterString.ToLower()));
        }
        public bool FilterPrice(object value)
        {
            var entry = value as Product;
            return entry != null && (string.IsNullOrEmpty(_filterString) || entry.Price.Price.ToString().Contains(_filterString.ToLower()));
        }
        public bool FilterCode(object value)
        {
            var entry = value as Product;
            return entry != null && (string.IsNullOrEmpty(_filterString) || entry.Code.ToString().Contains(_filterString.ToLower()));
        }
        public bool FilterBarCode(object value)
        {
            var entry = value as Product;
            return entry != null && (string.IsNullOrEmpty(_filterString) || entry.BarCode.ToString().Contains(_filterString.ToLower()));
        }
        public ApplicationViewModel()
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            AddNewCollection();

            db.Products.Load();
            db.Prices.Load();
            Products = db.Products.Local.ToObservableCollection();
            Prices = db.Prices.Local.ToObservableCollection();

            ProductColellection = CollectionViewSource.GetDefaultView(Products);
        }
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      ProductWindow productWindow = new ProductWindow(new Product());
                      if (productWindow.ShowDialog() == true)
                      {
                          Product product = productWindow.Product;
                          
                          Decimal price = decimal.Parse(s: productWindow.price);
                          Boolean flag = false;
                          for (int i = 0; i < Prices.Count; i++)
                          {
                              if (Prices[i].Price== price)
                              {
                                  product.price = Prices[i];
                                  flag = true;
                                  break;
                              }
                          }
                          if (!flag)
                          {
                              db.Prices.Add(new PriceProduct { Price = price });
                              product.price = Prices.Last();
                          }
                          
                          db.Products.Add(product);
                          db.SaveChanges();
                      }
                  }));
            }
        }
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {
                      Product? product = selectedItem as Product;
                      if (product == null) return;

                      Product vm = new Product
                      {
                          Id = product.Id,
                          Name = product.Name,
                          Code = product.Code,
                          Size = product.Size,
                          Quantity = product.Quantity,
                          IdPrice = product.IdPrice,
                          Price = product.price,
                          BarCode = product.BarCode,
                          Model = product.Model,
                          Sort = product.Sort,
                          Wight = product.Wight,
                          Color = product.Color,
                      };
                      ProductWindow productWindow = new ProductWindow(vm);

                      if (productWindow.ShowDialog() == true)
                      {
                          product.Name = productWindow.Product.Name;
                          product.Code = productWindow.Product.Code;
                          product.Size = productWindow.Product.Size;  
                          product.Quantity = productWindow.Product.Quantity;
                          product.Price = productWindow.Product.Price;
                          product.Price.Price = productWindow.Product.Price.Price;
                          product.BarCode = productWindow.Product.BarCode;
                          product.Model = productWindow.Product.Model;
                          product.Sort = productWindow.Product.Sort;
                          product.Wight = productWindow.Product.Wight;
                          product.Color = productWindow.Product.Color;
                          db.Entry(product).State = EntityState.Modified;
                          db.SaveChanges();
                      }
                  }));
            }
        }
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      Product? product = selectedItem as Product;
                      if (product == null) return;
                      db.Products.Remove(product);
                      db.SaveChanges();
                  }));
            }
        }
        private void AddNewCollection()
        {
            for (int i = 0; i < 100; i++)
            {
                Random rnd = new Random();
                PriceProduct price = new PriceProduct();
                price.Price = rnd.Next(0, 100);
                db.Prices.AddRange(price);
            }
            db.SaveChanges();
            db.Prices.Load();
            List<PriceProduct> Pr = db.Prices.Local.ToList();

            for (int i = 0; i <= 1000; i++)
            {
                Random rnd = new Random();
                Product prod = new Product();
                prod.Name = "Product_" + i;
                prod.Code = rnd.Next(10000, 99999);
                prod.Size = rnd.Next(100, 200).ToString();
                prod.Quantity = rnd.Next(1, 50);
                prod.BarCode = rnd.Next(1000, 5000).ToString();
                prod.Model = rnd.Next(1, 10).ToString();
                prod.Sort = rnd.Next(1, 4).ToString();
                prod.Color = "#" + i + rnd.Next(1000, 5000).ToString();
                prod.Wight = rnd.Next(50, 250).ToString();

                prod.price = Pr[rnd.Next(1, 100)];

                db.Products.AddRange(prod);
            }
            db.SaveChanges();
            db.Database.EnsureCreated();
        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
