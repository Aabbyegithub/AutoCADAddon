using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;

namespace AutoCADAddon.Common
{
    public static class LayerManager
    {
        public enum LayerType
        {
            Building,
            Floor,
            Room,
            Default
        }

        private static readonly Dictionary<LayerType, LayerConfig> LayerConfigs = new Dictionary<LayerType, LayerConfig>()
        {
            { LayerType.Building, new LayerConfig { ColorIndex = 1, Linetype = "CONTINUOUS", IsLocked = false } },
            { LayerType.Floor, new LayerConfig { ColorIndex = 3, Linetype = "CONTINUOUS", IsLocked = false } },
            { LayerType.Room, new LayerConfig { ColorIndex = 5, Linetype = "CONTINUOUS", IsLocked = false } },
            { LayerType.Default, new LayerConfig { ColorIndex = 7, Linetype = "CONTINUOUS", IsLocked = false } }
        };

        // 创建新图层（分离读写事务）
        public static string CreateLayer(string layerName, Document doc, LayerType layerType = LayerType.Default)
        {
            // 1. 先检查图层是否存在（使用独立只读事务）
            bool layerExists;
            using (var readTrans = doc.Database.TransactionManager.StartTransaction())
            {
                var layerTable = (LayerTable)readTrans.GetObject(
                    doc.Database.LayerTableId,
                    OpenMode.ForRead
                );
                layerExists = layerTable.Has(layerName);
            }

            // 2. 如果图层已存在，直接返回其ID
            if (layerExists)
            {
                doc.Editor.WriteMessage($"\n图层 {layerName} 已存在");

                // 使用安全方法获取图层ID
                return "";//GetLayerId(layerName, doc);
            }

            // 3. 创建新图层（使用独立写事务）
            //ObjectId layerId = ObjectId.Null;

            // 使用 Document.LockDocument 确保操作独占
            using (doc.LockDocument())
            {
                using (var writeTrans = doc.Database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        // 直接以写模式打开图层表（避免升级）
                        var layerTable = (LayerTable)writeTrans.GetObject(
                            doc.Database.LayerTableId,
                            OpenMode.ForWrite
                        );

                        // 获取图层配置
                        var config = LayerConfigs.TryGetValue(layerType, out var cfg)
                            ? cfg
                            : LayerConfigs[LayerType.Default];

                        // 创建新图层记录
                        var layer = new LayerTableRecord
                        {
                            Name = layerName,
                            Color = Color.FromColorIndex(ColorMethod.ByAci, config.ColorIndex),
                            IsLocked = config.IsLocked
                        };

                        // 设置线型（使用嵌套只读事务）
                        using (var linetypeTrans = doc.Database.TransactionManager.StartTransaction())
                        {
                            var linetypeTable = (LinetypeTable)linetypeTrans.GetObject(
                                doc.Database.LinetypeTableId,
                                OpenMode.ForRead
                            );

                            if (linetypeTable.Has(config.Linetype))
                            {
                                layer.LinetypeObjectId = linetypeTable[config.Linetype];
                            }
                            else
                            {
                                layer.LinetypeObjectId = linetypeTable["CONTINUOUS"];
                                doc.Editor.WriteMessage($"\n警告：线型 {config.Linetype} 不存在，使用 CONTINUOUS");
                            }
                            // 嵌套事务无需提交，外层事务会处理
                        }

                        // 添加图层到图层表
                        layerTable.Add(layer);
                        writeTrans.AddNewlyCreatedDBObject(layer, true);

                        // 保存图层ID并提交事务
                        //layerId = layer.ObjectId;
                        writeTrans.Commit();

                        doc.Editor.WriteMessage($"\n成功创建图层：{layerName}");
                    }
                    catch (System.Exception ex)
                    {
                        doc.Editor.WriteMessage($"\n创建图层失败: {ex.Message}");
                        writeTrans.Abort();
                    }
                }
            }

            return layerName;
        }

