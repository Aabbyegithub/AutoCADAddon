using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AutoCADAddon.Controls
{
    [DefaultEvent("ItemSelected")]
    public partial class TableDropdownBox : UserControl
    {
        private ToolStripDropDown _dropdown;
        private TableLayoutPanel _table;
        private List<ItemData> _items = new List<ItemData>();
        private ItemData _selectedItem;
        private Panel _scrollPanel;
        [Category("行为")]
        [Description("当选中项变化时触发")]
        public event EventHandler<ItemData> SelectedItemChanged;

        public event EventHandler<ItemData> ItemSelected;

        public TableDropdownBox()
        {
            InitializeComponent();
            InitDropdown();
        }

        private void InitDropdown()
        {
            _table = new TableLayoutPanel
            {
                AutoSize = true,
                Padding = new Padding(5),
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Dock = DockStyle.Top,
            };

            _scrollPanel = new Panel
            {
                AutoScroll = true,
                BackColor = Color.White,
                Width = this.Width,
                Height = 200, // 初始高度
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
              _scrollPanel.Controls.Add(_table);

            var host = new ToolStripControlHost(_scrollPanel) { AutoSize = false };
            _dropdown = new ToolStripDropDown();
            _dropdown.Items.Add(host);
        }

        private void MainButton_Click(object sender, EventArgs e)
        {
            if (_dropdown.Visible)
            {
                _dropdown.Close();
                return;
            }

            _table.PerformLayout();
            var size = new Size(this.Width, Math.Min(200, _table.PreferredSize.Height)+20);
            (_dropdown.Items[0] as ToolStripControlHost).Size = size;
            _dropdown.Show(this, new Point(0, this.Height));
        }

        public void SetItems(List<ItemData> items, string[] headers = null)
        {
            _items = items;
            _table.Controls.Clear();
            _table.RowStyles.Clear();

            int columnCount = _items.Max(i => i.Columns?.Length ?? 0);
            _table.ColumnCount = columnCount;
            _table.RowCount = 0;

            // 添加表头（如果提供了）
            if (headers != null && headers.Length > 0)
            {
                _table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                for (int col = 0; col < columnCount; col++)
                {
                    var text = (col < headers.Length) ? headers[col] : "";
                    var label = new Label
                    {
                        Text = text,
                        Font = new Font(Font, FontStyle.Bold),
                        AutoSize = true,
                        Padding = new Padding(3),
                        Margin = new Padding(2),
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Fill,
                        BackColor = Color.LightGray
                    };
                    _table.Controls.Add(label, col, _table.RowCount);
                }
                _table.RowCount++;
            }


            foreach (var item in _items)
            {
                int rowIndex = _table.RowCount++;
                _table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                for (int col = 0; col < columnCount; col++)
                {
                    var text = (item.Columns != null && col < item.Columns.Length) ? item.Columns[col] : "";
                    var label = new Label
                    {
                        Text = text,
                        AutoSize = true,
                        Padding = new Padding(3),
                        Margin = new Padding(2),
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Fill,
                        Cursor = Cursors.Hand
                    };

                    label.Click += (s, e) => OnItemClick(item);
                    _table.Controls.Add(label, col, rowIndex);
                }
            }
        }

        private void OnItemClick(ItemData item)
        {
            bool changed = _selectedItem != item;
            mainButton.Text = item.DisplayText + "▼";
            _dropdown.Close();

            ItemSelected?.Invoke(this, item);
            if (changed)
            {
                SelectedItemChanged?.Invoke(this, item);
            }
        }
        public void SetSelectedItem(string DisplayText)
        {
            _selectedItem = _items?.FirstOrDefault(i => i.DisplayText == DisplayText);
            mainButton.Text =DisplayText + "▼";
            SelectedItemChanged?.Invoke(this, _selectedItem);
        }

        /// <summary>
        /// 清空当前选择
        /// </summary>
        public void ClearSelection()
        {
            _selectedItem = null;
            mainButton.Text = "▼";  // 或者你想显示的默认文本
            SelectedItemChanged?.Invoke(this, null);  // 通知外部选择已清空
        }

        public string SelectedText => mainButton?.Text.Replace("▼", "");
        public object SelectedValue => _items?.FirstOrDefault(i => i.DisplayText.Trim() == SelectedText.Trim())?.Tag;
    }

    [Serializable]
    public class ItemData
    {
        public string DisplayText { get; set; }
        public object Tag { get; set; }
        public string[] Columns { get; set; }
    }
}
