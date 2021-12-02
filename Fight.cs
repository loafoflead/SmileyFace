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

}