        // 安全获取图层ID的辅助方法
        private static ObjectId GetLayerId(string layerName, Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                var layerTable = (LayerTable)trans.GetObject(
                    doc.Database.LayerTableId,
                    OpenMode.ForRead
                );

                if (layerTable.Has(layerName))
                {
                    return layerTable[layerName];
                }

                return ObjectId.Null;
            }
        }

        // 设置当前图层（使用独立事务）
        public static ObjectId SetCurrentLayer(string layerName, Document doc)
        {
            using (doc.LockDocument())
            {
                using (var trans = doc.Database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var layerTable = (LayerTable)trans.GetObject(doc.Database.LayerTableId, OpenMode.ForRead);
                        if (!layerTable.Has(layerName))
                        {
                            doc.Editor.WriteMessage($"\n错误：图层 {layerName} 不存在");
                            return ObjectId.Null;
                        }
                        var layerId = layerTable[layerName];
                        var layerObj = trans.GetObject(layerId, OpenMode.ForRead, false) as LayerTableRecord;
                        if (layerObj == null || layerObj.IsErased)
                        {
                            doc.Editor.WriteMessage($"\n错误：图层 {layerName} 已被删除");
                            return ObjectId.Null;
                        }
                        doc.Database.Clayer = layerId;
                        trans.Commit();
                        doc.Editor.WriteMessage($"\n当前图层切换为: {layerName}");
                        return layerId;
                    }
                    catch (System.Exception ex)
                    {
                        doc.Editor.WriteMessage($"\n设置图层失败: {ex.Message}");
                        return ObjectId.Null;
                    }
                }
            }
        }

        // 重命名图层
        public static bool RenameLayer(string oldName, string newName, Document doc)
        {
            if (!LayerExists(oldName, doc))
            {
                doc.Editor.WriteMessage($"\n错误：图层 {oldName} 不存在");
                return false;
            }

            if (LayerExists(newName, doc))
            {
                doc.Editor.WriteMessage($"\n错误：图层 {newName} 已存在");
                return false;
            }

            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                var layerTable = (LayerTable)trans.GetObject(
                    doc.Database.LayerTableId,
                    OpenMode.ForRead
                );

                var layerId = layerTable[oldName];
                var layer = (LayerTableRecord)trans.GetObject(layerId, OpenMode.ForRead);

                try
                {
                    layer.UpgradeOpen();
                    layer.Name = newName;
                    trans.Commit();
                    doc.Editor.WriteMessage($"\n图层 {oldName} 已重命名为 {newName}");
                    return true;
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage($"\n重命名图层失败: {ex.Message}");
                    return false;
                }
            }
        }

        // 删除图层
        public static bool DeleteLayer(string layerName, Document doc, string defaultLayer = "0")
        {
            if (!LayerExists(layerName, doc))
            {
                doc.Editor.WriteMessage($"\n错误：图层 {layerName} 不存在");
                return false;
            }

            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                var layerTable = (LayerTable)trans.GetObject(
                    doc.Database.LayerTableId,
                    OpenMode.ForRead
                );

                var layerId = layerTable[layerName];
                var layer = (LayerTableRecord)trans.GetObject(layerId, OpenMode.ForRead);

                if (layer.IsLocked || layer.IsFrozen || !layer.IsPlottable ||
                    layer.Name == "0" || layer.Name == "Defpoints")
                {
                    doc.Editor.WriteMessage($"\n错误：无法删除图层 {layerName}（系统保留或被锁定/冻结）");
                    return false;
                }

                // 移动实体到默认图层
                MoveEntitiesToLayer(layerName, defaultLayer, doc, trans);

                try
                {
                    layer.UpgradeOpen();
                    layer.Erase();
                    trans.Commit();
                    doc.Editor.WriteMessage($"\n图层 {layerName} 已删除");
                    return true;
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage($"\n删除图层失败: {ex.Message}");
                    return false;
                }
            }
        }

        // 移动实体到目标图层
        private static void MoveEntitiesToLayer(string sourceLayer, string targetLayer, Document doc, Transaction trans)
        {
            var layerTable = (LayerTable)trans.GetObject(
                doc.Database.LayerTableId,
                OpenMode.ForRead
            );

            if (!layerTable.Has(targetLayer))
            {
                // 确保目标图层存在
                using (var createTrans = doc.Database.TransactionManager.StartTransaction())
                {
                    CreateLayer(targetLayer, doc);
                    createTrans.Commit();
                }

                // 重新获取图层表
                layerTable = (LayerTable)trans.GetObject(
                    doc.Database.LayerTableId,
                    OpenMode.ForRead
                );
            }

            var targetLayerId = layerTable[targetLayer];
            var btr = (BlockTableRecord)trans.GetObject(
                doc.Database.CurrentSpaceId,
                OpenMode.ForRead
            );

            foreach (ObjectId objId in btr)
            {
                if (!objId.IsErased)
                {
                    var entity = (Entity)trans.GetObject(objId, OpenMode.ForRead);
                    if (entity.Layer == sourceLayer)
                    {
                        entity.UpgradeOpen();
                        entity.LayerId = targetLayerId;
                    }
                }
            }
        }

        // 检查图层是否存在
        public static bool LayerExists(string layerName, Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                var layerTable = (LayerTable)trans.GetObject(
                    doc.Database.LayerTableId,
                    OpenMode.ForRead
                );
                return layerTable.Has(layerName);
            }
        }

        // 获取图层名称
        private static string GetLayerName(ObjectId layerId, Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                var layer = (LayerTableRecord)trans.GetObject(layerId, OpenMode.ForRead);
                return layer.Name;
            }
        }

        private class LayerConfig
        {
            public short ColorIndex { get; set; }
            public string Linetype { get; set; }
            public bool IsLocked { get; set; }
        }
    }
}