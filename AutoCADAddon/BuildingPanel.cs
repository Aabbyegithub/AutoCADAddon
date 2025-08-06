using AutoCADAddon.Common;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using static AutoCADAddon.Model.ClassModel;
using static AutoCADAddon.Model.FloorBuildingDataModel;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADAddon
{
    public class BuildingPanel
    {
        private readonly PaletteSet _paletteSet;
        private readonly TreeView _treeView;
        private Document _doc;
        private bool _isProcessing = false;

        public BuildingPanel()
        {

            // 注册文件打开事件
            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentToBeDestroyed += DocumentManager_DocumentToBeDestroyed;


            _paletteSet = new PaletteSet("建筑结构管理", "SpaceManagementPlugin.BuildingPanel", new Guid("DE1F3EC7-C76D-495F-B19B-7BA3CFED24EA"));
            _paletteSet.Size = new System.Drawing.Size(400, 500);
            _treeView = new TreeView { Dock = DockStyle.Fill };
            _treeView.AfterSelect += TreeView_AfterSelect;
            _treeView.ContextMenuStrip = new ContextMenuStrip();

            // 绑定右键菜单
            _treeView.ContextMenuStrip.Items.Add("新建建筑", null, NewBuilding_Click);
            _treeView.ContextMenuStrip.Items.Add("新建楼层", null, NewFloor_Click);
            _treeView.ContextMenuStrip.Items.Add("新建房间", null, NewRoom_Click);
            _treeView.ContextMenuStrip.Items.Add("创建图层", null, CreateLayer_Click);
            _treeView.ContextMenuStrip.Items.Add("拾取绑定对象", null, BindObject_Click);
            _treeView.ContextMenuStrip.Items.Add("编辑", null, Edit_Click);
            _treeView.ContextMenuStrip.Items.Add("删除", null, Delete_Click);
            _treeView.ContextMenuStrip.Items.Add("图层同步设置", null, LayerSyncSettings_Click);
            // 图层同步设置菜单事件

            //Panel panel = new Panel();
            //panel.Controls.Add(_TopBarUserControl);
            //panel.Controls.Add(_treeView);
            //panel.Width = 400;

            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Dock = DockStyle.Fill;
            // 设置 2 行，第一行用于放顶部栏（高度自适应内容），第二行放 TreeView（填充剩余空间）
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            //tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            // 设置 1 列，让内容水平填充
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tableLayoutPanel.Controls.Add(_treeView, 0, 1); // 放在第二行第一列

            _paletteSet.Add("建筑结构", tableLayoutPanel);
            _paletteSet.Visible = true;
            Timer timer = new Timer();
            timer.Interval = 20; // 100毫秒延迟
            timer.Tick += (s, e) =>
            {
                timer.Dispose(); // 执行一次后销毁定时器
            };
            timer.Start();

            //if (!_doc.IsNamedDrawing)
            //{
            //    // 新建的文档（如 Drawing1.dwg），不弹窗
            //    return;
            //}
            //// 判断是否已绑定属性
            //var props = CacheManager.GetCurrentDrawingProperties(_doc.Window.Text);
            //if (!string.IsNullOrEmpty(props.Name)&& props.BuildingExternalId.HasValue)
            //{
            //    // 已绑定，直接刷新树
            //    LoadData();
            //    return;
            //}else
            //// 首次打开时弹出属性设置
            //      ShowDrawingPropertiesForm();
        // 文档激活时弹出属性设置窗口

        }



        private void LayerSyncSettings_Click(object sender, EventArgs e)
        {
            using (var form = new LayerSyncSettingsForm(_doc))
            {
                form.ShowDialog();
            }
        }
        private void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            _doc = e.Document;

            _doc.Database.SaveComplete += Database_SaveComplete;
            // 仅在打开已有文件时弹窗，新建不弹
            if (!e.Document.IsNamedDrawing)
            {
                // 新建的文档（如 Drawing1.dwg），不弹窗
                return;
            }
            //// 判断是否已绑定属性
            var props = CacheManager.GetCurrentDrawingProperties(_doc.Window.Text);
            if (!string.IsNullOrEmpty(props.Name)&&!string.IsNullOrEmpty(props.BuildingExternalCode) )
            {
                // 已绑定，直接刷新树
                LoadData();
                return;
            }

               ShowDrawingPropertiesForm();


        }

        private void DocumentManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
            // 文档即将关闭
            if (_doc != null && e.Document.Name == _doc.Name)
            {
                 _doc.Database.SaveComplete -= Database_SaveComplete;
                _doc = null;
                _treeView.Nodes.Clear();
            }
        }

        private async void Database_SaveComplete(object sender, EventArgs e)
        {
            if (_doc == null) return;
            if (_isProcessing) return; // 防止重复执行
            _isProcessing = true;
            //await Task.Run(() =>
            //{
                try
                {
                    ParseAndExportDrawingData();

                    Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.QueueForGraphicsFlush();
                    Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen();

                    //this._treeView.Invoke(new Action(() =>
                    //{
                        using (DocumentLock docLock = _doc.LockDocument())
                        {
                            LoadData();
                        }
                    //}));
                     _doc.Editor.WriteMessage($"\n插件保存成功");
                }
                catch (Exception ex)
                {
                    // 在主线程显示错误消息
                   //this._treeView.Invoke(new Action(() =>
                   // {
                        _doc.Editor.WriteMessage($"\n保存后操作失败：{ex.Message}");
                    //}));
                }
                finally
                {
                    _isProcessing = false;
                }
            //});
        }
        // 弹出属性设置窗口
        private void ShowDrawingPropertiesForm()
        {
            using (var form = new DrawingPropertiesForm(_doc,"OLD"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // 保存到本地sqlite，后续可提交到服务器
                    //CacheManager.SetCurrentDrawingProperties(_doc.Name, form.SelectedBuilding, form.SelectedFloor, form.SelectedRoom);
                    //_doc.Editor.WriteMessage($"\n已设置当前图纸属性：建筑={form.SelectedBuilding?.Name}，楼层={form.SelectedFloor?.Name}");
                    //Task.Run(() =>
                    //{
                        ParseAndExportDrawingData();
                        LoadData();
                    //});

                
                }
            }
        }


        // 加载建筑、楼层、房间数据
        public void LoadData()
        {
            //if (_isDataLoaded) return;
            _treeView.Nodes.Clear();

            var Blueprint = CacheManager.GetCurrentDrawingProperties(_doc.Window.Text);

            // 加载建筑
            var buildings = CacheManager.GetBuildings(Blueprint.BuildingExternalCode);
            foreach (var building in buildings)
            {
                var buildingNode = new TreeNode(building.Name) { Tag = building };

                // 加载楼层
                var floors = CacheManager.GetFloorsByBuilding(building.Code);
                foreach (var floor in floors)
                {
                    var floorNode = new TreeNode($"楼层 {floor.FloorNumber} - {floor.Name}") { Tag = floor };

                    // 加载房间
                    var rooms = CacheManager.GetRoomsByFloor(floor.Id);
                    foreach (var room in rooms)
                    {
                        var roomNode = new TreeNode($"房间 {room.Name}") { Tag = room };
                        floorNode.Nodes.Add(roomNode);
                    }

                    buildingNode.Nodes.Add(floorNode);
                }

                _treeView.Nodes.Add(buildingNode);
                //_isDataLoaded = true;
            }
            _treeView.ExpandAll();
        }

        // 选中节点时在命令行提示
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is Building building)
            {
                _doc.Editor.WriteMessage($"\n选中建筑：{building.Name}（编码：{building.Code}）");
                if (building.LayerId != 0)
                {
                    var layerId = new ObjectId((IntPtr)building.LayerId);
                    LayerManager.SetCurrentLayer(building.layerName, _doc);
                }
            }
            else if (e.Node.Tag is Floor floor)
            {
                _doc.Editor.WriteMessage($"\n选中楼层：{floor.FloorNumber}层（建筑ID：{floor.BuildingCode}）");
                if (floor.LayerId != 0)
                {
                    var layerId = new ObjectId((IntPtr)floor.LayerId);
                    LayerManager.SetCurrentLayer(floor.layerName, _doc);
                }
            }
            else if (e.Node.Tag is Room room)
            {
                _doc.Editor.WriteMessage($"\n选中房间：{room.Name}（楼层ID：{room.FloorCode}）");
                if (room.LayerId != 0)
                {
                    var layerId = new ObjectId((IntPtr)room.LayerId);
                    LayerManager.SetCurrentLayer(room.layerName, _doc);
                }
            }
        }

        // 新建建筑
        private void NewBuilding_Click(object sender, EventArgs e)
        {
            var form = new BuildingEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                CacheManager.UpsertBuildings(new List<Building>() { form.EditedBuilding });
                CacheManager.AddOfflineOperation(new OfflineOperation
                {
                    EntityType = "building",
                    OperationType = "create",
                    Data = JsonConvert.SerializeObject(form.EditedBuilding)
                });
                LoadData();
            }
        }

        // 新建楼层（需选中建筑节点）
        private void NewFloor_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = _treeView.SelectedNode;
            if (selectedNode == null || !(selectedNode.Tag is Building))
            {
                _doc.Editor.WriteMessage("\n请先选中建筑节点");
                return;
            }
            Building building = (Building)selectedNode.Tag;

            var form = new FloorEditForm(null, building.Code);
            if (form.ShowDialog() == DialogResult.OK)
            {
                CacheManager.UpsertFloors(new List<Floor>() { form.EditedFloor });
                CacheManager.AddOfflineOperation(new OfflineOperation
                {
                    EntityType = "floor",
                    OperationType = "create",
                    Data = JsonConvert.SerializeObject(form.EditedFloor)
                });
                LoadData();
            }
        }

        // 新建房间（需选中楼层节点）
        private void NewRoom_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = _treeView.SelectedNode;
            if (selectedNode == null || !(selectedNode.Tag is Floor))
            {
                _doc.Editor.WriteMessage("\n请先选中建筑节点");
                return;
            }
            Floor floor = (Floor)selectedNode.Tag;
            //var form = new RoomEditForm(null, floor.Id);
            //if (form.ShowDialog() == DialogResult.OK)
            //{
            //    CacheManager.UpsertRooms(new List<Room>() { form.EditedRoom });
            //    CacheManager.AddOfflineOperation(new OfflineOperation
            //    {
            //        EntityType = "room",
            //        OperationType = "create",
            //        Data = JsonConvert.SerializeObject(form.EditedRoom)
            //    });
            //    LoadData();
            //}
        }

        // 编辑（支持建筑、楼层、房间）
        private void Edit_Click(object sender, EventArgs e)
        {
            if (_treeView.SelectedNode?.Tag is Building building)
            {
                var form = new BuildingEditForm(building);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CacheManager.UpsertBuildings(new List<Building>() { form.EditedBuilding });
                    CacheManager.AddOfflineOperation(new OfflineOperation
                    {
                        EntityType = "building",
                        OperationType = "update",
                        Data = JsonConvert.SerializeObject(form.EditedBuilding)
                    });
                    LoadData();
                }
            }
            else if (_treeView.SelectedNode?.Tag is Floor floor)
            {
                var form = new FloorEditForm(floor, floor.BuildingCode);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CacheManager.UpsertFloors(new List<Floor>() { form.EditedFloor });
                    CacheManager.AddOfflineOperation(new OfflineOperation
                    {
                        EntityType = "floor",
                        OperationType = "update",
                        Data = JsonConvert.SerializeObject(form.EditedFloor)
                    });
                    LoadData();
                }
            }
            //else if (_treeView.SelectedNode?.Tag is Room room)
            //{
            //    var form = new RoomEditForm(room, room.FloorId);
            //    if (form.ShowDialog() == DialogResult.OK)
            //    {
            //        CacheManager.UpsertRooms(new List<Room>() { form.EditedRoom });
            //        CacheManager.AddOfflineOperation(new OfflineOperation
            //        {
            //            EntityType = "room",
            //            OperationType = "update",
            //            Data = JsonConvert.SerializeObject(form.EditedRoom)
            //        });
            //        LoadData();
            //    }
            //}
        }

        // 删除Delete_Click方法
        private void Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认删除？删除后需联网同步才会在服务端生效", "警告",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            if (_treeView.SelectedNode?.Tag is Building building)
            {
                // 记录离线删除操作
                CacheManager.AddOfflineOperation(new OfflineOperation
                {
                    EntityType = "building",
                    OperationType = "delete",
                    Data = JsonConvert.SerializeObject(new { id = building.Id })
                });
                _treeView.Nodes.Remove(_treeView.SelectedNode); // 从UI移除
            }
            else if (_treeView.SelectedNode?.Tag is Floor floor)
            {
                CacheManager.AddOfflineOperation(new OfflineOperation
                {
                    EntityType = "floor",
                    OperationType = "delete",
                    Data = JsonConvert.SerializeObject(new { id = floor.Id })
                });
                _treeView.SelectedNode.Parent?.Nodes.Remove(_treeView.SelectedNode);
            }
            else if (_treeView.SelectedNode?.Tag is Room room)
            {
                CacheManager.AddOfflineOperation(new OfflineOperation
                {
                    EntityType = "room",
                    OperationType = "delete",
                    Data = JsonConvert.SerializeObject(new { id = room.Id })
                });
                _treeView.SelectedNode.Parent?.Nodes.Remove(_treeView.SelectedNode);
            }
        }

        // 新建图层命令
        private void CreateLayer_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = _treeView.SelectedNode;
            if (selectedNode == null)
            {
                _doc.Editor.WriteMessage("\n请先选中节点");
                return;
            }

            string layerName = string.Empty;
            LayerManager.LayerType layerType;

            if (selectedNode.Tag is Building building)
            {
                layerName = $"BUILDING_{building.Id}";
                layerType = LayerManager.LayerType.Building;
            }
            else if (selectedNode.Tag is Floor floor)
            {
                layerName = $"FLOOR_{floor.Id}";
                layerType = LayerManager.LayerType.Floor;
            }
            else if (selectedNode.Tag is Room room)
            {
                layerName = $"ROOM_{room.Id}";
                layerType = LayerManager.LayerType.Room;
            }
            else
            {
                _doc.Editor.WriteMessage("\n不支持的节点类型");
                return;
            }

            var layer = LayerManager.CreateLayer(layerName, _doc, layerType);
            if (string.IsNullOrEmpty(layer))
            {
                return;
            }
            var layerId = LayerManager.SetCurrentLayer(layerName, _doc);

            // 更新节点的LayerId
            UpdateNodeLayerId(selectedNode.Tag, layerId.Handle.Value, layerName);
        }

        // 辅助方法：更新节点的LayerId并保存
        private void UpdateNodeLayerId(object node, long layerHandle,string layerName)
        {
            if (node is Building building)
            {
                building.LayerId = layerHandle;
                building.layerName = layerName;
                CacheManager.UpsertBuildings(new List<Building> { building });
            }
            else if (node is Floor floor)
            {
                floor.LayerId = layerHandle;
                floor.layerName = layerName;
                CacheManager.UpsertFloors(new List<Floor> { floor });
            }
            else if (node is Room room)
            {
                room.LayerId = layerHandle;
                room.layerName = layerName;
                CacheManager.UpsertRooms(new List<Room> { room });
            }
        }

        // 拾取对象绑定命令，补充Floor和Room逻辑
        private void BindObject_Click(object sender, EventArgs e)
        {
            var prompt = new PromptEntityOptions("\n选择要绑定的对象: ");
            var result = _doc.Editor.GetEntity(prompt);

            if (result.Status != PromptStatus.OK) return;

            TreeNode selectedNode = _treeView.SelectedNode;
            if (selectedNode == null)
            {
                _doc.Editor.WriteMessage("\n请先选中节点");
                return;
            }

            if (selectedNode.Tag is Building building)
            {
                ObjectBinder.BindObjectToNode(building, result.ObjectId, _doc);
            }
            else if (selectedNode.Tag is Floor floor)
            {
                ObjectBinder.BindObjectToNode(floor, result.ObjectId, _doc);
            }
            else if (selectedNode.Tag is Room room)
            {
                ObjectBinder.BindObjectToNode(room, result.ObjectId, _doc);
            }
        }

        public void Close()
        {
            // 注销事件
            Application.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentToBeDestroyed -= DocumentManager_DocumentToBeDestroyed;

            if (_doc != null)
            {
                _doc.Database.SaveComplete -= Database_SaveComplete;
            }

            _treeView.Nodes.Clear();
            _paletteSet.Visible = false;

            _doc = null;
        }

        #region CAD打开图纸插件自动同步数据


        // 新增：解析图纸数据生成目标格式
        public void ParseAndExportDrawingData()
        {

            if (_doc == null) return;

            // 1. 获取当前图纸绑定的楼层信息
            var props = CacheManager.GetCurrentDrawingProperties(_doc.Window.Text);

            // 2. 获取楼层详情（含floorCode和floorName）
            var floor = CacheManager.GetFloorsByBuilding(props.BuildingExternalCode).FirstOrDefault();
            if (floor == null)
            {
                _doc.Editor.WriteMessage("\n未找到绑定的楼层数据");
                return;
            }

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
                          

                                roomDataList2.Add(roomData);
                            
                            roomDataList.Add(roomData);
                            Room.Add(new Room { FloorCode = floor.Code,  Name = roomData.rmId,Area = roomData.area.ToString(),Code = roomData.rmId ,Coordinates = roomData.coordinate});

                            Debug.WriteLine($"{roomData.rmId}-----{roomData.area}");
                        }

                    }
                }
                tr.Commit();
            }

            //foreach (var item in roomDataList2)
            //{
            //    if (roomDataList.FirstOrDefault(a => a.rmId == item.rmId) != null) { var aa = roomDataList.FirstOrDefault(a => a.rmId == item.rmId); continue; }

            //    if (item.area == 0) { 
            //        item.rmId = ""; 
            //        //roomDataList.Add(item);
            //        Room.Add(new Room {FloorCode =floor.Code,  Name =$"Room_{Count}" ,Area = item.area.ToString(),Code =$"Room_{Count}" ,Coordinates = item.coordinate }); 
            //        Debug.WriteLine($"{item.rmId}-----{item.area}");}
            //    else { item.area = double.Parse(item.strings[1]);
            //        //roomDataList.Add(item); 
            //        Room.Add(new Room { FloorCode = floor.Code,  Name = item.rmId,Area = item.area.ToString(),Code = item.rmId  ,Coordinates = item.coordinate});
            //        Debug.WriteLine($"{item.rmId}-----{item.area}");}
            //}
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
            CacheManager.UpsertRooms(Room);

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
        private RoomData ParseRoomFromPolyline(Polyline polyline, Transaction tr)
        {
            var idArea =GetRoomIdFromAttribute(polyline, tr);
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
                rmId =area ==0?"": (idArea == null ? "":idArea[0]),
                area = area,
                coordinate = coordinate,
                
            };
        }

        /// <summary>
        /// 获取房间Id
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="tr"></param>
        /// <returns></returns>


        // 辅助：将多段线顶点坐标转换为目标字符串
        private string GetPolylineCoordinates(Polyline polyline)
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


        private string[] GetRoomIdFromAttribute(Polyline polyline, Transaction tr)
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
                    if (lines.Length > 3 )
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
        private bool IsPointInPolyline(Point3d point, Polyline polyline)
        {
            bool inside = false;
            int vertexCount = polyline.NumberOfVertices ;

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

        private string[] TryGetRoomInfoFromXData(Entity entity)
        {
            try
            {
                // 定义应用程序名（应与添加XData时使用的名称一致）
                string appName = "ROOM_INFO";

                // 尝试获取指定应用程序的XData
                ResultBuffer rb = entity.GetXDataForApplication(appName);

                if (rb == null )
                    return null;

                // 解析XData内容
                // 假设XData格式为: (1001 "ROOM_INFO") (1000 "房间ID") (1040 面积值)
                string roomId = null;
                double? area = null;

                foreach (TypedValue tv in rb)
                {
                    switch (tv.TypeCode)
                    {
                        case 1000: // 字符串数据
                            roomId = tv.Value.ToString();
                            break;
                        case 1040: // 双精度数值
                            area = Convert.ToDouble(tv.Value);
                            break;
                    }
                }

                // 如果成功获取到房间ID和面积，返回结果
                if (!string.IsNullOrEmpty(roomId) && area.HasValue)
                {
                    return new string[] { roomId, area.Value.ToString() };
                }

                return null;
            }
            catch (System.Exception ex)
            {
                // 记录错误但不中断程序
                Debug.WriteLine($"获取XData失败: {ex.Message}");
                return null;
            }
        }
        #endregion
    }
}
