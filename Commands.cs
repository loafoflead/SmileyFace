using System;
using System.Collections.Generic;
using System.Collections;

public class Commands {

	private Game game;

	public Commands(Game g) { 
		game = g;
	}



	public struct CommandAndArgs {
		public string command;
		public int argCount;
		public string helpMsg;

		public CommandAndArgs(string cmd, int argc, string help) {
			this.command = cmd;
			this.argCount= argc;
			this.helpMsg = help;
		}
	}

	string[] jokes = new string[] {
		"Knock knock! 'who's there?' Doctor 'Doctor who' ahahhaahahahAHAHAAH",
	};

	(string, int, string)[] commandList = new (string, int, string)[] {
		("box", 1, "box <content>: draws a box with your input in it"), 
		("cls", 0, "clears the screen"), 
		("quit", 0, "quits the game"),
		("help", 0, "help + opt:<page number> + opt:<command>"),
		("me", 0, "gives details on the current player"),
		("inv", 0, "shows you the contents of your inventory"),
		("run", 0, "run away from a fight"), ("exit", 0, "exits the current area"), ("fight", 0, "enters the fighting state"),
		("shop", 0, "opens the shop"), ("buy", 1, "buy <item name> => buys from the shop the item given, special cases are: 'all'"),
		("sell", 1, "sell <item name> => sells the item given"),
		("use", 1, "use <item> => uses the item given"),
		("detailed", 0, "Sets the stats list to be printed in more detail"),
	};

	(string, int, string)[] commandListCmd = new (string, int, string)[] {
		("enemy", 0, "enemy + opt:<-v/verbose>/<-t/tags> -> lists the current enemies in the game."), ("echo", 1, "echo <message>"), 
		("box", 1, "box opt:<-border:[char]/...> <content>"), ("recttest", 0, "creates a rectangle to test drawBox() function"),
		("postest", 0, "tests printing at a certain position"),
		("formattest", 0, "tests printing formatted text <formattest2 does the same thing but with highlighting options>"),
		("hurt", 1, "hurt <float>"),
		("pay", 1, "pay <byteCoins>.<byteCents>"), ("item", 0, "lists all items in the game. opt flags: <-t >> show item tags/...>"),
		("tell", 1, "tell <game object: item/enemy> gives verbose details on the item or enemy selected (use item and enemy tags)"),
		("gamestate", 1, "gamestate <Fight/Idle/Shop>"),
		("give", 1, "give <item tag>"), ("resetshop", 0, "resets the current shop"),
		("dmg", 1, "dmg <float>"), ("ko", 0, "ko <warning: can only be used during a fight."),
		("xp", 1, "xp <int>"), ("lvl", 1, "lvl <levels to add>"), ("lvlset", 1, "lvlset <int>"),
		("newenemy", 0, "resets the current enemy"),
	};

	public string parseCommand(string s) {

		if (string.IsNullOrEmpty(s)) {
			return "\n";
		}

		string[] args = s.Split(" ");
		int argv = args.Length;

		string to_ret = "";

		if (s.Length > 0) {
			if (s[0] == '#') {
				args[0] = args[0].Remove(0,1);
				to_ret = parsePrivCmd(args, argv);
			} else {
				to_ret = parsePriv(args, argv);
			}
		}

		
		if (!string.IsNullOrEmpty(to_ret)) {
			return to_ret;
		}

		return "";

	}

