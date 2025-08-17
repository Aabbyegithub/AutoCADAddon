using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
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

            // 3. 解析图纸中的房间数据
            //var roomDataList = new List<RoomData>();
            var Room = new List<Room>();
            //var roomDataList2 = new List<RoomData>();
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
                        Debug.WriteLine($"图层名称：{polyline.Layer}");
                        //if (!polyline.Layer.ToString().Contains("RM$")) continue;
                        var roomData = ParseRoomFromPolyline(polyline, tr);
                        if (roomData != null)
                        {
                           // roomDataList.Add(roomData);
                            Room.Add(new Room { FloorCode = FloorCode, Name = roomData.rmId, Area = roomData.area.ToString(), Code = roomData.rmId, Coordinates = roomData.coordinate,layerName = polyline.Layer });

                            Debug.WriteLine($"{roomData.rmId}-----{roomData.area}");
                        }

                    }
                }
                tr.Commit();
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
            return Room;
        }


        public static void GetLayerXData()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {



                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;

                foreach (ObjectId layerId in lt)
                {
                    LayerTableRecord ltr = tr.GetObject(layerId, OpenMode.ForRead) as LayerTableRecord;
                    ResultBuffer xdata = null;
                    Debug.WriteLine(ltr.Name);
                    if (ltr.Name.Equals("RM$", StringComparison.OrdinalIgnoreCase))
                    {
                        RegAppTable regTable = tr.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                        foreach (ObjectId id in regTable)
                        {
                            var app = tr.GetObject(id, OpenMode.ForRead) as RegAppTableRecord;
                            xdata = ltr.GetXDataForApplication(app.Name);
                            if (xdata != null)
                            {
                                Debug.WriteLine($"\n图层 {ltr.Name} 的 XData 来自 App：{app.Name}");
                                //ed.WriteMessage($"\n图层 {ltr.Name} 的 XData 来自 App：{app.Name}");
                                foreach (TypedValue tv in xdata)
                                {
                                    Debug.WriteLine($"\n  类型: {tv.TypeCode}, 值: {tv.Value}");
                                    //ed.WriteMessage($"\n  类型: {tv.TypeCode}, 值: {tv.Value}");
                                }
                            }
                        }
                    }
                }

                tr.Commit();
            }
        }


        // 辅助：判断是否为房间图层（根据命名规则）
        private bool IsRoomLayer(string layerName)
        {
            return layerName.StartsWith("ROOM_") ||
                   layerName == "房间边界" ||
                   layerName == "ROOM_BOUNDARY";
        }

        // 辅助：从多段线解析房间数据
        public static RoomData ParseRoomFromPolyline(Polyline polyline, Transaction tr)
        {
            // 计算面积（转换为平方米，假设图纸单位为毫米）
            double area = Math.Round(polyline.Area / 1000000, 2); // 1平方米 = 1e6平方毫米
            if (area.ToString() == "3.24")
            {
                var bb = "";
            }
            //var idArea = GetRoomIdFromAttribute(polyline, tr);
            var Roomcode = "";
            var XData = polyline.XData;
            if (XData != null)
            {
                foreach (var item in XData)
                {
                    if (item.Value.ToString().Contains("}"))
                        break;
                    if (item.TypeCode.ToString().Contains("1000"))
                    {
                        Roomcode = item.Value.ToString();
                    }
                    Debug.WriteLine($"键值{item.TypeCode}--value{item.Value}");

                }
            }



            // 提取坐标（转换为字符串格式）
            string coordinate = GetPolylineCoordinates(polyline);

            return new RoomData
            {
                rmId =Roomcode,
                area = area,
                coordinate = coordinate,
                //strings = idArea
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
