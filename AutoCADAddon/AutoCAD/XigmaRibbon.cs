using AutoCADAddon.Common;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Windows;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static AutoCADAddon.Model.FloorBuildingDataModel;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADAddon.AutoCAD
{
    public class XigmaRibbon
    {
        private static bool isLoggedIn = false;
        private static bool IsPanelAdded { get; set; } = false;
        private static RibbonButton signInOutButton;
        private static List<RibbonButton> _pluginButtons = new List<RibbonButton>();
        private static DrawingPanel _BuildingPanel = null;
        private static DrawingPropertiesForm _propertiesForm;
        private static RoomEditForm _EditData;
        private static PublishDrawing _PublishDrawing;
        private static Reconcile _Reconcile;


        public static void AddXigmaRibbon()
        {
            DataSyncService.TokenExpired += OnTokenExpired;
            RibbonControl ribbon = ComponentManager.Ribbon;
            if (ribbon == null) return;

            if (ribbon.Tabs.Any(t => t.Title == "Xigma")) return;

            // 创建标签页
            RibbonTab xigmaTab = new RibbonTab
            {
                Title = "Xigma",
                Id = "Xigma_Tab"
            };
            ribbon.Tabs.Add(xigmaTab);

            // 创建面板
            RibbonPanelSource panelSource1 = new RibbonPanelSource { Title = "Archibus" };
            RibbonPanelSource panelSource2 = new RibbonPanelSource { Title = "Drawing" };
            RibbonPanelSource panelSource3 = new RibbonPanelSource { Title = "Asset" };
            RibbonPanelSource panelSource4 = new RibbonPanelSource { Title = "Synchronization" };
            RibbonPanel panel = new RibbonPanel { Source = panelSource1 };
            RibbonPanel pane2 = new RibbonPanel { Source = panelSource2 };
            RibbonPanel pane3 = new RibbonPanel { Source = panelSource3 };
            RibbonPanel pane4 = new RibbonPanel { Source = panelSource4 };
            xigmaTab.Panels.Add(panel);
            xigmaTab.Panels.Add(pane2);
            xigmaTab.Panels.Add(pane3);
            xigmaTab.Panels.Add(pane4);

            // ==== 第1行按钮 ====
            RibbonRowPanel row1 = new RibbonRowPanel();
            signInOutButton = CreateButton(isLoggedIn ? "Sign Out" : "Sign In", "Xigma_SignInOut", "点击登录或登出", "点击进行登录连接服务器", "", "登陆.png", LoginAction);
            row1.Items.Add(signInOutButton);
            row1.Items.Add(CreateButton("Explorer", "Xigma_Explorer", "图纸管理", "主要进行图纸和楼层的绑定", "", "Explorer.png", ExplorerAction));
            panelSource1.Items.Add(row1);

            RibbonRowPanel row2 = new RibbonRowPanel();
            row2.Items.Add(CreateButton("Properties", "Xigma_Properties", "显示属性", "显示属性", "", "设置工具添加.png", PropertiesAction));
            panelSource2.Items.Add(row2);

            RibbonRowPanel row3 = new RibbonRowPanel();
            row3.Items.Add(CreateButton("Edit Data", "Xigma_EditData", "绑定数据", "主要进行多边形、房间号的绑定，房间信息的编辑", "", "编辑数据.png", EditDataAction));
            row3.Items.Add(CreateButton("Catalog", "Xigma_Catalog", "加入目录", "加入目录", "", "锁.png", CatalogAction));
            row3.Items.Add(CreateButton("Uncatalog", "Xigma_Uncatalog", "移除目录", "移除目录", "", "开锁.png", UncatalogAction));
            panelSource3.Items.Add(row3);

            RibbonRowPanel row4 = new RibbonRowPanel();
            row4.Items.Add(CreateButton("Publish", "Xigma_Publish", "发布", "将图纸数据发布到服务器", "", "发布.png", PublishAction));
            row4.Items.Add(CreateButton("Reconcile", "Xigma_Reconcile", "对比合并", "对比合并", "", "Reconcile.png", ReconcileAction));
            panelSource4.Items.Add(row4);
        }


        private static RibbonButton CreateButton(string text, string id, string tooltip, string tooltipContent, string commandName, string iconFileName, EventHandler clickHandler)
        {
            var button = new RibbonButton
            {
                Text = text,
                Id = id,
                ShowText = true,
                //ToolTip = tooltip,
                Orientation = System.Windows.Controls.Orientation.Vertical,
                // LargeImage = LoadImage(iconFileName)
                Image = LoadImage(iconFileName)
            };
            // 构建 RibbonToolTip
            var tip = new RibbonToolTip
            {
                Title = tooltip,                          // 主标题
                Content = tooltipContent,                      // 描述文本
                Command = commandName,                         // 命令名（显示加粗）
                ExpandedContent = "按 F1 键获得更多帮助",         // 底部帮助信息
                IsHelpEnabled = true
            };

            button.ToolTip = tip;
            button.CommandHandler = new RelayCommandHandler(clickHandler);
            if (text!= "Sign In" && text != "Sign Out")
            {
                button.IsEnabled = false;
                _pluginButtons.Add(button); // 添加到插件按钮列表
            }
            return button;
        }

        private static System.Windows.Media.ImageSource LoadImage(string imageName)
        {
            string resourceName = $"AutoCADAddon.Resources.{imageName}";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null) return null;

            var img = new System.Windows.Media.Imaging.BitmapImage();
            img.BeginInit();
            img.StreamSource = stream;
            img.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            img.DecodePixelWidth = 16;  // 限制宽度
            img.DecodePixelHeight = 16; // 限制高度
            img.EndInit();
            img.Freeze();
            return img;
        }
        /// <summary>
        /// 点击登录插件
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private static async void LoginAction(object s, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    //显示登录窗口和面板
                    var loginForm = new LoginForm();
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        isLoggedIn = !isLoggedIn;
                        signInOutButton.Text = "Sign Out";
                        OnLoginSuccess();
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\n已登录");
                        Document _doc = Application.DocumentManager.MdiActiveDocument;

                        await anewSave();
                        Opdc(_doc);
                    }
                }
                else
                {
                    //退出登录，清空登录信息
                    isLoggedIn = !isLoggedIn;
                    signInOutButton.Text = "Sign In";
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\n已登出");
                }
            }
            catch (Exception)
            {

                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\n登录过程出错");
            }



        }
        public async static void Opdc(Document _doc)
        {
            var Drawing = await DataSyncService.SyncDrawingServiceAsync(_doc.Window.Text);
            if (Drawing is string error)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\n获取图纸失败: {error}");
                return;
            }

            if (Drawing.total == 0)
            {
                var props = CacheManager.GetCurrentDrawingProperties(_doc.Window.Text);
                if (props == null || string.IsNullOrEmpty(props.BuildingExternalCode) || string.IsNullOrEmpty(props.FloorCode))
                {
                    PropertiesAction(null, null);
                    return;
                }
            }
            else
            {
                try
                {
                    string buildcode = Drawing.list[0].building;
                    string floorcode = Drawing.list[0].floor;
                    var BuildingName = ""; var floorname = "";
                    var build = await DataSyncService.SyncBuildingAsync();
                    var floor = await DataSyncService.SyncFloorAsync(buildcode);
                    foreach (var item in build.list)
                    {
                        if (item.building_code == buildcode)
                        {
                            BuildingName = item.building_name;
                        }
                    }
                    foreach (var item in floor.list)
                    {
                        if (item.floor_code == floorcode)
                        {
                            floorname = item.floor_name;
                        }
                    }
                    CacheManager.SetCurrentDrawingProperties(new Blueprint
                    {
                        SerId = Drawing.list[0].id,
                        BuildingExternalCode = buildcode,
                        BuildingName = BuildingName,
                        FloorCode = floorcode,
                        FloorName = floorname,
                        Name = Drawing.list[0].name,
                        Version = "",
                        UnitType = Drawing.list[0].unitType,
                        Unit = Drawing.list[0].units,
                        status = Drawing.list[0].status,
                    });
                    var RoomData = await DataSyncService.SyncRoomServicedataAsync(buildcode, floorcode);
                    var Room = new List<Room>();
                    foreach (var item in RoomData.list)
                    {
                        Room.Add(new Room
                        {
                            SerId = item.id,
                            Code = item.room_code,
                            Name = item.room_code,
                            BuildingExternalCode = buildcode,
                            BuildingName = BuildingName,
                            FloorCode = floorcode,
                            FloorName = floorname,
                            Area = item.room_area,
                            Length = item.perimeter,
                            Category = item.room_category,
                            Type = item.room_type,
                            divisionCode = item.senior_management,
                            DepartmentCode = item.department,
                            RoomStanardCode = item.standard,
                            Prorate = item.prorate,
                            //Coordinates = PolylineCommon.GetPolylineCoordinates(_Polyline),
                            //Extensions = EditedRoom?.Extensions ?? new Dictionary<string, ExtensionField>()
                        });
                    }

                    CacheManager.UpsertRooms(Room);
                }
                catch (Exception)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\n同步图纸数据失败");
                }

            }
        }

        /// <summary>
        /// 断连重试
        /// </summary>
        private async static Task anewSave()
        {
            try
            {
                var Drawing = CacheManager.GetCurrentDrawingIsSaveProperties();
                foreach (var item in Drawing)
                {
                    var res = await DataSyncService.SyncDrawingAsync(new
                    {
                        id = item.SerId,
                        name = item.Name,
                        title = item.Name,
                        building = item.BuildingExternalCode,
                        floor = item.FloorCode,
                        unitType = item.UnitType,
                        units = item.Unit
                    });
                    await Task.Delay(100);
                    var DrawingId = await DataSyncService.SyncDrawingServiceAsync(item.Name);

                    item.SerId = DrawingId.list[0].id;
                     CacheManager.SetCurrentDrawingProperties(item);
                    
                }
                var Room = CacheManager.GetRoomsByRoomIsSaveCode();
                foreach (var item in Room)
                {
                    var res = await DataSyncService.SyncRoomdataAsync(new
                    {
                        id = item.SerId,
                        building = item.BuildingExternalCode,
                        floor = item.FloorCode,
                        roomCode = item.Code,
                        standard = item.RoomStanardCode,
                        seniorManagement = item.divisionCode,
                        department = item.DepartmentCode,
                        roomCategory = item.Category,
                        roomType = item.Type,
                        prorate = item.Prorate,
                        roomArea = item.Area,
                        perimeter = item.Length,
                    });

                    await Task.Delay(100);
                    var room = await DataSyncService.SyncRoomServicedataAsync( item.BuildingExternalCode, item.FloorCode);
                    item.SerId = room.list[0].id;

                }
                CacheManager.UpsertRooms(Room);
            }
            catch (Exception)
            {

            }

        }
        /// <summary>
        /// 点击展示左侧面板
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private static void ExplorerAction(object s, EventArgs e)
        {
            if (!isLoggedIn)
            {
                Application.ShowAlertDialog("请先登录系统！");
                return;
            }
            if (IsPanelAdded && _BuildingPanel != null)
            {
                IsPanelAdded = false;
                //去掉面板
                _BuildingPanel.Close();
                return;
            }
            _BuildingPanel = new DrawingPanel();
            IsPanelAdded = true;
        }

        /// <summary>
        /// 打开属性窗口
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private static void PropertiesAction(object s, EventArgs e)
        {
            if (!isLoggedIn)
            {
                Application.ShowAlertDialog("请先登录系统！");
                return;
            }

            if (_propertiesForm != null && !_propertiesForm.IsDisposed)
            {
                _propertiesForm.Activate();
                return;
            }
            Document _doc = Application.DocumentManager.MdiActiveDocument;
            _propertiesForm = new DrawingPropertiesForm(_doc,"OLD");
            _propertiesForm.FormClosed += (sender, args) => { _propertiesForm = null; };

            _propertiesForm.Show();
        }

        /// <summary>
        /// 点击打开数据绑定
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private static void EditDataAction(object s, EventArgs e)
        {
            if (!isLoggedIn)
            {
                Application.ShowAlertDialog("请先登录系统！");
                return;
            }

            if (_EditData != null && !_EditData.IsDisposed)
            {
                _EditData.Activate();
                return;
            }

            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            var props = CacheManager.GetCurrentDrawingProperties(doc.Window.Text);
            if (props == null || string.IsNullOrEmpty(props.BuildingExternalCode) || string.IsNullOrEmpty(props.FloorCode))
            {
                Application.ShowAlertDialog("请先绑定图纸到楼层！");
                return;
            }

            // 提示用户选择房间元素（多段线或文本）
            PromptEntityOptions peo = new PromptEntityOptions("\n请选择房间多边线或文字: ");
            peo.SetRejectMessage("\n只能选择多段线或文字！");
            peo.AddAllowedClass(typeof(Polyline), exactMatch: false);
            peo.AddAllowedClass(typeof(DBText), exactMatch: false);
            peo.AddAllowedClass(typeof(MText), exactMatch: false);

            PromptEntityResult per = ed.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\n未选择任何实体，操作已取消。");
                return;
            }

            ObjectId entityId = per.ObjectId;

            using (var tr = doc.TransactionManager.StartTransaction())
            {
                var ent = tr.GetObject(entityId, OpenMode.ForRead);
                if (ent is Polyline poly)
                {
                    var mtext = FindMTextInPolyline(poly, tr);
                    // 这里可以传入 area / vertices 等参数
                    _EditData = new RoomEditForm(poly,mtext, tr,
                        new Building { Code = props.BuildingExternalCode, Name = props.BuildingName },
                        new Floor { BuildingCode = props.BuildingExternalCode, Code = props.FloorCode, Name = props.FloorName }
                        ); // 或：new RoomEditForm(polyId, area, vertices);
                }
                //else if (ent is DBText dbText)
                //{
                //    var textValue = dbText.TextString;
                //    var dbTextpoly = FindPolylineOnRMLayerContainingPoint(doc,tr,dbText.Position);
                //    _EditData = new RoomEditForm(dbTextpoly, tr,
                //        new Building { Code = props.BuildingExternalCode, Name = props.BuildingName },
                //        new Floor { BuildingCode = props.BuildingExternalCode, Code = props.FloorCode, Name = props.FloorName }
                //        );
                //}
                else if (ent is MText mText)
                {
                    var textValue = mText.Contents;
                    var mTextpoly = FindPolylineOnRMLayerContainingPoint(doc, tr, mText.Location);
                    _EditData = new RoomEditForm(mTextpoly,mText, tr,
                        new Building { Code = props.BuildingExternalCode, Name = props.BuildingName },
                        new Floor { BuildingCode = props.BuildingExternalCode, Code = props.FloorCode, Name = props.FloorName }
                        );
                }
                else
                {
                    ed.WriteMessage("\n选择的实体类型暂不支持！");
                    return;
                }

                tr.Commit();

                _EditData.FormClosed += (sender, args) => { _EditData = null; };
                _EditData.Show();
            }
        }

        /// <summary>
        /// 查找与多段线位置关联的 MText（示例：简单判断包围盒是否在多段线内）
        /// </summary>
        /// <param name="polyline">目标多段线</param>
        /// <param name="tr">事务</param>
        /// <returns>关联的 MText，无则返回 null</returns>
        private static MText FindMTextInPolyline(Polyline polyline, Transaction tr)
        {
            var db = polyline.Database;
            var mSpace = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);

            foreach (ObjectId id in mSpace)
            {
                var ent = tr.GetObject(id, OpenMode.ForRead) as MText;
                if (ent == null) continue;

                // 获取 MText 包围盒的两个对角点
                var textBbox = ent.GeometricExtents;
                Point3d minPt = textBbox.MinPoint;
                Point3d maxPt = textBbox.MaxPoint;

                // 用射线法判断点是否在多段线内
                bool isMinIn = IsPointInPolyline(polyline, minPt);
                bool isMaxIn = IsPointInPolyline(polyline, maxPt);

                // 若两个点都在多段线内，认为 MText 与多段线关联
                if (isMinIn && isMaxIn)
                {
                    return ent;
                }
            }
            return null;
        }

        /// <summary>
        /// 射线法判断点是否在多段线（多边形）内
        /// </summary>
        /// <param name="polyline">目标多段线</param>
        /// <param name="point">待判断点</param>
        /// <returns>点在多段线内返回 true，否则 false</returns>
        private static bool IsPointInPolyline(Polyline polyline, Point3d point)
        {
            // 转换为 2D 点（忽略 Z 轴，假设在同一平面）
            Point2d pt = new Point2d(point.X, point.Y);
            int intersections = 0;
            int numVertices = polyline.NumberOfVertices;

            for (int i = 0; i < numVertices; i++)
            {
                // 获取当前顶点和下一个顶点（处理闭合）
                Point2d v1 = polyline.GetPoint2dAt(i);
                Point2d v2 = polyline.GetPoint2dAt((i + 1) % numVertices);

                // 判断点是否在边的 Y 范围内
                if ((v1.Y > pt.Y) != (v2.Y > pt.Y))
                {
                    // 计算边与射线（水平向右）的交点 X 坐标
                    double xIntersect = (pt.Y - v1.Y) * (v2.X - v1.X) / (v2.Y - v1.Y) + v1.X;

                    // 若交点 X 大于点的 X，说明射线与边相交
                    if (pt.X < xIntersect)
                    {
                        intersections++;
                    }
                }
            }

            // 交点数为奇数则在内部
            return intersections % 2 == 1;
        }

        /// <summary>
        /// 查找RM$图层上包含指定点的多段线
        /// </summary>
        private static Polyline FindPolylineOnRMLayerContainingPoint(Document doc, Transaction tr, Point3d point)
        {
            // 获取RM$图层ID
            var blockTable = (BlockTable)tr.GetObject(doc.Database.BlockTableId, OpenMode.ForRead);
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
                    if (!polyline.Layer.ToString().Contains("RM$")) continue;

                    // 检查点是否在多段线内部
                    if (IsPointInsidePolyline(polyline, point))
                    {
                        return polyline;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 判断点是否在闭合多段线内部
        /// </summary>
        private static bool IsPointInsidePolyline(Polyline poly, Point3d point)
        {
            // 将3D点转换为2D点（忽略Z坐标）
            Point2d testPoint = new Point2d(point.X, point.Y);
            int vertexCount = poly.NumberOfVertices;
            bool inside = false;

            for (int i = 0; i < vertexCount; i++)
            {
                Point2d p1 = poly.GetPoint2dAt(i);
                Point2d p2 = poly.GetPoint2dAt((i + 1) % vertexCount);

                // 检查点是否在多边形内部的射线法
                if (((p1.Y > testPoint.Y) != (p2.Y > testPoint.Y)) &&
                    (testPoint.X < p1.X + (p2.X - p1.X) * (testPoint.Y - p1.Y) / (p2.Y - p1.Y)))
                {
                    inside = !inside;
                }
            }

            return inside;
        }


        private static void CatalogAction(object s, EventArgs e) => ShowMessage("Catalog clicked");
        private static void UncatalogAction(object s, EventArgs e) => ShowMessage("Uncatalog clicked");

        /// <summary>
        /// 发布上传图纸
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private static void PublishAction(object s, EventArgs e)
        {
            if (!isLoggedIn)
            {
                Application.ShowAlertDialog("请先登录系统！");
                return;
            }

            if (_PublishDrawing != null && !_PublishDrawing.IsDisposed)
            {
                _PublishDrawing.Activate();
                return;
            }
            //Document _doc = Application.DocumentManager.MdiActiveDocument;
            _PublishDrawing = new PublishDrawing();
            _PublishDrawing.FormClosed += (sender, args) => { _PublishDrawing = null; };

            _PublishDrawing.Show();
        }

        /// <summary>
        /// 对比图纸还有那些没有绑定的房间
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private static void ReconcileAction(object s, EventArgs e){
            if (!isLoggedIn)
            {
                Application.ShowAlertDialog("请先登录系统！");
                return;
            }

            if (_Reconcile != null && !_Reconcile.IsDisposed)
            {
                _Reconcile.Activate();
                return;
            }
             Document _doc = Application.DocumentManager.MdiActiveDocument;
            _Reconcile = new Reconcile(_doc);
            _Reconcile.FormClosed += (sender, args) => { _Reconcile = null; };

            _Reconcile.Show();
        }

        private static void ShowMessage(string msg)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\n{msg}");
        }

        private static void OnTokenExpired()
        {
            // 更新登录按钮文字
            if (signInOutButton != null)
                signInOutButton.Text = "Sign In";

            // 禁用插件功能按钮
            foreach (var btn in _pluginButtons)
            {
                btn.IsEnabled = false;
            }

            // 这里可以弹个提示
           Application.ShowAlertDialog("登录已过期，请重新登录！");
        }

        private static void OnLoginSuccess()
        {
            if (signInOutButton != null)
                signInOutButton.Text = "Sign Out";

            foreach (var btn in _pluginButtons)
            {
                btn.IsEnabled = true;
            }
        }


        // 用于处理 RibbonButton 的点击事件
        private class RelayCommandHandler : System.Windows.Input.ICommand
        {
            private readonly EventHandler _handler;

            public RelayCommandHandler(EventHandler handler) => _handler = handler;
            public bool CanExecute(object parameter) => true;
            public void Execute(object parameter) => _handler?.Invoke(this, EventArgs.Empty);
            public event EventHandler CanExecuteChanged { add { } remove { } }
        }
    }

}