	private string parsePrivCmd(string[] args, int argv) {

		(string, int, string) currentCommand;

		foreach((string, int, string) cmnd in commandListCmd) {
			if (args[0] == cmnd.Item1) {
				if ((argv - 1) < cmnd.Item2) {
					return "Too few arguments for command " + cmnd.Item1 + "! '" + cmnd.Item3 + "'";
				} else {
					currentCommand = cmnd;
				}
			}
		}

		switch(args[0]) {

			case "help":
			case "h":
			case "?":
				foreach((string, int, string) cmnd in commandListCmd) {
					game.input.print(cmnd.Item1 + ": usage => '" + cmnd.Item3 + "'");
				}
				return "";

			case "hurt":
			case "damage":
				try {
					float dmg = float.Parse(args[1]);
					game.status.dealDamage(dmg);
					return "damaged player for |darkblue|" + dmg.ToString() + "|white|hp.";
				} catch {
					return "incorrect argument for command 'hurt': 'hurt <float>'";
				}

			case "xp":
				try {
					int xp = int.Parse(args[1]);
					game.status.addXp(xp);
					return "added |darkred|" + xp.ToString() + "|white|xp.";
				} catch {
					return "incorrect argument for command 'xp': 'xp <int>'";
				}

			case "newenemy":
			case "nenemy":
				var random = new System.Random();
				game.currentEnemy = game.enemiesList[random.Next(0, game.enemiesList.Count)];
				return "randomized current enemy";

			case "lvl":
			case "level":
				try {
					int lvl = int.Parse(args[1]);
					game.status.level += lvl;
					return "added |red|" + lvl.ToString() + "|white| levels.";
				} catch {
					return "incorrect argument for command 'level': 'level <int>'";
				}

			case "lvlset":
			case "levelset":
				try {
					int lvll = int.Parse(args[1]);
					game.status.level = lvll;
					return "set player level to |red|" + lvll.ToString() + "|white|levels.";
				} catch {
					return "incorrect argument for command 'levelset': 'level <int>'";
				}

			case "ko":
				if (game.State == Game.GameState.Fight) {
					game.currentEnemy.Hp = 0;
					return "The current enemy has been removed.";
				}
				else {
					return "You can only use this command during a fight to defeat the current enemy!";
				}

			case "dmg":
				try {
					float newdmg = float.Parse(args[1]);
					game.status.damage = newdmg;
					return "set player damage to |darkblue|" + newdmg.ToString() + "|white|dmg.";
				} catch {
					return "incorrect argument for command 'dmg': 'dmg <float>'";
				}

			case "pay":
			case "donate":
				try {
					if (args[1].Contains('.')) {
						string coinsStr = args[1].Split('.')[0];
						string centsStr = args[1].Split('.')[1];
						int coins = int.Parse(coinsStr);
						int cents = int.Parse(centsStr);
						game.status.payBytes(coins, cents);
						return "paid player |magenta|" + coinsStr + "|white|.|darkmagenta|" + centsStr + "|white|B$!";
					} else {
						int money = int.Parse(args[1]);
						game.status.payBytes(money, 0);
						return "paid player |magenta|" + money.ToString() + "|white|B$!";
					}
				} catch {
					return "incorrect argument for command 'pay': 'pay <byteCoin>.<byteCents>'";
				}

			case "give":
				Item itemret = game.itemsList.Find(Item => Item.tag == args[1]);
				if (itemret == null) {
					itemret = game.itemsList.Find(Item => Item.name.ToLower() == args[1].ToLower());
				}
				if (itemret != null) {
					game.status.inventory.Add(itemret);
					return "added " + itemret.name + " to the player's inventory.";
				}
				else {
					return "couldn't find item " + args[1] + ".";
				}

			case "tell":
				if (argv > 2) {
					if (String.Join(' ', args, 1, args.Length - 1) == "me a joke") {
						var rand = new System.Random();
						return jokes[rand.Next(0, jokes.Length)];
					}
				}
				foreach(Item it in game.itemsList) {
					if (it.tag == args[1]) {
						return "name: " + it.name + ", tag: " + it.tag + ", description: [...], effect given: " + it.effect_given.ToString() + ", rarity: " + it.Rarity + ".";
					}
				}
				foreach(Enemy en in game.enemiesList) {
					if (en.tag == args[1]) {
						return "name: " + en.name + ", tag: " + en.tag + ", description: [...], dmg/basedmg: " + en.Dmg + "/" + en.baseDmg + ", hp/maxhp: " + en.Hp + "/" + en.maxHp + ", strength/basestrength: " + en.Strength + "/" + en.baseStrength + ", coinsgiven: |mag|" + en.coinsGiven + "|white|, xp: |red|" + en.xpGiven + "|white|.";
					}
				}
				return "could not find that enemy or item, try retyping the name. (hint: use the 'enemy' and 'item' commands to see a list of all enemies and items)";

			case "enemy":	
				if (argv > 1) {
					switch(getFlags(args)[0]) {
						case "v":
						case "verbose":
							foreach(Enemy en in game.enemiesList) {
								game.input.print(en.name);
								game.input.print(en.description);
								game.input.print(en.tag);
								game.input.print(en.Hp.ToString());
								game.input.print(en.Dmg.ToString());
							}
							break;
						case "t":
						case "tags":
							foreach(Enemy en in game.enemiesList) {
								game.input.print(en.tag);
							}
							break;
						default:
							return "unknown flag for command 'enemy', flags are: '-v/verbose, -t/tags, ...";
					}
					return "";
				} else {
					foreach(Enemy en in game.enemiesList) {
						game.input.print(en.name);
					}
				}
				return "";

			case "item":
				if (hasFlags(args)) {
					if (getFlags(args)[0].Contains("t")) {
						foreach(Item it in game.itemsList) {
							game.input.print(it.tag);
						}
					}
				} else {
					foreach(Item it in game.itemsList) {
						game.input.print(it.name);
					}
				}
				return "";

			case "gamestate":
				switch(args[1].ToLower()) {
					case "fight":
						game.State = Game.GameState.Fight;
						return "gamestate is now: " + args[1];;

					case "idle":
						game.State = Game.GameState.Idle;
						return "gamestate is now: " + args[1];;

					case "shop":
						game.State = Game.GameState.Shop;
						return "gamestate is now: " + args[1];

					default:
						return "unknown argument '" + args[1] + "'.";
				}

			case "resetshop":
				game.currentShop = new Shop(game.itemsList);
				return "reset shop";


			case "boxtest":
				game.input.drawBox("\nhello \nworld", 12, 10);
				return "";

			case "recttest":
				game.input.drawRect(10, 10, 13, 13);
				return "";

			case "formattest":
				game.input.print(WriteParser.formatString("benjamin james reed", WriteParser.colourFormatOptions.rainbow, "", ""));
				game.input.print(WriteParser.formatString("benjamin james reed", WriteParser.colourFormatOptions.two_colours, "green", "red"));
				game.input.print(WriteParser.formatString("benjamin james reed", WriteParser.colourFormatOptions.half_colour, "red", "cyan"));
				return "|white|printed using options.rainbow, options.two_colours, and options.half_colour";

			case "formattest2":
				game.input.print(WriteParser.formatString("benjamin james reed", WriteParser.colourFormatOptions.rainbow, "highlight", "black"));
				game.input.print(WriteParser.formatString("benjamin james reed", WriteParser.colourFormatOptions.two_colours, "green/white", "red/white"));
				game.input.print(WriteParser.formatString("benjamin james reed", WriteParser.colourFormatOptions.half_colour, "red/white", "cyan/white"));
				return "|white|printed using options.rainbow, options.two_colours, and options.half_colour";

			case "postest":
				game.input.print("|red/blue|hi", 0, 0);
				return "pos_test";

			case "echo":
				if (hasFlags(args)) {
					if (getFlags(args)[0].Contains("pos:")) {
						string pos = getFlags(args)[0].Split(':')[1];
						if (string.IsNullOrEmpty(pos) || !pos.Contains(',')) {
							return "incorrect use of -pos: flag: 'echo -pos:<x>,<y> <content>'";
						}
						string[] xy = pos.Split(',');
						try {
							int x = int.Parse(xy[0]);
							int y = int.Parse(xy[1]);
							game.input.print(String.Join(' ', args, 1, args.Length), x, y);
						} catch {
							return "'" + args[1] + "'incorrect use of -pos: flag: 'echo -pos:<x - must be non null>, <y - the same> <content>'";
						}
					}
					return "";
				}

				game.input.print(String.Join(' ', args, 1, args.Length - 1));
				return "";

			case "box":
				if(!string.IsNullOrEmpty(getFlags(args)[0])) {
					if (getFlags(args)[0].Contains("border:")) {
						char bord = ' ';
						try {
							bord = char.Parse(args[1].Split(":")[1]);
						} catch {
							return "incorrect format for border character: '" + args[1].Split(":")[0] + "', must be 1 char and non-null";
						}
						game.input.drawBox(String.Join(" ", args).Remove(0, args[0].Length + args[1].Length + 2), bord);
					} else if (String.Join("", getFlags(args)).Contains("pos:")) {
						string pos = String.Join("/", getFlags(args));
						pos = pos.Split(':',2)[1];
						if (getFlags(args).Length > 1) {
							pos = pos.Split("pos:")[1];
							if (pos.Contains(":")) {
								pos = pos.Split("/",2)[0];
							}
						}
						if (string.IsNullOrEmpty(pos) || !pos.Contains(',')) {
							return "incorrect use of -pos: flag '|darkgrey|" + pos + "|white|' 'box -pos:<x>,<y> <content>'";
						}
						string[] xy = pos.Split(',');
						try {
							int x = int.Parse(xy[0]);
							int y = int.Parse(xy[1]);
							game.input.drawBox(removeFlags(args), x, y);
						} catch {
							return "'" + args[1] + "'incorrect use of -pos: flag '|darkgrey|" + pos + "|white|' 'box -pos:<x - must be non null>, <y - the same> <content>'";
						}
					}
					else {
						return "unkown flag, flags are: -border:<border char>, ...";
					}
				} else {
					game.input.drawBox(String.Join(" ", args).Remove(0, args[0].Length + 1));
				}
				return "";

			default:
				return "|darkred|unknown command '" + args[0] + "'";

		}

		// return "|darkred|unknown command";

	}

