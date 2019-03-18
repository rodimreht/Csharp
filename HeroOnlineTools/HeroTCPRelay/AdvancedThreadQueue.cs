using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace HeroTCPRelay
{
	/// <summary>
	/// ������ ����ü (copy from NETS*IM 2.6)
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ThreadInfo
	{
		/// <summary>
		/// �����忡�� ���Ǵ� ������
		/// </summary>
		public object ThreadData;
		/// <summary>
		/// ������
		/// </summary>
		public Thread ThreadRef;
		/// <summary>
		/// ������ ID
		/// </summary>
		public int ID;
		/// <summary>
		/// ��� ������� �۵��� �������� ����
		/// </summary>
		public bool IsBackground;
		/// <summary>
		/// �����尡 ���� ������������ ����
		/// </summary>
		public bool IsRunning;
		/// <summary>
		/// �����尡 ����Ǿ������� ����
		/// </summary>
		public bool WasRunning;
	}
	
	/// <summary>
	/// ������ ť�� �߰��Ǵ� ���� �ٷιٷ� �����ϰų�,<br/>
	/// ������ �����带 ��� ť�� ������ �� ������ �� �ִ� ������ ť;<br/>
	/// ��� ��������� ���ÿ� ����Ǹ� ���(Background) ������� ������ ���� �ִ�.
	/// </summary>
	public class AdvancedThreadQueue
	{
		// Fields
		private int maxThreads;
		private ArrayList queues;

		private Thread stubThread;

		/// <summary>
		/// �ִ� ������ ��
		/// </summary>
		public int MaxThreads 
		{
			get { return maxThreads; }
			set { maxThreads = value; }
		}

		/// <summary>
		/// AdvancedThreadQueue ������
		/// </summary>
		/// <param name="Limit">�ִ� ������ ��</param>
		public AdvancedThreadQueue(int Limit)
		{
			maxThreads = Limit;
			queues = new ArrayList();
			this.stubThread = new Thread(new ThreadStart(threadMain));
			this.stubThread.Start();
		}

		/// <summary>
		/// �ڵ� ���� ���θ� ������ �� �ִ� AdvancedThreadQueue ������
		/// </summary>
		/// <param name="Limit">�ִ� ������ ��</param>
		/// <param name="autoStart">�ڵ� ���� ����</param>
		public AdvancedThreadQueue(int Limit, bool autoStart)
		{
			maxThreads = Limit;
			queues = new ArrayList();
			this.stubThread = new Thread(new ThreadStart(threadMain));
			if (autoStart)
				this.stubThread.Start();
		}

		/// <summary>
		/// �ڵ� ������ ���� ���¿��� ��ü �����带 ��������� �����Ѵ�.
		/// </summary>
		public void Start()
		{
			if (!this.stubThread.IsAlive)
				this.stubThread.Start();
		}

		/// <summary>
		/// �����Ͱ� ���� �� �����带 �߰��Ѵ�.
		/// </summary>
		/// <param name="thread">������</param>
		public void Add(Thread thread)
		{
			Add(thread, null);
		}

		/// <summary>
		/// �� �����带 �߰��Ѵ�.
		/// </summary>
		/// <param name="thread">������</param>
		/// <param name="data">�����忡�� ����� ������</param>
		public void Add(Thread thread, object data)
		{
			Add(thread, false, data);
		}

		/// <summary>
		/// ��� ������� ������ �������� ���θ� �����Ͽ� �� �����带 �߰��Ѵ�.
		/// </summary>
		/// <param name="thread">������</param>
		/// <param name="isBackground">��� ������� ������ �������� ����</param>
		/// <param name="data">�����忡�� ����� ������</param>
		public void Add(Thread thread, bool isBackground, object data)
		{
			int newID = 0;

			lock (queues)
			{
				ThreadInfo newInfo = new ThreadInfo();
				newInfo.ThreadData = data;
				newInfo.ThreadRef = thread;
				for (int queueCount = 0; queueCount < queues.Count; queueCount++)
				{
					ThreadInfo info = (ThreadInfo) queues[queueCount];
					if (info.ID >= newID)
						newID = info.ID;
				}

				newID++;
				newInfo.ID = newID;
				newInfo.IsBackground = isBackground;
				newInfo.IsRunning = false;
				newInfo.WasRunning = false;
				queues.Add(newInfo);
			}
		}

		/// <summary>
		/// �ش� �����忡 �߰��� �����͸� ��´�.
		/// </summary>
		/// <param name="thread"></param>
		/// <returns></returns>
		public object GetData(Thread thread)
		{
			lock (queues)
			{
				int totalCount = queues.Count;
				for (int queueCount = 0; queueCount < totalCount; queueCount++)
				{
					ThreadInfo info = (ThreadInfo) queues[queueCount];
					if (info.ThreadRef.Equals(thread))
						return info.ThreadData;
				}
			}
			return null;
		}

		/// <summary>
		/// ������ ť�� ��������� ����
		/// </summary>
		/// <returns></returns>
		public bool IsQueueEmpty()
		{
			if (queues.Count != 0)
				return false;
			else
				return true;
		}

		// Stub ������ ����
		private void threadMain()
		{
			int errCount = 0;

			while (true)
			{
				try
				{
					int runningCount = 0;
					lock (queues)
					{
						int totalCount = queues.Count;
						for (int queueCount = 0; queueCount < totalCount; queueCount++)
						{
							ThreadInfo info = (ThreadInfo) queues[queueCount];
							if ((info.ThreadRef.ThreadState != ThreadState.Running) &&
							    (info.ThreadRef.ThreadState != ThreadState.Background) &&
							    (info.ThreadRef.ThreadState != ThreadState.WaitSleepJoin) &&
							    (info.ThreadRef.ThreadState != (ThreadState.WaitSleepJoin | ThreadState.Background)))
							{
								info.IsRunning = false;
								queues[queueCount] = info;
							}

							if (info.IsRunning)
								runningCount++;
						}
					}

					lock (queues)
					{
						// �߰��� �����尡 ������
						int totalCount = queues.Count;
						for (int queueCount = 0; queueCount < totalCount; queueCount++)
						{
							ThreadInfo info = (ThreadInfo) queues[queueCount];
							if (!info.IsRunning && !info.WasRunning) // �߰� �� ������� �����常 ����
							{
								if (runningCount >= maxThreads) break;

								info.ThreadRef.IsBackground = info.IsBackground;
								info.ThreadRef.Start();

								info.IsRunning = true;
								info.WasRunning = true;

								queues[queueCount] = info;
								runningCount++;
							}
						}
					}

					lock (queues)
					{
						// ������ ����� ������ ����
						for (int queueCount = 0; queueCount < queues.Count; queueCount++)
						{
							ThreadInfo info = (ThreadInfo) queues[queueCount];
							if (!info.IsRunning && info.WasRunning)
							{
								// RemoveAt�ϸ� Count�� 1 �����ϹǷ� �ش� ��ġ�� �����带 �ٽ� üũ�ϱ� ���� 1 ����
								queues.RemoveAt(queueCount--);
							}
						}
					}

					errCount = 0;
				}
				catch (Exception ex)
				{
					// ���� ������ 10ȸ�̻� ������� �ʴ´�.
					if (errCount < 10)
						Logger.Log(LogLevel.ERROR, "AdvancedThreadQueue", ex.ToString());

					errCount++;
				}

				// 0.1�� ������ ���� / üũ �ݺ�
				Thread.Sleep(100);
			}
		}

		/// <summary>
		/// ��ü ������ ó���� �����Ѵ�.
		/// </summary>
		public void Stop()
		{
			this.stubThread.Abort();
		}

		/// <summary>
		/// Ư�� ������ ó���� ������ �����Ѵ�.
		/// </summary>
		/// <param name="name">The name.</param>
		public void Dequeue(string name)
		{
			try
			{
				lock (queues)
				{
					for (int queueCount = 0; queueCount < queues.Count; queueCount++)
					{
						ThreadInfo info = (ThreadInfo)queues[queueCount];
						if ((info.ThreadRef.Name != null) && info.ThreadRef.Name.Equals(name))
						{
							if (info.IsRunning) // �������̸� �����Ѵ�.
							{
								info.ThreadRef.Abort();
								queues[queueCount] = info;
							}
							else if (!info.WasRunning) // ���� ���� ���̸� �����Ѵ�.
							{
								queues.RemoveAt(queueCount);
							}
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log(LogLevel.WARNING, "AdvancedThreadQueue", ex.ToString());
			}
		}

		/// <summary>
		/// ��� ������ ó���� ������ �����Ѵ�.
		/// </summary>
		public void DequeueAll()
		{
			while ((queues != null) && (queues.Count > 0))
			{
				try
				{
					lock (queues)
					{
						for (int queueCount = 0; queueCount < queues.Count; queueCount++)
						{
							ThreadInfo info = (ThreadInfo) queues[queueCount];
							if (info.IsRunning && (info.ThreadRef != null)) // �������̸� �����Ѵ�.
							{
								info.ThreadRef.Abort();
								queues[queueCount] = info;
							}
							else if (!info.IsRunning && !info.WasRunning) // ���� ���� ���̸� �����Ѵ�.
							{
								queues.RemoveAt(queueCount--);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Log(LogLevel.WARNING, "AdvancedThreadQueue", ex.ToString());
				}
				Thread.Sleep(200);
			}
		}

		/// <summary>
		/// ��� ������ ó���� ���� ������ ����Ѵ�.
		/// </summary>
		public void WaitForAllThreadsToComplete()
		{
			while (!this.IsQueueEmpty())
				Thread.Sleep(50);
		}

		/// <summary>
		/// ��� ������ ó���� ���� ������ ������ �ð����� ����Ѵ�.
		/// </summary>
		public void WaitForAllThreadsToComplete(long millisec)
		{
			DateTime time1 = DateTime.Now.AddMilliseconds(millisec);
			while (!this.IsQueueEmpty() && (time1.CompareTo(DateTime.Now) > 0))
				Thread.Sleep(50);
		}
	}
}
