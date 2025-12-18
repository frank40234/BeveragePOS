using System;
using BeveragePOS.Models;
using BeveragePOS.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeveragePOS
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //再啟動UI前先初始化資料庫
            InitializeSystem();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainView());
        }

        static void InitializeSystem()
        {
            var dataService = new DataService();
            //呼叫 DataService 確保資料庫檔案和資料表存在
            dataService.InitializeDatabase();
        }
    }
}
