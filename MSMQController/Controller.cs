using System;
using System.Messaging;

namespace MSMQController
{
	/// <summary>
	/// Controller에 대한 요약 설명입니다.
	/// </summary>
	class Controller
	{
		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if ((args.Length < 1) || (args.Length > 3))
			{
				PrintUsage();
				return;
			}

			string option = "";
			string mqPath = "";
			bool isAllowedToEveryone = false;

			if (args[0].StartsWith("/"))
			{
				option = args[0].Substring(1, 1).ToLower();
				mqPath = args[1];
				if ((args.Length == 3) && (args[2].ToLower().Equals("everyone")))
					isAllowedToEveryone = true;
			}
			else
			{
				option = "c";
				mqPath = args[0];
				if ((args.Length == 2) && (args[1].ToLower().Equals("everyone")))
					isAllowedToEveryone = true;
			}

			//--- only for local computer
			//string mqPath = ".\\private$\\ssomq";

			//-- it requires transaction
			//string mqPath = "FormatName:Direct=OS:THERMY2002\\Private$\\ssomq";
			
			//-- it requires no-transaction
			//string mqPath = "FormatName:Direct=OS:THERMY2002\\Private$\\ssomq_5";

			//--- only for inner-domain computer
			//string mqPath = "FormatName:Public=9BDFDAA9-9DCB-4CEA-B0B9-EB55501715A6";

			if (option.Equals("c"))
				createQueue(mqPath, isAllowedToEveryone);
			else if (option.Equals("r"))
				removeQueue(mqPath);
			else
				PrintUsage();
		}

		private static void createQueue(string mqPath, bool isAllowedToEveryone)
		{
			if (!MessageQueue.Exists(mqPath)) 
			{
				try
				{
					using (MessageQueue queue = MessageQueue.Create(mqPath, true))
					{
						AccessControlList acl = new AccessControlList();
						Trustee trustee = new Trustee("Administrators");
						MessageQueueAccessControlEntry ace = new MessageQueueAccessControlEntry(trustee, MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
						acl.Add(ace);

						if (isAllowedToEveryone)
						{
							trustee = new Trustee("Everyone");
							ace = new MessageQueueAccessControlEntry(trustee, MessageQueueAccessRights.ReceiveMessage, AccessControlEntryType.Allow);
							acl.Add(ace);
						}
						queue.SetPermissions(acl);
						queue.Close();
					}
				}
				catch(MessageQueueException E)
				{
					Console.WriteLine(E.MessageQueueErrorCode);
				}
			}
			else
			{
				try
				{
					using (MessageQueue queue = new MessageQueue(mqPath))
					{
						AccessControlList acl = new AccessControlList();
						Trustee trustee = new Trustee("Administrators");
						MessageQueueAccessControlEntry ace = new MessageQueueAccessControlEntry(trustee, MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
						acl.Add(ace);

						if (isAllowedToEveryone)
						{
							trustee = new Trustee("Everyone");
							ace = new MessageQueueAccessControlEntry(trustee, MessageQueueAccessRights.ReceiveMessage, AccessControlEntryType.Allow);
							acl.Add(ace);
						}
						queue.SetPermissions(acl);
						queue.Close();
					}
				}
				catch(MessageQueueException E)
				{
					Console.WriteLine(E.MessageQueueErrorCode);
				}
			}
		}

		private static void removeQueue(string mqPath)
		{
			if (MessageQueue.Exists(mqPath)) 
			{
				try
				{
					MessageQueue.Delete(mqPath);
				}
				catch(MessageQueueException E)
				{
					Console.WriteLine(E.MessageQueueErrorCode);
				}
			}
			else
			{
				Console.WriteLine(mqPath + " queue does not exist.");
			}
		}

		/// <summary>
		/// Print the description of the program.
		/// Print the usage string.
		/// Provide version and author info.
		/// </summary>
		private static void PrintUsage()
		{
			Console.WriteLine(
				"Description: MSMQController creates or removes a transactional message queue.\r\n" +
				"\r\n\r\n"+
				"MSMQController [/C[reate] | /R[emove]] <queue name> [\"everyone\"]"+
				"\r\n\r\n" + "Eg. MSMQController sso_mq: default to creation" + 
				"\r\n\r\n"+
				"Version: 1.0" +
				"\r\n"+
				"Written by Thermidor(thermidor@nets.co.kr)"
				);
		}
	}
}
