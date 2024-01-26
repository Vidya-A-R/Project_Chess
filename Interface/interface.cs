namespace Project_Chess;

public interface IPiece{

	
	public int Id{get; }

	public Color PieceColor{get; }

	public Type PieceType{get; }

}

public interface IMove
{
	Position Start { get; }
	Position End { get; }
}

public interface IPlayer
{
	int Id { get; }
	string Name { get; }
}

public interface IPieceStatus
{
	bool IsAlive { get; set; }
}

public class SetPieceStatus : IPieceStatus
{
	public bool IsAlive { get; set; }

	public SetPieceStatus(bool isAlive)
	{
		IsAlive = isAlive;
	}
}