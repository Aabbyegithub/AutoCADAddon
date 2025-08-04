using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static AutoCADAddon.Model.ClassModel;

namespace AutoCADAddon.Common
{
    /// <summary>
    /// 配置管理器（单例模式）
    /// </summary>
    public static class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceManagementPlugin", "config.json");

        private static LocalConfig _config;
        private static readonly string _Key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:'\",.<>/?!";

        static ConfigManager()
        {
            LoadConfig();
        }

        // 加载配置（自动解密）
        private static void LoadConfig()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    _config = new LocalConfig();
                    SaveConfig();
                    return;
                }

                string json = File.ReadAllText(ConfigPath);
                _config = JsonConvert.DeserializeObject<LocalConfig>(json);

                // 自动解密（需实现 Decrypt 方法）
                if (_config.RememberPassword.IsEnabled)
                {
                    _config.RememberPassword.EncryptedUsername = Decrypt(_config.RememberPassword.EncryptedUsername);
                    _config.RememberPassword.EncryptedPassword = Decrypt(_config.RememberPassword.EncryptedPassword);
                }
            }
            catch (Exception ex)
            {
                // 配置损坏时重置
                _config = new LocalConfig();
                SaveConfig();
            }
        }

        // 保存配置（自动加密）
        public static void SaveConfig()
        {
            try
            {
                // 创建目录
                string dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                // 加密敏感数据
                if (_config.RememberPassword.IsEnabled)
                {
                    _config.RememberPassword.EncryptedUsername = Encrypt(_config.RememberPassword.EncryptedUsername);
                    _config.RememberPassword.EncryptedPassword = Encrypt(_config.RememberPassword.EncryptedPassword);
                }

                string json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                // 记录日志或提示（AutoCAD 中可写命令行）
                System.Diagnostics.Debug.WriteLine($"保存配置失败: {ex.Message}");
            }
        }

        // 添加服务器地址到历史列表
        public static void AddServer(string serverUrl)
        {
            if (!_config.Servers.Contains(serverUrl))
            {
                _config.Servers.Add(serverUrl);
                SaveConfig();
            }
        }

        // 获取当前配置
        public static LocalConfig GetConfig()
        {
            return _config;
        }

        // AES 加密（简单示例，实际需更安全的密钥管理）
        private static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_Key); // 需替换为安全密钥
                aes.IV = new byte[16];

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(plainBytes, 0, plainBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // AES 解密
        private static string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_Key); // 需替换为安全密钥
                aes.IV = new byte[16];

                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
