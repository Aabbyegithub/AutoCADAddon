using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static AutoCADAddon.Model.ClassModel;
using static AutoCADAddon.Model.FloorBuildingDataModel;

namespace AutoCADAddon.Common
{
    /// <summary>
    /// 数据同步类
    /// </summary>
    public class DataSyncService
    {
        // 静态HttpClient实例，确保全局复用
        private static readonly HttpClient _client = new HttpClient();
        // 存储当前有效的令牌
        private static string _authToken;

        // 服务器地址获取
        private static string ServerUrl => GetServerUrl();

        // 构造函数中初始化HttpClient默认设置
        static DataSyncService()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // 设置默认超时时间
            _client.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// 设置认证令牌（登录成功后调用）
        /// </summary>
        public static void SetAuthToken(string token)
        {
            _authToken = token;
            // 更新默认请求头
            UpdateAuthHeader();
        }

        /// <summary>
        /// 更新认证请求头
        /// </summary>
        private static void UpdateAuthHeader()
        {
            // 移除已存在的auth-token头
            if (_client.DefaultRequestHeaders.Contains("auth-token"))
            {
                _client.DefaultRequestHeaders.Remove("auth-token");
            }

            // 添加新的auth-token头
            if (!string.IsNullOrEmpty(_authToken))
            {
                _client.DefaultRequestHeaders.Add("auth-token", _authToken);
            }
        }

        /// <summary>
        /// 登录并设置令牌
        /// </summary>
        public static async Task<string> LoginAsync(string Url, string account, string password)
        {
            try
            {
                // 登录请求不需要携带令牌
                var originalToken = _authToken;
                _authToken = null;
                UpdateAuthHeader();

                var requestBody = new { account, password };
                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"{ServerUrl}/space/auth/login", content);
                var responseJson = await response.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<ResultModel>(responseJson);
                Debug.WriteLine(loginResult);
                if (loginResult != null && !string.IsNullOrEmpty(loginResult.data[1]))
                {
                    // 登录成功，设置令牌
                    SetAuthToken(loginResult.data[1]);
                    return "OK";
                }

                return "NG: 登录失败，未获取到令牌";
            }
            catch (Exception e)
            {
                return $"NG: {e.Message}";
            }
        }

        /// <summary>
        /// 带令牌验证的通用POST请求方法
        /// </summary>
        private static async Task<T> PostWithAuthAsync<T>(string url, object data)
        {
            try
            {
                // 确保令牌已设置
                if (string.IsNullOrEmpty(_authToken))
                {
                    throw new Exception("未进行登录认证，请先登录");
                }
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(url, content);

                // 处理401令牌过期
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // 触发令牌过期事件（可在UI层订阅处理重新登录）
                    OnTokenExpired();
                    throw new UnauthorizedAccessException("令牌已过期，请重新登录");
                }

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            catch (HttpRequestException ex)
            {
                // 处理其他HTTP错误
                throw new Exception($"请求失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 令牌过期事件（供外部订阅）
        /// </summary>
        public static event Action TokenExpired;

        /// <summary>
        /// 触发令牌过期事件
        /// </summary>
        private static void OnTokenExpired()
        {
            TokenExpired?.Invoke();
        }

        #region 业务接口封装（使用带认证的通用方法）

        /// <summary>
        /// 发布图纸数据
        /// </summary>
        public static async Task<string> SyncBlueprintAsync(ResultFloorRoom result)
        {
            try
            {
                var response = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/space/add", result);
                return response.code == "200" ? "OK" : $"NG: {response.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 同步服务器上绑定的图纸属性
        /// </summary>
        public static async Task<dynamic> SyncDrawingServiceAsync(string name)
        {

            try
            {
                var result = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceDraw/page", new { name });
                return result.code == "200" ? result.data : $"NG: {result.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 保存图纸属性
        /// </summary>
        public static async Task<string> SyncDrawingAsync(dynamic result)
        {
            try
            {
                var response = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceDraw/save", result);
                return response.code == "200" ? "OK" : $"NG: {response.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取楼栋数据
        /// </summary>
        public static async Task<dynamic> SyncBuildingAsync()
        {

            try
            {
                var result = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceBuilding/page", new { });
                return result.code == "200" ? result.data : $"NG: {result.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 新增楼栋
        /// </summary>
        public static async Task<string> AddBuildingAsync(string buildingCode, string buildingName)
        {

            try
            {
                var response = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceBuilding/save", new { buildingCode, buildingName });
                return response.code == "200" ? "OK" : response.msg;
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取楼层数据
        /// </summary>
        public static async Task<dynamic> SyncFloorAsync(string BuildCode)
        {

            try
            {
                var result = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceFloor/page", new { buildingCode = BuildCode });
                return result.code == "200" ? result.data : $"NG: {result.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 新增楼层
        /// </summary>
        public static async Task<string> AddFloorAsync(string buildingCode, string floorCode, string floorName)
        {

            try
            {
                var response = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceFloor/save", new { buildingCode, floorCode, floorName });
                return response.code == "200" ? "OK" : response.msg;
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取房间属性
        /// </summary>
        public static async Task<dynamic> SyncRoomServicedataAsync(string building, string floor)
        {

            try
            {
                var result = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceRoom/page", new { building, floor });
                return result.code == "200" ? result.data : $"NG: {result.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 保存房间数据
        /// </summary>
        public static async Task<string> SyncRoomdataAsync(dynamic result)
        {

            try
            {
                var response = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceRoom/save", result);
                return response.code == "200" ? "OK" : $"NG: {response.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取RoomStanard
        /// </summary>
        public static async Task<dynamic> GetRoomStanardAsync()
        {

            try
            {
                var result = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceRoomStandards/page", new { });
                return result.code == "200" ? result.data : $"NG: {result.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取房间类别
        /// </summary>
        public static async Task<dynamic> GetRoomCategoryAsync()
        {

            try
            {
                var result = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceRoomCategory/page", new { });
                return result.code == "200" ? result.data : $"NG: {result.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取房间类型
        /// </summary>
        public static async Task<dynamic> GetRoomTypeAsync(string categoryCode)
        {

            try
            {
                var result = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceRoomType/page", new { categoryCode });
                return result.code == "200" ? result.data : $"NG: {result.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取二级部门
        /// </summary>
        public static async Task<dynamic> GetDepartmentCodeAsync(string divisionCode)
        {

            try
            {
                var result = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceDepartment/page", new { divisionCode });
                return result.code == "200" ? result.data : $"NG: {result.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取一级部门
        /// </summary>
        public static async Task<dynamic> GetDivisionCodeAsync()
        {
            try
            {
                var result = await PostWithAuthAsync<ResultModel>($"{ServerUrl}/spaceDivision/page", new { });
                return result.code == "200" ? result.data : $"NG: {result.msg}";
            }
            catch (UnauthorizedAccessException)
            {
                return "NG: 令牌已过期，请重新登录";
            }
            catch (Exception ex)
            {
                return $"NG: {ex.Message}";
            }
        }

        #endregion

        private static string GetServerUrl()
        {
            var url = CacheManager.GetSys_Server()
                        .FirstOrDefault(a => a.IsTrue == "1")?.Url;
            if (string.IsNullOrEmpty(url))
                return "https://lam-bop-gateway-uat.nwplatform.com.cn/pms";

            return $"{url}/pms";
        }
    }
}
