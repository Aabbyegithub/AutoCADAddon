using Newtonsoft.Json;
using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace AutoCADAddon.Model
{
    public class FloorBuildingDataModel
    {
        // 建筑
        public class Building
        {
            [JsonProperty("id")]
            [PrimaryKey]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("update_time")]
            public DateTime UpdateTime { get; set; } = DateTime.Now;

            public long LayerId { get; set; }
            public string layerName { get; set; }
        }

        // 楼层
        public class Floor
        {
            [JsonProperty("id")]
            [PrimaryKey]
            public int Id { get; set; }

            [JsonProperty("building_id")]
            public string BuildingCode { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("floor_number")]
            public int FloorNumber { get; set; }

            [JsonProperty("update_time")]
            public DateTime UpdateTime { get; set; } = DateTime.Now;

            public long LayerId { get; set; }
            public string layerName { get; set; }
        }

        // 房间
        public class Room
        {
            [JsonProperty("id")]
            [PrimaryKey]
            public int Id { get; set; }
            public string SerId { get; set; } = "0";

            public string BuildingExternalCode { get; set; }
            public string BuildingName { get; set; }
            public string FloorCode { get; set; }
            public string FloorName { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("area")]
            public string Area { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
            public string RoomStanardCode { get; set; }

            [JsonProperty("category")]
            public string Category { get; set; }
            public string RoomType { get; set; }
            public string DepartmentCode { get; set; }
            public string divisionCode { get; set; }
            public string Prorate { get; set; }
            public string Length { get; set; }
            public string IsSave { get; set; } = "1";

            [JsonProperty("update_time")]
            public DateTime UpdateTime { get; set; } = DateTime.Now;

            public long LayerId { get; set; }
            public string layerName { get; set; }
            public string Coordinates { get; set; }

            // 扩展字段（支持动态类型）
            [JsonProperty("extensions")]
            public Dictionary<string, ExtensionField> Extensions { get; set; } = new Dictionary<string, ExtensionField>();
        }

        // 扩展字段定义
        public class ExtensionField
        {
            [JsonProperty("type")]
            public string Type { get; set; } // text/number/enum/date

            [JsonProperty("value")]
            public object Value { get; set; }

            [JsonProperty("enum_options")]
            public List<string> EnumOptions { get; set; } = new List<string>();
        }

        // 离线操作日志
        public class OfflineOperation
        {
            [JsonProperty("id")]
            [PrimaryKey, AutoIncrement]
            public int OperationId { get; set; }

            [JsonProperty("entity_type")]
            public string EntityType { get; set; } // building/floor/room

            [JsonProperty("operation_type")]
            public string OperationType { get; set; } // create/update/delete

            [JsonProperty("data")]
            public string Data { get; set; }

            [JsonProperty("create_time")]
            public DateTime CreateTime { get; set; } = DateTime.Now;
        }

        /// <summary>
        /// 图纸Model
        /// </summary>
        public class Blueprint
        {
            public int Id { get; set; }
            public string SerId { get; set; } = "0";
            public string Name { get; set; }
            public DateTime UpdateTime { get; set; } = DateTime.Now;
            public string BuildingExternalCode { get; set; }
            public string BuildingName { get; set; }
            public string FloorCode { get; set; }
            public string FloorName { get; set; }
            public string UnitType { get; set; }
            public string Unit { get; set; }
            public string Version { get; set; }
            public string status { get; set; } = "Unpublished";
            public string IsSave { get; set; } = "1";

        }
        /// <summary>
        /// 服务器
        /// </summary>
        public class Sys_Server
        {
            public int Id { get; set; }
            public string Url { get; set; }
            public string IsTrue { get; set; } = "0";
            public DateTime UpdateTime { get; set; } = DateTime.Now;
        }

        /// <summary>
        /// 账户信息
        /// </summary>
        public class Sys_User
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public DateTime UpdateTime { get; set; } = DateTime.Now;
        }
    }
}
