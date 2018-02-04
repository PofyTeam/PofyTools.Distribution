using System.Collections.Generic;

namespace PofyTools
{
	using UnityEngine;
	using System.Collections;

	public class Landmark : MonoBehaviour
	{
		public static int Layer;
		public static int Mask;

		#region State

		public enum State:int
		{
			None = 0,
			Initialized = 1,
			DataReady = 2,
			DoneCycle = 3,
			Invaded = 4
		}

		public State state;

		#endregion

		//Config
		public Range storeCount;
		public Range resourceFactor;
		public Range influenceFactor;

		//Calculated
		protected Range _settlerCount = default(Range);

		public int settlerCount {
			get{ return (int)this._settlerCount.current; }
			set{ this._settlerCount.current = value; }
		}

		protected float _unityFactor;
		protected float _unityChance;
		protected float _unityBias;
		protected float _influenceRadius;

		protected int _defensePoints;

		public int defensePoints {
			get{ return this._defensePoints; }
		}

		protected int _invasionPoints;

		public int invasionPoints {
			get{ return this._invasionPoints; }
		}

		protected Landmark _invadedBy = null;

		public Landmark invadedBy {
			get{ return this._invadedBy; }
		}

		[ContextMenu ("Initialize")]
		public virtual void Initialize ()
		{
			this.storeCount.current = this.storeCount.IntRandom;
			Debug.Log ("Store Count: " + this.storeCount.current);
			this.resourceFactor.current = this.resourceFactor.Random;
			Debug.Log ("Resource Factor: " + this.resourceFactor.current);
			this.influenceFactor.current = this.resourceFactor.Random;
			Debug.Log ("Influence Factor: " + this.influenceFactor.current);

			//for every store 10 settlers
			this._settlerCount = new Range (0, (int)this.storeCount.current * 10 + 2);
			this._settlerCount.current = this._settlerCount.IntRandom;
			Debug.Log ("Settlers Count: " + this._settlerCount.current);

			if (_settlerCount.current != 0) {
				Recalculate ();
			}

			Debug.LogFormat ("<color=green><b>Landmark {0} Initialized!</b></color>", this.name);
		}

		public void Recalculate ()
		{
			this._unityBias = Math.EvaluateConcave (a: 0.5f, b: 0.5f, t: this.influenceFactor.current);
			Debug.Log ("Unity Bias: " + this._unityBias);
			this._unityChance = 1 - Math.EvaluateLog (a: 1, n: 1, t: this._settlerCount.current);
			Debug.Log ("Unity Chance: " + this._unityChance);
			this._unityFactor = Mathf.Clamp01 (Random.Range (this._unityBias, this._unityBias + this._unityChance));
			Debug.Log ("Unity Factor: " + this._unityFactor);
			this._influenceRadius = this._settlerCount.current * this.influenceFactor.current * this._unityFactor * 10;
			Debug.Log ("Influence Radius: " + this._influenceRadius);
			this._defensePoints = (int)(this._settlerCount.current * this.resourceFactor.current * this._unityFactor * 10);
			Debug.Log ("Defense Points: " + this._defensePoints);
			this._invasionPoints = (int)(this._defensePoints * this.influenceFactor.current);
			Debug.Log ("Invasion Points: " + this._invasionPoints);
		}

		//		[ContextMenu ("Initialize Max")]
		//		public virtual void InitializeMax ()
		//		{
		//			this.storeCount.current = this.storeCount.max;
		//			Debug.Log ("Store Count: " + this.storeCount.current);
		//			this.resourceFactor.current = this.resourceFactor.max;
		//			Debug.Log ("Resource Factor: " + this.resourceFactor.current);
		//			this.influenceFactor.current = this.resourceFactor.max;
		//			Debug.Log ("Influence Factor: " + this.influenceFactor.current);
		//
		//			//for every store 10 settlers
		//			this._settlerCount = new Range (0, (int)this.storeCount.current * 10 + 2);
		//			this._settlerCount.current = this._settlerCount.max;
		//			Debug.Log ("Settlers Count: " + this._settlerCount.current);
		//
		//			if (_settlerCount.current != 0) {
		//				this._unityBias = Math.EvaluateConcave (a: 0.5f, b: 0.5f, t: this.influenceFactor.current);
		//				Debug.Log ("Unity Bias: " + this._unityBias);
		//				this._unityChance = 1 - Math.EvaluateLog (a: 1, n: 1, t: this._settlerCount.current);
		//				Debug.Log ("Unity Chance: " + this._unityChance);
		//				this._unityFactor = Mathf.Clamp01 (Random.Range (this._unityBias, this._unityBias + this._unityChance));
		//				Debug.Log ("Unity Factor: " + this._unityFactor);
		//				this._influenceRadius = this._settlerCount.current * this.influenceFactor.current * this._unityFactor * 10;
		//				Debug.Log ("Influence Radius: " + this._influenceRadius);
		//				this._defensePoints = (int)(this._settlerCount.current * this.resourceFactor.current * this._unityFactor * 10);
		//				Debug.Log ("Defense Points: " + this._defensePoints);
		//				this._invasionPoints = (int)(this._defensePoints * this.influenceFactor.current);
		//				Debug.Log ("Invasion Points: " + this._invasionPoints);
		//			}
		//
		//			//	Debug.Log (this.ToString ());
		//
		//
		//		}

