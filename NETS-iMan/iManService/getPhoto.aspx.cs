using System;
using System.Collections.Specialized;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace iManService
{
	public partial class getPhoto : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string userID = Request.QueryString["uid"];
			string tempid = Request.QueryString["tempid"];
			if (!string.IsNullOrEmpty(userID))
				showPhoto(userID);
			else if (!string.IsNullOrEmpty(tempid))
				showTempPhoto(tempid);
		}

		private void showPhoto(string userID)
		{
			try
			{
				string fileName = ADUserInfo.GetAttribute(userID, "personalTitle");
				if (fileName.Length > 0)
				{
					string photoPath = ConfigurationManager.AppSettings["photoPath"];
					FileInfo fi = new FileInfo(photoPath + @"\" + fileName);
					FileStream fs = fi.OpenRead();
					byte[] byteBuffer = new byte[fs.Length];
					fs.Read(byteBuffer, 0, (int)fs.Length);
					fs.Close();

					Response.ContentType = ManagedMimes.GetMimeType(fileName);
					Response.BinaryWrite(byteBuffer);
				}
				else
				{
					FileInfo fi = new FileInfo(Server.MapPath("./images/no_mark.gif"));
					FileStream fs = fi.OpenRead();
					byte[] byteBuffer = new byte[fs.Length];
					fs.Read(byteBuffer, 0, (int)fs.Length);
					fs.Close();

					Response.ContentType = "image/gif";
					Response.BinaryWrite(byteBuffer);
				}

			}
			catch
			{
				Response.BinaryWrite(new byte[] { 0x00 });
			}
		}

		private void showTempPhoto(string userID)
		{
			try
			{
				string fileName = ADUserInfo.GetTempAttribute(userID, "personalTitle");
				if (fileName.Length > 0)
				{
					string photoPath = ConfigurationManager.AppSettings["PhotoPath"];
					FileInfo fi = new FileInfo(photoPath + @"\" + fileName);
					FileStream fs = fi.OpenRead();
					byte[] byteBuffer = new byte[fs.Length];
					fs.Read(byteBuffer, 0, (int)fs.Length);
					fs.Close();

					Response.ContentType = ManagedMimes.GetMimeType(fileName);
					Response.BinaryWrite(byteBuffer);
				}
				else
				{
					FileInfo fi = new FileInfo(Server.MapPath("./images/no_mark.gif"));
					FileStream fs = fi.OpenRead();
					byte[] byteBuffer = new byte[fs.Length];
					fs.Read(byteBuffer, 0, (int)fs.Length);
					fs.Close();

					Response.ContentType = "image/gif";
					Response.BinaryWrite(byteBuffer);
				}

			}
			catch
			{
				Response.BinaryWrite(new byte[] { 0x00 });
			}
		}
	}
}
