using System;
using System.Collections.Generic;
using System.Linq;

namespace APA.Core
{
    public class APAEventManager
    {
        private Dictionary<APAEventName, List<Action<object>>> _activeListeners = new();

        public void AddListener(APAEventName eventName, Action<object> listener)
        {
            if (_activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                listOfEvents.Add(listener);
                return;
            }

            _activeListeners.Add(eventName, new List<Action<object>> { listener });
        }
        public void RemoveListener(APAEventName eventName, Action<object> listener)
        {
            if (_activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                listOfEvents.Remove(listener);

                if (listOfEvents.Count <= 0)
                {
                    _activeListeners.Remove(eventName);
                }
            }
        }
        public void InvokeEvent(APAEventName eventName, object obj)
        {
            if (_activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                var copyList = listOfEvents.ToList();
                for (int i = 0; i < copyList.Count; i++)
                {
                    copyList[i].Invoke(obj);
                }
            }
        }
    }
    public enum APAEventName
    {
        None,
        OnObjectActivate,
    }
}
