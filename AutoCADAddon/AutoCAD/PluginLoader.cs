using AutoCADAddon.Common;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(AutoCADAddon.AutoCAD.PluginLoader))]
namespace AutoCADAddon.AutoCAD
{
    public class PluginLoader : IExtensionApplication
    {
        static PluginLoader()
        {
            ResolveAcadAssemblies();
        }

        // 插件加载时执行（AutoCAD启动或NETLOAD时触发）
        public void Initialize()
        {
            // 使用Idle事件确保UI已完全加载
            Application.Idle += OnApplicationIdle;
            Application.DocumentManager.DocumentCreated += OnDocumentCreated;

        }

        private static Assembly ResolveAcadAssemblies()
        {
            // 从注册表获取 AutoCAD 路径
            string acadPath = AutoCADRegistryHelper.GetAutoCADPath();
             if (!string.IsNullOrEmpty(acadPath))
             {
                LoadAssembly(acadPath, "accoremgd.dll");
                LoadAssembly(acadPath, "acdbmgd.dll");
                LoadAssembly(acadPath, "acmgd.dll");
                LoadAssembly(acadPath, "AdWindows.dll");
            }

            return null; // 让系统继续尝试解析
        }
        private static void LoadAssembly(string basePath, string assemblyName)
        {
            string path = Path.Combine(basePath, assemblyName);
            if (File.Exists(path))
            {
                Assembly.LoadFrom(path);
            }
            else
            {
                throw new FileNotFoundException($"找不到 {assemblyName}，路径: {path}");
            }
        }

        private void OnApplicationIdle(object sender, EventArgs e)
        {
            // 取消订阅，避免重复触发
            Application.Idle -= OnApplicationIdle;

            // 延迟执行，确保UI初始化完成
           Timer timer = new Timer { Interval = 500 };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                XigmaRibbon.AddXigmaRibbon();
                SimulateSafeClick();
            };
            timer.Start();
        }
        // 插件卸载时执行（一般无需处理）
        public void Terminate() {
            // 卸载时移除监听
            Application.DocumentManager.DocumentCreated -= OnDocumentCreated;
        }
        private void OnDocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            // 打开新图纸或新建图纸时触发
            var doc = e.Document;
            XigmaRibbon.Opdc(doc);
            // 也可以在这里进行你自己的业务逻辑，比如绑定事件、初始化数据等
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        private static void SimulateMouseClick()
        {
            // 模拟鼠标点击当前位置
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private static void SimulateSafeClick()
        {
            // 保存当前光标位置
            var oldPos = Cursor.Position;

            // 移动到屏幕左上角点击（确保不会点击到 UI）
            SetCursorPos(0, 0);
            SimulateMouseClick();

            // 移回原位
            SetCursorPos(oldPos.X, oldPos.Y);
        }


    }
}
