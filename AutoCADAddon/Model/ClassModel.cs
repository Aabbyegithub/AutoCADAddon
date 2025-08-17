using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADAddon.Model
{
    public class ClassModel
    {
        public class LoginResult
        {
            public string Token { get; set; } // JWT令牌
            public string Username { get; set; } // 用户名
            public string Role { get; set; } // 角色（如管理员、普通用户）
        }


        public class LocalConfig
        {
            public List<string> Servers { get; set; } = new List<string>(); // 服务器列表
            public RememberPassword RememberPassword { get; set; } = new RememberPassword(); // 记住密码信息
        }

        public class RememberPassword
        {
            public bool IsEnabled { get; set; } = false;
            public string EncryptedUsername { get; set; } = "";
            public string EncryptedPassword { get; set; } = "";
        }


        public class RoomData
        {
            public string layerType { get; set; }
            public string rmId { get; set; }
            public double area { get; set; }
            public string coordinate { get; set; }


            //public string[] strings { get; set; }
        }


        public class ResultFloorRoom
        {
            public string buildingCode { get; set; }
            public string buildingName { get; set; }
            public string floorCode { get; set; }
            public string floorName { get; set; }
            public string drawId { get; set; }
            public List<RoomData> data { get; set; }

        }

        public class ResultModel
        {
            public dynamic data { get; set; }
            public string code { get; set; }
            public string ok { get; set; }
            public string msg { get; set; }
        }
    }
}
