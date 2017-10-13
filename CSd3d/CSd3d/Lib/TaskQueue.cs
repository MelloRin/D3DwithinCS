namespace MelloRin.CSd3d.Lib
{
	public interface ITask
	{
		void run();
	}

	public class TaskQueue
	{
		private QueueData head = null;
		private bool running = false;

		private class QueueData
		{
			private QueueData nextData = null;
			public ITask task { get; }

			public QueueData(ITask task) => this.task = task;

			public QueueData getNext() => nextData;
			public void setNext(QueueData next) => nextData = next;
		}

		public void addTask(ITask data)
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
	}
}