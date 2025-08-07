using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using static AutoCADAddon.Model.ClassModel;
using AutoCADAddon.Common;

namespace AutoCADAddon.AutoCAD
{
    /// <summary>
    /// AutoCAD命令类
    /// </summary>
    public class AutoCADCommands
    {
        // 静态变量：存储当前登录信息（全局可用）
        public static LoginResult CurrentLoginInfo { get; set; }

        private static BuildingPanel _buildingPanel;

        // 注册AutoCAD命令：输入"SPACELOGIN"触发登录
        [CommandMethod("SPACELOGIN", CommandFlags.Modal)]
        public void SpaceLogin()
        {
            // 获取AutoCAD当前文档编辑器
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            // 已登录则提示
            if (CurrentLoginInfo != null)
            {
                ed.WriteMessage($"\n已登录：{CurrentLoginInfo.Username}!");
                return;
            }

            // 打开登录窗口
            var loginForm = new LoginForm();
            // 在AutoCAD中显示Windows窗体（需用ShowDialog，否则会被CAD遮挡）
            if (loginForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)

            {
                CurrentLoginInfo = loginForm.LoginResult;
                // 记录服务器地址到配置
                string serverUrl = loginForm.cboServers.Text;
                ConfigManager.AddServer(serverUrl);
                ed.WriteMessage($"\n登录成功！用户：{CurrentLoginInfo.Username}");
            }
            else
            {
                ed.WriteMessage("\n登录已取消");
            }
        }

        [CommandMethod("SPACEMANAGE")]
        public void OpenManagementPanel()
        {
            if (CurrentLoginInfo == null)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n请先执行SPACELOGIN登录");
                return;
            }
            _buildingPanel = new BuildingPanel();
            _buildingPanel.LoadData();
        }

        //[CommandMethod("SPACEMANAGE")]
        //public void OpenPanel()
        //{
        //    if (_buildingPanel == null || _buildingPanel.IsDisposed)
        //    {
        //        _buildingPanel = new BuildingPanel(Application.DocumentManager.MdiActiveDocument);
        //    }
        //    else
        //    {
        //        _buildingPanel.Visible = true;  // 已存在则直接显示
        //    }
        //}
        // 同步建筑楼层数据命令
        //[CommandMethod("SPACESYNC", CommandFlags.Modal)]
        //public async void SpaceSync()
        //{
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Editor ed = doc.Editor;

        //    if (CurrentLoginInfo == null)
        //    {
        //        ed.WriteMessage("\n请先执行 SPACELOGIN 登录");
        //        return;
        //    }

        //    ed.WriteMessage("\n开始同步建筑数据...");
        //    await DataSyncService.SyncBuildingsAsync(
        //        serverUrl: "http://your-server-url", // 需替换为实际地址
        //        token: CurrentLoginInfo.Token
        //    );
        //    ed.WriteMessage("\n建筑数据同步完成，开始同步楼层数据...");
        //    await DataSyncService.SyncFloorsAsync(
        //        serverUrl: "http://your-server-url", // 需替换为实际地址
        //        token: CurrentLoginInfo.Token
        //    );
        //    ed.WriteMessage("\n楼层数据同步完成！");

        //    // 初始化并显示面板
        //    if (_buildingPanel == null)
        //    {
        //        _buildingPanel = new BuildingPanel();
        //    }
        //    _buildingPanel.LoadData();
        //}

        //// 刷新数据命令
        //[CommandMethod("SPACEREFRESH", CommandFlags.Modal)]
        //public async void SpaceRefresh()
        //{
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Editor ed = doc.Editor;

        //    if (CurrentLoginInfo == null)
        //    {
        //        ed.WriteMessage("\n请先执行 SPACELOGIN 登录");
        //        return;
        //    }

        //    //// 先同步离线操作
        //    //ed.WriteMessage("\n开始同步离线操作...");
        //    //await DataSyncService.SyncOfflineOperations(
        //    //    serverUrl: "http://your-server-url",
        //    //    token: CurrentLoginInfo.Token
        //    //);

        //    // 再同步最新数据
        //    ed.WriteMessage("\n开始同步最新数据...");
        //    await DataSyncService.SyncBuildingsAsync(
        //        serverUrl: "http://your-server-url",
        //        token: CurrentLoginInfo.Token
        //    );
        //    await DataSyncService.SyncFloorsAsync(
        //        serverUrl: "http://your-server-url",
        //        token: CurrentLoginInfo.Token
        //    );
        //    await DataSyncService.SyncRoomsAsync(
        //        serverUrl: "http://your-server-url",
        //        token: CurrentLoginInfo.Token
        //    );

        //    ed.WriteMessage("\n数据刷新完成！");

        //    // 刷新面板
        //    if (_buildingPanel != null)
        //    {
        //        _buildingPanel.LoadData();
        //    }
        //}
    }
}
