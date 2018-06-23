namespace PofyTools.Distribution
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Timeline<T, U>
    {
        protected List<TimeEvent> _events = null;
        public Dictionary<U, TimeEvent> _lastEventOfType = null;
        public TimeEvent nullVar { get { return null; } }
        protected Range _eventSpan = new Range ();

        #region Constructors

        private Timeline ()
        {
        }

        public Timeline (int initialCapacity)
        {
            this._events = new List<TimeEvent> (initialCapacity);
            this._lastEventOfType = new Dictionary<U, TimeEvent> (10);
            this._filteredEvents = new List<TimeEvent> (initialCapacity / 2 + 1);
        }

        public Timeline(int initialCapacity, IEqualityComparer<U> equalityComparer)
        {
            this._events = new List<TimeEvent>(initialCapacity);
            this._lastEventOfType = new Dictionary<U, TimeEvent>(10, equalityComparer);
            this._filteredEvents = new List<TimeEvent>(initialCapacity / 2 + 1);
        }
        #endregion

        #region Add/Remove Events

        public TimeEvent AddEvent (T content, U type)
        {
            TimeEvent tEvent = new TimeEvent (content, type, Time.time);
            this._events.Add (tEvent);
            this._lastEventOfType[type] = tEvent;

            this._eventSpan.max = Time.time;
            if (this._events.Count == 1)
                this._eventSpan.min = Time.time;

            return tEvent;
        }

        public void Purge ()
        {
            this._events.Clear ();
            this._filteredEvents.Clear ();
            this._lastEventOfType.Clear ();
            this._eventSpan = new Range ();
        }

        #endregion

        #region Type Compare

        public List<TimeEvent> GetAllEvents () { return this._events; }

        public void GetEvents (List<TimeEvent> events, float period, bool chronologicalOrder = false, params U[] types)
        {
            events.Clear ();
            for (int i = this._events.Count - 1; i >= 0; --i)
            {
                var tEvent = this._events[i];

                if (tEvent.elapsedTime <= period)
                {
                    if (types.Length == 0)
                    {
                        events.Add (tEvent);
                        continue;
                    }
                    foreach (var type in types)
                    {
                        if (tEvent.type.Equals (type))
                        {
                            events.Add (tEvent);
                            break;
                        }
                    }
                }
                else break;
            }
            if (chronologicalOrder)
                events.Reverse ();
        }

        public List<TimeEvent> GetEvents (float period, bool chronologicalOrder = false, params U[] types)
        {
            List<TimeEvent> events = new List<TimeEvent> ();
            GetEvents (events, period, chronologicalOrder, types);
            return events;
        }

        protected List<TimeEvent> _filteredEvents = null;

        public List<TimeEvent> GetEventsNonAlloc (float period, bool chronologicalOrder = false, params U[] types)
        {
            // this._filteredEvents.Clear ();
            GetEvents (this._filteredEvents, period, chronologicalOrder, types);
            return this._filteredEvents;
        }

        #endregion

        #region Range Compare

        public void GetEventsInTimeRange (List<TimeEvent> events, float fromTimestamp, float toTimestamp, bool chronologicalOrder = false, params U[] types)
        {
            //Clear provided list
            events.Clear ();

            //Cache everything!
            int count = this._events.Count;

            //No events - no service
            if (count == 0)
                return;

            //No Scope - no service
            if (fromTimestamp >= toTimestamp)
                return;

            //Clamp time range to event span
            fromTimestamp = (fromTimestamp < this._eventSpan.min) ? this._eventSpan.min : fromTimestamp;
            toTimestamp = (toTimestamp > this._eventSpan.max) ? this._eventSpan.max : toTimestamp;

            //Out of bounds
            if (!this._eventSpan.Contains (fromTimestamp) || !this._eventSpan.Contains (fromTimestamp))
                return;

            //Index to start the search from
            int partitionIndex = 0;

            //Element at partition index
            TimeEvent partition = null;

            //search direction
            int sign = 0;

            //is partition element inside search scope
            bool inRange = false;

            float diffLeft, diffRight;
            diffLeft = fromTimestamp - _eventSpan.min;
            diffRight = _eventSpan.max - toTimestamp;

            //If we assume somewhat uniform event distribution over event span
            partitionIndex = (diffLeft > diffRight) ? this._events.Count - 1 : 0;

            do
            {
                partitionIndex += sign;
                partition = this._events[partitionIndex];
                if (partition.timestamp < fromTimestamp) { if (sign != -1) sign = 1; else return; }
                else if (partition.timestamp > toTimestamp) { if (sign != 1) sign = -1; else return; }
                else
                {
                    inRange = true;
                }
            }
            while (!inRange);

            while (inRange)
            {
                if (types.Length == 0)
                {
                    events.Add (partition);
                }
                else
                {
                    foreach (var type in types)
                    {
                        if (partition.type.Equals (type))
                        {
                            events.Add (partition);
                            break;
                        }
                    }
                }

                partitionIndex += sign;
                partition = this._events[partitionIndex];
                inRange = (partition.timestamp >= fromTimestamp) && (partition.timestamp <= toTimestamp);
            }

            if (chronologicalOrder && sign == -1)
            {
                events.Reverse ();
            }
        }

        public List<TimeEvent> GetEventsInTimeRangeNonAlloc (float fromTimestamp, float toTimestamp, bool chronologicalOrder = false, params U[] types)
        {
            GetEventsInTimeRange (this._filteredEvents, fromTimestamp, toTimestamp, chronologicalOrder, types);
            return this._filteredEvents;
        }

        public List<TimeEvent> GetEventsInTimeRange (float fromTimestamp, float toTimestamp, bool chronologicalOrder = false, params U[] types)
        {
            List<TimeEvent> events = new List<TimeEvent> ();
            GetEventsInTimeRange (events, fromTimestamp, toTimestamp, chronologicalOrder, types);
            return events;
        }

        #endregion

        #region Until Compare

        public void GetEventsUntilInstance (List<TimeEvent> events, TimeEvent instance, bool chronologicalOrder = false, params U[] types)
        {
            events.Clear ();

            if (instance == null || !this._lastEventOfType.ContainsKey (instance.type))
            {
                //The whole thing!
                events.AddRange (this._events);
                return;
            }

            for (int i = this._events.Count - 1; i >= 0; --i)
            {
                var tEvent = this._events[i];

                if (tEvent == instance)
                {
                    break;
                }

                if (types.Length == 0)
                {
                    events.Add (tEvent);
                    continue;
                }

                foreach (var type in types)
                {
                    if (tEvent.type.Equals (type))
                    {
                        events.Add (tEvent);
                        break;
                    }
                }
            }

            if (chronologicalOrder)
                events.Reverse ();
        }

        public List<TimeEvent> GetEventsUntilInstance (TimeEvent instance, bool chronologicalOrder = false, params U[] types)
        {
            List<TimeEvent> events = new List<TimeEvent> ();
            GetEventsUntilInstance (events, instance, chronologicalOrder, types);
            return events;
        }

        public List<TimeEvent> GetEventsUntilInstanceNonAlloc (TimeEvent instance, bool chronologicalOrder = false, params U[] types)
        {
            GetEventsUntilInstance (this._filteredEvents, instance, chronologicalOrder, types);
            return this._filteredEvents;
        }

        #endregion

        public class TimeEvent
        {
            public T content = default (T);
            public float timestamp = 0f;
            public U type = default (U);

            public float elapsedTime
            {
                get { return Time.time - timestamp; }
            }

            public TimeEvent () { }
            public TimeEvent (T content, U type, float timestamp)
            {
                this.content = content;
                this.type = type;
                this.timestamp = timestamp;
            }
        }
    }

}