using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ev
{
    public class EventManager : MonoBehaviour
    {
        private Dictionary<string, Action<IEvent>> eventDictionary;

        private static EventManager eventManager;

        public static EventManager instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Init();

                        //  Sets this to not be destroyed when reloading scene
                        DontDestroyOnLoad(eventManager);
                    }
                }
                return eventManager;
            }
        }

        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, Action<IEvent>>();
            }
        }

        public static void StartListening(string eventName, Action<IEvent> listener)
        {
            Action<IEvent> thisEvent;

            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                instance.eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                instance.eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, Action<IEvent> listener)
        {
            if (eventManager == null) return;
            Action<IEvent> thisEvent;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent -= listener;
                instance.eventDictionary[eventName] = thisEvent;
            }
        }

        public static void TriggerEvent(string eventName, IEvent message)
        {
            Action<IEvent> thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                NS.ED.Log("TriggerEvent: " + eventName);
                thisEvent?.Invoke(message);
            }
        }

        public static void TriggerEvent(string eventName)
        {
            TriggerEvent(eventName, new Ev.Events.DefaultEvent(eventName));
        }
    }
}
