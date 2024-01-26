using System.Drawing;
using System.Linq.Expressions;
namespace Project_Chess;
using System.Text;
using System.Data;

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


public class Position
{
	public int Row { get; }
	public int Col { get; }

	public Position(int row, int col)
	{
		Row = row;
		Col = col;
	}
}

public class PlayerData
{
	public Color ColorPlayer { get; private set; }
	public PlayerStatus PlayerStatus { get; set; }
	public Dictionary<Type, List<IPieceStatus>> PiecesStatus { get; private set; }

	public PlayerData(Color colorPlayer, PlayerStatus playerStatus)
	{
		ColorPlayer = colorPlayer;
		PlayerStatus = playerStatus;
		PiecesStatus = new Dictionary<Type, List<IPieceStatus>>();
		InitializePiecesStatus();
	}

	private void InitializePiecesStatus()
	{
		PiecesStatus[Type.Pawn] = new List<IPieceStatus>(Enumerable.Repeat(new SetPieceStatus(true), 8));
		PiecesStatus[Type.Knight] = new List<IPieceStatus>(Enumerable.Repeat(new SetPieceStatus(true), 2));
		PiecesStatus[Type.Rook] = new List<IPieceStatus>(Enumerable.Repeat(new SetPieceStatus(true), 2));
		PiecesStatus[Type.Bishop] = new List<IPieceStatus>(Enumerable.Repeat(new SetPieceStatus(true), 2));
		PiecesStatus[Type.King] = new List<IPieceStatus>(Enumerable.Repeat(new SetPieceStatus(true), 2));
		PiecesStatus[Type.Queen] = new List<IPieceStatus>(Enumerable.Repeat(new SetPieceStatus(true), 2));
	}
}

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

public class Player : IPlayer
{
	public int Id { get; private set; }
	public string Name { get; private set; }

	public Player(int id, string name)
	{
		Id = id;
		Name = name;
	}

}