		protected List<Landmark> _inReach = new List<Landmark> ();
		protected Collider[] _collected = new Collider[100];
		public float tempDistance;

		public virtual void Survey ()
		{
			//Clearing Lists and Arays
			this._inReach.Clear ();
			for (int i = this._collected.Length - 1; i >= 0; --i) {
				this._collected [i] = null;
			}

			//Collection Colliders on Landmark Layer
			int count = Physics.OverlapSphereNonAlloc (this.transform.position, this._influenceRadius, this._collected, Landmark.Mask);

			//Add each Landmark to list
			if (count != 0) {
				foreach (var collider in this._collected) {
					if (collider == null)
						break;
					Landmark landmark = collider.GetComponent<Landmark> ();
					if (landmark != null && !this._inReach.Contains (landmark)) {
						if (landmark != this) {
							if (landmark.invadedBy != this) {
								landmark.tempDistance = Vector3.Distance (this.transform.position, landmark.transform.position);
								Debug.Log ("Distance is: " + landmark.tempDistance);
								this._inReach.Add (landmark);
							} else {
								Debug.LogWarning ("Aleadry invaded!");
							}
						} else {
							Debug.LogWarning ("Self Collision");
						}
					} else {
						Debug.LogWarning ("Landmark is null, or already in List:" + collider.name);
					}
				}
			}

			if (this._inReach.Count == 0) {
				Debug.Log ("nothing in reach");
				return;
			}

			this._inReach.Sort (((x, y) => (int)x.tempDistance - (int)y.tempDistance));
		}

		protected List<Landmark> _invaded = new List<Landmark> ();

		public virtual void Invade (Landmark other)
		{
			this._invasionPoints -= other.defensePoints;
			int aquiredSettlers = 0;

			if (other.settlerCount > this._settlerCount.current)
				aquiredSettlers = (int)(other.settlerCount / 3);
			else if (other.settlerCount == (int)this._settlerCount.current)
				aquiredSettlers = (int)(other.settlerCount / 2);
			else
				aquiredSettlers = (int)(other.settlerCount);


			this._settlerCount.current += aquiredSettlers;
			this.resourceFactor.current += other.resourceFactor.current / this._settlerCount.current; 
			other._invadedBy = this;
			other.state = State.Invaded;
			other._influenceRadius = 0;
			this._invaded.Add (other);
		}

		public virtual void CascadeNewInvader (Landmark newInvader)
		{
			this.state = State.Invaded;
			this._invadedBy = newInvader;

			foreach (var invaded in this._invaded) {
				invaded.CascadeNewInvader (newInvader);
			}
		}

		public virtual void InvadeAll ()
		{
			foreach (var other in this._inReach) {
				if (other.defensePoints <= this.invasionPoints) {
					if (other._invadedBy != null) {
						Landmark topInvader = other._invadedBy;
						while (topInvader._invadedBy != null) {
							topInvader = topInvader._invadedBy;
						}
						if (topInvader.defensePoints <= this.invasionPoints) {
							Invade (topInvader);
						}
					} else {
						Invade (other);
					}
				} else {
					Debug.Log ("Too strong to Attack!");
				}

				if (this.invasionPoints == 0)
					break;
			}

			//end of the cycle
			this.Recalculate ();
		}

		protected virtual void OnDrawGizmos ()
		{
			if (Application.isPlaying) {
				Gizmos.DrawWireSphere (this.transform.position, this._influenceRadius);
			}
		}
	}
}