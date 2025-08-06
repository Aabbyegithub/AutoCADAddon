using AutoCADAddon.Common;
using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AutoCADAddon.Model.ClassModel;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADAddon
{
    public partial class PublishDrawing : Form
    {
        /// <summary>
        /// 选中的上传图纸选项
        /// </summary>
        private string SelectDrawing = "ThisDrawingOnly";
        private string SelectView = "Extents";
        public PublishDrawing()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 发布上传图纸数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Publish_Click(object sender, EventArgs e)
        {
            try
            {
                PublishingDrawing.Minimum = 0;
                PublishingDrawing.Step = 1;
                PublishingDrawing.Value = 0;
                PublishingRule.Minimum = 0;
                PublishingRule.Step = 1;
                PublishingRule.Value = 0;
                if (SelectView != "Extents")
                {
                    Application.ShowAlertDialog($"暂不支持{SelectView}模式发布");
                    return;
                }
                if (SelectDrawing == "ThisDrawingOnly")
                {
                    var doc = Application.DocumentManager.MdiActiveDocument;
                    var props = CacheManager.GetCurrentDrawingProperties(doc.Window.Text);
                    if (props == null || string.IsNullOrEmpty(props.BuildingExternalCode) || string.IsNullOrEmpty(props.FloorCode))
                    {
                        Application.ShowAlertDialog("请先绑定图纸到楼层！");
                        return;
                    }
                    var room = CacheManager.GetRoomsByRoomCode(props.BuildingExternalCode, props.FloorCode);
                   // PolylineCommon.GetLayerXData();
                    var RoomList = PolylineCommon.ParseAndExportDrawingData(doc, props.FloorCode);
                    PublishingDrawing.Maximum = RoomList.Count + 1;
                    PublishingRule.Maximum = RoomList.Count;
                    if (RoomList.Count ==0 ) {
                        Application.ShowAlertDialog("解析图纸失败");
                        return;
                    }
                    foreach (var item in RoomList)
                    {
                        if (item.Code.Contains("Room_"))
                        {
                            PublishingDrawing.PerformStep();
                            PublishingRule.PerformStep();
                            continue;
                        }

                        if (room.Where(a => a.Code == item.Code).Count() == 0)
                        {
                            Application.ShowAlertDialog($"房间【{item.Code}】未绑定属性！请检查");
                            return;
                        }
                        PublishingDrawing.PerformStep();
                        PublishingRule.PerformStep();
                        await Task.Delay(10);
                    }
                    var roomData = new List<RoomData>();
                    foreach (var item in RoomList)
                    {
                        if (item.Code.Contains("Room_"))
                            item.Code = "";
                        roomData.Add(new RoomData
                        {
                            rmId = item.Code,
                            area = double.Parse(item.Area),
                            coordinate = item.Coordinates
                        });
                    }
                    await DataSyncService.SyncBlueprintAsync(new ResultFloorRoom
                    {
                        floorCode = props.FloorCode,
                        floorName = props.FloorName,
                        drawId = props.SerId,
                        data = roomData
                    });
                    props.status = "Published";
                    CacheManager.SetCurrentDrawingProperties(props);
                    PublishingDrawing.PerformStep();
                }
                else
                {
                    var ResultFloorRoom = await UploadAllDrawingsAsync();
                    if (ResultFloorRoom != null && ResultFloorRoom.Count != 0)
                    {
                        ResultFloorRoom.ForEach(async item =>
                        {
                            await DataSyncService.SyncBlueprintAsync(item);
                            PublishingDrawing.PerformStep();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Application.ShowAlertDialog($"发布失败!{ex.Message}");
                PublishingDrawing.Value = 0;
                PublishingRule.Value = 0;
            }
            finally
            {
                PublishingDrawing.Value = 0;
                PublishingRule.Value = 0;

            }
        }


        private async Task<List<ResultFloorRoom>> UploadAllDrawingsAsync()
        {
            var docs = Application.DocumentManager;
            var res = new List<ResultFloorRoom>();
            PublishingDrawing.Maximum = docs.Count * 2;
            PublishingRule.Minimum = docs.Count;
            foreach (Document doc in docs)
            {
                Application.DocumentManager.MdiActiveDocument = doc;
                await Task.Delay(100);

                var props = CacheManager.GetCurrentDrawingProperties(doc.Window.Text);
                if (props == null || string.IsNullOrEmpty(props.BuildingExternalCode) || string.IsNullOrEmpty(props.FloorCode))
                {
                    Application.ShowAlertDialog($"图纸 {doc.Name} 未绑定楼层，跳过。");
                    continue;
                }

                var room = CacheManager.GetRoomsByRoomCode(props.BuildingExternalCode, props.FloorCode);
                var roomList = PolylineCommon.ParseAndExportDrawingData(doc, props.FloorCode);

                foreach (var item in roomList)
                {
                    if (item.Code.Contains("Room_"))
                    {
                        continue;
                    }

                    if (room.Where(a => a.Code == item.Code).Count() == 0)
                    {
                        Application.ShowAlertDialog($"图纸 {doc.Name} 中有房间未绑定属性，发布失败");
                        res.Clear();
                        return res;
                    }
                }

                var roomData = roomList.Select(item => new RoomData
                {
                    rmId = item.Code.StartsWith("Room_") ? "" : item.Code,
                    area = double.Parse(item.Area),
                    coordinate = item.Coordinates
                }).ToList();

                res.Add(new ResultFloorRoom
                {
                    floorCode = props.FloorCode,
                    floorName = props.FloorName,
                    data = roomData
                });
                PublishingDrawing.PerformStep();
                PublishingRule.PerformStep();
                await Task.Delay(10);
            }
            return res;
        }


        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Options_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 选中仅当前图纸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisDrawingOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (ThisDrawingOnly.Checked)
            {
                AllDrawing.Checked = false;
                SelectDrawing = "ThisDrawingOnly";
            }
        }

        private void AllDrawing_CheckedChanged(object sender, EventArgs e)
        {
            if (AllDrawing.Checked)
            {
                ThisDrawingOnly.Checked = false;
                SelectDrawing = "AllDrawing";
            }
        }

        private void Extents_CheckedChanged(object sender, EventArgs e)
        {
            if (Extents.Checked)
            {
                FloorBoundary.Checked = false;
                CurrentView.Checked = false;
                NamedView.Checked = false;
                SelectView = "Extents";
            }
        }

        private void FloorBoundary_CheckedChanged(object sender, EventArgs e)
        {
            if (FloorBoundary.Checked)
            {
                Extents.Checked = false;
                CurrentView.Checked = false;
                NamedView.Checked = false;
                SelectView = "FloorBoundary";
            }
        }

        private void CurrentView_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentView.Checked)
            {
                FloorBoundary.Checked = false;
                Extents.Checked = false;
                NamedView.Checked = false;
                SelectView = "CurrentView";
            }
        }

        private void NamedView_CheckedChanged(object sender, EventArgs e)
        {
            if (NamedView.Checked)
            {
                FloorBoundary.Checked = false;
                CurrentView.Checked = false;
                Extents.Checked = false;
                SelectView = "NamedView";
            }
        }
    }
}
