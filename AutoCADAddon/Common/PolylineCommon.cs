using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutoCADAddon.Model.ClassModel;
using static AutoCADAddon.Model.FloorBuildingDataModel;

namespace AutoCADAddon.Common
{
    public class PolylineCommon
    {
        public static List<Room> ParseAndExportDrawingData( Document _doc,string FloorCode)
        {

            if (_doc == null) return null;

            // 1. 获取当前图纸绑定的楼层信息
            var props = CacheManager.GetCurrentDrawingProperties(_doc.Window.Text);

            // 2. 获取楼层详情（含floorCode和floorName）
            //var floor = CacheManager.GetFloorsByBuilding(props.BuildingExternalCode).FirstOrDefault();
            //if (floor == null)
            //{
            //    _doc.Editor.WriteMessage("\n未找到绑定的楼层数据");
            //    return null;
            //}

            // 3. 解析图纸中的房间数据
            var roomDataList = new List<RoomData>();
            var Room = new List<Room>();
            var roomDataList2 = new List<RoomData>();
            var Count = 1;
            using (Transaction tr = _doc.Database.TransactionManager.StartTransaction())
            {
                // 获取块表
                var blockTable = (BlockTable)tr.GetObject(_doc.Database.BlockTableId, OpenMode.ForRead);
                // 获取模型空间块表记录 ID
                var modelSpaceId = blockTable[BlockTableRecord.ModelSpace];
                // 打开模型空间
                var modelSpace = (BlockTableRecord)tr.GetObject(
                   modelSpaceId,
                    OpenMode.ForRead
                );

                // 遍历模型空间中的所有实体
                foreach (ObjectId objId in modelSpace)
                {
                    var entity = tr.GetObject(objId, OpenMode.ForRead) as Entity;
                    if (entity == null) continue;

                    // 过滤出房间边界（假设为闭合多段线且在房间图层）
                    if (entity is Polyline polyline)
                    {
                        var roomData = ParseRoomFromPolyline(polyline, tr);
                        if (roomData != null)
                        {

                            if (roomData.area != double.Parse(roomData.strings == null ? "0" : roomData.strings[1]) && roomData.rmId != "")
                            {
                                roomDataList2.Add(roomData); continue;
                            }
                            roomDataList.Add(roomData);
                            Room.Add(new Room { FloorCode = FloorCode, Name = roomData.rmId, Area = roomData.area.ToString(), Code = roomData.rmId, Coordinates = roomData.coordinate });

                            Debug.WriteLine($"{roomData.rmId}-----{roomData.area}");
                        }

                    }
                }
                tr.Commit();
            }

            foreach (var item in roomDataList2)
            {
                if (roomDataList.FirstOrDefault(a => a.rmId == item.rmId) != null) { var aa = roomDataList.FirstOrDefault(a => a.rmId == item.rmId); continue; }

                if (item.area == 0)
                {
                    item.rmId = "";
                    //roomDataList.Add(item);
                    Room.Add(new Room { FloorCode = FloorCode, Name = $"Room_{Count}", Area = item.area.ToString(), Code = $"Room_{Count}", Coordinates = item.coordinate });
                    Debug.WriteLine($"{item.rmId}-----{item.area}");
                }
                else
                {
                    item.area = double.Parse(item.strings[1]);
                    //roomDataList.Add(item); 
                    Room.Add(new Room { FloorCode = FloorCode, Name = item.rmId, Area = item.area.ToString(), Code = item.rmId, Coordinates = item.coordinate });
                    Debug.WriteLine($"{item.rmId}-----{item.area}");
                }
            }
            Room.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.Code))
                {
                    item.Code = $"Room_{Count}";
                    Count++;
                }
                if (string.IsNullOrEmpty(item.Name))
                {
                    item.Name = $"Room_{Count}";
                }
            });
            //// 4. 生成目标JSON格式
            //var result = new ResultFloorRoom
            //{
            //    floorCode = floor.Code,
            //    floorName = floor.Name,
            //    data = roomDataList
            //};
            //加入图纸房间
            //CacheManager.UpsertRooms(Room);
            return Room;

            //await DataSyncService.SyncBlueprintAsync(result);

        }

        // 辅助：判断是否为房间图层（根据命名规则）
        private bool IsRoomLayer(string layerName)
        {
            return layerName.StartsWith("ROOM_") ||
                   layerName == "房间边界" ||
                   layerName == "ROOM_BOUNDARY";
        }

        // 辅助：从多段线解析房间数据
        private static RoomData ParseRoomFromPolyline(Polyline polyline, Transaction tr)
        {
            var idArea = GetRoomIdFromAttribute(polyline, tr);
            //if (string.IsNullOrEmpty(rmId))
            //{
            //    // 如果属性块中未获取到，再从图层名提取
            //    rmId = polyline.Layer.Split('_').LastOrDefault();
            //}
            //if (string.IsNullOrEmpty(rmId)) return null;

            // 计算面积（转换为平方米，假设图纸单位为毫米）
            double area = Math.Round(polyline.Area / 1000000, 2); // 1平方米 = 1e6平方毫米

            // 提取坐标（转换为字符串格式）
            string coordinate = GetPolylineCoordinates(polyline);

            return new RoomData
            {
                rmId = area == 0 ? "" : (idArea == null ? "" : idArea[0]),
                area = area,
                coordinate = coordinate,
                strings = idArea
            };
        }

        /// <summary>
        /// 获取房间Id
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="tr"></param>
        /// <returns></returns>


        // 辅助：将多段线顶点坐标转换为目标字符串
        public static string GetPolylineCoordinates(Polyline polyline)
        {
            var coords = new List<string>();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                Point2d pt = polyline.GetPoint2dAt(i);
                coords.Add($"({pt.X:0.00},{pt.Y:0.00})");
            }
            // 闭合多段线补充起点（确保闭合）
            if (polyline.Closed && coords.Count > 0)
            {
                coords.Add(coords[0]);
            }
            return string.Join(",", coords);
        }


        public static string[] GetRoomIdFromAttribute(Polyline polyline, Transaction tr)
        {
            BlockTableRecord spaceBtr = tr.GetObject(
                polyline.OwnerId,
                OpenMode.ForRead
            ) as BlockTableRecord;

            if (spaceBtr == null) return null;

            //string[] roomInfo = TryGetRoomInfoFromXData(polyline);
            //if (roomInfo != null)
            //    return roomInfo;

            // 方法1：使用中心点距离筛选
            Point3d polylineCenter = new Point3d(
                (polyline.GeometricExtents.MinPoint.X + polyline.GeometricExtents.MaxPoint.X) / 2,
                (polyline.GeometricExtents.MinPoint.Y + polyline.GeometricExtents.MaxPoint.Y) / 2,
                0
            );

            // 收集所有候选MText
            var candidateMtexts = new List<MText>();

            foreach (ObjectId objId in spaceBtr)
            {
                Entity ent = tr.GetObject(objId, OpenMode.ForRead) as Entity;
                if (ent is MText mtext)
                {
                    // 获取多行文本内容并处理换行符
                    string content = mtext.Contents.Replace("\\P", "\n").Trim();
                    string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    // 至少需要两行（ID和面积）
                    if (lines.Length > 3)
                        continue;
                    // 提取第一行作为房间ID候选
                    string roomIdCandidate = lines[0].Trim();

                    candidateMtexts.Add(mtext);
                }
            }

            // 如果没有候选，返回null
            if (candidateMtexts.Count == 0)
                return null;

            // 方法2：根据中心点距离排序
            candidateMtexts.Sort((a, b) =>
            {
                Point3d aCenter = new Point3d(
                    (a.GeometricExtents.MinPoint.X + a.GeometricExtents.MaxPoint.X) / 2,
                    (a.GeometricExtents.MinPoint.Y + a.GeometricExtents.MaxPoint.Y) / 2,
                    0
                );

                Point3d bCenter = new Point3d(
                    (b.GeometricExtents.MinPoint.X + b.GeometricExtents.MaxPoint.X) / 2,
                    (b.GeometricExtents.MinPoint.Y + b.GeometricExtents.MaxPoint.Y) / 2,
                    0
                );

                double distanceA = Math.Sqrt(
                    Math.Pow(aCenter.X - polylineCenter.X, 2) +
                    Math.Pow(aCenter.Y - polylineCenter.Y, 2)
                );

                double distanceB = Math.Sqrt(
                    Math.Pow(bCenter.X - polylineCenter.X, 2) +
                    Math.Pow(bCenter.Y - polylineCenter.Y, 2)
                );

                return distanceA.CompareTo(distanceB);
            });

            // 方法3：检查文字是否在多段线内部
            foreach (var mtext in candidateMtexts)
            {
                Point3d mtextPosition = mtext.Location;

                // 使用射线法判断点是否在多边形内部
                if (IsPointInPolyline(mtextPosition, polyline))
                {
                    string content = mtext.Contents.Replace("\\P", "\n").Trim();
                    return content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            // 如果没有内部的，返回距离最近的
            string contentNearest = candidateMtexts[0].Contents.Replace("\\P", "\n").Trim();
            return contentNearest.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }

        // 使用射线法判断点是否在多段线内部
        private static bool IsPointInPolyline(Point3d point, Polyline polyline)
        {
            bool inside = false;
            int vertexCount = polyline.NumberOfVertices;

            for (int i = 0, j = vertexCount - 1; i < vertexCount; j = i++)
            {
                Point3d vertexI = polyline.GetPoint3dAt(i);
                Point3d vertexJ = polyline.GetPoint3dAt(j);

                if (((vertexI.Y > point.Y) != (vertexJ.Y > point.Y)) &&
                    (point.X < (vertexJ.X - vertexI.X) * (point.Y - vertexI.Y) / (vertexJ.Y - vertexI.Y) + vertexI.X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
