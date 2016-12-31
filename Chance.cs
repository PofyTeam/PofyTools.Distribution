namespace PofyTools.Distribution
{
	using UnityEngine;
	using System.Collections;

	/// <summary>
	/// Distribution based on chance with optional Auto Deck Size
	/// </summary>
	public class Chance
	{
		
		private bool _autoDeckSize = true;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PofyTools.Distribution.Chance"/> auto deck size.
		/// </summary>
		/// <value><c>true</c> if auto deck size; otherwise, <c>false</c>.</value>
		public bool autoDeckSize {
			get{ return this._autoDeckSize; }
			set {
				if (value != this._autoDeckSize)
					this._autoDeckSize = value;
				else
					Debug.LogWarning ("Chance: Auto Deck size is already " + value + ".");
			}
		}

		[Range (0f, 1f)] private float _chance = 0;

		/// <summary>
		/// Gets or sets the chance (0 - 1).
		/// </summary>
		/// <value>The chance.</value>
		public float chance {
			get{ return this._chance; }
			set {
				if (value != this._chance) {
					this._chance = value;
					BuildDeck ();
				}

			}
		}

		public float percent {
			get{ return this._chance * 100; }
		}

		private Deck<bool> _deck;

		/// <summary>
		/// Gets total card count in distribution deck.
		/// </summary>
		/// <value>The count.</value>
		public int Count {
			get{ return this._deck.Count; }
		}

		public bool Value {
			get{ return this._deck.PickNextCard ().instance; }
		}

		public bool RandomValue {
			get {
				return Random.Range (0f, 1f) < this._chance;
			}
		}

		void Initialize ()
		{
			BuildDeck ();
		}

		void BuildDeck ()
		{
			int deckSize = 0;
			float percent = this._chance * 100;
			if (this._autoDeckSize) {
				if (percent % 100 == 0) {
					deckSize = 1;
				} else if (percent % 50 == 0) {
					deckSize = 2;
				} else if (percent % 25 == 0) {
					deckSize = 4;
				} else if (percent % 20 == 0) {
					deckSize = 5;
				} else if (percent % 10 == 0) {
					deckSize = 10;
				} else if (percent % 5 == 0) {
					deckSize = 20;
				} else if (percent % 4 == 0) {
					deckSize = 25;
				} else if (percent % 2 == 0) {
					deckSize = 50;
				} else if (percent % 1 == 0) {
					deckSize = 100;
				} else {
					deckSize = 1000;
				}

			} else {
				deckSize = 1000;
			}

			this._deck = new Deck<bool> (deckSize);
			int trueCount = (int)(this._chance * deckSize);
			int falseCount = deckSize - trueCount;

			if (trueCount > 0)
				this._deck.AddCard (new Deck<bool>.Card (true, trueCount));
			if (falseCount > 0)
				this._deck.AddCard (new Deck<bool>.Card (false, falseCount));

			this._deck = this._deck.CreateDistributionDeck ();
		}

		public Chance () : this (1f)
		{
		}

		public Chance (float chance) : this (chance, true)
		{
		}

		public Chance (float chance, bool autoDeckSize)
		{
			this._chance = chance;
			this._autoDeckSize = autoDeckSize;

			Initialize ();
		}
	}
}