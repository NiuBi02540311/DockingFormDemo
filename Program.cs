using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Index());

            Login login = new Login();
            DialogResult Result = login.ShowDialog();
            //if (formLogin.ShowDialog() == DialogResult.OK)
            if (Result.Equals(DialogResult.OK))
            {

                Application.Run(new Index());
            }
            else
            {
                Application.Exit();
            }

        }
    }
}
