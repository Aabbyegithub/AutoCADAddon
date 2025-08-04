using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static AutoCADAddon.Model.FloorBuildingDataModel;

namespace AutoCADAddon.Common
{
    /// <summary>
    /// 空间对象绑定器
    /// </summary>
    public static class ObjectBinder
    {
        // 绑定对象到建筑结构节点
        public static void BindObjectToNode(
            object nodeTag, // Building/Floor/Room
            ObjectId entityId,
            Document doc)
        {
            var binding = new ObjectBinding
            {
                EntityId = entityId.Handle.Value,
                NodeType = nodeTag.GetType().Name,
                NodeId = GetNodeId(nodeTag)
            };

            // 存储到数据库或缓存
            //CacheManager.AddObjectBinding(binding);
            doc.Editor.WriteMessage($"\n对象已绑定到 {nodeTag.GetType().Name}");
        }

        // 从缓存加载绑定关系
        //public static List<ObjectBinding> GetBindingsForNode(object nodeTag)
        //{
        //    return CacheManager.GetObjectBindings()
        //        .Where(b => b.NodeType == nodeTag.GetType().Name
        //                && b.NodeId == GetNodeId(nodeTag))
        //        .ToList();
        //}

        private static int GetNodeId(object nodeTag)
        {
            switch (nodeTag)
            {
                case Building b:
                    return b.Id;
                case Floor f:
                    return f.Id;
                case Room r:
                    return r.Id;
                default:
                    return 0; // 或抛出异常，视具体需求而定
            }
        }
    }

    // 绑定关系模型
    public class ObjectBinding
    {
        public long EntityId { get; set; }
        public string NodeType { get; set; }
        public int NodeId { get; set; }
    }
}