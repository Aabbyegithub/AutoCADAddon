using AutoCADAddon.Common;
using AutoCADAddon.Controls;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AutoCADAddon.Model.FloorBuildingDataModel;

namespace AutoCADAddon
{
    public partial class RoomEditForm : Form
    {
        public Room EditedRoom { get; private set; }
        private readonly int _floorId;
        private readonly Polyline _Polyline;
        private readonly Transaction _tr;
        private readonly Building _Building;
        private readonly Floor _Floor;
        private  List<ItemData> _RoomStanardList;
        private  List<ItemData> _RoomCategory;
        private  List<ItemData> _RoomType;
        private  List<ItemData> _DivisionCode;
        private  List<ItemData> _DepartmentCode;
        private  string _SelectRoomType;
        private  string _SelectDepartmentCode;
        public  RoomEditForm(Polyline polyline, Transaction tr, Building building,Floor floor)
        {
            _Polyline = polyline;
            _tr = tr;
            _Building = building;
            _Floor = floor;
            InitializeComponent();
            Prorate.SelectedIndex = 0;
            Area.Text = polyline.Area.ToString();
            Length.Text = polyline.Length.ToString();
            LoeadingRoomData();
        }

        private async  void LoeadingRoomData()
        {
            var RoomCode = "";
            var RoomPro =PolylineCommon.GetRoomIdFromAttribute(_Polyline,_tr);
            await GetRoomDownLoadList();
            if (RoomPro.Length>0)
            {
                RoomCode = RoomPro[0];
            }
            var RoomData = CacheManager.GetRoomsByRoomCode(RoomCode).FirstOrDefault();
            if (RoomData!=null)
            {
                var Buildingitems = new List<ItemData>();
                Buildingitems.Add(new ItemData
                {
                    DisplayText = RoomData.BuildingName,
                    Tag = _Building,
                    Columns = new string[] { RoomData.BuildingExternalCode, RoomData.BuildingName },
                });
                BuildingCode.SetItems(Buildingitems, new[] { "Building Code", "Building Name" });
                BuildingCode.SetSelectedItem(_Building.Name);
                var Flooritems = new List<ItemData>();
                Flooritems.Add(new ItemData
                {
                    DisplayText = RoomData.FloorName,
                    Columns = new string[] { RoomData.BuildingExternalCode, RoomData.FloorCode, RoomData.FloorName },
                    Tag = _Floor
                });
                FloorCode.SetItems(Flooritems, new[] { "Building Code", "Floor Code", "Floor Name" });
                FloorCode.SetSelectedItem(_Floor.Name);
                var Roomitems = new List<ItemData>();
                Roomitems.Add(new ItemData
                {
                    DisplayText = RoomData.Name,
                    Columns = new string[] { RoomData.Code, RoomData.Name },
                    Tag = new JObject { ["code"] = RoomData.Code, ["name"] = RoomData.Name }
                });
                RoomCodeBox.SetItems(Roomitems, new[] { "Room Code", "Room Name" });
                RoomCodeBox.SetSelectedItem(RoomCode);

                //RoomStanard.SetItems(_RoomStanardList, new[] { "Code", "Description" });
                RoomStanard.SetSelectedItem(RoomData.RoomStanardCode);
                //DivisionCode.SetItems(_DivisionCode, new[] { "Code", "Nmae" });
                DivisionCode.SetSelectedItem(RoomData.divisionCode);
                //_SelectDepartmentCode = RoomData.divisionCode;
                DepartmentCode.SetSelectedItem(RoomData.DepartmentCode);
                //RoomCategory.SetItems(_RoomCategory, new[] { "Code", "Description" });
                RoomCategory.SetSelectedItem(RoomData.Category);
                //_SelectRoomType = RoomData.Category;
                RoomType.SetSelectedItem(RoomData.Type);


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
                    Tag = new JObject { ["code"] = RoomCode, ["name"] = RoomCode}
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
            var RoomStanardList =await DataSyncService.GetRoomStanardAsync();
            if ((RoomStanardList as string)!=null && (RoomStanardList as string).Contains("NG"))
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
            RoomStanard.SetItems(items, new[] { "Code", "Description" });

            var DivisionCodeList =await DataSyncService.GetDivisionCodeAsync();
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
            DivisionCode.SetItems(DivisionCodeitems, new[] { "Code", "Nmae" });



            var RoomCategoryList =await DataSyncService.GetRoomCategoryAsync();
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
            //if(string.IsNullOrWhiteSpace(RoomStanard.SelectedText))
            //{
            //    MessageCommon.Waring("请选择RoomStanard");
            //    return;
            //}
            //if (string.IsNullOrWhiteSpace(RoomCategory.SelectedText))
            //{
            //    MessageCommon.Waring("请选择RoomCategory");
            //    return;
            //}
            //if (string.IsNullOrWhiteSpace(RoomType.SelectedText))
            //{
            //    MessageCommon.Waring("请选择RoomType");
            //    return;
            //}
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
                Coordinates = PolylineCommon.GetPolylineCoordinates(_Polyline),
                Extensions = EditedRoom?.Extensions ?? new Dictionary<string, ExtensionField>()
            });
            CacheManager.UpsertRooms(Room);
            await DataSyncService.SyncRoomdataAsync(new
            {
                 building = _Building.Code,
                 floor = _Floor.Code,
                 roomCode = RoomCodeBox.SelectedText,
                 roomCategory = RoomCategory.SelectedText,
                 roomArea = Area.Text,
                 perimeter = Length.Text,
                 seniorManagement = DivisionCode.SelectedText,
                 facutly = RoomType.SelectedText,
                 department = DepartmentCode.SelectedText
            });
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
                var res =await DataSyncService.GetRoomTypeAsync(tag["code"].ToString());
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
                        Columns = new string[] {item.category_code, item.type_code, item.description },
                        Tag = item
                    });
                }
                _RoomType = items;
                RoomType.SetItems(items, new[]  { "Category Code","Type Code", "Description" });
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
                DepartmentCode.SetItems(items, new[] { "Division Code", "Code", "Name" });
            }
        }
    }
}
