using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public static class DataSyncService
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly string serverUrl ="https://lam-bop-gateway-uat.nwplatform.com.cn/pms";// CacheManager.GetSys_Server().FirstOrDefault(a=>a.IsTrue == "1")?.Url;

        // 同步建筑数据（从服务端到本地缓存）
        public static async Task SyncBuildingsAsync(string serverUrl, string token)
        {
            _client.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer", token);
            var requestBody = new
            {
                last_sync_time = CacheManager.GetLastSyncTime()
            };

            // 使用原生JSON序列化
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/api/buildings", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var Building = JsonConvert.DeserializeObject<List<Building>>(responseJson);
        }

        // 同步楼层数据（从服务端到本地缓存）
        public static async Task SyncFloorsAsync(string serverUrl, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var requestBody = new
            {
                last_sync_time = CacheManager.GetLastSyncTime()
            };

            // 使用原生JSON序列化
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/api/floors", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var Floor = JsonConvert.DeserializeObject<List<Floor>>(responseJson);
        }

        // 同步房间数据
        public static async Task SyncRoomsAsync(string serverUrl, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var requestBody = new
            {
                last_sync_time = CacheManager.GetLastSyncTime()
            };

            // 使用原生JSON序列化
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/api/rooms", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var Room = JsonConvert.DeserializeObject<List<Room>>(responseJson);



        }

        /// <summary>
        /// 初始打开图纸执行同步上传
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> SyncBlueprintAsync(ResultFloorRoom result)
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      
            // 使用原生JSON序列化
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/add", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200") return "OK";
            else return "NG";

        }

        #region 绑定图纸属性
        /// <summary>
        /// 获取楼栋数据
        /// </summary>
        /// <returns></returns>
        public static async Task<dynamic> SyncBuildingAsync()
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var result = new
            {

            };
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceBuilding/page", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200") 
                return res.data;
            else return "NG"+res.msg;

        }

        /// <summary>
        /// 新增楼栋
        /// </summary>
        /// <returns></returns>
        public static async Task<string> AddBuildingAsync( string buildingCode,string buildingName)
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var result = new
            {
              buildingCode ,
              buildingName ,
            };
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceBuilding/save", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200")
                return "OK";
            else return res.msg;

        }

        /// <summary>
        /// 获取楼层数据
        /// </summary>
        /// <returns></returns>
        public static async Task<dynamic> SyncFloorAsync(string BuildCode)
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var result = new
            {
                buildingCode = BuildCode
            };
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceFloor/page", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200")
                return res.data;
            else return "NG"+res.msg;

        }

        /// <summary>
        /// 新增楼层
        /// </summary>
        /// <returns></returns>
        public static async Task<string> AddFloorAsync(string buildingCode, string floorCode, string floorName)
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var result = new
            {
                buildingCode,
                floorCode,
                floorName
            };
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceFloor/save", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200")
                return "OK";
            else return  res.msg;

        }
        #endregion
        #region 获取房间属性

        /// <summary>
        /// 保存房间数据
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task<string> SyncRoomdataAsync(dynamic result)
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceRoom/save", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200") return "OK";
            else return "NG";

        }


        /// <summary>
        /// 获取RoomStanard
        /// </summary>
        /// <returns></returns>
        public static async Task<dynamic> GetRoomStanardAsync()
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var result = new
            {
               
            };
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceRoomStandards/page", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200")
                return res.data;
            else return "NG" + res.msg;

        }

        /// <summary>
        /// 获取房间类别
        /// </summary>
        /// <returns></returns>
        public static async Task<dynamic> GetRoomCategoryAsync()
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var result = new
            {

            };
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceRoomCategory/page", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200")
                return res.data;
            else return "NG" + res.msg;

        }

        /// <summary>
        /// 获取房间类型
        /// </summary>
        /// <returns></returns>
        public static async Task<dynamic> GetRoomTypeAsync(string categoryCode)
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var result = new
            {
                categoryCode
            };
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceRoomType/page", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200")
                return res.data;
            else return "NG" + res.msg;

        }

        /// <summary>
        /// 获取二级部门
        /// </summary>
        /// <returns></returns>
        public static async Task<dynamic> GetDepartmentCodeAsync(string divisionCode)
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var result = new
            {
                divisionCode
            };
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceDepartment/page", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200")
                return res.data;
            else return "NG" + res.msg;

        }


        /// <summary>
        /// 获取一级部门
        /// </summary>
        /// <returns></returns>
        public static async Task<dynamic> GetDivisionCodeAsync()
        {
            //DataSyncService.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 使用原生JSON序列化
            var result = new
            {
                
            };
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{serverUrl}/spaceDivision/page", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResultModel>(responseJson);
            if (res.code == "200")
                return res.data;
            else return "NG" + res.msg;

        }
        #endregion
        // 执行离线操作同步
        //public static async Task SyncOfflineOperations(string serverUrl, string token)
        //{
        //    var operations = CacheManager.GetPendingOperations();
        //    if (operations.Count == 0) return;

        //    foreach (var operation in operations)
        //    {
        //        try
        //        {
        //            var client = new RestClient(serverUrl);
        //            RestRequest request = null;

        //            // 根据操作类型构建请求
        //            switch (operation.OperationType)
        //            {
        //                case "create":
        //                    request = new RestRequest($"/api/{operation.EntityType}s", Method.Post);
        //                    break;
        //                case "update":
        //                    request = new RestRequest($"/api/{operation.EntityType}s", Method.Put);
        //                    break;
        //                case "delete":
        //                    request = new RestRequest($"/api/{operation.EntityType}s/{GetEntityId(operation.Data)}", Method.Delete);
        //                    break;
        //            }

        //            if (request != null)
        //            {
        //                request.AddHeader("Authorization", $"Bearer {token}");
        //                if (operation.OperationType != "delete")
        //                {
        //                    request.AddJsonBody(operation.Data);
        //                }

        //                var response = await client.ExecuteAsync(request);
        //                if (response.IsSuccessful)
        //                {
        //                    CacheManager.MarkOperationAsSynced(operation.OperationId);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // 记录错误，继续下一个操作
        //            System.Diagnostics.Debug.WriteLine($"同步离线操作失败: {ex.Message}");
        //        }
        //    }
        //}

        // 从JSON中提取ID（简化处理）
        private static string GetEntityId(string jsonData)
        {
            try
            {
                var obj = JObject.Parse(jsonData);
                return obj["id"]?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
}
