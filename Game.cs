using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;

using System;

using static Input;
using static Elements;

public class Game {

	public Input input;
	public Commands cmnd;
	public GameStatus status;
	public Fight fight;


	public bool running;

	public string playerName;
	public short playerHealth;
	public short playerMaxHealth;

	public Enemy currentEnemy;
	public Shop currentShop;

	public const int MaximumEnemies = 5000;

	public List<Enemy> enemiesList;
	public List<Item> itemsList;

	public string connectionString = "";
	public SQLiteConnection con;
	SQLiteCommand cmd;

	public enum GameState {
		Idle,
		Shop,
		Fight,
	}

	public GameState State;


	public List<Enemy> getEnemyList(string dbName) {
		if (!connectDB(dbName)) 
			return null;

		List<Enemy> to_return = new List<Enemy>();

		SQLiteDataReader reader = executeSQLiteRequest(dbName, "SELECT * FROM Enemies LIMIT " + MaximumEnemies.ToString());

		while (reader.Read()) {
			to_return.Add(
				new Enemy(
				reader.GetInt32(1), // damage
				reader.GetInt32(2),	// base strength
				reader.GetInt32(3), // base speed
				reader.GetInt32(4), // base evasion
				reader.GetInt32(5), // base resistance
				reader.GetInt32(6), // max hp
				reader.GetString(7), // name
				reader.GetString(8), // description
				reader.GetString(9) // immunes
				)
			);
		}

		reader.Close();

		return to_return;

	}

	public List<Item> getItemsList(string dbName) {
		if (!connectDB(dbName)) 
			return null;

		List<Item> to_return = new List<Item>();

		SQLiteDataReader reader = executeSQLiteRequest(dbName, "SELECT * FROM Items LIMIT " + MaximumEnemies.ToString());

		while (reader.Read()) {
			to_return.Add(
				new Item(
					reader.GetString(1),
					reader.GetString(2),
					getEffectStr(reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5)),
					reader.GetInt32(6),
					reader.GetInt32(7)
				)
			);
		}

		reader.Close();

		return to_return;

	}

	public void Run() {
		State = GameState.Idle;
	}

	public Game() {

		itemsList = getItemsList(@"URI=file:.\sqlite.db");

		input = new Input();
		input.xLimit = Console.WindowWidth - 50;
		cmnd = new Commands(this);
		status = new GameStatus(this);
		fight = new Fight(this);

		enemiesList = getEnemyList(@"URI=file:.\sqlite.db");

		currentShop = new Shop(itemsList);
		running = true;
		
	}

	public int yppos = 0;
	public int ypos = 0;

	public int height = 0;

	public void beginLoop() {
		State = GameState.Idle;
		status.loadSave();
		System.Console.Clear();

		height = Console.WindowHeight;

		while(running) {
			string inputt = input.getString();
			Console.SetCursorPosition(0, yppos);
			string res = cmnd.parseCommand(inputt);
			if (!string.IsNullOrEmpty(res)) {
				input.print(res);
			}
			evaluateTurnEnd();
			yppos = Console.CursorTop;

			Console.ForegroundColor = ConsoleColor.White;

			input.verticalLine(input.xLimit - 1, 0, Console.WindowHeight, '|');

			stats();

			//input.print("|red|blue|blue|: " + inputt);
		}
	}


	private void evaluateTurnEnd() {
		if (currentEnemy == null) {
			return;
		}
		if (currentEnemy.Hp == 0) {
			input.printf("|white/cyan|YOU WIN!!!!!!!!!!", Format.center);
			status.payBytes(currentEnemy.coinsGiven, 0);
			int levelsUp = status.addXp(currentEnemy.xpGiven);
			input.print("You gained |magenta|" + currentEnemy.coinsGiven + "|white|B$! and |darkcyan/white|" + currentEnemy.xpGiven + "|white|xp!");
			if (levelsUp != 0) {
				input.print("You leveled up " + levelsUp + " time" + ((levelsUp != 1) ? "s" : "") + "!");
			}
			State = GameState.Idle;
			currentEnemy = null;
		}
	}

	public void Use(Item it) {

	}


	private void EvaluateGameState(GameState state) {

		string finalString = "";

		switch(state) {

			case GameState.Idle:
				finalString = "COMMANDS: |black/cyan|IDLE|white| \n Fight: Start a battle \n Shop: Shop for items";
				input.drawBox(finalString, input.xLimit + 2, ypos + 15);
				break;

			case GameState.Fight:
				finalString = "ENEMY: |black/red|" + currentEnemy.name + "|white| \n HP: |cyan|" + currentEnemy.Hp + "|white|/|darkcyan|" + currentEnemy.maxHp + "|white| \n DMG: " + currentEnemy.Dmg + " \n Strength: " + currentEnemy.Strength + ".";
				input.drawBox(finalString, input.xLimit + 2, ypos + 15);
				break;

			case GameState.Shop:
				/* 
					FOR THE SHOP ADD PRICES FOR ITEMS -> ALTER TABLE
				*/
				finalString = "ITEMS: |black/cyan|SHOP|white| \n";
				if (currentShop.wares.Count < 1) {
					finalString += "SHOP EMPTY, COME BACK ANOTHER TIME";
				} else {
					foreach(Item it in currentShop.wares) {
						finalString += it.name + " -> |mag|" + it.price + "|w|B$ \n ";
					}	
				}
				input.drawBox(finalString, input.xLimit + 2, ypos + 15);
				break;

			default:
				input.drawBox("WARNING: GAMESTATE?", input.xLimit + 2, ypos + 15);
				break;

		}

	}

	private void stats() {
		ypos = yppos - height;
		if (ypos < 0) {
			ypos = 0;
		}

		input.drawRectFilled(input.xLimit, ypos, Console.WindowWidth - input.xLimit, Console.WindowHeight - 1, ' ');

		Console.ForegroundColor = ConsoleColor.White;
		status.printReadouts();
		Console.ForegroundColor = ConsoleColor.White;
		EvaluateGameState(State);

		Console.ForegroundColor = ConsoleColor.White;

		Console.SetCursorPosition(input.xLimit, ypos + (Console.WindowHeight - 10));
		Console.Write(" >".PadRight(Console.WindowWidth - Console.CursorLeft)); 
		Console.SetCursorPosition(input.xLimit + 2, ypos + (Console.WindowHeight - 10));
	}

	public void End() {
		running = false;
		con.Close();
	}













	public bool connectDB(string dbname) {
		con = new SQLiteConnection(dbname);
		con.Open();
		if (con != null) {
			connectionString = dbname;
			return true;
		}
		else {
			return false;
		}
	}

	public void executeSQLiteCommandNoReturn(string dbName, string command) {
		if (!connectDB(dbName)) {
			return;
		}

		if (cmd == null) {
			cmd = new SQLiteCommand(con);
		}

		cmd.CommandText = command;
		cmd.ExecuteNonQuery();
	}

	public SQLiteDataReader executeSQLiteRequest(string dbName, string request) {
		if (!connectDB(dbName)) {
			return null;
		}

		if (cmd == null) {
			cmd = new SQLiteCommand(con);
		}

		cmd.CommandText = request;
		return cmd.ExecuteReader();
	}
}