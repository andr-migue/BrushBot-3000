using System.Collections.Generic;

namespace BrushBot
{
	public class Scope
	{
		public Dictionary<string, object> Variables { get; set; }
		public Scope()
		{
			Variables = new();
		}
		public bool TryGet(string name, out object value)
		{
			return Variables.TryGetValue(name, out value);
		}
	}
}