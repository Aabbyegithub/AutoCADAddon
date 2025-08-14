using AutoCADAddon.Common;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AutoCADAddon.Model.ClassModel;
using static AutoCADAddon.Model.FloorBuildingDataModel;

namespace AutoCADAddon
{
    public partial class LoginForm : Form
    {
        public LoginResult LoginResult { get; private set; }
        public LoginForm()
        {
            InitializeComponent();
            // 加载服务器列表
            CacheManager.UpSys_Server();
            var Servers = CacheManager.GetSys_Server();
            var User = CacheManager.GetSys_User();
            cboServers.Items.AddRange(Servers.Select(a=>a.Url).ToArray());
            if (Servers.Count >0)
            {
                cboServers.SelectedIndex = 0;
            }

            // 加载记住密码状态
            if (User !=null)
            {
                IsRenember.Checked = true;
                UserName.Text = User.UserName;
                Password.Text = User.Password;
            }
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            // 输入校验
            if (string.IsNullOrWhiteSpace(cboServers.Text))
            {
                MessageBox.Show("请输入服务器地址", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(ProjectList.Text))
            {
                MessageBox.Show("请选择项目", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(UserName.Text))
            {
                MessageBox.Show("请输入用户名", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("请输入密码", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var result = await DataSyncService.LoginAsync(cboServers.Text,UserName.Text, Password.Text);
                if (!result.Contains("OK"))
                {
                    MessageBox.Show("登录失败，请检查用户名和密码", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CacheManager.SetSys_Server(new Sys_Server() { Url = cboServers.Text, IsTrue = "1" });
                //// 是否保存账户密码
                //if (IsRenember.Checked)
                //{
                //    CacheManager.SetSys_User(new Sys_User() { UserName = UserName.Text,Password = Password.Text});
                //}

                DialogResult = DialogResult.OK; // 登录成功，关闭窗口
                Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 取消登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private async Task<LoginResult> LoginAsync(string serverUrl, string project, string username, string password)
        {
            var client = new HttpClient();

            var requestBody = new
            {
                last_sync_time = CacheManager.GetLastSyncTime()
            };

            // 使用原生JSON序列化
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{serverUrl}/api/auth/login", content);

            // 确保响应成功
            response.EnsureSuccessStatusCode();

            // 使用原生JSON反序列化
            var responseJson = await response.Content.ReadAsStringAsync();
            var LoginUser = JsonConvert.DeserializeObject<LoginResult>(responseJson);

            return LoginUser;
        }
    }
}
