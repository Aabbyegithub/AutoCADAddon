using AutoCADAddon.Common;
using AutoCADAddon.Controls;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static AutoCADAddon.Model.ClassModel;
using static AutoCADAddon.Model.FloorBuildingDataModel;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADAddon
{
    public partial class RoomEditForm : Form
    {
        public Room EditedRoom { get; private set; }
        private readonly int _floorId;
        private readonly Polyline _Polyline;
        private readonly MText _mText;
        private readonly Transaction _tr;
        private readonly Building _Building;
        private readonly Floor _Floor;
        private List<ItemData> _RoomStanardList;
        private List<ItemData> _RoomCategory;
        private List<ItemData> _RoomType;
        private List<ItemData> _DivisionCode;
        private List<ItemData> _DepartmentCode;
        private string _SelectRoomType;
        private string _SelectDepartmentCode;
        private string _SerId;
        private const string XDATA_APP_ID = "ROOMDATA";
        public RoomEditForm(Polyline polyline, MText mText, Transaction tr, Building building, Floor floor)
        {
            _Polyline = polyline;
            _mText = mText;
            _tr = tr;
            _Building = building;
            _Floor = floor;
            InitializeComponent();
            Prorate.SelectedIndex = 0;
            Area.Text = Math.Round(polyline.Area / 1000000, 2).ToString();
            Length.Text = Math.Round(polyline.Length / 1000, 2).ToString();
            LoeadingRoomData();
        }

        private async void LoeadingRoomData()
        {
            var RoomCode = "";
            var RoomPro = PolylineCommon.ParseRoomFromPolyline(_Polyline, _tr);

            await GetRoomDownLoadList();
            RoomCode = RoomPro.rmId;
            var RoomData = CacheManager.GetRoomsByRoomCode(RoomCode).FirstOrDefault();
            if (RoomData != null)
            {
                var Buildingitems = new List<ItemData>();
                Buildingitems.Add(new ItemData
                {
                    DisplayText = RoomData.BuildingName,
                    Tag = _Building,
                    Columns = new string[] { RoomData.BuildingExternalCode, RoomData.BuildingName },
                });
                BuildingCode.SetItems(Buildingitems, new[] { "Building Code", "Building Name" });
                if (!string.IsNullOrEmpty(_Building.Name))
                    BuildingCode.SetSelectedItem(_Building.Name);
                var Flooritems = new List<ItemData>();
                Flooritems.Add(new ItemData
                {
                    DisplayText = RoomData.FloorName,
                    Columns = new string[] { RoomData.BuildingExternalCode, RoomData.FloorCode, RoomData.FloorName },
                    Tag = _Floor
                });
                FloorCode.SetItems(Flooritems, new[] { "Building Code", "Floor Code", "Floor Name" });
                if (!string.IsNullOrEmpty(_Floor.Name))
                    FloorCode.SetSelectedItem(_Floor.Name);
                var Roomitems = new List<ItemData>();
                Roomitems.Add(new ItemData
                {
                    DisplayText = RoomData.Name,
                    Columns = new string[] { RoomData.Code, RoomData.Name },
                    Tag = new JObject { ["code"] = RoomData.Code, ["name"] = RoomData.Name }
                });
                RoomCodeBox.SetItems(Roomitems, new[] { "Room Code", "Room Name" });
                if(!string.IsNullOrEmpty(RoomCode))
                   RoomCodeBox.SetSelectedItem(RoomCode);

                //RoomStanard.SetItems(_RoomStanardList, new[] { "Code", "Description" });
                if (!string.IsNullOrEmpty(RoomData.RoomStanardCode))
                    RoomStanard.SetSelectedItem(RoomData.RoomStanardCode);
                //DivisionCode.SetItems(_DivisionCode, new[] { "Code", "Nmae" });
                if (!string.IsNullOrEmpty(RoomData.divisionCode))
                    DivisionCode.SetSelectedItem(RoomData.divisionCode);
                //_SelectDepartmentCode = RoomData.divisionCode;
                if (!string.IsNullOrEmpty(RoomData.DepartmentCode))
                    DepartmentCode.SetSelectedItem(RoomData.DepartmentCode);
                //RoomCategory.SetItems(_RoomCategory, new[] { "Code", "Description" });
                if (!string.IsNullOrEmpty(RoomData.Category))
                    RoomCategory.SetSelectedItem(RoomData.Category);
                //_SelectRoomType = RoomData.Category;
                if (!string.IsNullOrEmpty(RoomData.Type))
                    RoomType.SetSelectedItem(RoomData.Type);
                Prorate.Text = RoomData.Prorate;
                _SerId = RoomData.SerId;


            }
            else
            {
                var Buildingitems = new List<ItemData>();
                Buildingitems.Add(new ItemData
                {
                    DisplayText = _Building.Name,
                    Tag = _Building,
                    Columns = new string[] { _Building.Code, _Building.Name },
                });
                BuildingCode.SetItems(Buildingitems, new[] { "Building Code", "Building Name" });
                BuildingCode.SetSelectedItem(_Building.Name);
                var Flooritems = new List<ItemData>();
                Flooritems.Add(new ItemData
                {
                    DisplayText = _Floor.Name,
                    Columns = new string[] { _Building.Code, _Floor.Code, _Floor.Name },
                    Tag = _Floor
                });
                FloorCode.SetItems(Flooritems, new[] { "Building Code", "Floor Code", "Floor Name" });
                FloorCode.SetSelectedItem(_Floor.Name);
                var Roomitems = new List<ItemData>();
                Roomitems.Add(new ItemData
                {
                    DisplayText = RoomCode,
                    Columns = new string[] { RoomCode, RoomCode },
                    Tag = new JObject { ["code"] = RoomCode, ["name"] = RoomCode }
                });
                RoomCodeBox.SetItems(Roomitems, new[] { "Room Code", "Room Name" });
                RoomCodeBox.SetSelectedItem(RoomCode);
            }
        }


        /// <summary>
        /// 获取房间下拉数据
        /// </summary>
        /// <returns></returns>
        private async Task GetRoomDownLoadList()
        {
            var RoomStanardList = await DataSyncService.GetRoomStanardAsync();
            if ((RoomStanardList as string) != null && (RoomStanardList as string).Contains("NG"))
            {
                MessageCommon.Error($"获取RoomStanard数据失败：{RoomStanardList}");
            }
            var items = new List<ItemData>();
            foreach (var item in RoomStanardList.list)
            {
                items.Add(new ItemData
                {
                    DisplayText = item.code,
                    Columns = new string[] { item.code, item.description },
                    Tag = item
                });
            }
            _RoomStanardList = items;
            if (items.Count > 0)
                RoomStanard.SetItems(items, new[] { "Code", "Description" });

            var DivisionCodeList = await DataSyncService.GetDivisionCodeAsync();
            if ((DivisionCodeList as string) != null && (DivisionCodeList as string).Contains("NG"))
            {
                MessageCommon.Error($"获取DivisionCode数据失败：{DivisionCodeList}");
            }
            var DivisionCodeitems = new List<ItemData>();
            foreach (var item in DivisionCodeList.list)
            {
                DivisionCodeitems.Add(new ItemData
                {
                    DisplayText = item.code,
                    Columns = new string[] { item.code, item.name },
                    Tag = item
                });
            }
            _DivisionCode = DivisionCodeitems;
            if (DivisionCodeitems.Count > 0)
                DivisionCode.SetItems(DivisionCodeitems, new[] { "Code", "Nmae" });



            var RoomCategoryList = await DataSyncService.GetRoomCategoryAsync();
            if ((RoomCategoryList as string) != null && (RoomCategoryList as string).Contains("NG"))
            {
                MessageCommon.Error($"获取RoomCategory数据失败：{RoomCategoryList}");
            }
            var RoomCategoryListitems = new List<ItemData>();
            foreach (var item in RoomCategoryList.list)
            {
                RoomCategoryListitems.Add(new ItemData
                {
                    DisplayText = item.code,
                    Columns = new string[] { item.code, item.description },
                    Tag = item
                });
            }
            _RoomCategory = RoomCategoryListitems;
            if (RoomCategoryListitems.Count > 0)
                RoomCategory.SetItems(RoomCategoryListitems, new[] { "Code", "Description" });
        }


        private void BtnExtensions_Click(object sender, EventArgs e)
        {
            var form = new ExtensionFieldsForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                EditedRoom.Extensions = form.UpdatedExtensions;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BuildingCode.SelectedText)||string.IsNullOrWhiteSpace(FloorCode.SelectedText)||string.IsNullOrWhiteSpace(RoomCodeBox.SelectedText))
            {
                MessageCommon.Waring("请选择楼栋，楼层或房间属性");
                return;
            }
            var Room = new List<Room>();
            Room.Add(new Room
            {
                Code = RoomCodeBox.SelectedText,
                Name = RoomCodeBox.SelectedText,
                BuildingExternalCode = _Building.Code,
                BuildingName = _Building.Name,
                FloorCode = _Floor.Code,
                FloorName = _Floor.Name,
                Area = Area.Text,
                Length = Length.Text,
                Category = RoomCategory.SelectedText,
                Type = RoomType.SelectedText,
                divisionCode = DivisionCode.SelectedText,
                DepartmentCode = DepartmentCode.SelectedText,
                RoomStanardCode = RoomStanard.SelectedText,
                Prorate = Prorate.Text,
                Coordinates = PolylineCommon.GetPolylineCoordinates(_Polyline),
                Extensions = EditedRoom?.Extensions ?? new Dictionary<string, ExtensionField>()
            });
            var res = "";
            try
            {
                res = await DataSyncService.SyncRoomdataAsync(new
                {
                    id = _SerId,
                    building = _Building.Code,
                    floor = _Floor.Code,
                    roomCode = RoomCodeBox.SelectedText,
                    standard = RoomStanard.SelectedText,
                    seniorManagement = DivisionCode.SelectedText,
                    department = DepartmentCode.SelectedText,
                    roomCategory = RoomCategory.SelectedText,
                    roomType = RoomType.SelectedText,
                    prorate = Prorate.Text,
                    roomArea = Area.Text,
                    perimeter = Length.Text,


                });
                await Task.Delay(100);
                var room = await DataSyncService.SyncRoomServicedataAsync(_Building.Code, _Floor.Code);
                Room.First().SerId = room.list[0].id;
            }
            catch (Exception)
            {

            }
            if (res == "NG")
            {
                Room.First().IsSave = "0";
            }

            CacheManager.UpsertRooms(Room);
            //WriteRoomDataToXdata(Room.FirstOrDefault());
            SetMtext(Room.FirstOrDefault());
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void RoomCategory_SelectedItemChanged(object sender, ItemData e)
        {
            RoomType.ClearSelection();
            var tag = null as JObject;
            //if (e != null)
            //{
            tag = e.Tag as JObject;
            //}
            //else
            //{
            //    tag.Add("code", _SelectRoomType);
            //}
            tag = e.Tag as JObject;
            if (tag != null && tag["code"] != null)
            {
                var res = await DataSyncService.GetRoomTypeAsync(tag["code"].ToString());
                if ((res as string) != null && (res as string).Contains("NG"))
                {
                    MessageCommon.Error($"获取RoomType数据失败{res}");
                    return;
                }

                var items = new List<ItemData>();
                foreach (var item in res.list)
                {
                    items.Add(new ItemData
                    {
                        DisplayText = item.type_code,
                        Columns = new string[] { item.category_code, item.type_code, item.description },
                        Tag = item
                    });
                }
                _RoomType = items;
                   if(items.Count > 0)
                RoomType.SetItems(items, new[] { "Category Code", "Type Code", "Description" });
            }
        }

        private async void DivisionCode_SelectedItemChanged(object sender, ItemData e)
        {
            DepartmentCode.ClearSelection();
            var tag = null as JObject;
            //if (e != null)
            //{
            tag = e.Tag as JObject;
            //}
            //else
            //{
            //    tag.Add("code", _SelectDepartmentCode);
            //}
            if (tag != null && tag["code"] != null)
            {
                var res = await DataSyncService.GetDepartmentCodeAsync(tag["code"].ToString());
                if ((res as string) != null && (res as string).Contains("NG"))
                {
                    MessageCommon.Error($"获取DepartmentCode数据失败{res}");
                    return;
                }

                var items = new List<ItemData>();
                foreach (var item in res.list)
                {
                    items.Add(new ItemData
                    {
                        DisplayText = item.code,
                        Columns = new string[] { item.division_code, item.code, item.name },
                        Tag = item
                    });
                }
                _DepartmentCode = items;
                if(items.Count > 0)
                DepartmentCode.SetItems(items, new[] { "Division Code", "Code", "Name" });
            }
        }

        private void SetMtext(Room roomData)
        {
            if (roomData == null) return;
            if (_mText != null)
            {
                var db = _Polyline.Database;
                var mtextId = _mText.ObjectId;
                try
                {
                    // 重新开启事务
                    var doc = Application.DocumentManager.MdiActiveDocument;
                    using (var docLock = doc.LockDocument())
                    using (var tr = db.TransactionManager.StartTransaction())
                    {
                        var mtext = tr.GetObject(mtextId, OpenMode.ForWrite) as MText;

                        if (mtext == null)
                        {
                            //MessageCommon.Error("无法获取 Polyline 对象。");
                            return;
                        }
                        var contents = new List<string>();

                        if (!string.IsNullOrEmpty(roomData.Code))
                            contents.Add(roomData.Code);
                        //if (!string.IsNullOrEmpty(roomData.RoomStanardCode))
                        //    contents.Add($"RoomStanardCode-{roomData.RoomStanardCode}");
                        if (!string.IsNullOrEmpty(roomData.Category))
                            contents.Add(roomData.Category);
                        if (!string.IsNullOrEmpty(roomData.Type))
                            contents.Add($"{roomData.Type}");

                        if (!string.IsNullOrEmpty(roomData.divisionCode))
                            contents.Add($"{roomData.divisionCode}");

                        if (!string.IsNullOrEmpty(roomData.DepartmentCode))
                            contents.Add($"{roomData.DepartmentCode}");
                        //if (roomData.Prorate != null)
                        //    contents.Add($"PRORATE-{roomData.Prorate}");
                        if (roomData.Area != null)
                            contents.Add($"{roomData.Area}");

                        // 最终拼接
                        mtext.Contents = string.Join("\n", contents);
                        //mtext.Contents = $"{roomData.Code}\n" +
                        //    $"AREA:{roomData.Area}\n" +
                        //    $"TYPE:{roomData.Type}\n" +
                        //    $"RoomStanardCode:{roomData.RoomStanardCode}\n" +
                        //    $"DIVISION:{roomData.divisionCode}\n" +
                        //    $"DEPARTMENT:{roomData.DepartmentCode}\n" +
                        //    $"PRORATE:{roomData.Prorate}";
                        tr.Commit(); // 提交事务
                    }
                    doc.Database.SaveAs(doc.Name, true, DwgVersion.Current, doc.Database.SecurityParameters);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        /// <summary>
        /// 根据对象的属性自动拼接非空值，每个属性一行
        /// 支持简单属性或自定义映射
        /// </summary>
        public static string BuildMTextContents(object data, Dictionary<string, string> displayNames = null)
        {
            if (data == null) return string.Empty;

            var lines = new List<string>();
            var type = data.GetType();

            foreach (var prop in type.GetProperties())
            {
                var value = prop.GetValue(data);
                if (value == null) continue;

                string strValue = value.ToString();
                if (string.IsNullOrWhiteSpace(strValue)) continue;

                string displayName = displayNames != null && displayNames.ContainsKey(prop.Name)
                    ? displayNames[prop.Name]
                    : prop.Name;

                // 如果 displayName 和值相同就只显示值，否则显示 "displayName:value"
                if (displayName.Equals(strValue, StringComparison.OrdinalIgnoreCase))
                    lines.Add(strValue);
                else
                    lines.Add($"{displayName}:{strValue}");
            }

            return string.Join("\n", lines);
        }

        /// <summary>
        /// 将房间数据写入 Polyline 的 Xdata
        /// </summary>
        private void WriteRoomDataToXdata(Room roomData)
        {
            if (roomData == null) return;

            var db = _Polyline.Database;
            var plineId = _Polyline.ObjectId; // 保存 ObjectId，避免用已失效的实体对象

            try
            {
                // 重新开启事务
                var doc = Application.DocumentManager.MdiActiveDocument;
                using (var docLock = doc.LockDocument())
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var pline = tr.GetObject(plineId, OpenMode.ForWrite) as Polyline;

                    if (pline == null)
                    {
                        MessageCommon.Error("无法获取 Polyline 对象。");
                        return;
                    }

                    // 注册 App ID
                    RegisterXdataAppId(db);

                    // 构造 XData 数据
                    var xdata = new Dictionary<string, string>
                    {
                        { "RoomCode", roomData.Code },
                        { "RoomName", roomData.Name },
                        { "BuildingCode", roomData.BuildingExternalCode },
                        { "BuildingName", roomData.BuildingName },
                        { "FloorCode", roomData.FloorCode },
                        { "FloorName", roomData.FloorName },
                        { "Area", roomData.Area },
                        { "Length", roomData.Length },
                        { "Category", roomData.Category },
                        { "Type", roomData.Type },
                        { "DivisionCode", roomData.divisionCode },
                        { "DepartmentCode", roomData.DepartmentCode },
                        { "StandardCode", roomData.RoomStanardCode },
                        { "Prorate", roomData.Prorate }
                    };

                    // 构造 ResultBuffer
                    var rb = new ResultBuffer();
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, XDATA_APP_ID));
                    foreach (var kvp in xdata)
                    {
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, kvp.Key));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, kvp.Value));
                    }

                    // 设置 XData
                    pline.XData = rb;

                    tr.Commit(); // 提交事务
                }
            }
            catch (System.Exception ex)
            {
                MessageCommon.Error($"写入 XData 失败：{ex.Message}");
            }
        }


        private void RegisterXdataAppId(Database db)
        {
            // 使用独立的事务来注册应用程序 ID
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // 以读模式打开 RegAppTable
                    RegAppTable regTable = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                    bool needCreate = !regTable.Has(XDATA_APP_ID);

                    if (needCreate)
                    {
                        // 尝试升级为可写模式
                        if (regTable.IsWriteEnabled == false)
                        {
                            regTable.UpgradeOpen();
                        }

                        RegAppTableRecord newRegApp = new RegAppTableRecord
                        {
                            Name = XDATA_APP_ID
                        };

                        regTable.Add(newRegApp);
                        trans.AddNewlyCreatedDBObject(newRegApp, true);
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Abort();
                    MessageCommon.Error($"注册 Xdata 应用程序 ID 失败: {ex.Message}");
                }
            }
        }


    }
}
