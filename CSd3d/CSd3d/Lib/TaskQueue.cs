namespace MelloRin.CSd3d.Lib
{
	public interface Itask
	{
		void run();
	}

	public class TaskQueue
	{
		private QueueData head = null;
		public QueueData getHead => head;

		private bool running = false;

		public uint taskInterval { get; }

		public TaskQueue(uint interval = 5)
		{
			taskInterval = interval;
		}

		public void addTask(Itask data)
		{
			if (head == null)
			{
				if (running)
					while (!running) ;

				running = true;
				head = new QueueData(data);

				QueueData temp = head;
				head = temp.getNext();

				temp.task.run();
				running = false;
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

		public void runNext()
		{
			if (head != null)
			{
				running = true;

				QueueData temp = head;
				head = temp.getNext();

				temp.task.run();
				running = false;
			}
		}

		public class QueueData
		{
			private QueueData nextData = null;
			public Itask task { get; }

			public QueueData(Itask task) => this.task = task;

			public QueueData getNext() => nextData;
			public void setNext(QueueData next) => nextData = next;
		}
	}
}