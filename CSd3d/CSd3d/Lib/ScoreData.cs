namespace MelloRin.CSd3d.Lib
{
	public class ScoreData
	{
		public int score { get; }
		public int perfect { get; }
		public int fail { get; }
		public int maxCombo { get; }

		public ScoreData(int score, int perfect, int fail, int maxCombo)
		{
			this.score = score;
			this.perfect = perfect;
			this.fail = fail;
			this.maxCombo = maxCombo;
		}
	}
}