class Program
{
	static void Main(string[] args)
	{
		StartAgain:
		Console.Clear();
		Color color = new();
		Board board = new();
		board.InitilizePiece();
		Console.WriteLine(board.pieces);
        Console.WriteLine(board.DrawBoard());
		board.GetPiece(8,8);
		GameController gameController = new GameController(board);
		gameController.ChangeGameStatus(GameStatus.NotInitialized);
		gameController.PlayerStatusChanged += PlayerStatusWinner;
		gameController.OnGameStatusChanged += GameStatusChanged;

		Console.WriteLine($"GameStatus right now: {gameController.GetGameStatus()}");
	    for (int i = 0; i < 2; i++) 
    	{
        	Console.WriteLine($"Input Player Data {i + 1}: ");
        	Console.Write("Name: ");
        	string name = Console.ReadLine();
        	gameController.AddPlayer(name, i); 
    	}
		foreach (var entry in gameController.Players)
		{
			IPlayer player = entry.Key;
			PlayerData playerData = entry.Value; 
			Console.WriteLine("");
			Console.WriteLine($"Player: ID = {player.Id}, Name = {player.Name}, Color: {playerData.ColorPlayer}");
			
		}
    	for (int i = 0; i < 2; i++)
    	{
       	 	Console.WriteLine($"Player {i + 1}, press 'Enter' if ready.");
        	Console.ReadLine();
    	}
		gameController.ChangeGameStatus(GameStatus.Ongoing);
		Console.WriteLine($"game status: {gameController.GetGameStatus()}");

		while (gameController.GetGameStatus() == GameStatus.Ongoing)
		{   
			IPlayer currentPlayer = gameController.GetCurrentPlayer();
        	Console.WriteLine($"{currentPlayer.Name} turn, color : {gameController.currentTurnColor}");
			Console.WriteLine($"Choose Piece by its ID");
			for (int row = 0; row < 8; row++){
				for (int col = 0; col < 8; col++)
            	{
					IPiece piece = board.GetPiece(row, col);
					if(gameController.currentTurnColor == Color.White){
						if(piece!=null){
							if(piece.PieceColor == Color.White && piece.PieceColor != Color.Black){
								Console.Write(piece.Id);
								Console.Write(".");
								Console.Write(piece.PieceType);
								Console.Write(" ");
								Console.Write(row + 1);
								Console.Write(col + 1);
								Console.WriteLine(" ");
							}
						}
					}
					if(gameController.currentTurnColor == Color.Black){
						if(piece!=null){
							if(piece.PieceColor == Color.Black && piece.PieceColor != Color.White){
								Console.Write(piece.Id);
								Console.Write(".");
								Console.Write(piece.PieceType);
								Console.Write(" ");
								Console.Write(row + 1);
								Console.Write(col + 1);
								Console.WriteLine(" ");
							}
						}
					}	
				}
			}
			startingSelection:
			string ChosenPiece = Console.ReadLine();
			int pieceId = Convert.ToInt32(ChosenPiece);
			IPiece pieceChosen = null;
			Position position = null;
			for (int row = 0; row < 8; row++){
				for (int col = 0; col < 8; col++)
            	{
                	IPiece piece = board.GetPiece(row, col);
					if(piece!=null && piece.Id == pieceId){
						pieceChosen = piece;
						position = new Position(row,col);
						break;
					}
					
            	}
				if (pieceChosen != null)
    			{
        			break; 
    			}
			}
	
			var availableMoves = gameController.GetAvailableMoves(pieceChosen,position,board);
			if (availableMoves.Any())
            {
				foreach (Move move in availableMoves)
    			{
					int i = 0;
					i++;
        			Console.WriteLine($"{i}.from ({move.Start.Row + 1}, {move.Start.Col+1}) to ({move.End.Row+1}, {move.End.Col+1})");
    			}
            }
			else{
				Console.WriteLine("No available move, choose other piece ");
            	goto startingSelection;
			}
			ChooseMoveAgain:
			Console.WriteLine("where do u want to move?");
			Console.Write("Row :");
	
			string coordinateRow = Console.ReadLine();
			Console.Write("Col :");
			string coordinateCol = Console.ReadLine();
			int Row = Convert.ToInt32(coordinateRow);
			int Col = Convert.ToInt32(coordinateCol);
			Row = Row-1;
			Col = Col-1;
			//Console.WriteLine(Row);
			//Console.WriteLine(Col);
			Position newPosition = new(Row,Col);
			Console.WriteLine(gameController.IsPossibleMoves(pieceChosen, newPosition, position,board));
			if (gameController.IsPossibleMoves(pieceChosen, newPosition, position,board))
			{
    			Console.WriteLine("Move is possible!");
				board.MovePiece(pieceChosen,newPosition,position);
				if(pieceChosen.PieceType == Type.Pawn){
					if(gameController.IsPromotePawn(pieceChosen,newPosition)){
						Console.WriteLine("Chose what piece that u want to promote (1.Queen, 2.Rook, 3.Knight, 4.Bishop):");

						string choice = Console.ReadLine();
						int choiceId = Convert.ToInt32(choice);
						gameController.ChangePawn(pieceChosen, board, newPosition,choiceId);
					}
				}
				Console.Clear();
				Console.WriteLine(board.DrawBoard());
				IPiece eatenPiece = gameController.GetEatenPiece(board);
				if (eatenPiece != null)
				{
    				Console.WriteLine($"eaten piece is : {eatenPiece.PieceType} {eatenPiece.PieceColor} ");
				}
					if (eatenPiece != null && eatenPiece.PieceType == Type.King)
					{
    					foreach (var entry in gameController.Players)
    					{
        					if (entry.Value.ColorPlayer == eatenPiece.PieceColor)
        					{
            					gameController.SetPlayerStatus(entry.Key, PlayerStatus.Lose);
        					}
        					else
        					{
            					gameController.SetPlayerStatus(entry.Key, PlayerStatus.Win);
        					}
    					}
						break;
					}

			}
			else
			{
    			Console.WriteLine("Move is not possible, chose another move");
				goto ChooseMoveAgain;
			}
			gameController.NextTurn();
			if (pieceChosen is Pawn pawn) 
    		{
				if(pawn.HasMoved == false)
        		pawn.UpdateFirstMove(pieceChosen,pieceChosen.Id); 
    		}
			
		}		
		gameController.ChangeGameStatus(GameStatus.End);
		goto StartAgain;
	}

	static void GameStatusChanged(GameStatus gameStatus)
	{
    	Console.WriteLine($"game status changed to: {gameStatus}");
    	
	}

	static void PlayerStatusWinner(IPlayer player, PlayerStatus status)
	{
    	Console.WriteLine($"Player {player} is the one who {status}");
    	
	}
	
	
}


