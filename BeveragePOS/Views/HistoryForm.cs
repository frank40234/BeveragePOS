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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 1. 檢查是否有選取訂單
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("請先選擇一筆要刪除的訂單！");
                return;
            }

            // 2. 取得選取的訂單 ID
            // 這裡我們將選取的物件轉型回我們定義的 Order 類別
            var selectedOrder = (Order)dgvOrders.SelectedRows[0].DataBoundItem;
            int orderId = selectedOrder.Id;

            // 3. 防呆確認 (非常重要！)
            var result = MessageBox.Show(
                $"確定要刪除「訂單編號 {orderId}」嗎？\n此動作無法復原！",
                "刪除確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 4. 呼叫 Controller 執行刪除
                    _controller.DeleteOrder(orderId);

                    MessageBox.Show("訂單已刪除。");

                    // 5. 刪除後必須「重新載入」清單，畫面才會更新
                    // 這邊我們重新呼叫一次載入邏輯
                    dgvOrders.DataSource = null; // 先斷開連結
                    dgvOrders.DataSource = _controller.GetOrderHistory(); // 重新取得資料

                    // 清空右邊的明細，因為左邊的選取可能跑掉了
                    dgvDetails.DataSource = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"刪除失敗: {ex.Message}");
                }
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
