using System;

namespace Simplex.Gui.RichText
{
	public class LineBreakPart : Part
	{
		public override string[] Split(ref Font font)
		{
			return new string[] { "\n" };
		}
	}
}
