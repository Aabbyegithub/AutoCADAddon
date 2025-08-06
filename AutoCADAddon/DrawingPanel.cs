using AutoCADAddon.Common;
using AutoCADControls.Controls;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADAddon
{
    public class DrawingPanel
    {
        private readonly PaletteSet _paletteSet;
        private static  ToolMenuData _ToolMenuData;
        private static Document _doc;
        private bool _isProcessing = false;
        public DrawingPanel()
        {
            // 注册文件打开事件
            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentToBeDestroyed += DocumentManager_DocumentToBeDestroyed;
            _paletteSet = new PaletteSet("建筑结构管理", "SpaceManagementPlugin.BuildingPanel", new Guid("DE1F3EC7-C76D-495F-B19B-7BA3CFED24EA"));
            _paletteSet.Size = new System.Drawing.Size(400, 500);
            _ToolMenuData = new ToolMenuData() {  Dock = DockStyle.Fill };
            
            Panel panel = new Panel();
            panel.Controls.Add(_ToolMenuData);
            panel.Width = 500;

            _paletteSet.Add("建筑结构", panel);
            _paletteSet.Visible = true;
            Timer timer = new Timer();
            timer.Interval = 20; // 100毫秒延迟
            timer.Tick += (s, e) =>
            {
                timer.Dispose(); // 执行一次后销毁定时器
            };
            _doc = Application.DocumentManager.MdiActiveDocument;
            if (_doc !=null)
            {
                GetDrawingData();
            }
            timer.Start();
        }

        /// <summary>
        /// 获取图纸数据
        /// </summary>
        public static void GetDrawingData()
        {
            _ToolMenuData.Invoke(new Action(() =>
            {
                _ToolMenuData.GetDrawingData(_doc);
            }));
        }
        private void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            _doc = e.Document;

            //_doc.Database.SaveComplete += Database_SaveComplete;
            // 仅在打开已有文件时弹窗，新建不弹
            if (!e.Document.IsNamedDrawing)
            {
                // 新建的文档（如 Drawing1.dwg），不弹窗
                return;
            }
            //// 判断是否已绑定属性
            var props = CacheManager.GetCurrentDrawingProperties(_doc.Window.Text);
            if (!string.IsNullOrEmpty(props.Name) && !string.IsNullOrEmpty(props.BuildingExternalCode))
            {
                return;
            }
            GetDrawingData();
        }

        private void DocumentManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
            // 文档即将关闭
            if (_doc != null && e.Document.Name == _doc.Name)
            {
                _ToolMenuData.Invoke(new Action(() =>
                {
                    _ToolMenuData.ClearDrawingData(_doc.Window.Text);
                }));
                _doc.Database.SaveComplete -= Database_SaveComplete;
                _doc = null;
            }
        }

        private async void Database_SaveComplete(object sender, EventArgs e)
        {
            if (_doc == null) return;
            if (_isProcessing) return; // 防止重复执行
            _isProcessing = true;
            //await Task.Run(() =>
            //{
            try
            {

                Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.QueueForGraphicsFlush();
                Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen();

                using (DocumentLock docLock = _doc.LockDocument())
                {

                }

                _doc.Editor.WriteMessage($"\n插件保存成功");
            }
            catch (Exception ex)
            {
                // 在主线程显示错误消息
                //this._treeView.Invoke(new Action(() =>
                // {
                _doc.Editor.WriteMessage($"\n保存后操作失败：{ex.Message}");
                //}));
            }
            finally
            {
                _isProcessing = false;
            }
            //});
        }
        public void Close()
        {
            // 注销事件
            Application.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentToBeDestroyed -= DocumentManager_DocumentToBeDestroyed;

            //if (_doc != null)
            //{
            //    _doc.Database.SaveComplete -= Database_SaveComplete;
            //}

            //_treeView.Nodes.Clear();
            _paletteSet.Visible = false;

            //_doc = null;
        }
    }
}
