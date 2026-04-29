using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using KosmicheskayaLozha.Services;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Views
{
    public partial class OrderWindow : Window
    {
        public OrderWindow()
        {
            InitializeComponent();
            DeliveryDatePicker.DisplayDateStart = DateTime.Today.AddDays(1);
            DeliveryDatePicker.DisplayDateEnd = DateTime.Today.AddDays(7);
            DeliveryDatePicker.SelectedDate = DateTime.Today.AddDays(1);
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveryDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату получения!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var deliveryDate = DeliveryDatePicker.SelectedDate.Value;

            if (deliveryDate < DateTime.Today.AddDays(1) ||
                deliveryDate > DateTime.Today.AddDays(7))
            {
                MessageBox.Show("Дата должна быть от завтра до 7 дней вперёд!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var payment = (PaymentCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();

            var result = MessageBox.Show("Подтвердить заказ?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using (var db = new AppDbContext())
            {
                var cartItems = db.Carts
                    .Include(c => c.Product)
                    .Where(c => c.ClientId == AppSession.CurrentUser.UserId)
                    .ToList();

                if (cartItems.Count == 0)
                {
                    MessageBox.Show("Корзина пуста!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создаём заказ
                var order = new Order
                {
                    ClientId = AppSession.CurrentUser.UserId,
                    OrderDate = DateTime.Now,
                    DeliveryDate = deliveryDate,
                    PaymentMethod = payment,
                    Status = "Новый"
                };
                db.Orders.Add(order);
                db.SaveChanges();

                // Добавляем позиции заказа
                foreach (var item in cartItems)
                {
                    db.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PriceAtOrder = item.Product.FinalPrice
                    });
                }

                // Очищаем корзину
                db.Carts.RemoveRange(cartItems);
                db.SaveChanges();
            }

            MessageBox.Show("Заказ успешно оформлен!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}