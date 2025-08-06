using AutoCADAddon.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static AutoCADAddon.Model.FloorBuildingDataModel;
using static System.Net.Mime.MediaTypeNames;

namespace AutoCADAddon.Common
{
    public static class CacheManager
    {
        private static string _connectionString;

        static CacheManager()
        {
            try
            {
                // 数据库路径（与原逻辑一致）
                string dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SpaceManagementPlugin", "cache.db"
                );

                // 创建目录
                string dir = Path.GetDirectoryName(dbPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // 连接字符串（System.Data.SQLite 专用）
                _connectionString = $"Data Source={dbPath};Version=3;";

                // 初始化表结构
                InitTables();
                Console.WriteLine("数据库初始化成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化失败: {ex.Message}");
                throw;
            }
        }

        // 初始化表结构（替代 EF Core 的自动创建）
        private static void InitTables()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                ExecuteNonQuery(conn, "DROP TABLE IF EXISTS OfflineOperation");
                ExecuteNonQuery(conn, "DROP TABLE IF EXISTS Building");
                ExecuteNonQuery(conn, "DROP TABLE IF EXISTS Floor");
                ExecuteNonQuery(conn, "DROP TABLE IF EXISTS Room");
                ExecuteNonQuery(conn, "DROP TABLE IF EXISTS Blueprint");
                ExecuteNonQuery(conn, "DROP TABLE IF EXISTS Server");

