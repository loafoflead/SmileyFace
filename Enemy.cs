using System;
using System.Data.SQLite;

using System.Collections;
using System.Collections.Generic;

using static Elements;

public class Enemy {

	public int id;
	public string name;
	public string tag;
	public string description;

	public int coinsGiven;
	public int xpGiven;

	public int maxHp;
	public float Hp;

	public int Dmg;

	public int baseSpeed;
	public int baseDmg;
	public int baseEvasion;
	public int baseStrength;
	public int baseResistance;

	public int Speed;
	public int Evasion;
	public int Strength;
	public int Resistance;


	public List<Effect> currentEffects;

	public List<Effect> effectsImmune;

	public Enemy() { }

	public Enemy(int basedmg, int basestr, int basespd,
		int baseevas, int baseres, int maxhp,
		string name, string desc, string effectimmune, int coinsGiven, int xpGiven) {
		this.baseDmg = basedmg;
		this.baseSpeed = basespd;
		this.baseStrength = basestr;
		this.baseEvasion = baseevas;
		this.baseResistance = baseres;

		this.coinsGiven = coinsGiven;
		this.xpGiven = xpGiven;

		this.maxHp = maxhp;
		this.Hp = maxhp;

		this.name = name;
		this.tag = name.ToLower().Replace(" ", "_");
		this.description = desc;

		this.currentEffects = new List<Effect>();
		this.effectsImmune = new List<Effect>();
		if (!string.IsNullOrEmpty(effectimmune) || !effectimmune.Contains('/')) {
			foreach(string h in effectimmune.Split("/")) {
				var values = Enum.GetValues(typeof(EffectTypes));
				foreach(var enumm in values) {
					if (h == enumm.ToString()) {
						effectsImmune.Add(getEffect((EffectTypes) enumm, 1, 1));
					}
				}
			}
		}

		this.reset();
	}


	public void reset() {
		currentEffects = new List<Effect>();
		this.Hp = this.maxHp;
		this.Dmg = this.baseDmg;
		this.Resistance = this.baseResistance;
		this.Speed = this.baseSpeed;
		this.Strength = this.baseStrength;
		this.Evasion = this.baseEvasion;
	}

	public void Update() {
		updateEffects();
	}

	public void updateEffects() {
		foreach(Effect eff in currentEffects) {
			eff.turnsLasted ++;
			if (eff.turnsLasted >= eff.duration) {
				resetStat(eff);
				currentEffects.Remove(eff);
			} else {
				executeEffect(eff);
			}
		}
	}



















	public void resetStat(Effect eff) {
		switch (eff.effectType) {
			case EffectTypes.Slowness:
			case EffectTypes.Speed:
				this.Speed = this.baseSpeed;
				break;

			case EffectTypes.Evasion:
			case EffectTypes.Clumsiness:
				this.Evasion = this.baseEvasion;
				break;

			case EffectTypes.Strength:
			case EffectTypes.Weakness:
				this.Strength = this.baseStrength;
				break;

			case EffectTypes.Resistance:
			case EffectTypes.Brittleness:
			case EffectTypes.Invulnerability:
				this.Resistance = this.baseResistance;
				break;

			default:
				break;

		}
	}



	public void executeEffect(Effect eff) {
		switch (eff.effectType) {
			case EffectTypes.InstHealth:
				this.Hp += (int) eff.value * (eff.level * levelMultiplier);
				break;

			case EffectTypes.InstDmg:
				this.Hp -= (int) eff.value * (eff.level * levelMultiplier);
				break;

			case EffectTypes.Poison:
				this.Hp -= (int) eff.value * (eff.level * levelMultiplier);
				break;

			case EffectTypes.Regeneration:
				this.Hp += (int) eff.value * (eff.level * levelMultiplier);
				break;

			case EffectTypes.Slowness:
				this.Speed = (int) (this.baseSpeed -  eff.value * (eff.level * levelMultiplier));
				break;

			case EffectTypes.Speed:
				this.Speed = (int) (this.baseSpeed +  eff.value * (eff.level * levelMultiplier));
				break;

			case EffectTypes.Evasion:
				this.Evasion = (int) (this.baseEvasion +  eff.value * (eff.level * levelMultiplier));
				break;

			case EffectTypes.Clumsiness:
				this.Evasion = (int) (this.baseEvasion -  eff.value * (eff.level * levelMultiplier));
				break;

			case EffectTypes.Strength:
				this.Strength = (int) (this.baseStrength +  eff.value * (eff.level * levelMultiplier));
				break;

			case EffectTypes.Weakness:
				this.Strength  = (int) (this.baseStrength -  eff.value * (eff.level * levelMultiplier));
				break;

			case EffectTypes.Resistance:
				this.Resistance =  (int) (this.baseResistance +  eff.value * (eff.level * levelMultiplier));
				break;

			case EffectTypes.Brittleness:
				this.Resistance =  (int) (this.baseResistance -  eff.value * (eff.level * levelMultiplier));
				break;

			case EffectTypes.Invulnerability:
				this.Resistance = 100000000;
				break;

			case EffectTypes.Nullify:
				foreach(Effect effect in currentEffects) {
					resetStat(effect);
				}
				break;

			default:
				break;
		}
	}


	public float getAttack() {
		return baseDmg *= this.Strength;
	}

	public int attack(float damage) {
		float dmg = damage;
		if (this.Resistance != 0) {
			dmg *= -this.Resistance;
			if (dmg < 0) {
				return 0; // if resistance is too storng
			}
		}
		this.Hp -= dmg;
		if (this.Hp <= 0) {
			this.Hp = 0;
			return 2; // if dead
		}
		return 1; // if attack succeeded
	}


	public bool giveEffect(Effect eff) {
		foreach(Effect efff in effectsImmune) {
			if (efff.effectType == eff.effectType) {
				return false;
			}
		}
		if (eff == null) {
			return false;
		}
		currentEffects.Add(eff);
		return true;
	}

}