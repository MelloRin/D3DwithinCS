using System;

namespace MelloRin.CSd3d.Lib
{
	public interface Itask
	{
		bool run(Itask nextTask = null);
	}

	public class TaskQueue
	{
		private QueueData head = null;
		public uint taskInterval { get; }

		public QueueData getHead => head;

		public TaskQueue(uint interval = 5)
		{
			taskInterval = interval;
		}

		public int length()
		{
			int count = 0;
			QueueData temp = head;
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

		public void addTask(Itask data)
		{
			if (head == null)
			{
				head = new QueueData(data);

				Itask nowTask = head.getTask();

				try
				{
					nowTask.run(head.getNext().getTask());
				}
				catch(NullReferenceException)
				{
					nowTask.run();
				}

				head = head.getNext();
			}
			else
			{
				QueueData temp = head;
				while (temp.getNext() != null)
				{
					temp = temp.getNext();
				}
				temp.setNext(new QueueData(data));
			}
		}

		public class QueueData
		{
			private QueueData nextData = null;
			private Itask task;

			public QueueData(Itask task) => this.task = task;

			public QueueData getNext() => nextData;
			public void setNext(QueueData next) => nextData = next;

			public Itask getTask() => task;
		}
	}
}