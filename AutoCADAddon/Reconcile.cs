using AutoCADAddon.Common;
using Autodesk.AutoCAD.ApplicationServices;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADAddon
{
    public partial class Reconcile : Form
    {
        private readonly Document _doc;
        public Reconcile(Document doc)
        {
            InitializeComponent();
            _doc = doc;
            ReconcileRoomData(doc);
        }

        private void ReconcileRoomData(Document doc)
        {
            var props = CacheManager.GetCurrentDrawingProperties(doc.Window.Text);
            if (props == null || string.IsNullOrEmpty(props.BuildingExternalCode) || string.IsNullOrEmpty(props.FloorCode))
            {
                Application.ShowAlertDialog("请先绑定图纸到楼层！");
                return;
            }
            var room = CacheManager.GetRoomsByRoomCode(props.BuildingExternalCode, props.FloorCode);
            // PolylineCommon.GetLayerXData();
            var RoomList = PolylineCommon.ParseAndExportDrawingData(doc, props.FloorCode);
            foreach (var item in RoomList)
            {
                var index = dataGridView1.Rows.Add();
                var row = dataGridView1.Rows[index];

                row.Cells["DrawingCode"].Value = item.Code;
                row.Cells["ReconcileRoomCode"].Value = room.FirstOrDefault(a=>a.Code == item.Code)?.Code;
            }
        }
    }
}
