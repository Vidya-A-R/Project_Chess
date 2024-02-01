namespace Project_Chess;

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