using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PofyTools.Distribution
{
	public interface IClonable
	{
		IClonable Clone (IClonable source);
	}

	/// <summary>
	/// Component deck. Support deep copying of cards for populating.
	/// </summary>
	[System.Serializable]
	public class ComponentDeck<T> where T : Component
	{
		[System.Serializable]
		public class Card
		{
			protected T _instance = null;

			public T instance {
				get{ return this._instance; }
			}

			protected int _weight = 0;

			public int weight {
				get{ return this._weight; }
			}

			public override string ToString ()
			{
				return string.Format ("[Card: instance={0}, weight={1}]", instance, weight);
			}

			#region IClonable implementation

			public IClonable Clone (IClonable source)
			{
				Card clone = new Card (source as Card);
				return clone as IClonable;
			}

			#endregion

			public Card ()
			{

			}

			public Card (T instance, int weight)
			{
				this._instance = instance;
				this._weight = weight;
			}

			public Card (Card card)
			{
				this._instance = GameObject.Instantiate<T> (card.instance);
				this._weight = card.weight;
			}
		}

		protected bool _isShuffled = false;

		public bool isShuffled {
			get{ return this._isShuffled; }
		}

		protected List<Card> _cards = new List<Card> ();
		protected int _head;

		public int Head {
			get {
				if (this.Count > 0)
					return this._head;
				else
					return -1;
			}
		}

		public int Count {
			get{ return this._cards.Count; }
		}

		protected int _maxWeight = -1;

		public int MaxWeight {
			get {
				if (this._maxWeight == -1) {
					this._maxWeight = GetMaxWeight ();
				}

				return this._maxWeight;
			}
		}

		protected int GetMaxWeight ()
		{
			int result = -1;
			foreach (var card in this._cards) {
				result = Mathf.Max (result, card.weight);
			}

			return result;
		}

		public ComponentDeck<T> ShuffleDeck ()
		{
			this._head = 0;

			while (this._head < this.Count) {
				int randomIndex = Random.Range (this._head, this.Count);
				Card randomCard = this._cards [randomIndex];
				this._cards.RemoveAt (randomIndex);
				this._cards.Insert (this._head, randomCard);

				++this._head;
			}

			this._isShuffled = true;

			this._head = 0;
			return this;
		}

		public bool ContainsCard (Card card)
		{
			return this._cards.Contains (card);
		}

		public void AddCard (Card card)
		{
			this._cards.Add (card);
			if (card.weight > this.MaxWeight) {
				this._maxWeight = card.weight;
			}
		}

		public bool ContainsInstance (T instance)
		{
			foreach (var card in this._cards) {
				if (card.instance == instance)
					return true;
			}
			return false;
		}

		public Card PickNextCard ()
		{
			Card card = this._cards [this.Head]; 
			++this._head;

			if (this.Head == this.Count) {
				ShuffleDeck ();	
			}

			return card;
		}

		public Card PickBiasCard (int minWeight = 0)
		{
			minWeight = Mathf.Min (minWeight, this.MaxWeight);
			Card biasCard = null; //TODO: Continue from here...
			while (biasCard == null) {
				foreach (var card in this._cards)
					if (card.weight >= minWeight)
						biasCard = card;
			}
			return biasCard;
		}

		public ComponentDeck<T> CreateDistributionDeck (int weightMultiplier = 1)
		{
			int allocatedCount = Math.ArithmeticSequenceSum (this.Count);
			Debug.LogWarningFormat ("allocatedCount: {0}", allocatedCount);

			ComponentDeck<T> distributionDeck = new ComponentDeck<T> (allocatedCount);

			foreach (var card in this._cards) {
				int totalNumberOfCopies = (this.MaxWeight - card.weight + 1) * weightMultiplier;
				while (totalNumberOfCopies > 0) {
					Card copy = new Card (card);
					distributionDeck._cards.Add (copy);
					--totalNumberOfCopies;
				}
			}

			distributionDeck.ShuffleDeck ();

			return distributionDeck;
		}

		public ComponentDeck ()
		{
			
		}

		public ComponentDeck (int size)
		{
			this._cards = new List<Card> (size);
		}
	}
}