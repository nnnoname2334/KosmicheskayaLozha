using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using KosmicheskayaLozha.Services;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KosmicheskayaLozha.Views
{
    public partial class ProductsWindow : Window
    {
        private List<Product> _allProducts;

        public ProductsWindow()
        {
            InitializeComponent();
            LoadFilters();
            LoadProducts();
        }

        private void LoadFilters()
        {
            using (var db = new AppDbContext())
            {
                var types = db.ProductTypes.ToList();
                types.Insert(0, new ProductType { ProductTypeId = 0, Name = "Все типы" });
                TypeFilterCombo.ItemsSource = types;
                TypeFilterCombo.DisplayMemberPath = "Name";
                TypeFilterCombo.SelectedIndex = 0;

                var manufacturers = db.Manufacturers.ToList();
                manufacturers.Insert(0, new Manufacturer { ManufacturerId = 0, Name = "Все производители" });
                ManufacturerFilterCombo.ItemsSource = manufacturers;
                ManufacturerFilterCombo.DisplayMemberPath = "Name";
                ManufacturerFilterCombo.SelectedIndex = 0;
            }
        }

        private void LoadProducts()
        {
            using (var db = new AppDbContext())
            {
                _allProducts = db.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.ProductType)
                    .Where(p => !p.IsFrozen)
                    .ToList();
            }
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allProducts.AsEnumerable();

            // Поиск
            var search = SearchBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(p => p.Name.ToLower().Contains(search));

            // Фильтр по типу
            var selectedType = TypeFilterCombo.SelectedItem as ProductType;
            if (selectedType != null && selectedType.ProductTypeId > 0)
                filtered = filtered.Where(p => p.ProductTypeId == selectedType.ProductTypeId);

            // Фильтр по производителю
            var selectedManufacturer = ManufacturerFilterCombo.SelectedItem as Manufacturer;
            if (selectedManufacturer != null && selectedManufacturer.ManufacturerId > 0)
                filtered = filtered.Where(p => p.ManufacturerId == selectedManufacturer.ManufacturerId);

            // Сортировка
            var sortItem = (SortCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            switch (sortItem)
            {
                case "По оценке ↓":
                    filtered = filtered.OrderByDescending(p => p.Rating);
                    break;
                case "По цене ↑":
                    filtered = filtered.OrderBy(p => p.FinalPrice);
                    break;
                case "По цене ↓":
                    filtered = filtered.OrderByDescending(p => p.FinalPrice);
                    break;
            }

            ShowProducts(filtered.ToList());
        }

        private void ShowProducts(List<Product> products)
        {
            ProductsPanel.Children.Clear();

            foreach (var product in products)
            {
                var card = CreateProductCard(product);
                ProductsPanel.Children.Add(card);
            }
        }

        private Border CreateProductCard(Product product)
        {
            var bgColor = product.HasBigDiscount
                ? new SolidColorBrush(Color.FromRgb(255, 243, 205))
                : Brushes.White;

            var card = new Border
            {
                Width = 200,
                Height = 280,
                Margin = new Thickness(8),
                Background = bgColor,
                BorderBrush = new SolidColorBrush(Color.FromRgb(221, 221, 221)),
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var stack = new StackPanel { Margin = new Thickness(12) };

            // Скидка
            if (product.Discount > 0)
            {
                var discountBadge = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                    Padding = new Thickness(6, 2, 6, 2),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                discountBadge.Child = new TextBlock
                {
                    Text = $"-{product.Discount}%",
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    FontSize = 12
                };
                stack.Children.Add(discountBadge);
            }

            // Название
            stack.Children.Add(new TextBlock
            {
                Text = product.Name,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51)),
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Производитель
            stack.Children.Add(new TextBlock
            {
                Text = product.Manufacturer?.Name,
                FontSize = 11,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Тип
            stack.Children.Add(new TextBlock
            {
                Text = product.ProductType?.Name,
                FontSize = 11,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 8)
            });

            // Оценка
            stack.Children.Add(new TextBlock
            {
                Text = $"⭐ {product.Rating:F1}",
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 8)
            });

            // Цена
            if (product.Discount > 0)
            {
                stack.Children.Add(new TextBlock
                {
                    Text = $"{product.Price:F0} руб.",
                    FontSize = 11,
                    Foreground = Brushes.Gray,
                    TextDecorations = TextDecorations.Strikethrough
                });
            }

            stack.Children.Add(new TextBlock
            {
                Text = $"{product.FinalPrice:F0} руб.",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(123, 45, 139)),
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Кнопка корзины
            var cartButton = new Button
            {
                Content = "🛒 В корзину",
                Height = 32,
                Background = new SolidColorBrush(Color.FromRgb(123, 45, 139)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = product.ProductId
            };
            cartButton.Click += AddToCartButton_Click;
            stack.Children.Add(cartButton);

            card.Child = stack;

            // Клик по карточке — открыть детали
            card.MouseLeftButtonUp += (s, e) =>
            {
                var detailWindow = new ProductDetailWindow(product.ProductId);
                detailWindow.ShowDialog();
            };

            return card;
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppSession.IsLoggedIn)
            {
                MessageBox.Show("Для добавления в корзину необходимо войти в аккаунт.",
                    "Требуется вход", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var button = sender as Button;
            var productId = (int)button.Tag;

            using (var db = new AppDbContext())
            {
                var existing = db.Carts.FirstOrDefault(c =>
                    c.ClientId == AppSession.CurrentUser.UserId &&
                    c.ProductId == productId);

                if (existing != null)
                {
                    existing.Quantity++;
                }
                else
                {
                    db.Carts.Add(new Cart
                    {
                        ClientId = AppSession.CurrentUser.UserId,
                        ProductId = productId,
                        Quantity = 1
                    });
                }
                db.SaveChanges();
            }

            MessageBox.Show("Товар добавлен в корзину!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_allProducts == null) return;
            ApplyFilters();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppSession.IsLoggedIn)
            {
                MessageBox.Show("Для просмотра корзины необходимо войти в аккаунт.",
                    "Требуется вход", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var cartWindow = new CartWindow();
            cartWindow.ShowDialog();
        }
    }
}