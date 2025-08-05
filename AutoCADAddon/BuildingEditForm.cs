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
    public partial class BuildingEditForm : Form
    {
         public Building EditedBuilding { get; private set; }
        public BuildingEditForm(Building building = null)
        {
            InitializeComponent();
            EditedBuilding = building ?? new Building();
              txtName.Text = EditedBuilding.Name;
             txtCode.Text = EditedBuilding.Code;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("建筑名称不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("建筑编码不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EditedBuilding.Name = txtName.Text;
            EditedBuilding.Code = txtCode.Text;
            EditedBuilding.UpdateTime = DateTime.Now;
            var res =await DataSyncService.AddBuildingAsync(txtCode.Text,txtName.Text);
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