	private string removeFlags(string[] args) {
		string retstring = "";
		for (int i = 1; i < args.Length - 1; i ++) {
			if (!args[i].Contains('-')) {
				retstring += args[i];
				if (i != args.Length - 2) retstring += " ";
			}
		}
		return retstring;
	}

	private string parsePriv(string[] args, int argv) {

		foreach((string, int, string) cmnd in commandList) {
			if (args[0] == cmnd.Item1) {
				if ((argv - 1) < cmnd.Item2) {
					return "Too few arguments for command " + cmnd.Item1 + "! '" + cmnd.Item3 + "'";
				}
			}
		}

		switch(args[0]) {

			case "help":
			case "h":
			case "?":
				foreach((string, int, string) cmnd in commandList) {
					game.input.print(cmnd.Item1 + ": usage => '" + cmnd.Item3 + "'");
				}
				return "";

			case "detailed":
				game.status.toggleDetailedMode();
				return "Toggled detailed character readings.";

			case "fight":
				if (game.State != Game.GameState.Fight && game.State == Game.GameState.Idle) {
					var random = new System.Random();
					game.currentEnemy = game.enemiesList[random.Next(0, game.enemiesList.Count - 1)];
					game.State = Game.GameState.Fight;
					return "Entering fight!!!";
				}
				else {
					return game.fight.playerFights(args);
				}

			case "shop":
				if (game.State != Game.GameState.Idle) {
					return "Can only enter shop while idle!";
				}
				if (game.State != Game.GameState.Shop) {
					game.State = Game.GameState.Shop;
					return "Entering shop.";
				}
				else {
					return "Already shopping!";
				}

			case "sell":
				if (game.State != Game.GameState.Shop) {
					return "Who are you trying to sell to?";
				}
				Item itemtosell = game.status.inventory.Find(Item => WriteParser.getStringFrom(Item.name).ToLower() == String.Join(' ', args, 1, args.Length - 1).ToLower());
				if (itemtosell != null) {
					game.status.inventory.Remove(itemtosell);
					game.status.payBytes((int)(itemtosell.price * 0.6f), 5);
					return "You sold " + itemtosell.name + " for |magenta|" + (int) ((float)itemtosell.price * 0.6f) + "|white|.|darkmagenta|50|white|B$.";
				} else {
					return "That item is not in your inventory.";
				}

			case "buy":
				if (game.State != Game.GameState.Shop) {
					return "There is nothing to buy here.";
				}
				Item itemret = game.currentShop.wares.Find(Item => WriteParser.getStringFrom(Item.name).ToLower() == String.Join(' ', args, 1, args.Length - 1).ToLower());
				if (itemret != null) {
					if (game.status.byteCoin >= itemret.price) {
						game.status.inventory.Add(itemret);
						game.status.payBytes(-itemret.price, 0);
						game.currentShop.wares.Remove(itemret);
						return "You bought " + itemret.name + "!";
					} else {
						return "You can't afford " + itemret.name + "!";
					}
				}
				else {
					switch(args[1]) {
						case "any":
							return "You can't expect the game to make choices for you. Write the name of the item you wish to purchase.";

						case "none":
							return "What? Your call I guess.";

						case "all":
							int price = 0;
							foreach(Item it in game.currentShop.wares) {
								price += it.price;
							}
							game.input.print("That'll be |magenta|" + price + "|white|B$, are you sure?");
							string answer = game.input.getString().ToLower();
							switch (answer) {
								case "yes":
								case "ya":
								case "y":
								case "yea":
								case "yeah":
									if (game.status.byteCoin < price) {
										return "You can't afford all this!";
									}
									else {
										foreach(Item it in game.currentShop.wares) {
											game.status.inventory.Add(it);
											game.currentShop.wares = new List<Item>();
										}
									}
									return "Purchased all items from the shop! Wowza!";

								case "no":
								case "nah":
								case "n":
								case "nope":
									return "You wisely decided not to buy everything at once.";

								case "i cant afford that":
								case "i can't afford that":
								case "too poor":
									return "How wise of you.";

								default:
									return "What was that? We're going to assume you meant no.";
							}
						}
					try {
						int invIndex = int.Parse(args[1]);
						try {
							Item itemfoundInd = game.currentShop.wares[invIndex - 1];
							if (game.status.byteCoin >= itemfoundInd.price) {
								game.status.inventory.Add(itemfoundInd);
								game.status.payBytes(-itemfoundInd.price, 0);
								game.currentShop.wares.Remove(itemfoundInd);
								return "You bought " + itemfoundInd.name + "!";
							} else {
								return "You can't afford " + itemfoundInd.name + "!";
							}
						} catch {
							return "Unknown item index, look at the side menu to find the item indeces.";
						}
					} catch {
						return "Don't know that one. Check to see if you spelt the name of the item right. Use 'inv' to open your inventory";
					}
				}

			case "run":
				if (game.State == Game.GameState.Fight) {
					game.Run();
					return "Running from fight...";
				}
				if (game.State == Game.GameState.Shop) {
					game.State = Game.GameState.Idle;
					return "You run out of the shop.";
				}
				if (game.State != Game.GameState.Fight) {
					return "Where are you trying to run to?";
				}
				return "You ran, but where to?";

			case "use":
				Item itemfound = game.status.inventory.Find(Item => WriteParser.getStringFrom(Item.name).ToLower() == String.Join(' ', args, 1, args.Length - 1).ToLower());
				if (itemfound != null) {
					game.status.inventory.Remove(itemfound);
					game.Use(itemfound);
					return "You use " + itemfound.name + "!!!";
				}
				else {
					if (game.itemsList.Find(Item => WriteParser.getStringFrom(Item.name).ToLower() == String.Join(' ', args, 1, args.Length - 1).ToLower()) != null) {
						return "That item isn't in your inventory.";
					} else {
						try {
							int invIndex = int.Parse(args[1]);
							try {
								Item itemfoundIndex = game.status.inventory[invIndex - 1];
								game.status.inventory.Remove(itemfoundIndex);
								game.Use(itemfoundIndex);
								return "You use " + itemfound.name + "!!!";
							} catch {
								return "Unknown item index, check item indeces using 'inv' command.";
							}
						} catch {
							return "Don't know that one. Check to see if you spelt the name of the item right. Use 'inv' to open your inventory";
						}
					}
				}

			case "back":
			case "exit":
				if (game.State == Game.GameState.Fight) {
					return "You can't exit a fight... did you mean 'run?'";
				}
				if (game.State == Game.GameState.Shop) {
					game.State = Game.GameState.Idle;
					return "You leave the shop.";
				}
				if (game.State != Game.GameState.Fight) {
					return "You look around for exits but there are none.";
				}
				return "|red|You can never exit.|white|";

			case "me":
			case "m":
				game.input.print("Health: " + (game.status.health < 5 ? "|red|" : "|cyan|") + game.status.health + "|white|/|gray|" + game.status.maxHealth);
				game.input.print("Resistance: " + game.status.resistance);
				game.input.print("Damage: " + game.status.damage);
				game.input.print("Evasion: " + game.status.evasion);
				game.input.print("Speed: " + game.status.speed);
				return "";

			case "inv":
				if (game.status.inventory.Count < 1) {
					return "Inventory empty!";
				}
				int index = 1;
				foreach(Item it in game.status.inventory) {
					game.input.print(index.ToString() + ": " + it.name + "|white|");
					index ++;
				}
				return "";

			case "q":
			case "quit":
				game.running = false;
				break;

			case "cls":
				System.Console.Clear();
				return "";

			case "box":
				game.input.drawBox(String.Join(" ", args).Remove(0, args[0].Length + 1));				
				return "";

			default:
				return "|red|unknown command '" + args[0] + "'";

		}

		return "";

	}

	private bool hasFlags(string[] args) {
		foreach(string str in args) {
			if (str.Contains('-')) {
				return true;
			}
		}
		return false;
	}

	private string[] getFlags(string[] args) {
		List<string> array = new List<string>();
		foreach(string str in args) {
			if (str.Contains('-')) {
				array.Add(str.Split('-',2)[1]);
			}
		}
		if (array.Count < 1) {
			array.Add("NO FLAGS");
		}
		return array.ToArray();
	}


}