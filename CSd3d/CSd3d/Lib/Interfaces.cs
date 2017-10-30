namespace MelloRin.CSd3d.Lib
{
	public class ListData { }
	
	public interface IListable
	{
		void add(string tag, ListData data);

		void delete(string tag);
	}

	public interface IDrawable
	{
		void draw();
	}

	public interface ITask
	{
		void run(TaskQueue taskQueue);
	}
}
