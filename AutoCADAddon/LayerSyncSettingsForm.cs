using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace AutoCADAddon
{
    public class LayerSyncSettingsForm : Form
    {
        private CheckedListBox clbLayers;
        private Button btnOK;
        private Button btnCancel;
        private List<string> allLayerNames;
        public List<string> SelectedSyncLayers { get; private set; } = new List<string>();

        public LayerSyncSettingsForm(Document doc)
        {
            InitializeComponent();
            LoadLayers(doc);
            LoadUserSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "图层同步设置";
            this.Width = 350;
            this.Height = 400;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            clbLayers = new CheckedListBox { Left = 20, Top = 20, Width = 280, Height = 280 };
            btnOK = new Button { Text = "确定", Left = 60, Top = 320, Width = 80 };
            btnCancel = new Button { Text = "取消", Left = 180, Top = 320, Width = 80 };

            this.Controls.AddRange(new Control[] { clbLayers, btnOK, btnCancel });
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
        }

        private void LoadLayers(Document doc)
        {
            allLayerNames = new List<string>();
            using (var tr = doc.Database.TransactionManager.StartTransaction())
            {
                var lt = (LayerTable)tr.GetObject(doc.Database.LayerTableId, OpenMode.ForRead);
                foreach (ObjectId id in lt)
                {
                    var layer = (LayerTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    allLayerNames.Add(layer.Name);
                }
            }
            clbLayers.Items.Clear();
            foreach (var name in allLayerNames)
                clbLayers.Items.Add(name);
        }

        private void LoadUserSettings()
        {
            // 假设用简单本地文件保存同步图层名列表
            var layers = LayerSyncSettingsManager.LoadSyncLayers();
            for (int i = 0; i < clbLayers.Items.Count; i++)
            {
                if (layers.Contains(clbLayers.Items[i].ToString()))
                    clbLayers.SetItemChecked(i, true);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            SelectedSyncLayers = clbLayers.CheckedItems.Cast<string>().ToList();
            LayerSyncSettingsManager.SaveSyncLayers(SelectedSyncLayers);
            this.DialogResult = DialogResult.OK;
        }
    }

    public static class LayerSyncSettingsManager
    {
        private static string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sync_layers.json");
        public static List<string> LoadSyncLayers()
        {
            if (!System.IO.File.Exists(configPath)) return new List<string>();
            var json = System.IO.File.ReadAllText(configPath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
        }
        public static void SaveSyncLayers(List<string> layers)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(layers);
            System.IO.File.WriteAllText(configPath, json);
        }
    }
}
