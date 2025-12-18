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

        // 建構函式：Controller 初始化時，建立 DataService 實例
        public MainController()
        {
            _dataService = new DataService();
        }

        public List<MenuItem> GetMenu()
        {
            // Controller 呼叫 Model 取得資料
            return _dataService.GetMenuItems();

        }

        // (未來：處理 AddToOrder, Checkout 等方法將會加在這裡)
    }
}
