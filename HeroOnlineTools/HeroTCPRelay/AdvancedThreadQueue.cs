using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace HeroTCPRelay
{
	/// <summary>
	/// 스레드 구조체 (copy from NETS*IM 2.6)
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ThreadInfo
	{
		/// <summary>
		/// 스레드에서 사용되는 데이터
		/// </summary>
		public object ThreadData;
		/// <summary>
		/// 스레드
		/// </summary>
		public Thread ThreadRef;
		/// <summary>
		/// 스레드 ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 배경 스레드로 작동될 것인지의 여부
		/// </summary>
		public bool IsBackground;
		/// <summary>
		/// 스레드가 현재 실행중인지의 여부
		/// </summary>
		public bool IsRunning;
		/// <summary>
		/// 스레드가 실행되었는지의 여부
		/// </summary>
		public bool WasRunning;
	}
	
	/// <summary>
	/// 스레드 큐에 추가되는 순간 바로바로 실행하거나,<br/>
	/// 실행할 스레드를 모두 큐에 적재한 후 실행할 수 있는 스레드 큐;<br/>
	/// 대상 스레드들은 동시에 실행되며 배경(Background) 스레드로 지정할 수도 있다.
	/// </summary>
	public class AdvancedThreadQueue
	{
		// Fields
		private int maxThreads;
		private ArrayList queues;

		private Thread stubThread;

		/// <summary>
		/// 최대 스레드 수
		/// </summary>
		public int MaxThreads 
		{
			get { return maxThreads; }
			set { maxThreads = value; }
		}

		/// <summary>
		/// AdvancedThreadQueue 생성자
		/// </summary>
		/// <param name="Limit">최대 스레드 수</param>
		public AdvancedThreadQueue(int Limit)
		{
			maxThreads = Limit;
			queues = new ArrayList();
			this.stubThread = new Thread(new ThreadStart(threadMain));
			this.stubThread.Start();
		}

		/// <summary>
		/// 자동 실행 여부를 설정할 수 있는 AdvancedThreadQueue 생성자
		/// </summary>
		/// <param name="Limit">최대 스레드 수</param>
		/// <param name="autoStart">자동 실행 여부</param>
		public AdvancedThreadQueue(int Limit, bool autoStart)
		{
			maxThreads = Limit;
			queues = new ArrayList();
			this.stubThread = new Thread(new ThreadStart(threadMain));
			if (autoStart)
				this.stubThread.Start();
		}

		/// <summary>
		/// 자동 실행이 꺼진 상태에서 전체 스레드를 명시적으로 실행한다.
		/// </summary>
		public void Start()
		{
			if (!this.stubThread.IsAlive)
				this.stubThread.Start();
		}

		/// <summary>
		/// 데이터가 없는 새 스레드를 추가한다.
		/// </summary>
		/// <param name="thread">스레드</param>
		public void Add(Thread thread)
		{
			Add(thread, null);
		}

		/// <summary>
		/// 새 스레드를 추가한다.
		/// </summary>
		/// <param name="thread">스레드</param>
		/// <param name="data">스레드에서 사용할 데이터</param>
		public void Add(Thread thread, object data)
		{
			Add(thread, false, data);
		}

		/// <summary>
		/// 배경 스레드로 동작할 것인지의 여부를 지정하여 새 스레드를 추가한다.
		/// </summary>
		/// <param name="thread">스레드</param>
		/// <param name="isBackground">배경 스레드로 동작할 것인지의 여부</param>
		/// <param name="data">스레드에서 사용할 데이터</param>
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
		/// 해당 스레드에 추가한 데이터를 얻는다.
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
		/// 스레드 큐가 비었는지의 여부
		/// </summary>
		/// <returns></returns>
		public bool IsQueueEmpty()
		{
			if (queues.Count != 0)
				return false;
			else
				return true;
		}

		// Stub 스레드 메인
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
						// 추가된 스레드가 있으면
						int totalCount = queues.Count;
						for (int queueCount = 0; queueCount < totalCount; queueCount++)
						{
							ThreadInfo info = (ThreadInfo) queues[queueCount];
							if (!info.IsRunning && !info.WasRunning) // 추가 후 대기중인 스레드만 실행
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
						// 실행이 종료된 스레드 제거
						for (int queueCount = 0; queueCount < queues.Count; queueCount++)
						{
							ThreadInfo info = (ThreadInfo) queues[queueCount];
							if (!info.IsRunning && info.WasRunning)
							{
								// RemoveAt하면 Count가 1 감소하므로 해당 위치의 스레드를 다시 체크하기 위해 1 감소
								queues.RemoveAt(queueCount--);
							}
						}
					}

					errCount = 0;
				}
				catch (Exception ex)
				{
					// 같은 에러는 10회이상 기록하지 않는다.
					if (errCount < 10)
						Logger.Log(LogLevel.ERROR, "AdvancedThreadQueue", ex.ToString());

					errCount++;
				}

				// 0.1초 단위로 실행 / 체크 반복
				Thread.Sleep(100);
			}
		}

		/// <summary>
		/// 전체 스레드 처리를 중지한다.
		/// </summary>
		public void Stop()
		{
			this.stubThread.Abort();
		}

		/// <summary>
		/// 특정 스레드 처리를 강제로 중지한다.
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
							if (info.IsRunning) // 실행중이면 종료한다.
							{
								info.ThreadRef.Abort();
								queues[queueCount] = info;
							}
							else if (!info.WasRunning) // 아직 실행 전이면 제거한다.
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
		/// 모든 스레드 처리를 강제로 중지한다.
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
							if (info.IsRunning && (info.ThreadRef != null)) // 실행중이면 종료한다.
							{
								info.ThreadRef.Abort();
								queues[queueCount] = info;
							}
							else if (!info.IsRunning && !info.WasRunning) // 아직 실행 전이면 제거한다.
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
		/// 모든 스레드 처리가 끝날 때까지 대기한다.
		/// </summary>
		public void WaitForAllThreadsToComplete()
		{
			while (!this.IsQueueEmpty())
				Thread.Sleep(50);
		}

		/// <summary>
		/// 모든 스레드 처리가 끝날 때까지 지정한 시간동안 대기한다.
		/// </summary>
		public void WaitForAllThreadsToComplete(long millisec)
		{
			DateTime time1 = DateTime.Now.AddMilliseconds(millisec);
			while (!this.IsQueueEmpty() && (time1.CompareTo(DateTime.Now) > 0))
				Thread.Sleep(50);
		}
	}
}
