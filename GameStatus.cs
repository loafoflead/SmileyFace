using System.Collections.Generic;
using System.Collections;

using System;

using static Elements;
using static WriteParser;

public class GameStatus {

	private Game g;

	/* the base threshold before which a level up happens, which is multiplied by the current level */
	public static int levelUpThreshold = 10;
	/* the amount by which the level is divided then added to one to multipy by the base stats */
	private static int levelUpDivisionPercentage = 3; // 2 is quite good havn't tested 3 yet but seems fine, need to tweak the rates at which things change	
	/* whether or not to show detailed readings to the player */
	private bool detailedMode = false; 

	public List<Item> inventory;

	public GameStatus(Game gm) {
		inventory = new List<Item>();
		g = gm;	
	}

	public void loadSave(string filename = "p") {
		if (filename == "p") {
			maxHealth = 10;
			health = 10;
			baseDamage = 1;
			baseSpeed = 1;
			baseEvasion = 1;
			baseResistance = 0;

			byteCoin = 10;
			byteCents = 0;
			byteAmethyts = 0;

			level = 0;
			xp = 0;
			//inventory.Add(new Item("|green|potion", "a potion", getEffect(EffectTypes.InstHealth, 1, 1)));
		}
		resetStats();
	}

	/*public void resetStats() {
		damage = baseDamage;
		speed = baseSpeed;
		evasion = baseEvasion;
		resistance = baseResistance;
	}*/

	public void resetStats() {
		damage = baseDamage * (1 + level / levelUpDivisionPercentage);
		speed = baseSpeed * (1 + level / levelUpDivisionPercentage);
		evasion = baseEvasion * (1 + level / levelUpDivisionPercentage);
		resistance = baseResistance * (1 + level / levelUpDivisionPercentage);
		effectEffects();
	}

	private void effectEffects() {}

	public void levelUp() {
		if (this.level == 99) {
			payBytes(99, 0);
			return;
		}
		level ++;
		damage = baseDamage * (1.0f + level / levelUpDivisionPercentage);
		speed = baseSpeed * (1f + level / levelUpDivisionPercentage);
		evasion = baseEvasion * (1f + level / levelUpDivisionPercentage);
		resistance = baseResistance * (1f + level / levelUpDivisionPercentage);
	}

	public int dealDamage(float damage) {
		float dmg = damage;
		if (this.resistance != 0) {
			dmg *= -this.resistance;
			if (dmg <= 0) {
				return 0;
			}
		}
		this.health -= dmg;
		if (this.health < 0) {
			this.health = 0;
			return 2;
		}
		return 1;
	}

	public int addXp(int xp) {
		int xps = xp;
		int levelsUp = 0;
		while (xps > 0) {
			this.xp ++;
			xps --;
			if (this.xp > (level == 0 ? 1 : level) * levelUpThreshold) {
				levelUp();
				levelsUp ++;
				if (this.level == 99) {
					levelsUp = -1;
				}
				this.xp = 0;
			}
		}
		return levelsUp;
	}

	public int payBytes(int coins, int cents) {
		int cts = cents;
		byteCoin += coins;
		while (cts > 0) {
			byteCents += 1;
			cts --;
			if (byteCents >= 10) {
				byteCoin ++;
				byteCents = 0;
			}
		}
		return 1;
	}

	public void toggleDetailedMode() {
		if (this.detailedMode == true) {
			this.detailedMode = false;
		} else {
			this.detailedMode = true;
		}
	}

	public void printReadouts() {

		string finalString = "";

		if (detailedMode == false) {

			finalString += "Health: " + (health < 5 ? "|red|" : "|cyan|") + (int)health + "|white|/|gray|" + maxHealth + "\n";
			finalString += "Resistance: " + (int)resistance + "\n";
			finalString += "Damage: " + (int)damage + "\n";
			finalString += "Evasion: " + (int)evasion + "\n";
			finalString += "Speed: " + (int)speed + "\n";

			finalString += "ByteCoin: |magenta|" + byteCoin + "|white|.|darkmagenta|" + byteCents + "\n";
			finalString += "LVL: |red|" + this.level + "|white| [xp:|darkred|" + this.xp + "|white|]\n";

		} else {

			finalString += "Health: " + (health < 5 ? "|red|" : "|cyan|") + health + "|white|/|gray|" + maxHealth + "\n";
			finalString += "Resistance: " + resistance + "\n";
			finalString += "Damage: " + damage + "\n";
			finalString += "Evasion: " + evasion + "\n";
			finalString += "Speed: " + speed + "\n";

			finalString += "ByteCoin: |magenta|" + byteCoin + "|white|.|darkmagenta|" + byteCents + "\n";
			finalString += "LVL: |red|" + this.level + "|white| [xp:|darkred|" + this.xp + "|white|]\n";

		}


		//finalString += "#####\n#   #\n#####\n";

		g.input.drawBox(finalString, g.input.xLimit + 2, g.ypos);

	}

	public int byteCoin = 0;
	public int byteCents = 0;
	public int byteAmethyts = 0;

	public int xp;
	public float level;

	public float health;
	public int maxHealth;

	public float damage;
	public float speed;
	public float evasion;
	public float resistance;

	public float baseDamage;
	public float baseSpeed;
	public float baseEvasion;
	public float baseResistance;



}

public class Shop {

	public List<Item> wares;

	public Shop(List<Item> itemsAvailable, int items = 3) {
		wares = new List<Item>();
		var random = new Random();
		int length = 0;
		while(length < 3) {
			Item it = itemsAvailable[random.Next(0, itemsAvailable.Count)];
			if (random.Next(0, 100) < it.Rarity) {
				if (random.Next(1, 500) < 250) {
					wares.Add(it);
					length ++;
				}
			}
		}
	}

	public Item buyFrom(Item it) {
		if (wares.Contains(it)) {
			wares.Remove(it);
			return it;
		}
		return new Item("", "", getEffect(EffectTypes.Nullify, 0, 0));
	}

}

public class Item {

	private int rarityInternal;
	public int Rarity {
		get {
			return rarityInternal;
		}
		set {
			if (value < 0) {
				rarityInternal = 1;
			} else if (value > 100) {
				rarityInternal = 99;
			} else {
				rarityInternal = 50;
			}
		}
	}

	public int price;

	public string name;
	public string tag;
	public string description;
	public Effect effect_given;

	public Item(string n, string desc, Effect effect) {
		this.name = n;
		this.tag = this.name.ToLower().Replace(' ', '_');
		this.tag = WriteParser.getStringFrom(this.tag);
		this.description = desc;
		this.effect_given = effect;
		this.Rarity = 50;
	}

	public Item(string n, string desc, Effect effect, int rarity, int price) {
		this.name = n;
		this.tag = this.name.ToLower().Replace(' ', '_');
		this.tag = WriteParser.getStringFrom(this.tag);
		this.description = desc;
		this.effect_given = effect;
		this.Rarity = rarity;
		this.price = price;
	}

}