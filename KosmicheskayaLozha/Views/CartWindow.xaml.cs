using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using KosmicheskayaLozha.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KosmicheskayaLozha.Views
{
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
            LoadCart();
        }

        private void LoadCart()
        {
            CartPanel.Children.Clear();

            using (var db = new AppDbContext())
            {
                var items = db.Carts
                    .Include(c => c.Product)
                    .Include(c => c.Product.Manufacturer)
                    .Where(c => c.ClientId == AppSession.CurrentUser.UserId)
                    .ToList();

                if (items.Count == 0)
                {
                    CartPanel.Children.Add(new TextBlock
                    {
                        Text = "Корзина пуста",
                        FontSize = 18,
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(20)
                    });
                    TotalText.Text = "Итого: 0 руб.";
                    return;
                }

                foreach (var item in items)
                    CartPanel.Children.Add(CreateCartCard(item));

                var total = items.Sum(i => i.Product.FinalPrice * i.Quantity);
                TotalText.Text = $"Итого: {total:F0} руб.";
            }
        }

        private Border CreateCartCard(Cart item)
        {
            var card = new Border
            {
                Width = 200,
                Height = 260,
                Margin = new Thickness(8),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(221, 221, 221)),
                BorderThickness = new Thickness(1)
            };

            var stack = new StackPanel { Margin = new Thickness(12) };

            // Название
            stack.Children.Add(new TextBlock
            {
                Text = item.Product.Name,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51)),
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Производитель
            stack.Children.Add(new TextBlock
            {
                Text = item.Product.Manufacturer?.Name,
                FontSize = 11,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Цена за штуку
            stack.Children.Add(new TextBlock
            {
                Text = $"{item.Product.FinalPrice:F0} руб./шт.",
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(123, 45, 139)),
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Кнопки количества
            var quantityPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var minusBtn = new Button
            {
                Content = "−",
                Width = 30,
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(123, 45, 139)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = item.CartId
            };

            var quantityText = new TextBlock
            {
                Text = item.Quantity.ToString(),
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 10, 0),
                Tag = item.CartId
            };

            var plusBtn = new Button
            {
                Content = "+",
                Width = 30,
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(123, 45, 139)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = item.CartId
            };

            minusBtn.Click += (s, e) => ChangeQuantity(item.CartId, -1);
            plusBtn.Click += (s, e) => ChangeQuantity(item.CartId, 1);

            quantityPanel.Children.Add(minusBtn);
            quantityPanel.Children.Add(quantityText);
            quantityPanel.Children.Add(plusBtn);
            stack.Children.Add(quantityPanel);

            // Итого за позицию
            stack.Children.Add(new TextBlock
            {
                Text = $"= {item.Product.FinalPrice * item.Quantity:F0} руб.",
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(123, 45, 139)),
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Кнопка удаления
            var deleteBtn = new Button
            {
                Content = "🗑 Удалить",
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = item.CartId
            };
            deleteBtn.Click += (s, e) => DeleteItem(item.CartId);
            stack.Children.Add(deleteBtn);

            card.Child = stack;
            return card;
        }

        private void ChangeQuantity(int cartId, int delta)
        {
            using (var db = new AppDbContext())
            {
                var item = db.Carts.FirstOrDefault(c => c.CartId == cartId);
                if (item == null) return;

                item.Quantity += delta;

                if (item.Quantity <= 0)
                    db.Carts.Remove(item);

                db.SaveChanges();
            }
            LoadCart();
        }

        private void DeleteItem(int cartId)
        {
            using (var db = new AppDbContext())
            {
                var item = db.Carts.FirstOrDefault(c => c.CartId == cartId);
                if (item != null)
                {
                    db.Carts.Remove(item);
                    db.SaveChanges();
                }
            }
            LoadCart();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var items = db.Carts
                    .Where(c => c.ClientId == AppSession.CurrentUser.UserId)
                    .ToList();

                if (items.Count == 0)
                {
                    MessageBox.Show("Корзина пуста!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var orderWindow = new OrderWindow();
            orderWindow.ShowDialog();
            LoadCart();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}