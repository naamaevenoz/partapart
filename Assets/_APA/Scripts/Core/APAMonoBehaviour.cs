using System;
using UnityEngine;

namespace APA.Core
{
    public class APAMonoBehaviour : MonoBehaviour
    {
        protected APAManager Manager => APAManager.Instance;
        protected APAMonoManagerObject MonoManager => APAMonoManagerObject.Instance;


        protected void AddListener(APAEventName eventName, Action<object> eventCallback)
        {
            Manager.EventManager.AddListener(eventName, eventCallback);
        }
        protected void RemoveListener(APAEventName eventName, Action<object> eventCallback)
        {
            Manager.EventManager.RemoveListener(eventName, eventCallback);
        }
        protected void InvokeEvent(APAEventName eventName, object obj)
        {
            Manager.EventManager.InvokeEvent(eventName, obj);
        }
    }
}
