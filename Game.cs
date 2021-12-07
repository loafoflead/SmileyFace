using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;

using System;

using static Input;
using static Elements;

using static WriteParser;

public class Game {


	/* public objects */
	public Input input;

	public GameStatus status;

	public Fight fight;

	/* private objects */
	private Commands cmnd;

	private DatabaseManager data;



	/* STATUS VARS */
	public bool running;


	/* ENTITY VARS */
	public Enemy currentEnemy;
	public Shop currentShop;

	/* final values */
	private const int maximumEnemies = 5000;
	private const int maximumItems = 5000;

	/* database values */
	public List<Enemy> enemiesList;
	public List<Item> itemsList;

	public enum GameState {
		Idle,
		Shop,
		Fight,
	}

	/* gamestate value */
	public GameState State;

	public void Run() {
		State = GameState.Idle;
	}

	public Game() {

		input = new Input();
		input.xLimit = Console.WindowWidth - 50;

		cmnd = new Commands(this);

		status = new GameStatus(this);

		fight = new Fight(this);

		data = new DatabaseManager();

		enemiesList = data.getEnemyList(@"URI=file:.\sqlite.db", maximumEnemies);
		itemsList = data.getItemsList(@"URI=file:.\sqlite.db", maximumItems);

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

		Console.SetCursorPosition(0, yppos);
		status.resetStats();
		yppos = Console.CursorTop;

		Console.ForegroundColor = ConsoleColor.White;

		// input.verticalLine(input.xLimit - 1, 0, ypos + Console.WindowHeight, '|');

		stats();

		while(running) {
			if (fight.currentTurn == Fight.Turn.Enemy) {
				Console.SetCursorPosition(0, yppos);
				fight.enemyTurn();
				Console.ForegroundColor = ConsoleColor.White;
				yppos = Console.CursorTop;
				status.resetStats();
				evaluateTurnEnd();
				yppos = Console.CursorTop;

				Console.ForegroundColor = ConsoleColor.White;

				stats();

				fight.currentTurn = Fight.Turn.Player;
				continue;
			}
			string inputt = input.getString();
			Console.SetCursorPosition(0, yppos);
			string res = cmnd.parseCommand(inputt);
			if (!string.IsNullOrEmpty(res)) {
				input.print(res);
			}
			status.resetStats();
			evaluateTurnEnd();
			yppos = Console.CursorTop;

			Console.ForegroundColor = ConsoleColor.White;

			fight.currentTurn = fight.nextTurn();


			// input.verticalLine(input.xLimit - 1, 0, ypos + Console.WindowHeight, '|');

			stats();

			//input.print("|red|blue|blue|: " + inputt);
		}
	}


	private void evaluateTurnEnd() {
		if (State != GameState.Fight) {
			return;
		}
		var random = new System.Random();
		if (currentEnemy.Hp == 0) {
			input.printf("|white/cyan|YOU WIN!!!!!!!!!!", Format.center);
			status.payBytes(currentEnemy.coinsGiven, 0);
			int levelsUp = status.addXp(currentEnemy.xpGiven);
			input.print("You gained |magenta|" + currentEnemy.coinsGiven + "|white|B$! and |darkred|" + currentEnemy.xpGiven + "|white|xp!");
			if (levelsUp !> 0) {
				input.print("You leveled up " + levelsUp + " time" + ((levelsUp != 1) ? "s" : "") + "!");
			}
			if (levelsUp == -1) {
				input.print("You reached max level! Added |magenta|99|white|B$ to your balance!");
			}
			State = GameState.Idle;
			currentEnemy.reset();
			currentEnemy = enemiesList[random.Next(0, enemiesList.Count)];
			return;
		}
		if (status.health <= 0.5f) {
			input.printf(formatString("YOU LOST!!!!!!!!!!", colourFormatOptions.two_colours, "black/red", "white/red"), Format.center);
			int coinsLost = random.Next(5, currentEnemy.coinsGiven);
			status.payBytes(-coinsLost, 0);
			input.print("You lost " + coinsLost + "!");
			input.print("Returning to idle state.");
			State = GameState.Idle;
			currentEnemy.reset();
			currentEnemy = enemiesList[random.Next(0, enemiesList.Count)];
			return;
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
					int index = 1;
					foreach(Item it in currentShop.wares) {
						finalString += index.ToString() + ": " + it.name + " -> |mag|" + it.price + "|w|B$ \n ";
						index ++;
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
		data.closeConnection();
	}
}