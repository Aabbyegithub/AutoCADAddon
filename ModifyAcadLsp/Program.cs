using System.Text;

namespace ModifyAcadLsp
{
    internal class Program
    {
        static void Main(string[] args)
        {
			try
			{
                File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", "程序启动\n");
                // 插件安装目录（Inno Setup 传入参数，默认为当前目录）
                string pluginPath = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
                string dllPath = Path.Combine(pluginPath, "AutoCADAddon.dll");

                if (!File.Exists(dllPath))
                {
                    Console.WriteLine("未找到 AutoCADAddon.dll，路径：" + dllPath);
                     File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"未找到 AutoCADAddon.dll，路径：{dllPath}\n");
                    return;
                }

                // 查找已安装的 AutoCAD 版本
                var cadPaths = AutoCADRegistryHelper.GetAutoCADPath();

                if (string.IsNullOrEmpty(cadPaths))
                {
                    Console.WriteLine("未找到 AutoCAD 安装路径。");
                            File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"未找到 AutoCAD 安装路径。\n");
                    return;
                }

                string dllPathForLisp = dllPath.Replace("\\", "/");

                string lispCode = $@"

                (if (not c:__MyPluginLoaded)
                    (progn
                        (command ""NETLOAD"" ""{dllPathForLisp}"")
                        (setq c:__MyPluginLoaded t)
                    )
                )
                (princ)
                ";

                try
                {
                    for (int i = 2020; i <= DateTime.Now.Year; i++)
                    {
                        var cadPathsvr = Path.Combine(cadPaths, "Support","zh-CN", $"acad{i}doc.lsp");
                        if (!File.Exists(cadPathsvr))
                        {
                            Console.WriteLine($"未找到文件: {cadPathsvr}");
        File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"未找到文件: {cadPathsvr}\n");
                            continue;
                        }

                        string content = File.ReadAllText(cadPathsvr, Encoding.Default);

                        if (!content.Contains(dllPathForLisp))
                        {
                            File.AppendAllText(cadPathsvr, lispCode, Encoding.Default);
                            Console.WriteLine($"已修改: {cadPathsvr}");
                                    File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"已修改: {cadPathsvr}\n");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"已包含加载代码: {cadPathsvr}");
                                    File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"已包含加载代码: {cadPathsvr}\n");
                            return;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"修改 {cadPaths} 失败: {ex.Message}");
                            File.AppendAllText(@"C:\Users\luqiang\Desktop\log.txt", $"修改 {cadPaths} 失败: {ex.Message}\n");
                    return;
                }


            }
			catch (Exception)
			{

				 Console.WriteLine($"安装失败！");
			}
        }
    }
}
