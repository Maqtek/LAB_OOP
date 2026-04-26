using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Lab1
{
    public sealed class OrderItem
    {
        public OrderItem(string name, int quantity, decimal unitPrice)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Название товара не может быть пустым.", nameof(name));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Количество должно быть больше нуля.");
            }

            if (unitPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unitPrice), "Цена не может быть отрицательной.");
            }

            Name = name;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public string Name { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }

    public sealed class Order
    {
        public Order(string id, string customerEmail, IReadOnlyList<OrderItem> items)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Идентификатор заказа не может быть пустым.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                throw new ArgumentException("Email покупателя не может быть пустым.", nameof(customerEmail));
            }

            if (items is null || items.Count == 0)
            {
                throw new ArgumentException("Заказ должен содержать хотя бы один товар.", nameof(items));
            }

            Id = id;
            CustomerEmail = customerEmail;
            Items = items;
        }

        public string Id { get; }
        public string CustomerEmail { get; }
        public IReadOnlyList<OrderItem> Items { get; }
        public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
    }

    public sealed class OrderResult
    {
        public OrderResult(bool success, string message, string? shippingLabel = null)
        {
            Success = success;
            Message = message;
            ShippingLabel = shippingLabel;
        }

        public bool Success { get; }
        public string Message { get; }
        public string? ShippingLabel { get; }
    }

    public interface IInventoryService
    {
        bool CheckAndReserve(Order order);
    }

    public interface IPaymentService
    {
        bool ProcessPayment(Order order);
    }

    public interface IShippingService
    {
        string CreateAndPrintLabel(Order order);
    }

    public interface INotificationService
    {
        void SendOrderConfirmation(Order order, string shippingLabel);
    }

    public sealed class InventoryService : IInventoryService
    {
        public bool CheckAndReserve(Order order)
        {
            Console.WriteLine("1) Проверка наличия и резервирование товаров на складе...");
            Console.WriteLine("   Товары успешно зарезервированы.");
            return true;
        }
    }

    public sealed class PaymentService : IPaymentService
    {
        public bool ProcessPayment(Order order)
        {
            Console.WriteLine("2) Оформление платежа...");
            Console.WriteLine($"   Сумма к оплате: {order.TotalAmount} руб.");
            Console.WriteLine("   Платеж успешно проведен.");
            return true;
        }
    }

    public sealed class ShippingService : IShippingService
    {
        public string CreateAndPrintLabel(Order order)
        {
            Console.WriteLine("3) Создание и печать этикетки доставки...");
            string label = $"SHIP-{order.Id}";
            Console.WriteLine($"   Этикетка создана: {label}");
            Console.WriteLine("   Этикетка отправлена на печать.");
            return label;
        }
    }

    public sealed class NotificationService : INotificationService
    {
        public void SendOrderConfirmation(Order order, string shippingLabel)
        {
            Console.WriteLine("4) Отправка уведомления покупателю...");
            Console.WriteLine($"   Письмо отправлено на {order.CustomerEmail}, номер этикетки: {shippingLabel}");
        }
    }

    public sealed class OrderFacade
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPaymentService _paymentService;
        private readonly IShippingService _shippingService;
        private readonly INotificationService _notificationService;

        public OrderFacade()
            : this(new InventoryService(), new PaymentService(), new ShippingService(), new NotificationService())
        {
        }

        public OrderFacade(
            IInventoryService inventoryService,
            IPaymentService paymentService,
            IShippingService shippingService,
            INotificationService notificationService)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _shippingService = shippingService ?? throw new ArgumentNullException(nameof(shippingService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public OrderResult PlaceOrder(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            bool inStock = _inventoryService.CheckAndReserve(order);
            if (!inStock)
            {
                return new OrderResult(false, "Оформление остановлено: товара нет в наличии.");
            }

            bool paymentProcessed = _paymentService.ProcessPayment(order);
            if (!paymentProcessed)
            {
                return new OrderResult(false, "Оформление остановлено: платеж не прошел.");
            }

            string shippingLabel = _shippingService.CreateAndPrintLabel(order);
            _notificationService.SendOrderConfirmation(order, shippingLabel);

            return new OrderResult(true, "Заказ успешно оформлен.", shippingLabel);
        }
    }
}
