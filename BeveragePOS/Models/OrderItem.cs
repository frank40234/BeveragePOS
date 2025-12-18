using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeveragePOS.Models
{
    /// <summary>
    /// 代表訂單中的單一商品明細
    /// </summary>
    public class OrderItem
    {
        //該商品在菜單中的ID(用於連結資料庫)
        public int MenuItemId { get; set; }
        //該商品的名稱(例如:珍珠奶茶)
        public string Name { get; set; }
        //該商品的單價
        public decimal Price { get; set; }
        //點選數量
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// 計算該訂單明細的總小計
        /// </summary>
        public decimal Subtotal => Price * Quantity;// 調用subtotal時執行Price * Quantity
        public override string ToString()
        {
            // 範例輸出: [1] 珍珠奶茶 @ $55.00 x 1 = $55.00
            return $"[#{MenuItemId}] {Name} @ ${Price:N2} x {Quantity} = ${Subtotal:N2}";
        }
    }
}
