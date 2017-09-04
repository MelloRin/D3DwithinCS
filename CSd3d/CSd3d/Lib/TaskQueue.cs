using System;
using System.Threading;

using DataSet = MelloRin.FileManager.DataSet;

namespace MelloRin.CSd3d.Lib
{
	interface task
	{
		bool run();
	}

	class TaskDataSet
	{
		DataSet dataSet;



	}
	
	class TaskDataSetBuilder
	{

		public void addDataSet(DataSet dataSet) => this.dataSet = dataSet;

		public TaskDataSet getTaskDataSet() => new TaskDataSet()
	}


	class TaskQueue<E> : task
	{
		private QueueData<E> head = null;
		private uint taskInterval;
		
		public TaskQueue(uint interval = 5)
		{
			taskInterval = interval;
		}

		private void mainLoop()
		{
			while(true)
			{
				if(head != null)
				{
					((task)getTesk()).run();
				}

				Thread.Sleep((int)taskInterval);
			}
		}

		public bool run()
		{
			Thread _TmainThread = new Thread(mainLoop);

			_TmainThread.Start();

			return true;
		}

		public int length()
		{
			int count = 0;
			QueueData<E> temp = head;
			if (head != null)
			{
				count++;
				while (temp.getNext() != null)
				{
					count++;
					temp = temp.getNext();
				}
			}
			return count;
		}

		public void addTask(E data)
		{
			if (head == null)
			{
				head = new QueueData<E>(data);
			}
			else
			{
				QueueData<E> temp = head;
				while (temp.getNext() != null)
				{
					temp = temp.getNext();
				}
				temp.setNext(new QueueData<E>(data));
			}
		}

		private E getTesk()
		{
			if (head != null)
			{
				QueueData<E> temp = head;
				head = head.getNext();
				return temp.getData();
			}
			else
			{
				Console.WriteLine("no data");
				return default(E);
			}
		}

		class QueueData<e>
		{
			private QueueData<e> nextData = null;
			public e data;

			public QueueData(e data) => this.data = data;

			public QueueData<e> getNext() => nextData;
			public void setNext(QueueData<e> next) => nextData = next;
			public e getData() => data;
		}
	}
}