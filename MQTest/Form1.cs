using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Messaging;

namespace MQTest
{
	/// <summary>
	/// Form1에 대한 요약 설명입니다.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button btnReceive;
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Windows Form 디자이너 지원에 필요합니다.
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
			//
		}

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.btnReceive = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(112, 32);
			this.button1.Name = "button1";
			this.button1.TabIndex = 0;
			this.button1.Text = "Send";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnReceive
			// 
			this.btnReceive.Location = new System.Drawing.Point(112, 72);
			this.btnReceive.Name = "btnReceive";
			this.btnReceive.TabIndex = 1;
			this.btnReceive.Text = "Receive";
			this.btnReceive.Click += new System.EventHandler(this.btnReceive_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(292, 133);
			this.Controls.Add(this.btnReceive);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			string sMessage = "";

			//--- only for local computer
			//string mqPath = ".\\private$\\ssomq";

			/*--- check and creation are only for local computer
			 * 
			if(!MessageQueue.Exists(mqPath)) 
			{
				try
				{
					MessageQueue.Create(mqPath);
				}
				catch(MessageQueueException E)
				{
					Console.WriteLine(E.MessageQueueErrorCode);
				}
			}
			*/

			//-- available for all environments
			//-- it requires transaction
			//string mqPath = "FormatName:Direct=OS:THERMY2002\\Private$\\ssomq";
			
			//-- it requires no-transaction
			string mqPath = "FormatName:Direct=OS:THERMY2002\\Private$\\ssomq_5";

			//--- only for inner-domain computer
			//string mqPath = "FormatName:Public=9BDFDAA9-9DCB-4CEA-B0B9-EB55501715A6";

			try
			{
				MessageQueue mq = new MessageQueue(mqPath);
				/*
				if (mq.Transactional)	// only for Path-Name
				{
					MessageQueueTransaction mqTran = new MessageQueueTransaction();
					mqTran.Begin();

					mq.Formatter = new ActiveXMessageFormatter();
					mq.Send("This is test string...", "test", mqTran);

					mqTran.Commit();
				}
				else
				*/
				{
					mq.Formatter = new ActiveXMessageFormatter();
					mq.Send("This is test string...");
				}
			}
			catch (ArgumentException aex)
			{
				sMessage = aex.ToString() + " " + aex.Message;
			}
			catch (MessageQueueException mex)
			{
				sMessage = mex.ToString() + " " + mex.Message;
			}
			catch (InvalidOperationException iex)
			{
				sMessage = iex.ToString() + " " + iex.Message;
			}
			finally
			{
				if (sMessage.Length > 0)
				{
					MessageBox.Show("Error: " + sMessage);
				}
				else
				{
					MessageBox.Show("Succeeded.");
				}
			}
		}

		private void btnReceive_Click(object sender, System.EventArgs e)
		{
			string sMessage = "";
			
			//-- it requires no-transaction
			string mqPath = "FormatName:Direct=OS:win2003ent\\Private$\\NETSPwdSync";
			//string mqPath = "FormatName:Direct=OS:.\\Private$\\NETSPwdSync";

			try
			{
				MessageQueue mq = new MessageQueue(mqPath);
				//MessageQueueTransaction mqTran = new MessageQueueTransaction();
				//mqTran.Begin();

				mq.Formatter = new ActiveXMessageFormatter();
				System.Messaging.Message msg = mq.Receive();

				Stream stream = msg.BodyStream;
				byte[] buffer = new byte[stream.Length];
				stream.Read(buffer, 0, (int)stream.Length);
				MessageBox.Show(System.Text.Encoding.Unicode.GetString(buffer));

				//mqTran.Commit();
			}
			catch (ArgumentException aex)
			{
				sMessage = aex.ToString() + " " + aex.Message;
			}
			catch (MessageQueueException mex)
			{
				sMessage = mex.ToString() + " " + mex.Message;
			}
			catch (InvalidOperationException iex)
			{
				sMessage = iex.ToString() + " " + iex.Message;
			}
			finally
			{
				if (sMessage.Length > 0)
				{
					MessageBox.Show("Error: " + sMessage);
				}
				else
				{
					MessageBox.Show("Succeeded.");
				}
			}
		}
	}
}
