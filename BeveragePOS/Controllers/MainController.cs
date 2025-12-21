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
        private List<OrderItem> _currentOrderItems;
        private List<OrderItem> _currentOrder = new List<OrderItem>();

        // 建構函式：Controller 初始化時，建立 DataService 實例
        public MainController()
        {
            _dataService = new DataService();
            //初始化當前訂單
            _currentOrderItems = new List<OrderItem>();
            
        }

        public List<MenuItem> GetMenu()
        {
            // Controller 呼叫 Model 取得資料
            return _dataService.GetMenuItems();

        }

        // (未來：處理 AddToOrder, Checkout 等方法將會加在這裡)

        /// <summary>
        /// 將菜單項目加入當前訂單
        /// </summary>
        /// <param name="item"></param>
        public void AddToOrder(MenuItem item)
        {
            var existingItem = _currentOrderItems.Find(oi => oi.MenuItemId == item.Id);
            if(existingItem != null)
            {
                //如果存在，數量加一
                existingItem.Quantity++;
            }
            else
            {
                //如果不存在，創建新的orderitem 並加入列表
                _currentOrderItems.Add(new OrderItem
                {
                    MenuItemId= item.Id,
                    Name = item.Name,
                    Price= item.Price,
                    Quantity=1

                });
            }
        }
        /// <summary>
        /// 計算當前訂單總金額
        /// </summary>
        /// <returns></returns>
        public decimal GetOrderTotal()
        {
            return _currentOrderItems.Sum(oi => oi.Subtotal);
        }
        /// <summary>
        /// 取得當前訂單的明細列表，供 View 顯示
        /// </summary>
        public List<OrderItem> GetCurrentOrderItems()
        {
            return _currentOrderItems;
        }
        /// <summary>
        /// 處理結帳邏輯並清空訂單 (未來會將訂單存入資料庫)
        /// </summary>
        public void Checkout()
        {
            // (未來：將 _currentOrderItems 存入 Order 和 OrderItem 資料表)

            // 清空當前訂單
            _currentOrderItems.Clear();
        }

        public List<OrderItem> GetCurrentOrder() => _currentOrder;
        public decimal GetTotal() => _currentOrder.Sum(x => x.Subtotal);
        public void ClearOrder() => _currentOrder.Clear();
    }
}
