using AutoCADAddon.Common;
using AutoCADAddon.Controls;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static AutoCADAddon.Model.FloorBuildingDataModel;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADAddon
{
    public partial class DrawingPropertiesForm : Form
    {
        public Document _doc;
        private string _versionCode;
        private string _SerId;
        private string _IsNew;
        public DrawingPropertiesForm(Document doc,string IsNew)
        {
            _doc = doc;
            InitializeComponent();
            cmbMet.Items.AddRange(new[] { "mm", "cm", "dm", "m", "km" });
            cmbMet.SelectedIndex = 0;
            DrawingProperties();
            Metric.Checked = true;
            _IsNew = IsNew;
        }

        /// <summary>
        /// 获取下拉框数据
        /// </summary>
        public async Task GetcmbList()
        {
            var res = await DataSyncService.SyncBuildingAsync();
            if ((res as string) != null && (res as string).Contains("NG"))
            {
                MessageCommon.Error($"楼栋数据获取失败{res}");
                return;
            }

            var items = new List<ItemData>();
            foreach (var item in res.list)
            {
                items.Add(new ItemData
                {
                    DisplayText = item.building_name,
                    Columns = new string[] { item.building_code, item.building_name },
                    Tag = item
                });
            }
            cmbBuilding.SetItems(items, new[] { "Building Code", "Building Name" });

        }

        /// <summary>
        /// 监控下拉选项变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmbBuilding_SelectedItemChanged(object sender, ItemData e)
        {
            var tag = e.Tag as JObject;
            if (tag != null && tag["building_code"] != null)
            {
                var res = await DataSyncService.SyncFloorAsync(tag["building_code"].ToString());
                if ((res as string) != null && (res as string).Contains("NG"))
                {
                    MessageCommon.Error($"楼层数据获取失败{res}");
                    return;
                }

                var items = new List<ItemData>();
                foreach (var item in res.list)
                {
                    items.Add(new ItemData
                    {
                        DisplayText = item.floor_name,
                        Columns = new string[] { item.building_code, item.floor_code, item.floor_name },
                        Tag = item
                    });
                }
                cmbFloor.SetItems(items, new[] { "Building Code", "Floor Code", "Floor Name" });
            }
        }

        private async Task DrawingProperties()
        {
            await GetcmbList();
            if (_doc != null)
            {
                Filename.Text = _doc.Window.Text;//图纸名称
                Title.Text = _doc.Window.Text;//图纸标题
                Path1.Text = Path1.Text + _doc.Name ?? "(none)"; //图纸路径

                // 使用反射调用内部方法
                var method = _doc.Database.GetType().GetMethod("GetSysVar",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                string versionCode = "未知";
                if (method != null)
                {
                    ResultBuffer rb = method.Invoke(_doc.Database, new object[] { "ACADVER" }) as ResultBuffer;
                    versionCode = rb?.AsArray()[0].Value.ToString() ?? "未知";
                }
                _versionCode = versionCode;
                Version.Text = Version.Text + versionCode;

                var props = CacheManager.GetCurrentDrawingProperties(_doc.Window.Text);
                if (props == null || string.IsNullOrEmpty(props.BuildingExternalCode) || string.IsNullOrEmpty(props.FloorCode))
                {
                    return;
                }
                _SerId = props.SerId;
                cmbBuilding.SetSelectedItem(props.BuildingName);
                cmbFloor.SetSelectedItem(props.FloorName);
            }
        }

        private async void BtnNewBuilding_Click(object sender, EventArgs e)
        {
            var form = new BuildingEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await GetcmbList();
            }
        }

        private void BtnNewFloor_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbBuilding.SelectedText)) return;
            var tag = cmbBuilding.SelectedValue as JObject;
            if (tag != null && tag["building_code"] != null)
            {
                var form = new FloorEditForm(null, tag["building_code"].ToString());
                if (form.ShowDialog() == DialogResult.OK)
                {
                    cmbBuilding_SelectedItemChanged(null,new ItemData { Tag = tag});
                }
            }

        }

        private async void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbBuilding.SelectedText) || string.IsNullOrEmpty(cmbFloor.SelectedText))
            {
                MessageBox.Show("请完整选择建筑、楼层和房间！");
                return;
            }
            DocumentCollection docs = null; Document newDoc = null;string fullPath = "";
            if (_IsNew == "ADD")
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "请选择一个文件夹用来保存图纸";
                    folderDialog.ShowNewFolderButton = true;  // 允许用户新建文件夹

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                         // 1. 新建图纸
                         docs = Application.DocumentManager;
                         newDoc = docs.Add("");


                        // 2. 保存为指定名称（改变标题）
                        string folder = folderDialog.SelectedPath; // 返回用户选择的路径
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                         fullPath = Path.Combine(folder, $"{Filename.Text.Replace(".dwg", "")}.dwg");

                        using (DocumentLock docLock = newDoc.LockDocument())
                        {
                            newDoc.Database.SaveAs(fullPath, DwgVersion.Current);
                        }
                        // 3. 注册 DocumentActivated 事件（提前监听）
                        var dm = Application.DocumentManager;
                        dm.DocumentActivated += (s, e1) =>
                        {
                            _doc = e1.Document;
                        };
                    }
                    else
                    {
                        return; // 用户取消
                    }
                }

            }

            var blueprint = new Blueprint();
            blueprint.BuildingExternalCode = cmbBuilding.SelectedValue is JObject jObject ? jObject["building_code"].ToString() : cmbBuilding.SelectedText;
            blueprint.BuildingName = cmbBuilding.SelectedText;
            blueprint.FloorCode = cmbFloor.SelectedValue is JObject jObjectFloor ? jObjectFloor["floor_code"].ToString() : cmbFloor.SelectedText;
            blueprint.FloorName = cmbFloor.SelectedText;
            blueprint.Name = Filename.Text;
            blueprint.Version = _versionCode;
            if (Metric.Checked)
            {
                blueprint.UnitType = Metric.Text;
                blueprint.Unit = cmbMet.Text;
            }
            if (Imperial.Checked)
            {
                blueprint.UnitType = Imperial.Text;
                blueprint.Unit = "";
            }
            var res = "";
            try
            {
                res = await DataSyncService.SyncDrawingAsync(new
                {
                    id = _SerId,
                    name = Filename.Text,
                    title = Filename.Text,
                    building = blueprint.BuildingExternalCode,
                    floor = blueprint.FloorCode,
                    unitType = blueprint.UnitType,
                    units = blueprint.Unit
                });
                await Task.Delay(100);
                var Drawing = await DataSyncService.SyncDrawingServiceAsync(Filename.Text);

                blueprint.SerId = Drawing.list[0].id;
            }
            catch (Exception)
            {

            }

            if (res == "NG")
            {
                blueprint.IsSave = "0";
            }
            CacheManager.SetCurrentDrawingProperties(blueprint);

            if (_IsNew == "ADD")
            {
                // 4. 打开刚刚保存的图纸（系统自动激活）
                docs.Open(fullPath, false);

                // 5. 关闭原来的临时图纸（不是当前活动文档了，可以安全关闭）
                newDoc.CloseAndDiscard();
                DrawingPanel.GetDrawingData();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Imperial_Click(object sender, EventArgs e)
        {
            Imperial.Checked = true;
            Metric.Checked = false;
        }

        private void Metric_Click(object sender, EventArgs e)
        {
            Imperial.Checked = false;
            Metric.Checked = true;
        }
    }
}
