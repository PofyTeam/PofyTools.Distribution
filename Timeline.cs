namespace PofyTools.Distribution
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Timeline<T, U>
    {
        protected List<TimeEvent> _events = null;
        public Dictionary<U, TimeEvent> _lastEventOfType = null;
        private Timeline ()
        {
        }

        public Timeline (int initialCapacity)
        {
            this._events = new List<TimeEvent> (initialCapacity);
            this._lastEventOfType = new Dictionary<U, TimeEvent> (10);
            this._filteredEvents = new List<TimeEvent> (initialCapacity / 2 + 1);
        }

        public TimeEvent AddEvent (T content, U type, float timestamp = -1f)
        {
            timestamp = (timestamp == -1) ? Time.time : timestamp;
            TimeEvent tEvent = new TimeEvent (content, type, timestamp);
            this._events.Add (tEvent);
            this._lastEventOfType[type] = tEvent;
            return tEvent;
        }

        #region Type Compare

        public List<TimeEvent> GetAllEvents () { return this._events; }

        public void GetEvents (List<TimeEvent> events, float period, U type, bool chronologicalOrder = false)
        {
            events.Clear ();
            for (int i = this._events.Count - 1; i >= 0; --i)
            {
                var tEvent = this._events[i];
                if (tEvent.timestamp + period >= Time.time)
                {
                    if (tEvent.type.Equals (type))
                    {
                        events.Add (tEvent);
                    }
                }
                else break;
            }
            if (chronologicalOrder)
                events.Reverse ();
        }

        public List<TimeEvent> GetEvents (float period, U type, bool chronologicalOrder = false)
        {
            List<TimeEvent> events = new List<TimeEvent> ();
            GetEvents (events, period, type, chronologicalOrder);
            return events;
        }

        protected List<TimeEvent> _filteredEvents = null;

        public List<TimeEvent> GetEventsNonAlloc (float period, U type, bool chronologicalOrder = false)
        {
            this._filteredEvents.Clear ();
            GetEvents (this._filteredEvents, period, type, chronologicalOrder);
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