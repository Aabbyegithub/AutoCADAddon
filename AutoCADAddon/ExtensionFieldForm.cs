using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AutoCADAddon.Model.FloorBuildingDataModel;

namespace AutoCADAddon
{
    public partial class ExtensionFieldsForm : Form
    {
        public Dictionary<string, ExtensionField> UpdatedExtensions { get; private set; }
        private readonly TableLayoutPanel _table;

        public ExtensionFieldsForm()
        {
            UpdatedExtensions = new Dictionary<string, ExtensionField>();
            _table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 4,
                RowCount = 1
            };
            _table.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            _table.Controls.Add(new Label { Text = "字段名" }, 0, 0);
            _table.Controls.Add(new Label { Text = "类型" }, 1, 0);
            _table.Controls.Add(new Label { Text = "值" }, 2, 0);
            _table.Controls.Add(new Label { Text = "操作" }, 3, 0);

            var btnAdd = new Button { Text = "新增字段", Dock = DockStyle.Bottom, Height = 30 };
            btnAdd.Click += BtnAdd_Click;

            Controls.Add(_table);
            Controls.Add(btnAdd);
            InitializeComponent();
            LoadExistingFields();
        }

        private void InitializeComponent()
        {
            Width = 600;
            Height = 400;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "扩展字段管理";
        }

        private void LoadExistingFields()
        {
            foreach (var item in UpdatedExtensions)
            {
                AddFieldToTable(item.Key, item.Value);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox("字段名:", "新增字段");
            if (string.IsNullOrWhiteSpace(name) || UpdatedExtensions.ContainsKey(name)) return;

            var field = new ExtensionField { Type = "text" };
            UpdatedExtensions.Add(name, field);
            AddFieldToTable(name, field);
        }

        private void AddFieldToTable(string name, ExtensionField field)
        {
            var row = _table.RowCount++;
            _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));

            // 字段名
            _table.Controls.Add(new Label { Text = name, Dock = DockStyle.Fill }, 0, row);

            // 类型选择
            var cboType = new ComboBox { Dock = DockStyle.Fill };
            cboType.Items.AddRange(new[] { "text", "number", "enum", "date" });
            cboType.Text = field.Type;
            cboType.SelectedIndexChanged += (s, e) => UpdateFieldType(name, cboType.Text, row);
            _table.Controls.Add(cboType, 1, row);

            // 值控件
            var valueCtrl = GetValueControl(field);
            valueCtrl.Tag = $"{name}_value";
            _table.Controls.Add(valueCtrl, 2, row);

            // 删除按钮
            var btnDel = new Button { Text = "删除", Dock = DockStyle.Fill, BackColor = System.Drawing.Color.LightPink };
            btnDel.Click += (s, e) => RemoveField(name, row);
            _table.Controls.Add(btnDel, 3, row);
        }

        private Control GetValueControl(ExtensionField field)
        {
            switch (field.Type)
            {
                case "number":
                    return new NumericUpDown { Value = Convert.ToDecimal(field.Value ?? 0), Dock = DockStyle.Fill };
                case "date":
                    return new DateTimePicker { Value = field.Value != null ? Convert.ToDateTime(field.Value) : DateTime.Now, Dock = DockStyle.Fill };
                case "enum":
                    var cbo = new ComboBox { Dock = DockStyle.Fill };
                    cbo.Items.AddRange(field.EnumOptions.ToArray());
                    cbo.Text = field.Value?.ToString();
                    cbo.LostFocus += (s, e) => field.Value = cbo.Text;
                    return cbo;
                default:
                    return new TextBox { Text = field.Value?.ToString() ?? "", Dock = DockStyle.Fill, Tag = field };
            }
        }

        private void UpdateFieldType(string name, string type, int row)
        {
            UpdatedExtensions[name].Type = type;
            var oldCtrl = _table.GetControlFromPosition(2, row);
            _table.Controls.Remove(oldCtrl);
            _table.Controls.Add(GetValueControl(UpdatedExtensions[name]), 2, row);
        }

        private void RemoveField(string name, int row)
        {
            UpdatedExtensions.Remove(name);
            for (int i = 0; i < 4; i++)
            {
                var ctrl = _table.GetControlFromPosition(i, row);
                _table.Controls.Remove(ctrl);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 保存所有控件值
            foreach (var item in UpdatedExtensions)
            {
                for (int row = 1; row < _table.RowCount; row++)
                {
                    var lbl = _table.GetControlFromPosition(0, row) as Label;
                    if (lbl?.Text != item.Key) continue;

                    var valueCtrl = _table.GetControlFromPosition(2, row);

                    switch (valueCtrl)
                    {
                        case TextBox textBox:
                            item.Value.Value = textBox.Text;
                            break;
                        case NumericUpDown numericUpDown:
                            item.Value.Value = numericUpDown.Value;
                            break;
                        case DateTimePicker dateTimePicker:
                            item.Value.Value = dateTimePicker.Value;
                            break;
                        case ComboBox comboBox:
                            item.Value.Value = comboBox.Text;
                            break;
                        default:
                            item.Value.Value = item.Value.Value;
                            break;
                    }

                    break;
                }
            }
            DialogResult = DialogResult.OK;
            base.OnFormClosing(e);
        }
    }
}
