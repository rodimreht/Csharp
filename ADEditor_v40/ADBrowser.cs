using System;
using System.Windows.Forms;

namespace ADEditor
{
	public partial class ADBrowser : Form
	{
		private const int Idx_CNCLOSE = 0;
		private const int Idx_OUCLOSE = 1;
		private const int idx_CNOPEN = 2;
		private const int idx_OUOPEN = 3;

		private bool isOK = false;
		private string ldapPath;
		private string reptype;
		private string server;
		private string uid;
		private string pwd;
		private string ldapHeader;

		public ADBrowser()
		{
			InitializeComponent();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.isOK = false;
			this.Hide();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (tviewLdap.SelectedNode == null)
			{
				MessageBox.Show("LDAP 경로를 선택해 주십시요.");
				return;
			}

			this.ldapPath = tviewLdap.SelectedNode.Tag.ToString();
			this.isOK = true;
			this.Hide();
		}

		private void tviewLdap_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node != null)
				getChildNodes(e.Node);
		}

		public bool IsOk
		{
			get { return isOK; }
		}

		public string LdapPath
		{
			get { return ldapPath; }
			set { ldapPath = value; }
		}

		private void getChildNodes(TreeNode pNode)
		{
			TreeNode childNodeItem;
			string nodeTag;

			// 이전에 하위에 더미로 넣어두었던 노드가 있다면 삭제한다.
			if (pNode.Nodes.Count == 1)
				if (pNode.Nodes[0].Text.Equals("")) pNode.Nodes.Clear();

			if (pNode.Nodes.Count == 0)
			{
				string[] childNodes = LdapServer.GetChildEntry(reptype, server, uid, pwd, pNode.Tag.ToString()).Split(new char[] { LdapServer.delim });
				
				foreach (string childNode in childNodes)
				{
					if (childNode.Equals("")) continue;

					nodeTag = childNode.Replace(ldapHeader, "");
					childNodeItem = pNode.Nodes.Add(getNodeName(childNode));
					childNodeItem.Tag = nodeTag;
					childNodeItem.ImageIndex = getNodeImageIndex(nodeTag, false);
					childNodeItem.SelectedImageIndex = getNodeImageIndex(nodeTag, false);
					childNodeItem.Nodes.Add("");
				}
			}

			if (pNode.Nodes.Count > 0)
			{
				pNode.SelectedImageIndex = getNodeImageIndex((string)pNode.Tag, true);
				pNode.ImageIndex = getNodeImageIndex((string)pNode.Tag, true);
			}
		}

		private int getNodeImageIndex(string nodePath, bool expanded)
		{
			string nodeType = nodePath.Substring(0, 2);
			if (nodeType.ToUpper().Equals("CN"))
				return expanded ? idx_CNOPEN : Idx_CNCLOSE;
			else
				return expanded ? idx_OUOPEN : Idx_OUCLOSE;
		}

		private string getNodeName(string nodePath)
		{
			return getNodeName(nodePath, false);
		}

		private string getNodeName(string nodePath, bool isRoot)
		{
			int startIdx = nodePath.LastIndexOf("/");
			int endIdx = nodePath.IndexOf(",");
			if (isRoot)
				return nodePath.Substring(startIdx + 1);
			else
				return nodePath.Substring(startIdx + 1, endIdx - startIdx - 1);
		}

		/// <summary>
		/// 폼이 로드될 때 필요한 값을 설정하고 초기화면을 구성한다.
		/// </summary>
		/// <param name="repType"></param>
		/// <param name="serverName"></param>
		/// <param name="userID"></param>
		/// <param name="userPwd"></param>
		public void SetDialog(string repType, string serverName, string userID, string userPwd)
		{
			this.reptype = repType;
			this.server = serverName;
			this.uid = userID;
			this.pwd = userPwd;
			this.ldapHeader = "LDAP://" + serverName + "/";

			string rootNode = LdapServer.GetRootEntry(repType, serverName, userID, userPwd) ?? ldapPath;

			tviewLdap.Nodes.Clear();
			TreeNode rootNodeItem = tviewLdap.Nodes.Add(getNodeName(rootNode, true));
			rootNodeItem.Tag = rootNode;
			rootNodeItem.ImageIndex = idx_OUOPEN;
			rootNodeItem.Expand();

			getChildNodes(rootNodeItem);
		}

		private void tviewLdap_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Nodes.Count > 0)
			{
				e.Node.SelectedImageIndex = getNodeImageIndex((string)e.Node.Tag, false);
				e.Node.ImageIndex = getNodeImageIndex((string)e.Node.Tag, false);
			}
		}
	}
}