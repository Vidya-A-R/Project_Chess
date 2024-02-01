namespace Project_Chess;
public class Move : IMove
{
	public Position Start { get; private set; }
	public Position End { get; private set; }

	public Move(Position start, Position end)
	{
		Start = start;
		End = end;
	}
}
