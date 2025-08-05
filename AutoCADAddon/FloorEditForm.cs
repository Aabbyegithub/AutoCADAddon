using AutoCADAddon.Common;
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
    public partial class FloorEditForm : Form
    {
        public Floor EditedFloor { get; private set; }
        private readonly string _buildingCode;
        public FloorEditForm(Floor floor = null, string buildingCode = "")
        {
            InitializeComponent();
            _buildingCode = buildingCode;
            EditedFloor = floor ?? new Floor { BuildingCode = buildingCode };
            txtName.Text = EditedFloor.Name;
            txtCode.Text = EditedFloor.Code;
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("楼层名称不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("楼层编码不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            EditedFloor.Name = txtName.Text;
            EditedFloor.Code = txtCode.Text;
            EditedFloor.BuildingCode = _buildingCode;
            EditedFloor.UpdateTime = DateTime.Now;
            var res = await DataSyncService.AddFloorAsync(_buildingCode,txtCode.Text, txtName.Text);
            if (res as string == "OK")
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                MessageCommon.Error(res);
        }
    }
}
