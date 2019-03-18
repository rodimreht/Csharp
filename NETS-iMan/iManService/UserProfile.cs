using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace iManService
{
	/// <summary>
	/// UserProfile에 대한 요약 설명입니다.
	/// </summary>
	public class UserProfile
	{
		private static readonly string CONN_STRING;

		static UserProfile()
		{
			try
			{
				CONN_STRING = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
			}
			catch (Exception)
			{
				CONN_STRING = "Data Source=(local);Initial Catalog=NetsUserProfile;User ID=sa;Password=sosemfdksk?;Connect Timeout=12";
			}
		}

		public static DataSet GetAllLoginStatus()
		{
			DataSet ds = null;
			try
			{
				using (SqlConnection conn = new SqlConnection(CONN_STRING))
				{
					string selectCommand = String.Format("SELECT cn, loginStatus, loginIP FROM BaseInfo WITH (NOLOCK)");
					using (SqlDataAdapter adapter = new SqlDataAdapter(selectCommand, conn))
					{
						ds = new DataSet();
						adapter.Fill(ds, "LoginStatus");
					}
					conn.Close();
				}
			}
			catch (Exception)
			{
			}
			return ds;
		}

		public static int GetLoginStatus(string cn)
		{
			int result = 0;
			try
			{
				using (SqlConnection conn = new SqlConnection(CONN_STRING))
				{
					string selectCommand = String.Format("SELECT loginStatus FROM BaseInfo WITH (NOLOCK) WHERE cn='{0}'", cn);
					using (SqlCommand comm = new SqlCommand(selectCommand, conn))
					{
						conn.Open();

						result = (int)comm.ExecuteScalar();
					}
					conn.Close();
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		private static bool isExistRecordinBaseInfo(SqlConnection conn, SqlTransaction tran, string cn)
		{
			bool bRet = false;
			try
			{
				string selectCommand = String.Format("SELECT COUNT(cn) FROM BaseInfo WHERE cn='{0}'", cn);
				using (SqlCommand comm = new SqlCommand(selectCommand, conn, tran))
				{
					int result = (int)comm.ExecuteScalar();
					if (result >= 1)
						bRet = true;
				}
			}
			catch (Exception)
			{
				bRet = false;
			}

			return bRet;
		}

		public static bool Login(string cn, string ipAddr)
		{
			bool bRet;
			try
			{
				using (SqlConnection conn = new SqlConnection(CONN_STRING))
				{
					conn.Open();

					using (SqlTransaction tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
					{
						string commandSql = isExistRecordinBaseInfo(conn, tran, cn)
										? String.Format("UPDATE BaseInfo SET loginStatus=1, loginIP='{1}' WHERE cn='{0}'", cn, ipAddr)
										: String.Format("INSERT BaseInfo(cn, loginStatus, loginIP) VALUES('{0}', 1, '{1}')", cn, ipAddr);

						using (SqlCommand comm = new SqlCommand(commandSql, conn, tran))
						{
							try
							{
								comm.ExecuteNonQuery();
								tran.Commit();
								bRet = true;
							}
							catch (SqlException)
							{
								tran.Rollback();
								bRet = false;
							}
						}
					}
					conn.Close();
				}
			}
			catch (Exception)
			{
				bRet = false;
			}

			return bRet;
		}

		public static bool Logout(string cn)
		{
			bool bRet;
			try
			{
				using (SqlConnection conn = new SqlConnection(CONN_STRING))
				{
					conn.Open();

					using (SqlTransaction tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
					{
						string commandSql = isExistRecordinBaseInfo(conn, tran, cn)
										? String.Format("UPDATE BaseInfo SET loginStatus=0 WHERE cn='{0}'", cn)
										: String.Format("INSERT BaseInfo(cn, loginStatus) VALUES('{0}', 0)", cn);

						using (SqlCommand comm = new SqlCommand(commandSql, conn, tran))
						{
							try
							{
								comm.ExecuteNonQuery();
								tran.Commit();
								bRet = true;
							}
							catch (SqlException)
							{
								tran.Rollback();
								bRet = false;
							}
						}
					}
					conn.Close();
				}
			}
			catch (Exception)
			{
				bRet = false;
			}

			return bRet;
		}

		public static bool Absent(string cn)
		{
			bool bRet;
			try
			{
				using (SqlConnection conn = new SqlConnection(CONN_STRING))
				{
					conn.Open();

					using (SqlTransaction tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
					{
						string commandSql = isExistRecordinBaseInfo(conn, tran, cn)
										? String.Format("UPDATE BaseInfo SET loginStatus=9 WHERE cn='{0}'", cn)
										: String.Format("INSERT BaseInfo(cn, loginStatus) VALUES('{0}', 9)", cn);

						using (SqlCommand comm = new SqlCommand(commandSql, conn, tran))
						{
							try
							{
								comm.ExecuteNonQuery();
								tran.Commit();
								bRet = true;
							}
							catch (SqlException)
							{
								tran.Rollback();
								bRet = false;
							}
						}
					}
					conn.Close();
				}
			}
			catch (Exception)
			{
				bRet = false;
			}

			return bRet;
		}

		public static bool Busy(string cn)
		{
			bool bRet;
			try
			{
				using (SqlConnection conn = new SqlConnection(CONN_STRING))
				{
					conn.Open();

					using (SqlTransaction tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
					{
						string commandSql = isExistRecordinBaseInfo(conn, tran, cn)
										? String.Format("UPDATE BaseInfo SET loginStatus=2 WHERE cn='{0}'", cn)
										: String.Format("INSERT BaseInfo(cn, loginStatus) VALUES('{0}', 2)", cn);

						using (SqlCommand comm = new SqlCommand(commandSql, conn, tran))
						{
							try
							{
								comm.ExecuteNonQuery();
								tran.Commit();
								bRet = true;
							}
							catch (SqlException)
							{
								tran.Rollback();
								bRet = false;
							}
						}
					}
					conn.Close();
				}
			}
			catch (Exception)
			{
				bRet = false;
			}

			return bRet;
		}
	}
}
