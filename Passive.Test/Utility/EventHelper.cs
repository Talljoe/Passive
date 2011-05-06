// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.Utility
{
    using System;

    public static class EventHelper
    {
        public static IDisposable SetEventTemporarily(Action setEvent, Action unsetEvent)
        {
            return new EventDetach(setEvent, unsetEvent);
        }

        private class EventDetach : IDisposable
        {
            private readonly Action unsetEvent;

            public EventDetach(Action setEvent, Action unsetEvent)
            {
                setEvent();
                this.unsetEvent = unsetEvent;
            }


            public void Dispose()
            {
                this.unsetEvent();
            }
        }
    }
}