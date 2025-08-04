using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AutoCADAddon.Common
{
    /// <summary>
    /// 消息提醒封装
    /// </summary>
    public class MessageCommon
    {
        /// <summary>
        /// 错误
        /// </summary>
        public static void Error(string context)
        {
            MessageBox.Show(context, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 提醒
        /// </summary>
        /// <param name="context"></param>
        public static void Info(string context)
        {
            MessageBox.Show(context, "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="context"></param>
        public static void Waring(string context)
        {
            MessageBox.Show(context, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
