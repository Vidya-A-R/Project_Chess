using System.Drawing;
using System.Linq.Expressions;
namespace Project_Chess;
using System.Text;
using System.Data;

class Program
{
	static void Main(string[] args)
	{
		StartAgain:
		Console.Clear();
		Color color = new();
		Board board = new();
		board.InitilizePiece();
		//Console.WriteLine(board.pieces);
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


