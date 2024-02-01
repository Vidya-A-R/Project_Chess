namespace Project_Chess;

public class Pawn : IPiece
{
	public int Id { get; }
	public Color PieceColor { get; }
	public Type PieceType => Type.Pawn;
	public bool HasMoved{get; private set;}

	public Pawn(int id, Color color)
	{
		Id = id;
		PieceColor = color;
		HasMoved = false;
	}
	public void UpdateFirstMove(IPiece piece, int pieceId)
	{
    	if (piece is Pawn pawn && pawn.Id == pieceId)
    	{
        	if (!pawn.HasMoved)
        	{
            	pawn.HasMoved = true;
        	}
    	}
	}
}

public class Rook : IPiece
{
	public int Id { get; }
	public Color PieceColor { get; }
	public Type PieceType => Type.Rook;

	public Rook(int id, Color color)
	{
		Id = id;
		PieceColor = color;
	}
}

public class Knight : IPiece
{
	public int Id { get; }
	public Color PieceColor { get; }
	public Type PieceType => Type.Knight;

	public Knight(int id, Color color)
	{
		Id = id;
		PieceColor = color;
	}
}

public class Bishop : IPiece
{
	public int Id { get; }
	public Color PieceColor { get; }
	public Type PieceType => Type.Bishop;

	public Bishop(int id, Color color)
	{
		Id = id;
		PieceColor = color;
	}
}

public class Queen : IPiece
{
	public int Id { get; }
	public Color PieceColor { get; }
	public Type PieceType => Type.Queen;

	public Queen(int id, Color color)
	{
		Id = id;
		PieceColor = color;
	}
}

public class King : IPiece
{
	public int Id { get; }
	public Color PieceColor { get; }
	public Type PieceType => Type.King;

	public King(int id, Color color)
	{
		Id = id;
		PieceColor = color;
	}
}