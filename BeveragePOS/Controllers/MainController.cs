using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeveragePOS.Models;

namespace BeveragePOS.Controllers
{
    /// <summary>
    /// 主控制器，負責處理MainView的使用者和model(dataservice)的存取資料
    /// </summary>
    public class MainController
    {
        private readonly DataService _dataService;
        // 統一使用這一個變數來儲存訂單項目
        private List<OrderItem> _currentOrderItems;

        public MainController()
        {
            _dataService = new DataService();
            _currentOrderItems = new List<OrderItem>();
        }

        public List<MenuItem> GetMenu()
        {
            return _dataService.GetMenuItems();
        }

        public void AddToOrder(MenuItem item)
        {
            var existingItem = _currentOrderItems.Find(oi => oi.MenuItemId == item.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                _currentOrderItems.Add(new OrderItem
                {
                    MenuItemId = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = 1
                });
            }
        }
        // 修正：從 _currentOrderItems 中刪除項目
        public void DeleteOrder(int orderId)
        {
            _dataService.DeleteOrder(orderId);
        }

        // 修正：統一回傳 _currentOrderItems
        public List<OrderItem> GetCurrentOrder() => _currentOrderItems;

        // 修正：計算總額也使用 _currentOrderItems
        public decimal GetTotal() => _currentOrderItems.Sum(x => x.Subtotal);

        // 修正：清空也是清空 _currentOrderItems
        public void ClearOrder() => _currentOrderItems.Clear();

        // 這個方法與 GetCurrentOrder 重複了，可以考慮保留一個即可，這裡為了相容舊程式碼先留著
        public List<OrderItem> GetCurrentOrderItems()
        {
            return _currentOrderItems;
        }

        public decimal GetOrderTotal()
        {
            return _currentOrderItems.Sum(oi => oi.Subtotal);
        }

        public void Checkout()
        {
            if (_currentOrderItems.Count == 0) return;
            _dataService.SaveOrder(GetTotal(), _currentOrderItems);
            ClearOrder();
        }
        public List<Order> GetOrderHistory()
        {
            return _dataService.GetOrderHistory();
        }

        public List<OrderItem> GetOrderDetails(int orderId)
        {
            return _dataService.GetOrderDetails(orderId);
        }

        // [MainController.cs]

        public void AddNewProduct(string name, decimal price, string category)
        {
            // 簡單的資料驗證
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("飲料名稱不能為空");
            if (price < 0)
                throw new ArgumentException("價格不能為負數");

            var newItem = new MenuItem
            {
                Name = name,
                Price = price,
                Category = category,
                IsAvailable = true // 預設上架
            };

            _dataService.AddMenuItem(newItem);
        }

        public string GetDailyReport() => _dataService.GetDailyReport();
    }
}
