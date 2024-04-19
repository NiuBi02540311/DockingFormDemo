using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
namespace WindowsFormsApplication1
{
    public partial class frmLefMenu : DockContent
    {
        public delegate void openSubMenu(string MenuID, string MenuName, string MenuClasspath);
        public openSubMenu OpenSubMenu;
        private DataTable MenuDataTable;
        public frmLefMenu()
        {
            InitializeComponent();
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(treeView1_NodeMouseDoubleClick);
        }
        private void frmLefMenu_Load(object sender, EventArgs e)
        {
            MenuDataTable = GetMenuData();
            frmTree_Load();
            LoadContextMenuStrip();
        }

        private void LoadContextMenuStrip()
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            var toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            var toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();

            contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem1,toolStripMenuItem2});
            contextMenuStrip.Name = "contextMenuStrip1";
            contextMenuStrip.Size = new System.Drawing.Size(125, 26);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(124, 22);
            toolStripMenuItem1.Text = "展开菜单";

            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(124, 22);
            toolStripMenuItem2.Text = "收起菜单";

            this.treeView1.ContextMenuStrip = contextMenuStrip;
            toolStripMenuItem1.Click += (object sender, EventArgs e) =>
            {
                //MessageBox.Show($"关闭当前界面【{fullTabControl1.SelectedTab.Text}】");
                this.treeView1.ExpandAll();
                 
            };
            toolStripMenuItem2.Click += (object sender, EventArgs e) =>
            {
                //MessageBox.Show("关闭所有界面");
                //fullTabControl1.TabPages.Clear();
                this.treeView1.CollapseAll();
            };

        }
        private DataTable GetMenuData()
        {
            //(string assemblyName, string typeName);
            DataTable ErrorData = new DataTable("ErrorData");

            ErrorData.Columns.Add("ID", System.Type.GetType("System.String"));
            ErrorData.Columns.Add("ParentID", System.Type.GetType("System.String"));
            ErrorData.Columns.Add("Name", System.Type.GetType("System.String"));
            ErrorData.Columns.Add("assemblyName", System.Type.GetType("System.String"));
            ErrorData.Columns.Add("typeName", System.Type.GetType("System.String"));

            ErrorData.Rows.Add("0", "-1", "root");
            ErrorData.Rows.Add("1", "0", "学生管理");
            ErrorData.Rows.Add("2", "0", "教师管理");
            ErrorData.Rows.Add("3", "1", "学生信息录入", "WindowsFormsApplication1", "WindowsFormsApplication1.View.Form1");
            ErrorData.Rows.Add("4", "2", "教师信息录入", "WindowsFormsApplication1", "WindowsFormsApplication1.View.Form2");
            ErrorData.Rows.Add("5", "0", "View");
            ErrorData.Rows.Add("6", "5", "Form6", "WindowsFormsApplication1", "WindowsFormsApplication1.View.Form1");
            ErrorData.Rows.Add("7", "5", "Form7", "WindowsFormsApplication1", "WindowsFormsApplication1.View.Form2");
            ErrorData.Rows.Add("8", "5", "Form8", "WindowsFormsApplication1", "WindowsFormsApplication1.View.Form1");
            ErrorData.Rows.Add("9", "5", "Form9", "WindowsFormsApplication1", "WindowsFormsApplication1.View.Form2");
            ErrorData.Rows.Add("10", "5", "Form10", "WindowsFormsApplication1", "WindowsFormsApplication1.View.Form1");
            ErrorData.Rows.Add("11", "5", "Demo", "WindowsFormsApplication1", "WindowsFormsApplication1.View.Form2");

            return ErrorData;
        }

        /// <summary>
        /// 主窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmTree_Load(DataTable NewTable = null)
        {
            // 根节点ID值
            int i = 0;
            i = -1;
            this.treeView1.Nodes.Clear();
            AddTree(i, (TreeNode)null, NewTable);
            treeView1.HideSelection = true;
            treeView1.ShowLines = true;
        }
        /// <summary>
        /// 创建树形菜单
        /// </summary>
        public void AddTree(int ParentID, TreeNode pNode, DataTable NewTable = null)
        {
            // 数据库名字字段
            string strName = "Name";
            // 数据库ID字段
            string strID = "ID";
            // 数据库父级ID字段
            string strParentID = "ParentID";
            //DataTable dt = SetErrorSData();
            //DataView dvTree = new DataView(dt);
            DataView dvTree = new DataView(MenuDataTable);
            if (NewTable != null)
            {
                dvTree = new DataView(NewTable);
            }
            dvTree.RowFilter = strParentID + " = " + ParentID;
            foreach (DataRowView Row in dvTree)
            {
                TreeNode Node = new TreeNode();
                if (pNode == null)
                {
                    Node.Text = Row[strName].ToString();
                    Node.Name = Row[strName].ToString();
                    Node.Tag = Row[strID].ToString();
                    Node.ImageIndex = 1;
                    this.treeView1.Nodes.Add(Node);
                    AddTree(Int32.Parse(Row[strID].ToString()), Node, NewTable); //再次递归
                }
                else
                {
                    Node.Text = Row[strName].ToString();
                    Node.Name = Row[strName].ToString();
                    Node.Tag = Row[strID].ToString();
                    Node.ImageIndex = 1;
                    pNode.Nodes.Add(Node);
                    AddTree(Int32.Parse(Row[strID].ToString()), Node, NewTable); //再次递归
                }
            }
        }


        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Nodes.Count <= 0)//当非父节点（即：实际的功能节点）
            {
                //ShowChildForm<Form1>();
                string strID = e.Node.Tag.ToString();
                var find = MenuDataTable.Select($" ID = '{strID}' ");
                if (find == null || find.Length == 0) return;

                //CreateInstance(string assemblyName, string typeName);
                //ObjectHandle t = Activator.CreateInstance("WindowsFormsApplication2", "WindowsFormsApplication2.View.Form2");
                // ObjectHandle t = Activator.CreateInstance(find[0]["assemblyName"].ToString(), find[0]["typeName"].ToString());
                //Form mzhj = (Form)t.Unwrap();

                //DataRow dRow = (DataRow)e.Node.Tag;

                //string MenuID = dRow["ID"].ToString();
                //string MenuName = dRow["Name"].ToString();
                //string MenuClasspath = dRow["typeName"].ToString();

                string MenuID = find[0]["ID"].ToString();
                string MenuName = find[0]["Name"].ToString();
                string MenuClasspath = find[0]["typeName"].ToString();

                if (MenuName != "" && MenuClasspath != "")
                {
                    //string NodeText = e.Node.Text;
                    //string NodeName = e.Node.Name;
                    //UseCount(MenuID);
                    OpenSubMenu(MenuID, MenuName, MenuClasspath);
                }
            }
             
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string str = textBox1.Text.Trim();
            if (str == "")
            {
                frmTree_Load();
                treeView1.ExpandAll();
                return;
            }
            
           var find = MenuDataTable.Select($" Name like '%{str}%' ");
            if(find == null || find.Length == 0)
            {
                treeView1.Nodes.Clear();
                return;
            }
            DataTable newdt = find.CopyToDataTable();
            //DataTable copy = MenuDataTable.Clone();
            //DataTable ErrorData = new DataTable("ErrorData");

            //ErrorData.Columns.Add("ID", System.Type.GetType("System.String"));
            //ErrorData.Columns.Add("ParentID", System.Type.GetType("System.String"));
            //ErrorData.Columns.Add("Name", System.Type.GetType("System.String"));
            //ErrorData.Columns.Add("assemblyName", System.Type.GetType("System.String"));
            //ErrorData.Columns.Add("typeName", System.Type.GetType("System.String"));

            List<int> ID = new List<int>();
            ID.Add(0);
            foreach (DataRow w in newdt.Rows)
            {
                //var find2 = MenuDataTable.Select($" ID = '{w["ParentID"]}' ");
                //if (find2 == null || find2.Length == 0)
                //{
                //    continue;
                //}
                //copy.Rows.Add(find2[0]);
                ID.Add(int.Parse(w["ID"].ToString()));
                ID.Add(int.Parse(w["ParentID"].ToString()));
            }

            var newList = ID.Distinct().ToList();

            str = string.Join(",",newList);
            find = MenuDataTable.Select($" ID in ({str}) ");
            if (find == null || find.Length == 0)
            {
                treeView1.Nodes.Clear();
                return;
            }
            newdt = find.CopyToDataTable();
            //newdt.Merge(copy, false, MissingSchemaAction.Add);
            frmTree_Load(newdt);
            treeView1.ExpandAll();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
