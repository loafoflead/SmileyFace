using System;

public class Elements {

	public enum EffectTypes {
		InstHealth, // gain health
		InstDmg, // lose health 
		Poison, // lose health over time
		Regeneration, // gain health over time
		Slowness, // lower speed stat (less chance to be the first to attack)
		Speed, // higher speed stat (higher chace to attack first)
		Evasion, // dodge more often
		Clumsiness, // dodge less often
		Strength, // deal more damage
		Weakness, // causes you to deal less damage
		Invulnerability, // cases you to take no damage
		Resistance,	// causes you to take more damage
		Brittleness, // causes you to take more damage
		Nullify,
	}

	public static float levelMultiplier = 1.5f;

	public static Effect getEffect(EffectTypes type, int level, int duration) {
		switch (type) {
			case EffectTypes.InstHealth:
				return new Effect(type, level, 2, duration);
			case EffectTypes.Poison:
			case EffectTypes.Regeneration:
			case EffectTypes.Slowness:
			case EffectTypes.Speed:
			case EffectTypes.Evasion:
			case EffectTypes.Invulnerability:
			case EffectTypes.Resistance:
			case EffectTypes.Brittleness:
			case EffectTypes.Strength:
			case EffectTypes.Weakness:
			case EffectTypes.InstDmg:
				return new Effect(type, level, 2, duration);

			case EffectTypes.Nullify:
				return new Effect(type, 1, 1, duration);

			default:
				return new Effect(type, level, 2, duration);

		}	
	}

	public static Effect getEffectStr(string name, int lvl, int duration) {
		foreach(EffectTypes eft in Enum.GetValues(typeof(EffectTypes))) {
			if (eft.ToString() == name) {
				return Elements.getEffect(eft, lvl, duration);
			}
		}
		return Elements.getEffect(EffectTypes.Nullify, lvl, duration);
	}

	public class Effect {

		public Effect(EffectTypes type, int lvl, float val, int durat) {
			effectType = type;
			name = type.ToString();
			level = lvl;
			value = val;
			duration = durat;
		}
		public EffectTypes effectType;
		public string name;
		public int level;

		public float value;

		public int duration;
		public int turnsLasted;
	}

}