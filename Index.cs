using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace WindowsFormsApplication1
{
    public partial class Index : Form
    {
        /// <summary>
        /// </summary>
        private const int CP_NOCLOSE_BUTTON = 0x200;
        /// <summary>
        /// 解决窗体加载时闪烁问题, 以及禁用关闭按钮
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams paras = base.CreateParams;
                paras.ExStyle |= 0x02000000;

                paras.ClassStyle = paras.ClassStyle | CP_NOCLOSE_BUTTON;
                return paras;
            }
        }

        private DeserializeDockContent m_deserializeDockContent;
        private frmLefMenu _frmLefMenu;
        
        public Index()
        {
            InitializeComponent();

            

            this.DockPanelpanel1.DocumentStyle = DocumentStyle.DockingMdi;
            this.DockPanelpanel1.Theme = this.vS2015BlueTheme1;
            //this.DockPanelpanel1.Theme = this.vS2015DarkTheme1;
            //this.DockPanelpanel1.Theme = this.vS2015LightTheme1;
            //this.DockPanelpanel1.Theme = this.vS2005MultithreadingTheme1;
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
        }
        private void Index_Load(object sender, EventArgs e)
        {
            LoadConfigXml();
            CreateLeftMenu();
           
        }
        private void CreateLeftMenu()
        {
            _frmLefMenu = new frmLefMenu() { TabText = "左侧菜单栏" };
            _frmLefMenu.RightToLeftLayout = RightToLeftLayout;
            _frmLefMenu.HideOnClose = true;
            _frmLefMenu.Show(this.DockPanelpanel1, DockState.DockLeft);
            _frmLefMenu.OpenSubMenu += this.OpenSubMenu;
            //_frmLefMenu.ShowUpdateMessage += this.showUpdateMessage;

        }
      

        private async void OpenSubMenu(string MenuID, string MenuName, string MenuClasspath)
        {

            //if (SysStop)
            //{
            //    MessageBox.Show(SystemMessage);
            //    return;
            //}
            //label1.Text = "窗口加载中,请稍候......";
            //Loading.BeginWaiting();
            await Task.Delay(100);
            DockContent SubMenu = null;
            await Task.Run(() => {
                SubMenu = CheckSubMenu(MenuName);
            });

            #region


                if (SubMenu != null)
                {
                    //激活子窗体
                    SubMenu.Activate();
                    return;
                }
             
            

                await Task.Run(() => {
                    SubMenu = GetCurrentForm(MenuClasspath);
                });
                if (SubMenu == null)
                {
                   return;
                }

                //_frmCenterMenu = new frmCenterMenu() { TabText = "DockCenter" };
                SubMenu.TabText = MenuName;
                SubMenu.Tag = MenuID;
                SubMenu.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmAutoTrackInOutNew_FormClosing);
                SubMenu.RightToLeftLayout = RightToLeftLayout;
                //_frmCenterMenu.ShowIcon = true;
                //_frmCenterMenu.CloseButton = true;
                //_frmCenterMenu.CloseButtonVisible = true;
                //_frmCenterMenu.HideOnClose = true;

                if (DockPanelpanel1.DocumentStyle == DocumentStyle.DockingSdi)
                {
                    SubMenu.MdiParent = this;
                    SubMenu.Show();
                }
                else
                {
                    SubMenu.Show(this.DockPanelpanel1, DockState.Document);
                    // SubMenu.Show(this.dockPanel1);
                }
                //CheckFormButton(SubMenu);
            #endregion



        }
        private DockContent GetCurrentForm(string className)
        {
            try
            {

                //CreateInstance(string assemblyName, string typeName);
                //ObjectHandle t = Activator.CreateInstance("WindowsFormsApplication2", "WindowsFormsApplication2.View.Form2");
                // ObjectHandle t = Activator.CreateInstance(find[0]["assemblyName"].ToString(), find[0]["typeName"].ToString());
                //Form mzhj = (Form)t.Unwrap();

                string[] menu = className.Split('.');
                //ObjectHandle t = Activator.CreateInstance(Application.ProductName, Application.ProductName + "." + className.Trim());
                ObjectHandle t = Activator.CreateInstance(menu[0], className);
                if (t != null)
                {
                    // Form frm = (Form)t.Unwrap();
                    DockContent frm = (DockContent)t.Unwrap();

                    return frm;
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            //反射调用子窗口

        }
        private DockContent CheckSubMenu(string MenuName)
        {

            // var SubMenus = this.dockPanel1.Contents.ToList();
            // var sub = from p in SubMenus.Where(x=>x.)
            DockContent rs = null;
            foreach (DockContent frm in this.DockPanelpanel1.Contents)
            {
                if (frm.TabText == MenuName)
                {
                    rs = frm;
                    break;
                }
                //这样设置后，所有的继承与DockContent的窗体都会被计算在内的

            }
            return rs;
        }
        private void frmAutoTrackInOutNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (Timer01.Enabled == false || CloseAllContentsFlag == true)
            //{

            //}
            if (CloseAllContentsFlag == true)
            {
                return;
            }
            //else
            {
                try
                {
                    DockContent fm = sender as DockContent;
                    if (fm != null)
                    {
                        string fmName = fm.Name.ToLower();
                        string menuname = fm.TabText;
                        //view.productionmanagement.frmbinpartition
                        //if (AllMenus != null && AllMenus.Rows.Count > 0)
                        //{
                        //    string menuclasspath = "";

                        //    for (int i = 0; i < AllMenus.Rows.Count; i++)
                        //    {
                        //        menuclasspath = AllMenus.Rows[i]["menuclasspath"].ToString();
                        //        string[] ls = menuclasspath.Split('.');
                        //        if (ls[ls.Length - 1] == fmName)
                        //        {
                        //            menuname = AllMenus.Rows[i]["menuname"].ToString();
                        //            break;
                        //        }
                        //    }
                        //}

                        if (MessageBox.Show($"确定要关闭当前窗体({menuname})？", "提醒", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            e.Cancel = true;
                            //return;
                        }
                    }
                }
                catch
                {

                }


            }

        }
        private IDockContent GetContentFromPersistString(string persistString)
        {
            //PersistString="iJedha.Automation.GUI.View.IndexSubMenu.frmLefMenu"
            if (persistString == typeof(frmLefMenu).ToString())
            {
                return _frmLefMenu;
            }
            //else
            //{
            //    string[] parsedStrings = persistString.Split(new char[] { ',' });
            //    if (parsedStrings.Length != 3)
            //        return null;
            //}


            return null;


        }
        /// <summary>
        /// 是否保存配置文件
        /// </summary>
        private bool m_bSaveLayout = true;
        /// <summary>
        /// 保存配置文件
        /// </summary>
        private void SaveConfigXml()
        {

            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (m_bSaveLayout)
                DockPanelpanel1.SaveAsXml(configFile);

            else if (File.Exists(configFile))
                File.Delete(configFile);
        }


        /// <summary>
        /// 读取配置文件,注意要先加载配置文件，再创建左侧菜单栏，否则会报错
        /// </summary>
        private void LoadConfigXml()
        {
            //aa = iJedha.Automation.GUI.View.IndexSubMenu.frmLefMenu
            //var aa = typeof(frmLefMenu).ToString();
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (File.Exists(configFile))
                //dockPanel1.LoadFromXml(configFile, m_deserializeDockContent);
                DockPanelpanel1.LoadFromXml(configFile, m_deserializeDockContent);
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"确认要退出系统？", "善意提醒", MessageBoxButtons.YesNo) == DialogResult.No) return;
                CloseAllContentsFlag = true;
                SaveConfigXml();
                Thread.Sleep(1000);
                Application.Exit();
             
        }

        private void showMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_frmLefMenu.DockPanel != null)
            {
                if (_frmLefMenu.IsHidden)
                {
                    _frmLefMenu.IsHidden = false;
                }
                else
                {
                    _frmLefMenu.IsHidden = true;
                }
            }
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllDocuments();
        }
        private bool CloseAllContentsFlag = false;
        private void CloseAllDocuments()
        {
            CloseAllContentsFlag = true;
            if (DockPanelpanel1.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    form.Close();
            }
            else
            {
                foreach (IDockContent document in DockPanelpanel1.DocumentsToArray())
                {
                    // IMPORANT: dispose all panes.
                    document.DockHandler.DockPanel = null;
                    document.DockHandler.Close();
                }
            }
            CloseAllContentsFlag = false;
        }
    }
}
