using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModifyAcadLsp
{
    /// <summary>
    /// 获取CAD  Api引用
    /// </summary>
    public static class AutoCADRegistryHelper
    {
        // 基础版本（从 2024 开始）
        private const string BaseRegistryKey = @"SOFTWARE\Autodesk\AutoCAD";

        // 可能的 R 版本号后缀（处理不同更新版本）
        private static List<string> PossibleRVersionSuffixes = new List<string> { ".0", ".1", ".2", ".3" };

        // 可能的产品 ID
        private static List<string> PossibleProductIds = new List<string> { "6001", "7101" };

        // 可能的语言 ID（根据实际需求扩展）
        private static List<string> PossibleLanguageIds = new List<string> { "804", "409" };

        public static string GetAutoCADPath(string preferredVersion = null)
        {
            int currentYear = DateTime.Now.Year;
            var allPossiblePaths = GenerateAllPossiblePaths(2020, currentYear);

            // 优先尝试首选版本
            if (!string.IsNullOrEmpty(preferredVersion) &&
                allPossiblePaths.TryGetValue(preferredVersion, out var paths))
            {
                string path = TryPaths(paths);
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            // 尝试所有其他版本（从最新到最旧）
            for (int year = currentYear; year >= 2020; year--)
            {
                if (allPossiblePaths.TryGetValue(year.ToString(), out var pathss))
                {
                    string path = TryPaths(pathss);
                    if (!string.IsNullOrEmpty(path))
                        return path;
                }
            }

            return null;
        }

        private static Dictionary<string, List<string>> GenerateAllPossiblePaths(int startYear, int endYear)
        {
            var map = new Dictionary<string, List<string>>();

            for (int year = startYear; year <= endYear; year++)
            {
                // 计算基础 R 版本（如 2024 → 25）
                int baseRVersion = year - 2024 + 25;

                // 生成所有可能的 R 版本组合（如 R25.0, R25.1, ...）
                var rVersions = PossibleRVersionSuffixes.Select(suffix =>
                    $"R{baseRVersion}{suffix}").ToList();

                // 生成所有可能的注册表路径组合
                var paths = new List<string>();
                foreach (var rVersion in rVersions)
                {
                    foreach (var productId in PossibleProductIds)
                    {
                        foreach (var languageId in PossibleLanguageIds)
                        {
                            paths.Add($@"{BaseRegistryKey}\{rVersion}\ACAD-{productId}:{languageId}");
                        }
                    }
                }

                map.Add(year.ToString(), paths);
            }

            return map;
        }

        private static string TryPaths(List<string> registryPaths)
        {
            foreach (string path in registryPaths)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        return key.GetValue("AcadLocation") as string;
                    }
                }
            }

            return null;
        }
    }
}
