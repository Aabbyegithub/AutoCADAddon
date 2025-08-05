using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoCADAddon
{
    public partial class PublishDrawing : Form
    {
        public PublishDrawing()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 发布上传图纸数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Publish_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Options_Click(object sender, EventArgs e)
        {

        }
    }
}
