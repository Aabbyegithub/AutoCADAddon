using AutoCADAddon;
using AutoCADAddon.Common;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADControls.Controls
{
    public partial class ToolMenuData : UserControl
    {
        private static DrawingPropertiesForm _propertiesFormAdd;
        private static DrawingPropertiesForm _propertiesFormNew;
        public ToolMenuData()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 添加已打开图纸
        /// </summary>
        /// <param name="_doc"></param>
        public void GetDrawingData(Document _doc)
        {
            var props = CacheManager.GetCurrentDrawingProperties(_doc.Window.Text);
            if (props == null || string.IsNullOrEmpty(props.BuildingExternalCode) || string.IsNullOrEmpty(props.FloorCode))
            {
                return;
            }
            var index = dataGridView1.Rows.Add();
            var row = dataGridView1.Rows[index];

            row.Cells["DrawingTitle"].Value = props.Name;
            row.Cells["DrawingName"].Value = props.Name;

            if (props.status == "Published")
            {
                row.DefaultCellStyle.BackColor = Color.LightGreen; // 绿色
            }
        }

        public void ClearDrawingData(string name)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["DrawingName"].Value?.ToString() == name)
                {
                    dataGridView1.Rows.Remove(row);
                    break; // 删除后必须 break，否则会抛异常
                }
            }
        }

        /// <summary>
        /// 打开图纸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, EventArgs e)
        {
            var currentRow = dataGridView1.CurrentRow;
            if (currentRow != null)
            {
                var drawingName = currentRow.Cells["DrawingName"].Value?.ToString();
                var docs = Application.DocumentManager;

                foreach (Document doc in docs)
                {
                    // 注意：doc.Name 是包含完整路径的
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(doc.Name);
                    if (fileName.Equals(drawingName, StringComparison.OrdinalIgnoreCase))
                    {
                        docs.MdiActiveDocument = doc; // 切换激活
                        return;
                    }
                }

                Application.ShowAlertDialog($"没有找到名为 \"{drawingName}\" 的图纸");
            }
            CloseAllPropertyForms();
        }

        /// <summary>
        /// 新建属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Click(object sender, EventArgs e)
        {
            CloseAllPropertyForms();
            if (_propertiesFormNew != null && !_propertiesFormNew.IsDisposed)
            {
                _propertiesFormNew.Activate();
                return;
            }
            Document _doc = Application.DocumentManager.MdiActiveDocument;
            _propertiesFormNew = new DrawingPropertiesForm(_doc);
            _propertiesFormNew.FormClosed += (sender1, args) => { _propertiesFormNew = null; };

            _propertiesFormNew.Show();
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, EventArgs e)
        {
            CloseAllPropertyForms();
            if (_propertiesFormAdd != null && !_propertiesFormAdd.IsDisposed)
            {
                _propertiesFormAdd.Activate();
                return;
            }
            Document _doc = Application.DocumentManager.MdiActiveDocument;
            _propertiesFormAdd = new DrawingPropertiesForm(_doc);
            _propertiesFormAdd.FormClosed += (sender1, args) => { _propertiesFormAdd = null; };

            _propertiesFormAdd.Show();
        }

        /// <summary>
        /// 移除属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Click(object sender, EventArgs e)
        {
            CloseAllPropertyForms();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Properties_Click(object sender, EventArgs e)
        {
            CloseAllPropertyForms();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, EventArgs e)
        {
            CloseAllPropertyForms();
        }

        private void CloseAllPropertyForms()
        {
            if (_propertiesFormNew != null && !_propertiesFormNew.IsDisposed)
            {
                _propertiesFormNew.Close();
                _propertiesFormNew = null;
            }

            if (_propertiesFormAdd != null && !_propertiesFormAdd.IsDisposed)
            {
                _propertiesFormAdd.Close();
                _propertiesFormAdd = null;
            }

            // 可以继续扩展其他窗体
        }

    }
}
