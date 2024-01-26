namespace Project_Chess;

public enum GameStatus
{
	NotInitialized,
	Ongoing,
	End
}

public enum Color{
	Black,
	White

}

public enum Type
{
	Pawn,
	Knight,
	Bishop,
	Rook,
	Queen,
	King
}


public enum PlayerStatus
{
	Win,
	Draw,
	Lose,
	Play
}

public enum DrawRequest
{
	Agree,
	Disagree
}
