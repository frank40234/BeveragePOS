using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeveragePOS.Models
{
    /// <summary>
    /// 代表飲料店菜單上的單一項目(例如：紅茶、珍珠奶茶)
    /// </summary>
    public class MenuItem
    {
        // 1. 識別碼(Primary Key in DB)
        public int Id { get; set; }
        //2. 飲料名
        public string Name { get; set; }
        //3. 飲料價格(使用decimal 避免福點數誤差)
        public decimal Price { get; set; }
        //4. 飲料類別 (例如: 茶類、奶茶類、特調
        public string Category { get; set; }
        // 5. 庫存狀態或是可販售
        public bool IsAvailable { get; set; } = true;
        ////甜度
        //public string suger { get; set; }
        ////冰塊
        //public string ice { get; set; }

    }
}
