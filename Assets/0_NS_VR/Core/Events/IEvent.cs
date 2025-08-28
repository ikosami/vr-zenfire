using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ev
{
    public interface IEvent
    {
        public string EventName { get; }
        public Dictionary<string, object> ToDic();

        public void TriggerEvent()
        {
            EventManager.TriggerEvent(EventName, this);
        }
    }

    public abstract class EventHandlerBase : ScriptableObject
    {
        public List<string> HandleEventNames;
        public abstract void Handle(IEvent e);

        Action<IEvent> _onEnd;

        void _Handle(IEvent e)
        {
            Handle(e);
            _onEnd?.Invoke(e);
        }

        public void StartListening(Action<IEvent> onEnd)
        {
            _onEnd = onEnd;
            for (int i = 0; i < HandleEventNames.Count; i++)
            {
                Ev.EventManager.StartListening(HandleEventNames[i], _Handle);
            }
        }

        public void StopListening()
        {
            _onEnd = null;
            for (int i = 0; i < HandleEventNames.Count; i++)
            {
                Ev.EventManager.StopListening(HandleEventNames[i], _Handle);
            }
        }

    }
}