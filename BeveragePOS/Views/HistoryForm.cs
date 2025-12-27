using BeveragePOS.Controllers;
using BeveragePOS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeveragePOS.Controllers;
using BeveragePOS.Models;

namespace BeveragePOS.Views
{
    public partial class HistoryForm : Form
    {
        private readonly MainController _controller;

        public HistoryForm()
        {
            InitializeComponent();
            _controller = new MainController(); // 這裡也可以用依賴注入，但先簡單 new 一個

            this.Load += HistoryForm_Load;

            // 設定 DataGridView 的屬性，讓畫面好看一點
            SetupDataGridView(dgvOrders);
            SetupDataGridView(dgvDetails);
            closeBtn.Click += (s, e) => this.Close();
        }

        private void SetupDataGridView(DataGridView dgv)
        {
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // 整行選取
            dgv.MultiSelect = false; // 不能多選
            dgv.ReadOnly = true;     // 唯讀
            dgv.AllowUserToAddRows = false; // 隱藏最下面空白行
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // 欄位自動填滿
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            // 1. 載入訂單列表
            dgvOrders.DataSource = _controller.GetOrderHistory();

            // 2. 綁定選取事件：當使用者點選左邊某筆訂單時，右邊要顯示明細
            dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;
        }

        private void DgvOrders_SelectionChanged(object sender, EventArgs e)
        {
            // 確保有選到東西
            if (dgvOrders.SelectedRows.Count > 0)
            {
                // 取得選取的整行資料物件
                var selectedOrder = (Order)dgvOrders.SelectedRows[0].DataBoundItem;

                // 呼叫 Controller 查明細，並顯示在右邊表格
                dgvDetails.DataSource = _controller.GetOrderDetails(selectedOrder.Id);
            }
        }
    }
}
