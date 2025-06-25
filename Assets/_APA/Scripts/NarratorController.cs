
namespace _APA.Scripts.Managers
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    public class NarratorController : MonoBehaviour
    {
        [Tooltip("Assign all NarrationEventData Scriptable Objects here.")] [SerializeField]
        private List<NarrationEventData> narrationEvents;

        private Dictionary<string, NarrationEventData> narrationLookup;

        private void Awake()
        {
            narrationLookup = new Dictionary<string, NarrationEventData>();
            if (narrationEvents != null)
            {
                foreach (var evtData in narrationEvents)
                {
                    if (evtData == null) continue;
                    if (!narrationLookup.ContainsKey(evtData.TriggerID))
                    {
                        evtData.lastPlayedTime = -1000f; 
                        evtData.hasBeenPlayed = false;
                        narrationLookup.Add(evtData.TriggerID, evtData);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate TriggerID '{evtData.TriggerID}' found in Narration Events!",
                            evtData);
                    }
                }
            }
            else
            {
                Debug.LogError("NarratorController has no NarrationEventData assigned!", this);
            }
        }


        private void OnEnable()
        {
            EventManager.OnObjectActivate += HandleObjectActivate; 
            EventManager.OnObjectDeactivate += HandleObjectDeactivate; 
        }

        private void OnDisable()
        {
            // Unsubscribe from all events
            EventManager.OnObjectActivate -= HandleObjectActivate; // *** ADD THIS ***
            EventManager.OnObjectDeactivate -= HandleObjectDeactivate; // *** ADD THIS ***
        }

        // --- Event Handlers ---
        private void
            HandleObjectActivate(string objectID, GameObject source) // We get the ID and the source (e.g., the button)
        {
            // Decide on a convention for the narration TriggerID.
            // Option 1: Use the objectID directly.
            // TryPlayNarration(objectID);

            // Option 2 (Recommended): Append suffix for clarity and flexibility.
            // This allows different narration for activation vs deactivation if needed.
            /*string narrationTriggerID = objectID + "_Activate";*/
            string narrationTriggerID = objectID;
            // /*Debug.Log($"NarratorController: Handling Object Activate. Trying narration ID: {narrationTriggerID}");*/
            TryPlayNarration(narrationTriggerID);
        }

        private void HandleObjectDeactivate(string objectID, GameObject source)
        {
            string narrationTriggerID = objectID;
            Debug.Log($"NarratorController: Handling Object Deactivate. Trying narration ID: {narrationTriggerID}");
            TryPlayNarration(narrationTriggerID);
        }

        private void TryPlayNarration(string triggerID)
        {
            if (narrationLookup.TryGetValue(triggerID, out NarrationEventData eventData))
            {
                if (eventData.PlayOnlyOnce && eventData.hasBeenPlayed)
                {
                    return;
                }

                if (Time.time < eventData.lastPlayedTime + eventData.Cooldown)
                {
                    return;
                }

                if (SoundManager.Instance != null && eventData.VoiceLine != null)
                {
                    Debug.Log($"NarratorController: Playing '{triggerID}' (Clip: {eventData.VoiceLine.name})");
                    SoundManager.Instance.PlayVoiceLine(eventData.VoiceLine, eventData.Delay);

                    eventData.lastPlayedTime = Time.time;
                    eventData.hasBeenPlayed = true;

                    if (!string.IsNullOrEmpty(eventData.SubtitleText))
                    {
                        SubtitleManager.Instance?.ShowSubtitle(eventData.SubtitleText, eventData.VoiceLine.length);
                    }
                }
                else
                {
                    if (eventData.VoiceLine == null)
                        Debug.LogError($"NarrationEventData for '{triggerID}' is missing an AudioClip!", eventData);
                    if (SoundManager.Instance == null) Debug.LogError("SoundManager Instance is missing!");
                }
            }
            else
            {
               
            }
        }
    }
}