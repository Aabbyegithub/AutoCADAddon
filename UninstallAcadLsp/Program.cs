using System;
using System.IO;
using System.Text;

namespace UninstallAcadLsp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", "卸载程序启动\n");

                // 插件安装目录（Inno Setup 传入参数，默认为当前目录）
                string pluginPath = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
                string dllPath = Path.Combine(pluginPath, "AutoCADAddon.dll");

                if (!File.Exists(dllPath))
                {
                    //File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"未找到 AutoCADAddon.dll，路径：{dllPath}\n");
                }

                // AutoCAD 安装路径
                var cadPaths = AutoCADRegistryHelper.GetAutoCADPath();
                if (string.IsNullOrEmpty(cadPaths))
                {
                    //File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", "未找到 AutoCAD 安装路径。\n");
                    return;
                }

                string dllPathForLisp = dllPath.Replace("\\", "/");

                // 和安装时生成的完全一致
                string lispCode = $@"

                (if (not c:__MyPluginLoaded)
                    (progn
                        (command ""NETLOAD"" ""{dllPathForLisp}"")
                        (setq c:__MyPluginLoaded t)
                    )
                )
                (princ)
                ";

                for (int i = 2020; i <= DateTime.Now.Year; i++)
                {
                    var cadPathsvr = Path.Combine(cadPaths, "Support", "zh-CN", $"acad{i}doc.lsp");
                    if (!File.Exists(cadPathsvr))
                    {
                        //File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"未找到文件: {cadPathsvr}\n");
                        continue;
                    }

                    string content = File.ReadAllText(cadPathsvr, Encoding.Default);

                    if (content.Contains(dllPathForLisp))
                    {
                        // 删除写入的 Lisp 代码
                        string newContent = content.Replace(lispCode, "");
                        File.WriteAllText(cadPathsvr, newContent, Encoding.Default);

                        //File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"已清理: {cadPathsvr}\n");
                        Console.WriteLine($"已清理: {cadPathsvr}");
                    }
                    else
                    {
                        //File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"未找到加载代码: {cadPathsvr}\n");
                    }
                }

                Console.WriteLine("卸载完成！");
            }
            catch (Exception ex)
            {
                //File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"卸载失败: {ex.Message}\n");
                Console.WriteLine("卸载失败！");
            }
        }
    }
}
