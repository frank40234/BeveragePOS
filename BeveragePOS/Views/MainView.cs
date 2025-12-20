using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeveragePOS.Controllers; // 引入 Controller
using BeveragePOS.Models;     // 引入 Model

namespace BeveragePOS.Views
{
    public partial class MainView : Form
    {
        private readonly MainController _controller;
        public MainView()
        {
            InitializeComponent();
            // 建立 controller實例
            _controller = new MainController();
            //設定表單仔入事件
            this.Load += new EventHandler(MainView_Load);

        }

        private void MainView_Load(object sender, EventArgs e)
        {
            // 在表單載入完成後，執行菜單生成
            LoadMenuButtons();
        }
        /// <summary>
        /// 從 Controller 取得菜單資料並動態生成按鈕。
        /// </summary>
        private void LoadMenuButtons()
        {
            List<BeveragePOS.Models.MenuItem> menu = _controller.GetMenu();
            int x = 10;
            int y = 10;
            const int ButtonWidth = 150;
            const int ButtonHeight = 80;

            //清空Panel(防止重複載入)
            pnlMenu.Controls.Clear();

            foreach(var item in menu)
            {
                Button btn = new Button()
                {
                    Text = $"{item.Name}\n${item.Price:N2}",
                    Tag = item,
                    Width = ButtonWidth,
                    Height= ButtonHeight,
                    Location= new System.Drawing.Point(x, y),
                    Font=new System.Drawing.Font("微軟正黑體",12F,System.Drawing.FontStyle.Bold)
                };
                // 設置點擊事件 (下一步會實作)
                btn.Click += MenuItem_Click;

                pnlMenu.Controls.Add(btn);

                // 佈局邏輯：每行放兩個按鈕
                x += ButtonWidth + 10;
                if (x + ButtonWidth > pnlMenu.Width)
                {
                    x = 10;
                    y += ButtonHeight + 10;
                }

            }
        }
        // 菜單按鈕點擊事件處理函式 (下一步實作)
        private void MenuItem_Click(object sender, EventArgs e)
        {
            // (Placeholder for adding item to order)
            Button clickedButton = (Button)sender;
            // 從 Tag 中取出我們儲存的 Model 物件 (修正命名衝突)
            BeveragePOS.Models.MenuItem selectedItem =
                (BeveragePOS.Models.MenuItem)clickedButton.Tag;
            //View 呼叫 Controller 執行業務邏輯
            _controller.AddToOrder(selectedItem);
            //更新介面顯示
            UpdateOrderDisplay();

        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        //結帳按鈕
        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if(_controller.GetOrderTotal() > 0)
            {
                //呼叫Controller 處理結帳邏輯
                _controller.Checkout();

                MessageBox.Show("結帳成功","交易完成",MessageBoxButtons.OK, MessageBoxIcon.Information);

                //更新介面顯示 (清空)
                UpdateOrderDisplay();
            }
        }
        /// <summary>
        /// 更新 ListBox 和總金額 Label 的顯示
        /// </summary>
        private void UpdateOrderDisplay()
        {
            // 重新繫結 ListBox
            lbxOrder.DataSource = null; // 必須先解除綁定
            lbxOrder.DataSource = _controller.GetCurrentOrderItems();

            // 更新總金額 Label
            decimal total = _controller.GetOrderTotal();
            lblTotal.Text = $"總計: ${total:N2}";
        }

    }
}
