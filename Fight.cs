using System.Threading;
using System.Threading.Tasks;

public class Fight {

	private Game game;

	public enum Turn {
		Player,
		Enemy,
	}

	public Turn currentTurn;

	public Fight(Game g) {
		game = g;
		currentTurn = Turn.Player;
	}

	public string playerFights(string[] args) {

		float enemyPrevHp = game.currentEnemy.Hp;

		int status = game.currentEnemy.attack(game.status.damage);
		string to_ret =  "Attacked enemy for |red|" + (int)game.status.damage + "|white|dmg!!!\n";

		to_ret += "The enemy lost |cyan|" + (game.currentEnemy.Hp - enemyPrevHp) + "|white|hp, leaving it on |yellow|" + game.currentEnemy.Hp + "|white|hp.\n";

		switch(status) {
			case 0:	
				to_ret += "The enemy's resistance was more powerful than your attack!";
				break;

			case 1:
				break;

			case 2:
				to_ret += "The enemy is on |yellow|0|white|hp!";
				break;

			default:
				break;
		}	

		currentTurn = Turn.Enemy;

		return to_ret;

		/*if (args.Length == 2) {
			try {
				float dmg = float.Parse(args[1]);
				game.currentEnemy.attack(dmg);
				return "Attacked " + game.currentEnemy.name + " for |red|" + dmg + "|white| damage!!";
			} catch {
				return "Fight '" + args[1] + " is not a valid command. 'fight <";
			}*/


	}

	public int enemyTurn() {

		var rand = new System.Random();

		int whatWillTheyDo = rand.Next(1, 4); // number between 1 and 3

		System.Console.CursorVisible = false;

		game.input.printnln("The enemy is deciding what to do... ");

		for (int i = 0; i < rand.Next(4, 8); i ++) {
			game.input.printnln("/");
			Thread.Sleep(200);
			game.input.printnln("\b\\\b");
			Thread.Sleep(200);
		}

		System.Console.CursorVisible = true;

		switch (whatWillTheyDo) {

			case 1:
				
				break;

		}

		currentTurn = Turn.Player;

		return whatWillTheyDo;

	}

}