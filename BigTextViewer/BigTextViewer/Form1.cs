using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BigTextViewer
{
	public partial class Form1 : Form
	{
		private string _path = null;
		private bool _searching = false;
		private long _total = -1;
		private long _lastPos = -1;
		private int _strPos = -1;
		private StreamReader _sr = null;
		private char[] _buffer = new char[32767];

		public Form1()
		{
			InitializeComponent();
		}

		private void cmdOpen_Click(object sender, EventArgs e)
		{
			try
			{
				DialogResult r = openFileDialog1.ShowDialog();
				if (r.Equals(DialogResult.OK))
				{
					_path = openFileDialog1.FileName;
					txtFilePath.Text = _path;
				}

				if (_path != null)
				{
					FileInfo fi = new FileInfo(_path);
					FileStream fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					_total = fs.Length;
					if (_sr != null)
					{
						_sr.Close();
						_sr = null;
					}
					_sr = new StreamReader(fs, cboEncoding.SelectedIndex == 0 ? Encoding.Default : Encoding.UTF8, true, _buffer.Length * 10);
					int size = _sr.Read(_buffer, 0, _buffer.Length);
					txtPreview.Text = new string(_buffer, 0, size);
					_lastPos = 0;
					_strPos = -1;
					txtOrgString.ReadOnly = false;
					cmdSearch.Enabled = true;
					cmdSearch.BackColor = Color.YellowGreen;
					cmdNext.Visible = true;
					chkReverse.Enabled = true;
				}
			}
			catch (Exception ex)
			{
				txtStatus.Text = "에러: " + ex.Message;
				if (_sr != null)
				{
					_sr.Close();
					_sr = null;
				}
				txtOrgString.ReadOnly = true;
				cmdSearch.Enabled = false;
				cmdSearch.BackColor = Color.LightGray;
				cmdNext.Visible = false;
				chkReverse.Enabled = false;
				chkReverse.Enabled = false;
			}
		}

		private void cmdSearch_Click(object sender, EventArgs e)
		{
			bool isReverse = chkReverse.Checked;

			if (!_searching)
			{
				if (txtOrgString.Text.Length == 0)
				{
					MessageBox.Show("찾을 문자열을 입력하세요!", "입력", MessageBoxButtons.OK, MessageBoxIcon.Information);
					txtOrgString.Focus();
					return;
				}

				_searching = true;
				if (isReverse)
				{
					_lastPos = _sr.BaseStream.Length - 1;
					_strPos = txtPreview.Text.Length;
				}
				else
				{
					_lastPos = 0;
					_strPos = -1;
				}
				cmdOpen.Enabled = false;
				cboEncoding.Enabled = false;
				txtOrgString.ReadOnly = true;
				cmdSearch.Text = "다음찾기";
				cmdSearch.BackColor = Color.LightSeaGreen;
				cmdSearch.Width = 73;
				cmdSearch.Refresh();
				cmdStop.Enabled = true;
				cmdStop.BackColor = Color.Red;
			}

			try
			{
				string orgStr = txtOrgString.Text;
				int buffLen = _buffer.Length;

				if (!isReverse) // 정방향 검색
				{
					if (_lastPos > 0)
					{
						string str = txtPreview.Text;
						_strPos = str.IndexOf(orgStr, _strPos + 1, StringComparison.CurrentCultureIgnoreCase);
						if (_strPos >= 0)
						{
							txtStatus.Text = "검색 중...(" + ((double)(_lastPos + _strPos + 1) * 100 / _total).ToString("#0.##") + "%)";
							txtStatus.Refresh();
							txtPreview.Select(_strPos, orgStr.Length);
							txtPreview.ScrollToCaret();
							return;
						}
					}

					_sr.BaseStream.Position = _lastPos;
					while (_sr.Peek() >= 0)
					{
						int size = _sr.Read(_buffer, 0, buffLen);
						string str = new string(_buffer, 0, size);
						if (size == buffLen) str += _sr.ReadLine(); // 줄 끝까지 읽는다
						long pos = _sr.BaseStream.Position;
						txtStatus.Text = "검색 중...(" + ((double)(pos + 1) * 100 / _total).ToString("#0.##") + "%)";
						txtStatus.Refresh();
						Application.DoEvents();

						_strPos = str.IndexOf(orgStr, _strPos + 1, StringComparison.CurrentCultureIgnoreCase);
						if (_strPos >= 0)
						{
							_lastPos = pos;
							txtPreview.Text = str;
							txtPreview.Refresh();
							txtPreview.Select(_strPos, orgStr.Length);
							txtPreview.ScrollToCaret();
							break;
						}
					}
					if (_sr.Peek() < 0)
					{
						MessageBox.Show("더 이상 일치하는 문자열이 없습니다.\r\n다시 검색하시겠습니까?", "검색", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

						_searching = false;
						_lastPos = 0;
						_strPos = -1;
						cmdOpen.Enabled = true;
						cboEncoding.Enabled = true;
						txtOrgString.ReadOnly = false;
						cmdSearch.Text = "검색";
						cmdSearch.BackColor = Color.YellowGreen;
						cmdSearch.Width = 53;
						cmdSearch.Refresh();
						cmdStop.Enabled = false;
						cmdStop.BackColor = Color.LightGray;
					}
				}
				else // 역방향 검색
				{
					if (_lastPos < _total - 1)
					{
						string str = txtPreview.Text.Substring(0, _strPos);
						_strPos = str.LastIndexOf(orgStr, StringComparison.CurrentCultureIgnoreCase);
						if (_strPos >= 0)
						{
							txtStatus.Text = "검색 중...(" + ((double)(_lastPos - buffLen + _strPos + 1) * 100 / _total).ToString("#0.##") + "%)";
							txtStatus.Refresh();
							txtPreview.Select(_strPos, orgStr.Length);
							txtPreview.ScrollToCaret();
							return;
						}
					}

					while (_lastPos >= 0)
					{
						_lastPos -= buffLen;
						if (_lastPos < 0)
						{
							buffLen = (int)_lastPos + buffLen;
							_lastPos = 0;
						}
						_sr.BaseStream.Position = _lastPos;
						_sr.DiscardBufferedData();
						long pos = _lastPos;
						txtStatus.Text = "검색 중...(" + ((double)(pos + 1) * 100 / _total).ToString("#0.##") + "%)";
						txtStatus.Refresh();
						Application.DoEvents();

						int size = _sr.Read(_buffer, 0, buffLen);
						string str = new string(_buffer, 0, size);
						str += _sr.ReadLine(); // 줄 끝까지 읽는다
						_strPos = str.LastIndexOf(orgStr, StringComparison.CurrentCultureIgnoreCase);
						if (_strPos >= 0)
						{
							txtPreview.Text = str;
							txtPreview.Refresh();
							txtPreview.Select(_strPos, orgStr.Length);
							txtPreview.ScrollToCaret();
							break;
						}

						if (_lastPos == 0) _lastPos = -1;
					}

					if (_lastPos < 0)
					{
						MessageBox.Show("더 이상 일치하는 문자열이 없습니다.\r\n다시 검색하시겠습니까?", "검색", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

						_searching = false;
						_lastPos = _sr.BaseStream.Length - 1;
						_strPos = txtPreview.Text.Length;
						cmdOpen.Enabled = true;
						cboEncoding.Enabled = true;
						txtOrgString.ReadOnly = false;
						cmdSearch.Text = "검색";
						cmdSearch.BackColor = Color.YellowGreen;
						cmdSearch.Width = 53;
						cmdSearch.Refresh();
						cmdStop.Enabled = false;
						cmdStop.BackColor = Color.LightGray;
					}
				}
			}
			catch (Exception ex)
			{
				txtStatus.Text = "에러: " + ex.Message;
				_searching = false;
				_lastPos = -1;
				_strPos = -1;
				if (_sr != null)
				{
					_sr.Close();
					_sr = null;
				}
				cmdOpen.Enabled = true;
				cboEncoding.Enabled = true;
				txtOrgString.ReadOnly = false;
				cmdSearch.Text = "검색";
				cmdSearch.BackColor = Color.YellowGreen;
				cmdSearch.Width = 53;
				cmdSearch.Refresh();
				cmdStop.Enabled = false;
				cmdStop.BackColor = Color.LightGray;
				chkReverse.Enabled = false;
			}
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			if (_sr != null)
			{
				_sr.Close();
				_sr = null;
			}
			Application.Exit();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.Size = new Size(1024, 768);
			cboEncoding.SelectedIndex = 1;
			txtOrgString.ReadOnly = true;
			cmdSearch.Enabled = false;
			cmdSearch.BackColor = Color.LightGray;
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_sr != null)
			{
				_sr.Close();
				_sr = null;
			}
		}

		private void cmdStop_Click(object sender, EventArgs e)
		{
			_searching = false;
			_lastPos = -1;
			_strPos = -1;
			cmdOpen.Enabled = true;
			cboEncoding.Enabled = true;
			txtOrgString.ReadOnly = false;
			cmdSearch.Text = "검색";
			cmdSearch.BackColor = Color.YellowGreen;
			cmdSearch.Width = 53;
			cmdSearch.Refresh();
			cmdStop.Enabled = false;
			cmdStop.BackColor = Color.LightGray;
		}

		private void txtOrgString_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				cmdSearch_Click(null, null);
		}

		private void cmdNext_Click(object sender, EventArgs e)
		{
			bool isReverse = chkReverse.Checked;
			int buffLen = _buffer.Length;

			if (!isReverse)
			{
				if (_lastPos < 0) _lastPos = 0;
				_sr.BaseStream.Position = _lastPos;
				if (_sr.Peek() >= 0)
				{
					int size = _sr.Read(_buffer, 0, buffLen);
					string str = new string(_buffer, 0, size);
					if (size == buffLen) str += _sr.ReadLine(); // 줄 끝까지 읽는다
					_lastPos = _sr.BaseStream.Position;
					_strPos = -1;
					txtPreview.Text = str;
					txtPreview.Refresh();
					txtStatus.Text = "읽음...(" + ((double)(_lastPos + 1) * 100 / _total).ToString("#0.##") + "%)";
					txtStatus.Refresh();
				}
				else
				{
					MessageBox.Show("파일의 끝부분입니다. 계속하시면 다시 파일의 시작부터 읽습니다.\r\n계속하시겠습니까?", "건너뛰기", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

					_searching = false;
					_lastPos = 0;
					_strPos = -1;
					cmdOpen.Enabled = true;
					cboEncoding.Enabled = true;
					txtOrgString.ReadOnly = false;
					cmdSearch.Text = "검색";
					cmdSearch.BackColor = Color.YellowGreen;
					cmdSearch.Width = 53;
					cmdSearch.Refresh();
					cmdStop.Enabled = false;
					cmdStop.BackColor = Color.LightGray;
				}
			}
			else
			{
				if (_lastPos >= 0)
				{
					_lastPos -= buffLen;
					if (_lastPos < 0)
					{
						buffLen = (int)_lastPos + buffLen;
						_lastPos = 0;
					}
					_sr.BaseStream.Position = _lastPos;
					_sr.DiscardBufferedData();

					int size = _sr.Read(_buffer, 0, buffLen);
					string str = new string(_buffer, 0, size);
					str += _sr.ReadLine(); // 줄 끝까지 읽는다
					_strPos = str.Length - 1;
					txtPreview.Text = str;
					txtPreview.Refresh();
					txtStatus.Text = "읽음...(" + ((double)(_lastPos + 1) * 100 / _total).ToString("#0.##") + "%)";
					txtStatus.Refresh();

					if (_lastPos == 0) _lastPos = -1;
				}
				else
				{
					MessageBox.Show("파일의 시작부분입니다. 계속하시면 다시 파일의 끝부터 읽습니다.\r\n계속하시겠습니까?", "건너뛰기", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

					_searching = false;
					_lastPos = _sr.BaseStream.Length - 1;
					_strPos = txtPreview.Text.Length;
					cmdOpen.Enabled = true;
					cboEncoding.Enabled = true;
					txtOrgString.ReadOnly = false;
					cmdSearch.Text = "검색";
					cmdSearch.BackColor = Color.YellowGreen;
					cmdSearch.Width = 53;
					cmdSearch.Refresh();
					cmdStop.Enabled = false;
					cmdStop.BackColor = Color.LightGray;
				}
			}
		}

		private void chkReverse_CheckedChanged(object sender, EventArgs e)
		{
			if (!chkReverse.Checked)
			{
				if (_lastPos == _sr.BaseStream.Length - 1) _lastPos = 0;
				if (_strPos == txtPreview.Text.Length) _strPos = -1;
			}
			else
			{
				if (_lastPos == 0) _lastPos = _sr.BaseStream.Length - 1;
				if (_strPos == -1) _strPos = txtPreview.Text.Length;
			}
		}
	}
}
