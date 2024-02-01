namespace Project_Chess;

using System;
using System.Collections.Generic;

public class GameController
{
	public Dictionary<IPlayer, PlayerData> Players { get; set; }
	public event Action<GameStatus> OnGameStatusChanged;
	public event Action<PlayerStatus> OnGamePlayerStatusChanged;
	//public ChessBoard Board { get; private set; }
	public DrawRequest DrawRequest { get; private set; }
	public Position Position { get; private set; }
	public PlayerStatus PlayerStatus { get; private set; }
	private Random random = new();
	private Color? firstPlayerColor = null;
	public GameStatus gameStatus;
	public PlayerStatus playerStatus;
	private IPlayer currentPlayer;
	public Color currentTurnColor = Color.White; // Putih mulai pertama
	private Board board;
	public List<IPiece> InitialPieces { get; private set; }
	public Type type;
	public delegate void PlayerStatusChangedHandler(IPlayer player, PlayerStatus newStatus); //ganti ke action
	public event PlayerStatusChangedHandler PlayerStatusChanged;

	
	

	public GameController(Board board) //board need abstraction
	{
		Players = new Dictionary<IPlayer, PlayerData>{};
		this.board = board;
		InitialPieces = new List<IPiece>();
		InitializeInitialPieces(); // better in program.cs
		
	}
	private void InitializeInitialPieces() //move to program.cs
	{
		for (int row = 0; row < 8; row++)
		{
			for (int col = 0; col < 8; col++)
			{
				var piece = board.pieces[row, col];
				if (piece != null)
				{
					InitialPieces.Add(piece);
				}
			}
		}
	}
	

	public void AddPlayer(string name, int playerIndex) //instane must be on instance gak solid
	{
		int id = playerIndex + 1; 
		IPlayer player = new Player(id, name); 
		Color playerColor = SetPlayerColor(player, playerIndex);
		PlayerData data = new PlayerData(playerColor, PlayerStatus.Play);
		Players.Add(player, data); 
	}

	public IEnumerable<IPlayer> GetPlayers()
	{
		return Players.Keys;
	}


	public Color SetPlayerColor(IPlayer player, int playerIndex) //playerindex change to color random or not karepe user
	{
		if (firstPlayerColor == null)
		{
			firstPlayerColor = random.Next(2) == 0 ? Color.White : Color.Black;
		}
		
		if (playerIndex == 0)
		{
			return firstPlayerColor.Value;
		}
		else
		{
			return firstPlayerColor == Color.White ? Color.Black : Color.White;
		}
	}

	public IPlayer FindPlayerByColor(Color color)
	{
		return Players.FirstOrDefault(pair => pair.Value.ColorPlayer == color).Key;
	}

	public bool ChangeGameStatus(GameStatus newStatus)
	{
		OnGameStatusChanged?.Invoke(newStatus);
		gameStatus = newStatus;
		return true;
	}

	public GameStatus GetGameStatus()
	{
		return gameStatus;
	}

	public void NextTurn()
	{
		currentTurnColor = currentTurnColor == Color.White ? Color.Black : Color.White;
	}
	public bool IsTurn(Color color)
	{
		return currentTurnColor == color;
	}
	public IPlayer GetCurrentPlayer()
	{
		return Players.FirstOrDefault(p => p.Value.ColorPlayer == currentTurnColor).Key;
	}

