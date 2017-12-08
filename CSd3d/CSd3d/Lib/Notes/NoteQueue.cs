namespace MelloRin.CSd3d.Lib.Notes
{
	public class QueueData
	{
		private QueueData nextData = null;
		public string data;

		public QueueData(string data) { this.data = data; }

		public QueueData getNext() { return nextData; }
		public void setNext(QueueData next) { nextData = next; }
		public string getData() { return data; }
	}

	public class NoteQueue
	{
		public QueueData head { get; private set; }

		public void addQuque(string data)
		{
			if (head == null)
			{
				head = new QueueData(data);
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

		public string pop()
		{
			if (head != null)
			{
				QueueData temp = head;
				head = head.getNext();
				return temp.getData();
			}
			else
			{
				return null;
			}
		}

		public bool search(string data)
		{
			if (head != null)
			{
				QueueData temp = head;
				while (temp.getNext() != null)
				{
					if (temp.getData().Equals(data))
						return true;
					temp = temp.getNext();
				}
				if (temp.getData().Equals(data))
					return true;
				else
					return false;
			}
			else
			{
				return false;
			}
		}
	}
}