
using System;
using UnityEngine;

namespace APA.Core
{
    public class APAManager
    {
        public static APAManager Instance { get; private set; }

        public APAEventManager EventManager;

        public APAManager()
        {
            Instance = this;
        }

        public void LoadManagers(Action onComplete)
        {
            var monoManager = new GameObject("MonoManager");
            monoManager.AddComponent<APAMonoManagerObject>();

            EventManager = new();

            APADebug.Log("LoadManagers Completed");
            onComplete?.Invoke();
        }
    }
}
