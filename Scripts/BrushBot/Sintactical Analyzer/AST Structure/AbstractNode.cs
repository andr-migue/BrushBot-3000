namespace BrushBot
{
	public abstract class Node
    {
        public (int, int) Location;
        public Node((int, int) location)
        {
            Location = location;
        }
    }
}