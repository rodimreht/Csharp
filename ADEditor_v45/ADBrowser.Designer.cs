using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace ADEditor
{
	partial class ADBrowser
	{
		private Panel pnlButtons;
		private Button btnCancel;
		private Button btnOK;
		private Panel pnlBottom;
		private Panel pnlFill;
		private TreeView tviewLdap;

		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ADBrowser));
			this.tviewLdap = new System.Windows.Forms.TreeView();
			this.imgList = new System.Windows.Forms.ImageList(this.components);
			this.pnlButtons = new System.Windows.Forms.Panel();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.pnlFill = new System.Windows.Forms.Panel();
			this.pnlButtons.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			this.pnlFill.SuspendLayout();
			this.SuspendLayout();
			// 
			// tviewLdap
			// 
			this.tviewLdap.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tviewLdap.ImageIndex = 0;
			this.tviewLdap.ImageList = this.imgList;
			this.tviewLdap.Location = new System.Drawing.Point(5, 5);
			this.tviewLdap.Name = "tviewLdap";
			this.tviewLdap.SelectedImageIndex = 0;
			this.tviewLdap.Size = new System.Drawing.Size(350, 347);
			this.tviewLdap.TabIndex = 0;
			this.tviewLdap.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.tviewLdap_AfterCollapse);
			this.tviewLdap.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tviewLdap_BeforeExpand);
			// 
			// imgList
			// 
			this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
			this.imgList.TransparentColor = System.Drawing.Color.Transparent;
			this.imgList.Images.SetKeyName(0, "");
			this.imgList.Images.SetKeyName(1, "");
			this.imgList.Images.SetKeyName(2, "");
			this.imgList.Images.SetKeyName(3, "");
			// 
			// pnlButtons
			// 
			this.pnlButtons.Controls.Add(this.btnOK);
			this.pnlButtons.Controls.Add(this.btnCancel);
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlButtons.Location = new System.Drawing.Point(200, 0);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(160, 40);
			this.pnlButtons.TabIndex = 1;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(-1, 6);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(80, 6);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.pnlButtons);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 357);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(360, 40);
			this.pnlBottom.TabIndex = 2;
			// 
			// pnlFill
			// 
			this.pnlFill.Controls.Add(this.tviewLdap);
			this.pnlFill.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlFill.Location = new System.Drawing.Point(0, 0);
			this.pnlFill.Name = "pnlFill";
			this.pnlFill.Padding = new System.Windows.Forms.Padding(5);
			this.pnlFill.Size = new System.Drawing.Size(360, 357);
			this.pnlFill.TabIndex = 3;
			// 
			// ADBrowser
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(360, 397);
			this.Controls.Add(this.pnlFill);
			this.Controls.Add(this.pnlBottom);
			this.Name = "ADBrowser";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Active Directory - 디렉터리 검색";
			this.pnlButtons.ResumeLayout(false);
			this.pnlBottom.ResumeLayout(false);
			this.pnlFill.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ImageList imgList;
	}
}