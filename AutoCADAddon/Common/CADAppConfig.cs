using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoCADAddon.Common
{
     public class CADAppConfig
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private readonly string _configFilePath = AppDomain.CurrentDomain.BaseDirectory + "CADAppConfig.xml";

        /// <summary>
        /// 配置文件xml
        /// </summary>
        private XDocument _xml;

        /// <summary>
        /// 文件监控器
        /// </summary>
        private FileSystemWatcher _watcher;

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static Lazy<CADAppConfig> _config = new Lazy<CADAppConfig>(() => new CADAppConfig());

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static CADAppConfig GetInstance() => _config.Value;

        /// <summary>
        /// 构造函数
        /// </summary>
        private CADAppConfig()
        {
            CheckConfig(_configFilePath);
            LoadConfig();
            InitializeWatcher();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private void LoadConfig()
        {
            if (File.Exists(_configFilePath))
            {
                _xml = XDocument.Load(_configFilePath);
            }
        }

        /// <summary>
        /// 初始化文件监控器
        /// </summary>
        private void InitializeWatcher()
        {
            try
            {
                _watcher = new FileSystemWatcher
                {
                    Path = Path.GetDirectoryName(_configFilePath),
                    Filter = Path.GetFileName(_configFilePath),
                    NotifyFilter = NotifyFilters.LastWrite
                };

                _watcher.Changed += (sender, e) =>
                {
                    // 延迟加载，避免文件还在写入中
                    Thread.Sleep(200);
                    ReloadConfig();
                };

                _watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// 手动触发重新加载配置
        /// </summary>
        public void ReloadConfig()
        {
            try
            {
                // 使用线程锁避免多线程冲突
                lock (this)
                {
                    LoadConfig();
                    // 触发配置变更事件，通知其他模块
                    ConfigChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// 配置变更事件
        /// </summary>
        public event EventHandler ConfigChanged;

       

        /// <summary>
        /// 获取服务器配置
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetServiseUrl
        {
            get
            {
                var settings = new Dictionary<string, string>();

                try
                {
                    XElement settingsNode = _xml.Root?.Element("ServiseUrl");

                    if (settingsNode != null)
                    {
                        foreach (XElement element in settingsNode.Elements())
                        {
                            if (element.Value.Contains("新服务器地址")) continue;
                            settings[element.Name.LocalName] = element.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"读取设置时出错: {ex.Message}");
                }

                return settings;
            }
            set
            {
                try
                {
                    // 获取或创建 Settings 节点
                    XElement settingsNode = _xml.Root?.Element("ServiseUrl");
                    if (settingsNode == null)
                    {
                        settingsNode = new XElement("ServiseUrl");
                        if (_xml.Root != null)
                            _xml.Root.Add(settingsNode);
                        else
                            _xml = new XDocument(new XElement("Root", settingsNode));
                    }
                    else
                    {
                        // 清空现有设置
                        settingsNode.RemoveAll();
                    }

                    // 添加新设置
                    foreach (var item in value)
                    {
                        settingsNode.Add(new XElement(item.Key, item.Value));
                    }

                    // 保存到文件
                    SaveConfig();
                }
                catch (Exception ex)
                {
                  
                }
            }
        }

        /// <summary>
        /// 获取项目配置
        /// </summary>
        public Dictionary<string, string> GetProject
        {
            get
            {
                var settings = new Dictionary<string, string>();

                try
                {
                    XElement settingsNode = _xml.Root?.Element("Project");

                    if (settingsNode != null)
                    {
                        foreach (XElement element in settingsNode.Elements())
                        {
                            settings[element.Name.LocalName] = element.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"读取设置时出错: {ex.Message}");
                }

                return settings;
            }
            set
            {
                try
                {
                    // 获取或创建 Settings 节点
                    XElement settingsNode = _xml.Root?.Element("Project");
                    if (settingsNode == null)
                    {
                        settingsNode = new XElement("Project");
                        if (_xml.Root != null)
                            _xml.Root.Add(settingsNode);
                        else
                            _xml = new XDocument(new XElement("Root", settingsNode));
                    }
                    else
                    {
                        // 清空现有设置
                        settingsNode.RemoveAll();
                    }

                    // 添加新设置
                    foreach (var item in value)
                    {
                        settingsNode.Add(new XElement(item.Key, item.Value));
                    }

                    // 保存到文件
                    SaveConfig();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void EnsureConfigLoaded()
        {
            if (_xml == null)
            {
                LoadConfig();
            }
        }

        private void SaveConfig()
        {
            try
            {
                // 关闭监控器，避免触发循环加载
                _watcher.EnableRaisingEvents = false;
                _xml.Save(_configFilePath);
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                // 重新开启监控器
                _watcher.EnableRaisingEvents = true;
            }
        }



        private void CheckConfig(string filePath)
        {
            if (File.Exists(filePath))
                return;

            XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            XElement root = new XElement("CADAppConfig");
            XElement ServiseUrl = new XElement("ServiseUrl");
            // 添加默认的键值对参数示例
            ServiseUrl.Add(new XElement("Url-USE", "https://lam-bop-gateway-uat.nwplatform.com.cn"));
            ServiseUrl.Add(new XElement("Url-1", "新服务器地址"));

            root.Add(ServiseUrl);

            XElement Project = new XElement("Project");
            // 添加默认的键值对参数示例
            Project.Add(new XElement("Project-1", "DrawingPro1"));
            Project.Add(new XElement("Project-2", "DrawingPro2"));
            root.Add(Project);
            xml.Add(root);

            //保存文档
            xml.Save(filePath);
        }
    }
}