                //创建服务器列表
                ExecuteNonQuery(conn, @"
                    CREATE TABLE IF NOT EXISTS Server (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Url TEXT UNIQUE,
                        IsTrue TEXT ,
                        UpdateTime DATETIME
                    )
                ");

                //创建账户表
                ExecuteNonQuery(conn, @"
                    CREATE TABLE IF NOT EXISTS Sys_User (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserName TEXT UNIQUE,
                        Password TEXT ,
                        UpdateTime DATETIME
                    )
                ");

                //创建图纸表
                ExecuteNonQuery(conn, @"
                    CREATE TABLE IF NOT EXISTS Blueprint (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SerId TEXT,
                        Name TEXT UNIQUE,
                        UpdateTime DATETIME,
                        BuildingExternalCode TEXT NOT NULL,
                        BuildingName TEXT,
                        FloorCode TEXT,
                        FloorName TEXT NOT NULL,
                        UnitType TEXT,
                        Unit TEXT,
                        Version TEXT,
                        status TEXT,
                        IsSave TEXT
                    )
                ");
                //// 创建建筑表
                //ExecuteNonQuery(conn, @"
                //    CREATE TABLE IF NOT EXISTS Building (
                //        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                //        Name TEXT,
                //        Code TEXT UNIQUE,
                //        UpdateTime DATETIME,
                //        LayerId INTEGER ,
                //        layerName TEXT
                //    )
                //");

                //// 创建楼层表
                //ExecuteNonQuery(conn, @"
                //    CREATE TABLE IF NOT EXISTS Floor (
                //        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                //        BuildingCode TEXT NOT NULL,
                //        Code TEXT UNIQUE,
                //        Name TEXT,
                //        FloorNumber INTEGER,
                //        UpdateTime DATETIME,
                //        LayerId INTEGER ,
                //        layerName TEXT
                //    )
                //");

                // 创建房间表（Extensions 以 JSON 存储）
                ExecuteNonQuery(conn, @"
                    CREATE TABLE IF NOT EXISTS Room (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SerId TEXT,
                        BuildingExternalCode TEXT NOT NULL,
                        BuildingName TEXT,
                        FloorCode TEXT,
                        FloorName TEXT NOT NULL,
                        Name TEXT,
                        Code TEXT UNIQUE,
                        Area TEXT,
                        Type TEXT,
                        RoomStanardCode TEXT,
                        Category TEXT,
                        RoomType TEXT,
                        DepartmentCode TEXT,
                        divisionCode TEXT,
                        Length TEXT,
                        Prorate TEXT,
                        UpdateTime DATETIME,
                        LayerId INTEGER  ,
                        layerName TEXT,
                        Coordinates TEXT,
                        IsSave TEXT,
                        OldCode TEXT,
                        Extensions TEXT  -- 存储 JSON 字符串
                    )
                ");

                // 创建离线操作表
                ExecuteNonQuery(conn, @"
                    CREATE TABLE IF NOT EXISTS OfflineOperation (
                        OperationId INTEGER PRIMARY KEY AUTOINCREMENT,
                        EntityType TEXT,
                        OperationType TEXT,
                        Data TEXT,
                        CreateTime DATETIME
                    )
                ");
            }
        }

        /// <summary>
        /// 保存服务器信息（Sys_Server）
        /// </summary>
        public static void SetSys_Server(Sys_Server sys_Server)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var checkCmd = new SQLiteCommand("PRAGMA table_info(Server);", conn);
                        using (var reader = checkCmd.ExecuteReader())
                        {
                            Console.WriteLine("表结构验证：");
                            while (reader.Read())
                            {
                                Debug.WriteLine($"字段名: {reader["name"]}, 类型: {reader["type"]}, 约束: {reader["pk"]}");
                            }
                        }
                        var cmd = new SQLiteCommand(@"
                            INSERT OR REPLACE INTO Server (Url, IsTrue, UpdateTime) 
                            VALUES (@Url, @IsTrue, @UpdateTime)
                        ", conn);
                        cmd.Parameters.AddWithValue("@Url", sys_Server.Url);
                        cmd.Parameters.AddWithValue("@IsTrue", sys_Server.IsTrue);
                        cmd.Parameters.AddWithValue("@UpdateTime", sys_Server.UpdateTime);
                        cmd.ExecuteNonQuery();                      

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }

            }
        }


        /// <summary>
        /// 批量更新服务器信息（Sys_Server）置为空
        /// </summary>
        public static void UpSys_Server()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 一句SQL批量清除所有 IsTrue
                        var clearCmd = new SQLiteCommand("UPDATE Server SET IsTrue = '0'", conn);
                        clearCmd.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 获取服务器信息（Sys_Server）
        /// </summary>
        public static List<Sys_Server> GetSys_Server()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(@"SELECT * FROM Server", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    var res = new List<Sys_Server>();
                    while (reader.Read())
                    {
                        res.Add(new Sys_Server
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Url = reader["Url"].ToString(),
                            IsTrue = reader["IsTrue"].ToString(),
                            UpdateTime = Convert.ToDateTime(reader["UpdateTime"])
                        });
                        return res;
                    }
                }
            }
            return new List<Sys_Server>();
        }


        /// <summary>
        /// 设置当前登录用户信息（Sys_User）
        /// </summary>
        public static void SetSys_User(Sys_User sys_User)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(@"
                            INSERT OR REPLACE INTO Sys_User (UserName，Password, UpdateTime)
                            VALUES (@UserName,@Password， @UpdateTime)
                        ", conn);
                cmd.Parameters.AddWithValue("@UserName", sys_User.UserName);
                cmd.Parameters.AddWithValue("@Password", sys_User.Password);
                cmd.Parameters.AddWithValue("@UpdateTime", sys_User.UpdateTime);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取当前登录用户信息（Sys_User）
        /// </summary>
        public static Sys_User GetSys_User()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(@"SELECT * FROM Sys_User", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    var res = new Sys_User();
                    if (reader.Read())
                    {
                        res.Id = Convert.ToInt32(reader["Id"]);
                        res.UserName = reader["UserName"].ToString();
                        res.Password = reader["Password"].ToString();
                        res.UpdateTime = Convert.ToDateTime(reader["UpdateTime"]);
                        return res;
                    }
                }
            }
            return new Sys_User();
        }

        /// <summary>
        /// 设置当前图纸属性（保存到本地sqlite）
        /// </summary>
        public static void SetCurrentDrawingProperties(Blueprint blueprint)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(@"
                            INSERT OR REPLACE INTO Blueprint (Name, UpdateTime, BuildingExternalCode,BuildingName,FloorCode,FloorName, UnitType,Unit,Version,status, SerId,IsSave)
                            VALUES (@Name, @UpdateTime, @BuildingExternalCode,@BuildingName,@FloorCode,@FloorName, @UnitType,@Unit,@Version,@status,@SerId,@IsSave)
                        ", conn);
                cmd.Parameters.AddWithValue("@Name", blueprint.Name);
                cmd.Parameters.AddWithValue("@UpdateTime", blueprint.UpdateTime);
                cmd.Parameters.AddWithValue("@BuildingExternalCode", blueprint.BuildingExternalCode);
                cmd.Parameters.AddWithValue("@BuildingName", blueprint.BuildingName);
                cmd.Parameters.AddWithValue("@FloorCode", blueprint.FloorCode);
                cmd.Parameters.AddWithValue("@FloorName", blueprint.FloorName);
                cmd.Parameters.AddWithValue("@UnitType", blueprint.UnitType);
                cmd.Parameters.AddWithValue("@Unit", blueprint.Unit);
                cmd.Parameters.AddWithValue("@Version", blueprint.Version);
                cmd.Parameters.AddWithValue("@status", blueprint.status);
                cmd.Parameters.AddWithValue("@SerId", blueprint.SerId);
                cmd.Parameters.AddWithValue("@IsSave", blueprint.IsSave);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取当前图纸属性
        /// </summary>
        public static Blueprint GetCurrentDrawingProperties(string drawingName)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(@"SELECT * FROM Blueprint WHERE Name = @DrawingName", conn);
                cmd.Parameters.AddWithValue("@DrawingName", drawingName);
                using (var reader = cmd.ExecuteReader())
                {
                    var res = new Blueprint();
                    if (reader.Read())
                    {
                        res.BuildingExternalCode = reader["BuildingExternalCode"].ToString();
                        res.BuildingName = reader["BuildingName"].ToString();
                        res.FloorCode = reader["FloorCode"].ToString();
                        res.FloorName = reader["FloorName"].ToString();
                        res.Name = reader["Name"].ToString();
                        res.UnitType = reader["UnitType"].ToString();
                        res.Unit = reader["Unit"].ToString();
                        res.Version = reader["Version"].ToString();
                        res.status = reader["status"].ToString();
                        res.SerId = reader["SerId"].ToString();
                        res.IsSave = reader["IsSave"].ToString();
                        return res;
                    }
                }
            }
            return new Blueprint();
        }

        public static List<Blueprint> GetCurrentDrawingIsSaveProperties()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(@"SELECT * FROM Blueprint WHERE IsSave ='0'", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    var res = new List<Blueprint>();

                    while (reader.Read())
                    {
                        res.Add(new Blueprint
                        {
                            BuildingExternalCode = reader["BuildingExternalCode"].ToString(),
                            BuildingName = reader["BuildingName"].ToString(),
                            FloorCode = reader["FloorCode"].ToString(),
                            FloorName = reader["FloorName"].ToString(),
                            Name = reader["Name"].ToString(),
                            UnitType = reader["UnitType"].ToString(),
                            Unit = reader["Unit"].ToString(),
                            Version = reader["Version"].ToString(),
                            status = reader["status"].ToString(),
                            SerId = reader["SerId"].ToString(),
                            IsSave = reader["IsSave"].ToString(),
                        });
                    }

                    return res;
                }
            }
        }
        // 工具方法：执行 SQL 命令
        private static void ExecuteNonQuery(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        // 插入/更新建筑
        public static void UpsertBuildings(List<Building> buildings)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var b in buildings)
                        {
                            var cmd = new SQLiteCommand(@"
                                INSERT OR REPLACE INTO Building(
                                    Name, Code, UpdateTime,LayerId,layerName
                                ) VALUES(@Name, @Code, @UpdateTime,@LayerId,@layerName)
                            ", conn, tran);
                            cmd.Parameters.AddWithValue("@Name", b.Name);
                            cmd.Parameters.AddWithValue("@Code", b.Code);
                            cmd.Parameters.AddWithValue("@UpdateTime", b.UpdateTime);
                            cmd.Parameters.AddWithValue("@LayerId", b.LayerId);
                            cmd.Parameters.AddWithValue("@layerName", b.layerName);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }


        public static int UpsertBuilding(Building building)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {

                        var cmd = new SQLiteCommand(@"
                                INSERT OR REPLACE INTO Building(
                                    Name, Code, UpdateTime,LayerId,layerName
                                ) VALUES(@Name, @Code, @UpdateTime,@LayerId,@layerName);
                             SELECT last_insert_rowid(); 
                            ", conn, tran);
                        cmd.Parameters.AddWithValue("@Name", building.Name);
                        cmd.Parameters.AddWithValue("@Code", building.Code);
                        cmd.Parameters.AddWithValue("@UpdateTime", building.UpdateTime);
                        cmd.Parameters.AddWithValue("@LayerId", building.LayerId);
                        cmd.Parameters.AddWithValue("@layerName", building.layerName);
                        var id = cmd.ExecuteScalar();
                        tran.Commit();
                        return Convert.ToInt32(id);
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }



        /// <summary>
        ///  插入/更新图纸
        /// </summary>
        /// <param name="buildings"></param>
        public static void UpsertBlueprint(List<Building> buildings)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var b in buildings)
                        {
                            var cmd = new SQLiteCommand(@"
                                INSERT OR REPLACE INTO Building(
                                    Name, Code, UpdateTime,LayerId,layerName
                                ) VALUES(@Name, @Code, @UpdateTime,@LayerId,@layerName)
                            ", conn, tran);
                            cmd.Parameters.AddWithValue("@Name", b.Name);
                            cmd.Parameters.AddWithValue("@Code", b.Code);
                            cmd.Parameters.AddWithValue("@UpdateTime", b.UpdateTime);
                            cmd.Parameters.AddWithValue("@LayerId", b.LayerId);
                            cmd.Parameters.AddWithValue("@layerName", b.layerName);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        // 获取所有建筑
        public static List<Building> GetBuildings(string BuildCode)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Building WHERE Code=@Code", conn);
                cmd.Parameters.AddWithValue("@Code", BuildCode);
                using (var reader = cmd.ExecuteReader())
                {
                    var result = new List<Building>();
                    while (reader.Read())
                    {
                        result.Add(new Building
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            Name = reader["Name"].ToString(),
                            Code = reader["Code"].ToString(),
                            UpdateTime = Convert.ToDateTime(reader["UpdateTime"]),
                            LayerId = long.Parse(reader["LayerId"].ToString()),
                            layerName = reader["layerName"].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        // 插入/更新楼层（类似建筑方法）
        public static void UpsertFloors(List<Floor> floors)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var f in floors)
                        {
                            var cmd = new SQLiteCommand(@"
                                INSERT OR REPLACE INTO Floor (
                                    BuildingCode, Name, FloorNumber, UpdateTime,Code,LayerId,layerName
                                ) VALUES ( @BuildingCode, @Name, @FloorNumber, @UpdateTime,@Code,@LayerId,@layerName)
                            ", conn, tran);
                            cmd.Parameters.AddWithValue("@BuildingCode", f.BuildingCode);
                            cmd.Parameters.AddWithValue("@Name", f.Name);
                            cmd.Parameters.AddWithValue("@FloorNumber", f.FloorNumber);
                            cmd.Parameters.AddWithValue("@UpdateTime", f.UpdateTime);
                            cmd.Parameters.AddWithValue("@Code", f.Code);
                            cmd.Parameters.AddWithValue("@LayerId", f.LayerId);
                            cmd.Parameters.AddWithValue("@layerName", f.layerName);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        // 获取指定建筑的楼层
        public static List<Floor> GetFloorsByBuilding(string buildingCode)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT * FROM Floor WHERE BuildingCode = @BuildingCode", conn);
                cmd.Parameters.AddWithValue("@BuildingCode", buildingCode);

                using (var reader = cmd.ExecuteReader())
                {
                    var result = new List<Floor>();
                    while (reader.Read())
                    {
                        result.Add(new Floor
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            BuildingCode = reader["BuildingCode"].ToString(),
                            Name = reader["Name"].ToString(),
                            Code = reader["Code"].ToString(),
                            layerName = reader["layerName"].ToString(),
                            FloorNumber = Convert.ToInt32(reader["FloorNumber"]),
                            UpdateTime = Convert.ToDateTime(reader["UpdateTime"]),
                            LayerId = long.Parse(reader["LayerId"].ToString())
                        });
                    }
                    return result;
                }
            }
        }

        // 房间操作（Extensions 以 JSON 存储）
        public static void UpsertRooms(List<Room> rooms)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var r in rooms)
                        {
                            var cmd = new SQLiteCommand(@"
                                INSERT OR REPLACE INTO Room (
                                     BuildingExternalCode,BuildingName,FloorCode,FloorName,  Name, Code, Area, Type, RoomStanardCode,Category,RoomType,Length,DepartmentCode,divisionCode,Prorate,
                                    UpdateTime, Extensions,LayerId,layerName,Coordinates,SerId,IsSave,OldCode
                                ) VALUES (
                                     @BuildingExternalCode,@BuildingName,@FloorCode,@FloorName,  @Name, @Code, @Area, @Type,@RoomStanardCode, @Category,@RoomType,@Length,@DepartmentCode,@divisionCode,@Prorate,
                                    @UpdateTime, @Extensions,@LayerId,@layerName,@Coordinates,@SerId,@IsSave,@OldCode
                                )
                            ", conn, tran);
                            cmd.Parameters.AddWithValue("@BuildingExternalCode", r.BuildingExternalCode);
                            cmd.Parameters.AddWithValue("@BuildingName", r.BuildingName);
                            cmd.Parameters.AddWithValue("@FloorCode", r.FloorCode);
                            cmd.Parameters.AddWithValue("@FloorName", r.FloorName);
                            cmd.Parameters.AddWithValue("@Name", r.Name);
                            cmd.Parameters.AddWithValue("@Code", r.Code);
                            cmd.Parameters.AddWithValue("@Area", r.Area);
                            cmd.Parameters.AddWithValue("@Type", r.Type);
                            cmd.Parameters.AddWithValue("@RoomStanardCode", r.RoomStanardCode);
                            cmd.Parameters.AddWithValue("@Category", r.Category);
                            cmd.Parameters.AddWithValue("@RoomType", r.RoomType);
                            cmd.Parameters.AddWithValue("@DepartmentCode", r.DepartmentCode);
                            cmd.Parameters.AddWithValue("@divisionCode", r.divisionCode);
                            cmd.Parameters.AddWithValue("@Length", r.Length);
                            cmd.Parameters.AddWithValue("@Prorate", r.Prorate);
                            cmd.Parameters.AddWithValue("@UpdateTime", r.UpdateTime);
                            cmd.Parameters.AddWithValue("@LayerId", r.LayerId);
                            cmd.Parameters.AddWithValue("@layerName", r.layerName);
                            cmd.Parameters.AddWithValue("@Coordinates", r.Coordinates);
                            cmd.Parameters.AddWithValue("@SerId", r.SerId);
                            cmd.Parameters.AddWithValue("@IsSave", r.IsSave);
                            cmd.Parameters.AddWithValue("@OldCode", r.OldCode);
                            cmd.Parameters.AddWithValue("@Extensions", JsonConvert.SerializeObject(r.Extensions));
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        // 获取指定楼层的房间
        public static List<Room> GetRoomsByFloor(int floorId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT * FROM Room WHERE FloorExternalId = @FloorId", conn);
                cmd.Parameters.AddWithValue("@FloorId", floorId);

                using (var reader = cmd.ExecuteReader())
                {
                    var result = new List<Room>();
                    while (reader.Read())
                    {
                        result.Add(new Room
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            BuildingExternalCode = reader["BuildingExternalCode"].ToString(),
                            BuildingName = reader["BuildingName"].ToString(),
                            FloorCode = reader["FloorCode"].ToString(),
                            FloorName = reader["FloorName"].ToString(),
                            Name = reader["Name"].ToString(),
                            Code = reader["Code"].ToString(),
                            Area = reader["Area"].ToString(),
                            Type = reader["Type"].ToString(),
                            RoomStanardCode = reader["RoomStanardCode"].ToString(),
                            layerName = reader["layerName"].ToString(),
                            Category = reader["Category"].ToString(),
                            RoomType = reader["RoomType"].ToString(),
                            DepartmentCode = reader["DepartmentCode"].ToString(),
                            divisionCode = reader["divisionCode"].ToString(),
                            Length = reader["Length"].ToString(),
                            Prorate = reader["Prorate"].ToString(),
                            UpdateTime = Convert.ToDateTime(reader["UpdateTime"]),
                            LayerId = long.Parse(reader["LayerId"].ToString()),
                            Coordinates = reader["Coordinates"].ToString(),
                            SerId = reader["SerId"].ToString(),
                            IsSave = reader["IsSave"].ToString(),
                            OldCode = reader["OldCode"].ToString(),
                            Extensions = JsonConvert.DeserializeObject<Dictionary<string, ExtensionField>>(
                                reader["Extensions"].ToString() ?? "{}")
                        });
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// 根据房间编码获取
        /// </summary>
        /// <param name="RoomCode"></param>
        /// <returns></returns>
        public static List<Room> GetRoomsByRoomCode(string RoomCode)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT * FROM Room WHERE Code = @RoomCode", conn);
                cmd.Parameters.AddWithValue("@RoomCode", RoomCode);

                using (var reader = cmd.ExecuteReader())
                {
                    var result = new List<Room>();
                    while (reader.Read())
                    {
                        result.Add(new Room
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            BuildingExternalCode = reader["BuildingExternalCode"].ToString(),
                            BuildingName = reader["BuildingName"].ToString(),
                            FloorCode = reader["FloorCode"].ToString(),
                            FloorName = reader["FloorName"].ToString(),
                            Name = reader["Name"].ToString(),
                            Code = reader["Code"].ToString(),
                            Area = reader["Area"].ToString(),
                            Type = reader["Type"].ToString(),
                            RoomStanardCode = reader["RoomStanardCode"].ToString(),
                            layerName = reader["layerName"].ToString(),
                            Category = reader["Category"].ToString(),
                            RoomType = reader["RoomType"].ToString(),
                            DepartmentCode = reader["DepartmentCode"].ToString(),
                            divisionCode = reader["divisionCode"].ToString(),
                            Length = reader["Length"].ToString(),
                            Prorate = reader["Prorate"].ToString(),
                            UpdateTime = Convert.ToDateTime(reader["UpdateTime"]),
                            LayerId = long.Parse(reader["LayerId"].ToString()),
                            Coordinates = reader["Coordinates"].ToString(),
                            SerId = reader["SerId"].ToString(),
                            IsSave = reader["IsSave"].ToString(),
                            OldCode = reader["OldCode"].ToString(),
                            Extensions = JsonConvert.DeserializeObject<Dictionary<string, ExtensionField>>(
                                reader["Extensions"].ToString() ?? "{}")
                        });
                    }
                    return result;
                }
            }
        }

        public static List<Room> GetRoomsByRoomIsSaveCode()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT * FROM Room WHERE IsSave = '0'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    var result = new List<Room>();
                    while (reader.Read())
                    {
                        result.Add(new Room
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            BuildingExternalCode = reader["BuildingExternalCode"].ToString(),
                            BuildingName = reader["BuildingName"].ToString(),
                            FloorCode = reader["FloorCode"].ToString(),
                            FloorName = reader["FloorName"].ToString(),
                            Name = reader["Name"].ToString(),
                            Code = reader["Code"].ToString(),
                            Area = reader["Area"].ToString(),
                            Type = reader["Type"].ToString(),
                            RoomStanardCode = reader["RoomStanardCode"].ToString(),
                            layerName = reader["layerName"].ToString(),
                            Category = reader["Category"].ToString(),
                            RoomType = reader["RoomType"].ToString(),
                            DepartmentCode = reader["DepartmentCode"].ToString(),
                            divisionCode = reader["divisionCode"].ToString(),
                            Length = reader["Length"].ToString(),
                            Prorate = reader["Prorate"].ToString(),
                            UpdateTime = Convert.ToDateTime(reader["UpdateTime"]),
                            LayerId = long.Parse(reader["LayerId"].ToString()),
                            Coordinates = reader["Coordinates"].ToString(),
                            SerId = reader["SerId"].ToString(),
                            IsSave = reader["IsSave"].ToString(),
                            OldCode = reader["OldCode"].ToString(),
                            Extensions = JsonConvert.DeserializeObject<Dictionary<string, ExtensionField>>(
                                reader["Extensions"].ToString() ?? "{}")
                        });
                    }
                    return result;
                }
            }
        }

        /// <summary>
        ///根据楼层和房间获取
        /// </summary>
        /// <param name="BuildingCode"></param>
        /// <param name="floorCode"></param>
        /// <returns></returns>
        public static List<Room> GetRoomsByRoomCode(string BuildingCode, string floorCode)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT * FROM Room WHERE BuildingExternalCode = @BuildingCode AND FloorCode = @floorCode", conn);
                cmd.Parameters.AddWithValue("@BuildingCode", BuildingCode);
                cmd.Parameters.AddWithValue("@floorCode", floorCode);

                using (var reader = cmd.ExecuteReader())
                {
                    var result = new List<Room>();
                    while (reader.Read())
                    {
                        result.Add(new Room
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            BuildingExternalCode = reader["BuildingExternalCode"].ToString(),
                            BuildingName = reader["BuildingName"].ToString(),
                            FloorCode = reader["FloorCode"].ToString(),
                            FloorName = reader["FloorName"].ToString(),
                            Name = reader["Name"].ToString(),
                            Code = reader["Code"].ToString(),
                            Area = reader["Area"].ToString(),
                            Type = reader["Type"].ToString(),
                            RoomStanardCode = reader["RoomStanardCode"].ToString(),
                            layerName = reader["layerName"].ToString(),
                            Category = reader["Category"].ToString(),
                            RoomType = reader["RoomType"].ToString(),
                            DepartmentCode = reader["DepartmentCode"].ToString(),
                            divisionCode = reader["divisionCode"].ToString(),
                            Length = reader["Length"].ToString(),
                            Prorate = reader["Prorate"].ToString(),
                            UpdateTime = Convert.ToDateTime(reader["UpdateTime"]),
                            LayerId = long.Parse(reader["LayerId"].ToString()),
                            Coordinates = reader["Coordinates"].ToString(),
                            Extensions = JsonConvert.DeserializeObject<Dictionary<string, ExtensionField>>(
                                reader["Extensions"].ToString() ?? "{}")
                        });
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// 获取最后同步时间（从数据库读取，若不存在则返回最小时间）
        /// </summary>
        public static DateTime GetLastSyncTime()
        {
            // 1. 检查是否存在存储同步时间的表（首次运行时创建）
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                // 创建配置表（仅首次运行时执行）
                ExecuteNonQuery(conn, @"
                    CREATE TABLE IF NOT EXISTS SyncConfig (
                        ConfigKey TEXT PRIMARY KEY,
                        ConfigValue TEXT
                    )
                ");

                // 2. 查询最后同步时间
                var cmd = new SQLiteCommand(
                    "SELECT ConfigValue FROM SyncConfig WHERE ConfigKey = 'LastSyncTime'",
                    conn
                );
                var result = cmd.ExecuteScalar()?.ToString();

                if (DateTime.TryParse(result, out DateTime lastSync))
                {
                    return lastSync;
                }
                // 若未设置，返回最小时间
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 更新最后同步时间（调用时机：每次成功同步后）
        /// </summary>
        public static void UpdateLastSyncTime(DateTime time)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                // 插入或更新同步时间
                var cmd = new SQLiteCommand(@"
                    INSERT OR REPLACE INTO SyncConfig (
                        ConfigKey, ConfigValue
                    ) VALUES ('LastSyncTime', @Time)
                ", conn);
                cmd.Parameters.AddWithValue("@Time", time.ToString("o")); // 用 ISO 格式存储
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取所有待同步的离线操作
        /// </summary>
        public static List<FloorBuildingDataModel.OfflineOperation> GetPendingOperations()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT * FROM OfflineOperation ORDER BY CreateTime ASC",
                    conn
                );

                using (var reader = cmd.ExecuteReader())
                {
                    var operations = new List<FloorBuildingDataModel.OfflineOperation>();
                    while (reader.Read())
                    {
                        operations.Add(new FloorBuildingDataModel.OfflineOperation
                        {
                            OperationId = Convert.ToInt32(reader["OperationId"]),
                            EntityType = reader["EntityType"].ToString(),
                            OperationType = reader["OperationType"].ToString(),
                            Data = reader["Data"].ToString(),
                            CreateTime = Convert.ToDateTime(reader["CreateTime"])
                        });
                    }
                    return operations;
                }
            }
        }

        /// <summary>
        /// 标记操作已同步（从离线日志中删除）
        /// </summary>
        public static void MarkOperationAsSynced(int operationId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "DELETE FROM OfflineOperation WHERE OperationId = @OperationId",
                    conn
                );
                cmd.Parameters.AddWithValue("@OperationId", operationId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加离线操作日志（创建/更新/删除时调用）
        /// </summary>
        public static void AddOfflineOperation(FloorBuildingDataModel.OfflineOperation operation)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand(@"
                    INSERT INTO OfflineOperation (
                        EntityType, OperationType, Data, CreateTime
                    ) VALUES (@EntityType, @OperationType, @Data, @CreateTime)
                ", conn);
                cmd.Parameters.AddWithValue("@EntityType", operation.EntityType);
                cmd.Parameters.AddWithValue("@OperationType", operation.OperationType);
                cmd.Parameters.AddWithValue("@Data", operation.Data);
                cmd.Parameters.AddWithValue("@CreateTime", operation.CreateTime);
                cmd.ExecuteNonQuery();
            }
        }
        // 其他方法（GetPendingOperations、MarkOperationAsSynced 等）
        // 均参考上述 SQL 命令模式实现
    }
}