using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PofyTools
{
	public interface IShufflable
	{
		bool isShuffled {
			get;
		}

		void Shuffle ();
	}

	/// <summary>
	/// Generic Deck. Deck contains cards that contain references to the instances of the teplated type.
	/// </summary>
	[System.Serializable]
	public class Deck<T> : IShufflable
	{
		[System.Serializable]
		public class Card
		{
			protected T _instance = default(T);

			public T instance {
				get{ return this._instance; }
			}

			protected int _weight = 0;

			public int weight {
				get{ return this._weight; }
				set{ this._weight = Mathf.Max (1, value); }
			}

			public override string ToString ()
			{
				return string.Format ("[Card: instance={0}, weight={1}]", instance, weight);
			}

			public Card (T instance, int weight = 1)
			{
				this._instance = instance;
				this._weight = weight;
			}

			public Card (Card card)
			{
				this._instance = card.instance; 
				this._weight = card.weight;
			}
		}

        #region State
        public enum State : int
        {
            Empty = 0,
            Initialized = 1,
            Populated = 2,
            Shuffled = 3,
        }

        protected State _state = State.Empty;
        public State state { get { return this._state; } }
        #endregion

        #region IShufflable implementation

        //protected bool _isShuffled = false;

		/// <summary>
		/// Gets a value indicating whether this <see cref="PofyTools.Deck`1"/> is shuffled.
		/// </summary>
		/// <value><c>true</c> if is shuffled; otherwise, <c>false</c>.</value>
		public bool isShuffled {
			get{ return this._state == State.Shuffled; }
		}

		/// <summary>
		/// Shuffles this Deck.
		/// </summary>
		public void Shuffle ()
		{
			this._head = 0;

			while (this._head < this.Count) {
				int randomIndex = Random.Range (this._head, this.Count);
				Card randomCard = this._cards [randomIndex];
				this._cards.RemoveAt (randomIndex);
				this._cards.Insert (this._head, randomCard);

				++this._head;
			}

            this._state = State.Shuffled;

			this._head = 0;
		}

		#endregion

		protected List<Card> _cards = new List<Card> ();

		public List<Card> Cards {
			get{ return this._cards; }
		}

		protected int _head;

		/// <summary>
		/// Gets the currenct position in the deck or -1 if the Deck is empty.
		/// </summary>
		/// <value>The head.</value>
		public int Head {
			get {
				if (this.Count > 0)
					return this._head;
				else
					return -1;
			}
		}

		/// <summary>
		/// Gets the total count of<see cref="PofyTools.Deck.Card"/>in the Deck.
		/// </summary>
		/// <value>The count.</value>
		public int Count {
			get{ return this._cards.Count; }
		}

		protected int _maxWeight = int.MinValue;

		/// <summary>
		/// Returns cached max weight or gets the max weight present in the Deck or <c>int.MinValue</c> if empty.
		/// </summary>
		/// <value>The max weight.</value>
		public int MaxWeight {
			get {
				if (this._maxWeight == int.MinValue) {
					this._maxWeight = GetMaxWeight ();
				}

				return this._maxWeight;
			}
		}

		protected int GetMaxWeight ()
		{
			int result = int.MinValue;
			for (int i = 0, max_cardsCount = this._cards.Count; i < max_cardsCount; i++) {
				var card = this._cards [i];
				result = Mathf.Max (result, card.weight);
			}

			return result;
		}

		protected int _minWeight = int.MaxValue;

		/// <summary>
		/// Returns cached min weight or gets the min weight present in the Deck or <c>int.MaxValue</c> if empty.
		/// </summary>
		/// <value>The min weight.</value>
		public int MinWeight {
			get {
				if (this._minWeight == int.MaxValue) {
					this._minWeight = GetMinWeight ();
				}

				return this._minWeight;
			}
		}

		protected int GetMinWeight ()
		{
			int result = int.MaxValue;
			for (int i = 0, max_cardsCount = this._cards.Count; i < max_cardsCount; i++) {
				var card = this._cards [i];
				result = Mathf.Min (result, card.weight);
			}

			return result;
		}


		/// <summary>
		/// Returns whether the <see cref="PofyTools.Deck.Card"/>instance is present in the Deck.
		/// </summary>
		/// <returns><c>true</c>, if the Card instance is present in the Deck, <c>false</c> otherwise.</returns>
		/// <param name="card">Card.</param>
		public bool ContainsCard (Card card)
		{
			return this._cards.Contains (card);
		}

		/// <summary>
		/// Adds the card to the Deck. Sets the Deck's Max Weight to the Card's weight if greater than current Max Weight.
		/// </summary>
		/// <param name="card">Card.</param>
		public void AddCard (Card card)
		{
			this._cards.Add (card);
			if (card.weight > this.MaxWeight) {
				this._maxWeight = card.weight;
			} else if (card.weight < this.MinWeight) {
				this._minWeight = card.weight;
			}
		}

		/// <summary>
		/// Creates and adds the card freom the instance provided with the weight provided (default 1).
		/// </summary>
		/// <param name="instance">Instance.</param>
		/// <param name="weight">Weight.</param>
		public Card CreateAndAddCard (T instance, int weight = 1)
		{
			Card card = new Card (instance, weight);
			AddCard (card);
			return card;
		}

		public void CreateAndAddIdentityCard (T instance, int weight = 1)
		{
			Card identityCard = CreateAndAddCard (instance, weight);

			SetIdentityCard (identityCard);
		}

		public void RemoveCard (Card card)
		{
			this._cards.RemoveAll (c => c == card);
			this._maxWeight = GetMaxWeight ();
			this._minWeight = GetMinWeight ();
			//TODO: collect extremes in one iteration
		}

		public void RemoveInstanceCard (T instance)
		{
			this._cards.RemoveAll (c => (object)c.instance == (object)instance);
			this._maxWeight = GetMaxWeight ();
			this._minWeight = GetMinWeight ();
			//TODO: collect extremes in one iteration
		}

		/// <summary>
		/// Returns whether the Deck contains the Card with the instance.
		/// </summary>
		/// <returns><c>true</c>, if instance was containsed, <c>false</c> otherwise.</returns>
		/// <param name="instance">Instance.</param>
		public bool ContainsInstance (T instance)
		{
			for (int i = 0, max_cardsCount = this._cards.Count; i < max_cardsCount; i++) {
				var card = this._cards [i];
				if ((object)card.instance == (object)instance)
					return true;
			}
			return false;
		}

		public bool ContainsInstance (T instance, out Card instanceCard)
		{
			for (int i = 0, max_cardsCount = this._cards.Count; i < max_cardsCount; i++) {
				var card = this._cards [i];
				if ((object)card.instance == (object)instance) {
					instanceCard = card;
					return true;
				}
			}
			instanceCard = null;
			return false;
		}

		public Card FindInstanceCard (T instance)
		{
			Card card = null;
			if (ContainsInstance (instance, out card)) {
				Debug.Log ("Instance card found!");
				return card;
			}
			Debug.Log ("Instance card not found!");
			return card;
		}

		public Card PickInstanceCard (T instance)
		{
			Card instanceCard = null;
			for (int i = this._head, max_cardsCount = this._cards.Count; i < max_cardsCount; i++) {
				var card = this._cards [i];
				if ((object)card.instance == (object)instance) {
					instanceCard = PickCardAt (i);
					break;
				}
			}
			return instanceCard;
		}

		protected Card PickCardAt (int index)
		{
			Card resultCard = this._cards [index];

			if (index != 0) {
				this._cards.RemoveAt (index);
				this._cards.Insert (0, resultCard);
			}
			++this._head;
			if (this.Head == this.Count) {
				Shuffle ();	
			}
			return resultCard;
		}

		/// <summary>
		/// Picks the Card on the Head position and moves the Head to next position.
		/// If Head gets to the end of the Deck, the Deck gets reshuffled. 
		/// </summary>
		/// <returns>The next card.</returns>
		public Card PickNextCard ()
		{
			Card card = this._cards [this.Head]; 
			++this._head;

			if (this.Head == this.Count) {
				Shuffle ();	
			}

			return card;
		}

		/// <summary>
		/// Picks the first card with minWeight or higher and removes it from the
		/// </summary>
		/// <returns>The bias card.</returns>
		/// <param name="minWeight">Minimum weight.</param>
		public Card PickBiasCard (int minWeight = 0)
		{
			minWeight = Mathf.Min (minWeight, this.MaxWeight);

			Card biasCard = null;
			for (int i = this._head, max_cardsCount = this._cards.Count; i < max_cardsCount; ++i) {
				var card = this._cards [i];
				if (card.weight >= minWeight) {
					biasCard = PickCardAt (i);
					break;
				}
			}
				
			return biasCard;
		}

		//TODO: different distribution types
		public Deck<T> CreateDistributionDeck ()
		{
			Deck<T> distributionDeck = new Deck<T> ();
			foreach (var card in this._cards) {
				card.weight = Mathf.Max (1, card.weight);
				int totalNumberOfCopies = Mathf.RoundToInt ((float)this.MaxWeight / (float)card.weight);
				AlertCanvas.Instance.alert (string.Format ("{0} : {1}", card.instance, totalNumberOfCopies), AlertPanel.Type.INFO);
				while (totalNumberOfCopies > 0) {
					Card copy = new Card (card);
					distributionDeck._cards.Add (copy);
					--totalNumberOfCopies;
				}
			}

			distributionDeck.Shuffle ();

			return distributionDeck;
		}

		#region Identity

		protected Card _identityCard = null;

		public Card IdentityCard {
			get{ return this._identityCard; }
		}

		public bool hasIdentityCard {
			get{ return this._identityCard != null; }
		}

		public bool IsIdentityCard (Card card)
		{
			return card == this._identityCard;
		}

		public void SetIdentityCard (Card card)
		{
			if (!this.hasIdentityCard)
				this._identityCard = card;
			else if (IsIdentityCard (card))
				Debug.LogWarningFormat ("Deck: card {0} is already identity card!", card);
			else
				Debug.LogWarningFormat ("Deck: Identity card already set!");
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="PofyTools.Deck`1"/> class.
		/// </summary>
		public Deck ()
		{
			this._cards = new List<Card> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PofyTools.Deck`1"/> class.
		/// </summary>
		/// <param name="capacity">Capacity of the card list.</param>
		public Deck (int capacity)
		{
			this._cards = new List<Card> (capacity);
		}

		public Deck (T[] instances)
		{
			this._cards = new List<Card> (instances.Length);

			for (int i = 0, instancesLength = instances.Length; i < instancesLength; i++) {
				var instance = instances [i];
				CreateAndAddCard (instance);
			}
		}

		public Deck (List<Card> cards)
		{
			this._cards = new List<Card> (cards);
		}

		public Deck (Deck<T> source)
		{
			this._cards = new List<Card> (source._cards);
			this._head = source._head;
            //this._isShuffled = source._isShuffled;
            this._state = source._state;
			this._maxWeight = source._maxWeight;
			this._minWeight = source._minWeight;
			if (source.hasIdentityCard)
				this._identityCard = FindInstanceCard (source._identityCard.instance);
		}
	}
}

