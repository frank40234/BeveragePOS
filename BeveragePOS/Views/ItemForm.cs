using BeveragePOS.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeveragePOS.Views
{
    public partial class ItemForm : Form
    {
        private readonly MainController _controller;
        public ItemForm()
        {
            InitializeComponent();
            _controller = new MainController();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                decimal price = numPrice.Value;
                string category = txtCategory.Text.Trim();

                // 呼叫 Controller 新增
                _controller.AddNewProduct(name, price, category);

                MessageBox.Show("新增成功！");

                // 關閉視窗，並回傳 DialogResult.OK 讓主視窗知道我們成功了
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"錯誤: {ex.Message}", "警告");
            }
        }

        private void numPrice_ValueChanged(object sender, EventArgs e)
        {

        }
        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCategory_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
