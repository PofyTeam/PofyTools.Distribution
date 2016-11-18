namespace PofyTools.Distribution
{
	using UnityEngine;
	using System.Collections;

	public class Chance
	{
		[Range (0f, 1f)] private float _chance = 0;

		public float chance {
			get{ return this._chance; }
			set {
				if (value != this._chance) {
					//rebuild deck;
					this._chance = value;
				}

			}
		}

		private Deck<bool> _deck;

		public bool Value {
			get{ return this._deck.PickNextCard ().instance; }
		}

		public bool RandomValue {
			get {
				return Random.Range (0, 1) < this._chance;
			}
		}

		void Initialize ()
		{
			
		}

		void BuildDeck ()
		{
			this._deck = new Deck<bool> (1000);
			int trueCount = (int)(this._chance * 1000);
			int falseCount = 1000 - trueCount;

			this._deck.AddCard (new Deck<bool>.Card (true, trueCount));
			this._deck.AddCard (new Deck<bool>.Card (false, falseCount));

			this._deck = this._deck.CreateDistributionDeck ();
		}

		public Chance () : this (1f)
		{
		}

		public Chance (float chance)
		{
			this._chance = chance;
		}
	}
}