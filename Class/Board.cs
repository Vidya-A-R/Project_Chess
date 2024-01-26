namespace Project_Chess;
using System.Text;

public class Board
{
	private string[,] coordinates;
	private static Board instance;
	public IPiece[,] pieces { get; set; }
	
	
	public Board()
	{
		coordinates = new string[8, 8];
		pieces = new IPiece[8,8];
	}

	public void InitilizePiece()
	{
		for (int i = 0; i < 8; i++)
		{
			pieces[1, i] = new Pawn(i + 1, Color.White); // ID 1-8 untuk pawns putih
			pieces[6, i] = new Pawn(i + 17, Color.Black); // ID 17-24 untuk pawns hitam
		}

		// rooks
		pieces[0, 0] = new Rook(9, Color.White); // ID 9
		pieces[0, 7] = new Rook(10, Color.White); // ID 10
		pieces[7, 0] = new Rook(25, Color.Black); // ID 25
		pieces[7, 7] = new Rook(26, Color.Black); // ID 26

		// knights
		pieces[0, 1] = new Knight(11, Color.White); // ID 11
		pieces[0, 6] = new Knight(12, Color.White); // ID 12
		pieces[7, 1] = new Knight(27, Color.Black); // ID 27
		pieces[7, 6] = new Knight(28, Color.Black); // ID 28

		// bishops
		pieces[0, 2] = new Bishop(13, Color.White); // ID 13
		pieces[0, 5] = new Bishop(14, Color.White); // ID 14
		pieces[7, 2] = new Bishop(29, Color.Black); // ID 29
		pieces[7, 5] = new Bishop(30, Color.Black); // ID 30

		// queens dan kings
		pieces[0, 3] = new Queen(15, Color.White); // ID 15
		pieces[0, 4] = new King(16, Color.White); // ID 16
		pieces[7, 3] = new Queen(31, Color.Black); // ID 31
		pieces[7, 4] = new King(32, Color.Black); // ID 32
		
	}

    public IPiece GetPiece(int row, int col)
    {
        if (row >= 0 && row < pieces.GetLength(0) && col >= 0 && col < pieces.GetLength(1))
        {
            return pieces[row, col];
        }
        else
        {
            return null;
        }
    }

    public string DrawBoard()
    {
        var boardString = new StringBuilder();

        boardString.Append("  "); 

        for (int col = 0; col < 8; col++)
        {
            boardString.AppendFormat("      {0}     ", col + 1); 
        }
        boardString.AppendLine();

        for (int row = 0; row < 8; row++)
        {
            boardString.AppendFormat("{0} ", (int)(1 + row));

            for (int col = 0; col < 8; col++)
            {
                IPiece piece = GetPiece(row, col);
                string pieceRepresentation = piece != null 
                    ? $"{piece.PieceType.ToString()}_{piece.PieceColor.ToString().Substring(0, 1)}" 
                    :  "        "; 

                boardString.AppendFormat("[{0,-10}]", pieceRepresentation); 
            }
            boardString.AppendLine();
        }

        return boardString.ToString();
    }
	public void MovePiece(IPiece piece, Position newPosition,Position currentPosition)
	{
    	// get position
    	currentPosition = currentPosition;
    	// Border
    	if (newPosition.Row >= 0 && newPosition.Row < pieces.GetLength(0) && 
        	newPosition.Col >= 0 && newPosition.Col < pieces.GetLength(1))
    	{
        	// delete from past coordinate
        	pieces[currentPosition.Row, currentPosition.Col] = null;

        	//new coordinate
        	pieces[newPosition.Row, newPosition.Col] = piece;

    	}
    	else
    	{
        	throw new InvalidOperationException("Gerakan tidak valid: posisi baru di luar batas papan catur.");
    	}
	}

}