	public IEnumerable<IMove> GetAvailableMoves(IPiece pieces, Position position, Board board)
	{
		// Implementasi untuk mendapatkan semua gerakan yang tersedia untuk bidak tertentu
		var availableMoves = new List<Move>();
		Position currentPos = position;
		int currentRow = position.Row;
		int currentCol = position.Col;
		if (pieces.PieceColor == Color.White){
			if (pieces.PieceType == Type.Pawn){
				Pawn pawn = pieces as Pawn;
				if(pawn.HasMoved == false && currentRow <= 3){
					Position twoStepsForward = new Position(currentRow + 2, currentCol);
					if (currentCol >= 0 && currentRow < 8)
					{
						if (!IsTileNotEmpty(twoStepsForward,board)) // Di sini harusnya membandingkan dengan warna lawan, bukan selalu hitam
						{
							availableMoves.Add(new Move(currentPos, twoStepsForward));
						}
					
					Position oneStepForward = new Position(currentRow + 1, currentCol);
					if (!IsTileNotEmpty(oneStepForward,board)) // Di sini harusnya membandingkan dengan warna lawan, bukan selalu hitam
					{
						availableMoves.Add(new Move(currentPos, oneStepForward));
					}
					}
				}
				if(pawn.HasMoved == true){
					Position oneStepForward = new Position(currentRow + 1, currentCol);
					if (currentCol >= 0 && currentRow < 8){
						if (!IsTileNotEmpty(oneStepForward,board)) // Di sini harusnya membandingkan dengan warna lawan, bukan selalu hitam
						{
							availableMoves.Add(new Move(currentPos, oneStepForward));
						}
						}
					}
					foreach (var offset in new[] { -1, 1 }) // Cek kedua arah diagonal
					{
						int captureCol = currentCol + offset;
						if (captureCol >= 0 && captureCol < 8) // Kolom harus di antara 0 dan 7
						{
							int captureRow = currentRow + 1;
							Position capturePos = new Position(captureRow, captureCol);
							if (IsTileOccupied(capturePos, Color.White, board)) // Di sini harusnya membandingkan dengan warna lawan, bukan selalu hitam
							{
								availableMoves.Add(new Move(currentPos, capturePos));
							}
						}
					}
			}
		}
		if (pieces.PieceColor == Color.Black){
			if (pieces.PieceType == Type.Pawn){
				Pawn pawn = pieces as Pawn;
				if(pawn.HasMoved == false && currentRow > 5){
					Position twoStepsForward = new Position(currentRow - 2, currentCol);
					if (currentCol >= 0 && currentRow < 8)
					{
						if (!IsTileNotEmpty(twoStepsForward,board)){
							availableMoves.Add(new Move(currentPos, twoStepsForward));
						}
					}
					Position oneStepForward = new Position(currentRow - 1, currentCol);
					if (currentCol >= 0 && currentRow < 8){
						if (!IsTileNotEmpty(oneStepForward,board)){
							availableMoves.Add(new Move(currentPos, oneStepForward));
						}
					}
				}
				if(pawn.HasMoved == true){
					Position oneStepForward = new Position(currentRow - 1, currentCol);
					if (currentCol >= 0 && currentRow < 8){
						if (!IsTileNotEmpty(oneStepForward,board)){
							availableMoves.Add(new Move(currentPos, oneStepForward));
						}
					}
				}
				
					foreach (var offset in new[] { -1, 1 }) // Cek kedua arah diagonal
					{
						int captureRow = currentRow - 1; // Pion selalu bergerak ke baris berikutnya untuk penangkapan
						int captureCol = currentCol + offset;

						// Cek apakah posisi penangkapan masih berada di dalam papan
						if (captureCol >= 0 && captureCol < 8) // Kolom harus di antara 0 dan 7
						{
							Position capturePos = new Position(captureRow, captureCol);
							if (IsTileOccupied(capturePos, Color.Black, board)) // Di sini harusnya membandingkan dengan warna lawan, bukan selalu hitam
							{
								availableMoves.Add(new Move(currentPos, capturePos));
							}
						}
					}
				}
			}

		if(pieces.PieceType ==  Type.Rook){
			Rook rook = pieces as Rook;
			// Arah horizontal ke kiri
			for (int col = currentCol - 1; col >= 0; col--)
			{                 
				Position newPos = new Position(currentRow, col);   
				if (!IsTileNotEmpty(newPos, board))
					{
						Console.WriteLine("tes");
						availableMoves.Add(new Move(currentPos, newPos));
					}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			 // Arah horizontal ke kanan
			for (int col = currentCol + 1; col < 8; col++)
			{
				Position newPos = new Position(currentRow, col);
				if (!IsTileNotEmpty(newPos, board))
					{
						Console.WriteLine("tes");
						availableMoves.Add(new Move(currentPos, newPos));
					}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Arah vertikal ke atas
			for (int row = currentRow - 1; row >= 0; row--)
			{
				Position newPos = new Position(row, currentCol);
				if (!IsTileNotEmpty(newPos, board))
					{
						Console.WriteLine("tes");
						availableMoves.Add(new Move(currentPos, newPos));
					}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Arah vertikal ke bawah
			for (int row = currentRow + 1; row < 8; row++)
			{
				Position newPos = new Position(row, currentCol);
				if (!IsTileNotEmpty(newPos, board))
					{
						Console.WriteLine("tes");
						availableMoves.Add(new Move(currentPos, newPos));
					}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}
		}
		if(pieces.PieceType == Type.Knight){
			Knight knight = pieces as Knight;
			int[] rowOffsets = { -2, -1, 1, 2, 2, 1, -1, -2 };
			int[] colOffsets = { 1, 2, 2, 1, -1, -2, -2, -1 };

			for (int i = 0; i < 8; i++)
			{
				int newRow = currentRow + rowOffsets[i];
				int newCol = currentCol + colOffsets[i];

				if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8)
				{
					var potentialPosition = new Position(newRow, newCol);
					var pieceAtNewPosition = board.GetPiece(newRow, newCol);

					// Knight can jump over pieces, so we just check if the new position is not occupied by a piece of the same color
					if (pieceAtNewPosition == null || pieceAtNewPosition.PieceColor != knight.PieceColor)
					{
						availableMoves.Add(new Move(currentPos, potentialPosition));
					}
				}
			}
		}
		if (pieces.PieceType == Type.Bishop)
		{
			Bishop bishop = pieces as Bishop;
			// Diagonal kiri atas
			for (int i = 1; currentRow - i >= 0 && currentCol - i >= 0; i++)
			{
				Position newPos = new Position(currentRow - i, currentCol - i);
				if (!IsTileNotEmpty(newPos, board))
				{
					availableMoves.Add(new Move(currentPos, newPos));
				}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Diagonal kanan atas
			for (int i = 1; currentRow - i >= 0 && currentCol + i < 8; i++)
			{
				Position newPos = new Position(currentRow - i, currentCol + i);
				if (!IsTileNotEmpty(newPos, board))
				{
					availableMoves.Add(new Move(currentPos, newPos));
				}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Diagonal kiri bawah
			for (int i = 1; currentRow + i < 8 && currentCol - i >= 0; i++)
			{
				Position newPos = new Position(currentRow + i, currentCol - i);
				if (!IsTileNotEmpty(newPos, board))
				{
					availableMoves.Add(new Move(currentPos, newPos));
				}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Diagonal kanan bawah
			for (int i = 1; currentRow + i < 8 && currentCol + i < 8; i++)
			{
				Position newPos = new Position(currentRow + i, currentCol + i);
				if (!IsTileNotEmpty(newPos, board))
				{
					availableMoves.Add(new Move(currentPos, newPos));
				}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}
		}
		if(pieces.PieceType == Type.Queen){
			Queen queen = pieces as Queen;

			//diagonal kiri atas
			for (int i = 1; currentRow - i >= 0 && currentCol - i >= 0; i++)
			{
				Position newPos = new Position(currentRow - i, currentCol - i);
				if (!IsTileNotEmpty(newPos, board))
				{
					availableMoves.Add(new Move(currentPos, newPos));
				}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Diagonal kanan atas
			for (int i = 1; currentRow - i >= 0 && currentCol + i < 8; i++)
			{
				Position newPos = new Position(currentRow - i, currentCol + i);
				if (!IsTileNotEmpty(newPos, board))
				{
					availableMoves.Add(new Move(currentPos, newPos));
				}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Diagonal kiri bawah
			for (int i = 1; currentRow + i < 8 && currentCol - i >= 0; i++)
			{
				Position newPos = new Position(currentRow + i, currentCol - i);
				if (!IsTileNotEmpty(newPos, board))
				{
					availableMoves.Add(new Move(currentPos, newPos));
				}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Diagonal kanan bawah
			for (int i = 1; currentRow + i < 8 && currentCol + i < 8; i++)
			{
				Position newPos = new Position(currentRow + i, currentCol + i);
				if (!IsTileNotEmpty(newPos, board))
				{
					availableMoves.Add(new Move(currentPos, newPos));
				}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}
			//horizontal kiri           
			for (int col = currentCol - 1; col >= 0; col--)
			{                 
				Position newPos = new Position(currentRow, col);   
				if (!IsTileNotEmpty(newPos, board))
					{
						Console.WriteLine("tes");
						availableMoves.Add(new Move(currentPos, newPos));
					}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			 // Arah horizontal ke kanan
			for (int col = currentCol + 1; col < 8; col++)
			{
				Position newPos = new Position(currentRow, col);
				if (!IsTileNotEmpty(newPos, board))
					{
						Console.WriteLine("tes");
						availableMoves.Add(new Move(currentPos, newPos));
					}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Arah vertikal ke atas
			for (int row = currentRow - 1; row >= 0; row--)
			{
				Position newPos = new Position(row, currentCol);
				if (!IsTileNotEmpty(newPos, board))
					{
						Console.WriteLine("tes");
						availableMoves.Add(new Move(currentPos, newPos));
					}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}

			// Arah vertikal ke bawah
			for (int row = currentRow + 1; row < 8; row++)
			{
				Position newPos = new Position(row, currentCol);
				if (!IsTileNotEmpty(newPos, board))
					{
						Console.WriteLine("tes");
						availableMoves.Add(new Move(currentPos, newPos));
					}
				else
				{
					if (IsTileOccupied(newPos, pieces.PieceColor, board))
					{
						availableMoves.Add(new Move(currentPos, newPos));
					}
					break;
				}
			}
		}
		if(pieces.PieceType == Type.King){

			King king = pieces as King;
			// Periksa gerakan satu kotak di sekeliling Raja
			int[] rowOffsets = {-1, -1, -1, 0, 0, 1, 1, 1};
			int[] colOffsets = {-1, 0, 1, -1, 1, -1, 0, 1};

			for (int i = 0; i < 8; i++)
			{
				int newRow = currentRow + rowOffsets[i];
				int newCol = currentCol + colOffsets[i];

				// Pastikan posisi baru masih berada dalam papan
				if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8)
				{
					Position newPos = new Position(newRow, newCol);
					// Tambahkan gerakan jika kotak tujuan tidak ditempati oleh bidak dengan warna yang sama
					// future || IsTileOccupiedByOpponent(newPos, pieces.PieceColor, board)
					if(!IsTileNotEmpty(newPos, board)){
						availableMoves.Add(new Move(currentPos, newPos));
					}
					else{
						if (IsTileOccupied(newPos, pieces.PieceColor, board) )
						{
							availableMoves.Add(new Move(currentPos, newPos));
						}
						continue;
					}
				}
			}
		}

		return availableMoves;
	}

	public bool IsPossibleMoves(IPiece piece, Position newPosition, Position position, Board board)
	{
		var availableMoves = GetAvailableMoves(piece, position,board);

		foreach (Move move in availableMoves)
		{   
			if (move.End.Row == newPosition.Row && move.End.Col == newPosition.Col )
			{
				return true; 
			}
		}
	
		return false; 
	}

	public bool IsTileOccupied(Position position, Color pieceColor, Board board)
	{
		//Board board = new();
		int currentRow = position.Row;
		int currentCol = position.Col;
		var pieceAtPos = board.GetPiece(currentRow, currentCol);
		return pieceAtPos != null && pieceAtPos.PieceColor != pieceColor;
	}

	public bool IsTileNotEmpty(Position position, Board board)
	{
		//Board board = new();
		int currentRow = position.Row;
		int currentCol = position.Col;
		var pieceAtPos = board.GetPiece(currentRow, currentCol);
		return pieceAtPos != null;
	}

	public IPiece GetEatenPiece(Board board)
	{
		foreach (var initialPiece in InitialPieces)
		{
			bool found = false;
			for (int row = 0; row < 8; row++)
			{
				for (int col = 0; col < 8; col++)
				{
					var pieceAtPosition = board.GetPiece(row, col);
					if (pieceAtPosition != null && pieceAtPosition.Id == initialPiece.Id)
					{
						found = true;
						break;
					}
				}
				if (found) break;
			}
			if (!found) 
			{
				return initialPiece;
			}
		}
		return null;
	}


	public bool PerformCastling(IPiece piece, IMove move)
	{
		throw new NotImplementedException();
	}

	public bool IsPromotePawn(IPiece piece, Position newPosition)
	{
		if (piece.PieceType != Type.Pawn)
		{
			return false;
		}
		if ((piece.PieceColor == Color.White && newPosition.Row == 7) ||
			(piece.PieceColor == Color.Black && newPosition.Row == 0))
		{
			return true;
		}

		return false;
	}
	public bool ChangePawn(IPiece pawn, Board board, Position position, int Chosen)
	{
		if (pawn.PieceType != Type.Pawn)
		{
			return false; 
		}

		IPiece newPiece = null;
		Type newPieceType;
		if(Chosen == 1){
			newPieceType =  Type.Queen;
			newPiece = new Queen(pawn.Id, pawn.PieceColor);
		}
		else if(Chosen == 2){
			newPieceType =  Type.Rook;
			newPiece = new Rook(pawn.Id, pawn.PieceColor);
		}
		else if(Chosen == 3){
			newPieceType =  Type.Knight;
			newPiece = new Knight(pawn.Id, pawn.PieceColor);
		}
		else{
			newPieceType = Type.Bishop;
			newPiece = new Bishop(pawn.Id, pawn.PieceColor);
		}
		board.pieces[position.Row,position.Col] = newPiece;
		return true;
	}

	public IPlayer SetPlayerStatus(IPlayer player, PlayerStatus status){
	if (Players.TryGetValue(player, out PlayerData playerData))
		{
			playerData.PlayerStatus = status;
			PlayerStatusChanged?.Invoke(player, status);
			return player;
		}
		else{
			return null;
		}
	}


	public PlayerStatus GetPlayerStatus(IPlayer player)
	{
		// Implementasi untuk mendapatkan status pemain
		throw new NotImplementedException();
	}
	
}
