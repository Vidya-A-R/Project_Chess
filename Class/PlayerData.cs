namespace Project_Chess;

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