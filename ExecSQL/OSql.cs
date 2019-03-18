using System;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace ExecSQL
{
	/// <summary>
	/// OSql에 대한 요약 설명입니다.
	/// </summary>
	class OSql
	{
		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if ((args.Length < 4) || (args.Length > 5))
			{
				PrintUsage();
				return;
			}

			Which which = new Which();
			try
			{
				if (which.IsExists("osql"))
				{
					ProcessStartInfo pi = new ProcessStartInfo(which.FirstFoundPath);
					pi.WorkingDirectory = Environment.CurrentDirectory;
					pi.CreateNoWindow = true;
					pi.WindowStyle = ProcessWindowStyle.Hidden;

					if (args.Length == 4)
						pi.Arguments = String.Format("-S {0} -U {1} -P \"{2}\" -d master -i \"{3}\"",
							args[0],
							args[1],
							args[2],
							args[3]);
					else if (args.Length == 5)
						pi.Arguments = String.Format("-S {0} -U {1} -P \"{2}\" -d {3} -i \"{4}\"",
							args[0],
							args[1],
							args[2],
							args[3],
							args[4]);

					Console.WriteLine(pi.FileName);
					Console.WriteLine(pi.Arguments);

					Process p = Process.Start(pi);
					p.WaitForExit();
				}
				else
				{
					string connection = "";
					string sqlText = "";
					if (args.Length == 4)
					{
						connection = String.Format("data source={0};user id={1};password={2};Initial Catalog=master;",
							args[0],
							args[1],
							args[2]);
						sqlText = getSql(args[3]);
					}
					else if (args.Length == 5)
					{
						connection = String.Format("data source={0};user id={1};password={2};Initial Catalog={3};",
							args[0],
							args[1],
							args[2],
							args[3]);
						sqlText = getSql(args[4]);
					}

					using(SqlConnection conn = new SqlConnection(connection))
					{
						conn.Open();
						using (SqlCommand comm = new SqlCommand())
						{
							comm.Connection = conn;
							comm.CommandType = CommandType.Text;
							comm.CommandTimeout = 120;

							int pos1 = 0;
							int pos2 = sqlText.IndexOf("GO");
							while (pos2 > 0)
							{
								comm.CommandText = sqlText.Substring(pos1, pos2 - pos1);
								comm.ExecuteNonQuery();

								pos1 = pos2 + 4;
								pos2 = sqlText.IndexOf("GO", pos1);
							}

							string lastToken = sqlText.Substring(pos1);
							if ((lastToken != null) && (lastToken.Length > 0))
							{
								comm.CommandText = lastToken;
								comm.ExecuteNonQuery();
							}
						}
						conn.Close();
					}
				}
				Console.WriteLine("ExecSQL is executed successfully!");
			}
			catch(Exception e)
			{
				Console.WriteLine("Error!"+"\r\n"+e.Message+"\r\n"+e.StackTrace);
			}
		}

		private static string getSql(string sqlfile)
		{
			StreamReader reader = new StreamReader(sqlfile, Encoding.Default);
			StringBuilder sb = new StringBuilder();
			string text = reader.ReadLine();
			while (text != null)
			{
				text = text.Trim();
				if (text.Equals("go")) text = text.ToUpper();

				sb.Append(text);
				sb.Append("\r\n");
				text = reader.ReadLine();
			}
			reader.Close();
			return sb.ToString();
		}

		/// <summary>
		/// Print the description of the program.
		/// Print the usage string.
		/// Provide version and author info.
		/// </summary>
		private static void PrintUsage()
		{
			Console.WriteLine(
				"Description: ExecSQL searches for an 'osql' executable in all PATH locations and execute it.\r\n" +
				"             If there is no 'osql', internal SQL Engine will execute the given SQL statement." +
				"\r\n\r\n"+
				"ExecSQL <database server name> <user> <password> [default database] <full path of SQL file>"+
				"\r\n\r\n" + "Eg. ExecSQL guss user1 password1 northwind c:\\data.sql" + 
				"\r\n\r\n"+
				"Version: 1.0" +
				"\r\n"+
				"Written by Thermidor(thermidor@nets.co.kr)"
				);
		}
	}
}
