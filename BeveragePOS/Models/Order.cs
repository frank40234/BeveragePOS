using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeveragePOS.Models
{
    public class Order
    {
        public int Id { get; set; } // 訂單編號
        public DateTime OrderDate { get; set; } // 下單時間
        public decimal TotalAmount { get; set; } // 總金額
    }
}
