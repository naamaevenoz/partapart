namespace _APA.Scripts.Managers
{
    using System;
    using APA.Core;
    using UnityEngine;

    public static class EventManager
    {
        public static event Action<Rigidbody2D> OnRopeInteractStart;
        public static event Action<Rigidbody2D> OnLadderInteractStart;
        public static event Action<Rigidbody2D> OnTrajectoryInteractStart; 
        public static event Action<Rigidbody2D> OnRopeInteractStop;
        public static event Action<Rigidbody2D> OnLadderInteractStop;

        public static event Action<Rigidbody2D> OnTrajectoryInteractStop;
        public static int CurrentLevelIndex { get; set; } = 0; 

        public static event Action<string, GameObject> OnObjectActivate; 
        public static event Action<string, GameObject> OnObjectDeactivate;

        public static event Action<string>
            OnObjectToggle; // Optional: Can keep for things that only toggle without distinct on/off
        public static event Action<int> OnLevelSectionCompleted;

        public static event Action<bool> OnGameEndedStep1;
        public static event Action OnGameEndedStep2;
        public static event Action OnGameEndedFinal;
        public static event Action OnPlayersTeleportedForEndGame; 
        public static event Action OnGameEndedFinalInputReceived;

        public static event Action<string> OnLoadSceneRequested;

        public static event Action<LightInteractionController> OnDarkPlayerStuckInLight;
        public static event Action<LightInteractionController> OnDarkPlayerStuckInLightCamera;

        public static event Action<int> OnBarrierOpened;

        public static event Action<int>
            OnPlayersPassedBarrier; 

        public static event Action<LightInteractionController> OnShowStuckDecisionUI;

        public static void TriggerGameEndedStep1(bool coop) => OnGameEndedStep1?.Invoke(coop);
        public static void TriggerPlayersTeleportedForEndGame() => OnPlayersTeleportedForEndGame?.Invoke();
        public static void TriggerGameEndedFinalInputReceived() => OnGameEndedFinalInputReceived?.Invoke();

        public static void TriggerBarrierOpened(int openedBarrierIndex) => OnBarrierOpened?.Invoke(openedBarrierIndex);

        public static void TriggerPlayersPassedBarrier(int passedBarrierIndex) =>
            OnPlayersPassedBarrier?.Invoke(passedBarrierIndex);

        public static void TriggerDarkPlayerStuckInLight(LightInteractionController player)
        {
            if (player == null)
            {
                Debug.LogWarning("EventManager: TriggerDarkPlayerStuckInLight called with null player.");
                return;
            }

            Debug.Log($"EventManager: Dark Player '{player.gameObject.name}' is STUCK in light!");
            OnDarkPlayerStuckInLight?.Invoke(player);
            OnDarkPlayerStuckInLightCamera?.Invoke(player);
        }

        public static void TriggerShowStuckDecisionUI(LightInteractionController player)
        {
            if (player == null)
            {
                Debug.LogWarning("EventManager: TriggerShowStuckDecisionUI called with null player.");
                return;
            }

            Debug.Log($"EventManager: Requesting to show Stuck Decision UI for player {player.gameObject.name}");
            OnShowStuckDecisionUI?.Invoke(player);
        }

        public static void TriggerLoadScene(string sceneName)
        {
            Debug.Log($"EventManager: Requesting to load scene: {sceneName}");
            OnLoadSceneRequested?.Invoke(sceneName);
        }

        public static void TriggerOnGameEndedFinal()
        {
            Debug.Log("EventManager: Triggering OnGameEndedFinal.");
            OnGameEndedFinal?.Invoke();
        }

        public static void TriggerGameEndedStep2()
        {
            Debug.Log("EventManager: Triggering Game Ended Step 2.");
            OnGameEndedStep2?.Invoke();
        }


        public static void TriggerLevelSectionCompleted(int completedLevelIndex)
        {
            Debug.Log($"EventManager: Level Section {completedLevelIndex} Completed!");
            OnLevelSectionCompleted?.Invoke(completedLevelIndex);
        }

        public static void SetCurrentLevel(int levelIndex)
        {
            CurrentLevelIndex = levelIndex;
            Debug.Log($"EventManager: Current Level set to {CurrentLevelIndex}");
        }

        public static void TriggerObjectActivate(string objectID, GameObject source) 
        {
            if (string.IsNullOrEmpty(objectID) || source == null) return;
            Debug.Log($"EventManager: Triggering Object Activate for ID: {objectID} from Source: {source.name}");
            OnObjectActivate?.Invoke(objectID, source);
        }

        public static void TriggerObjectDeactivate(string objectID, GameObject source) 
        {
            if (string.IsNullOrEmpty(objectID) || source == null) return;
            Debug.Log($"EventManager: Triggering Object Deactivate for ID: {objectID} from Source: {source.name}");
            OnObjectDeactivate?.Invoke(objectID, source);
        }


        public static void TriggerObjectToggle(string objectID) 
        {
            if (string.IsNullOrEmpty(objectID)) return;
            OnObjectToggle?.Invoke(objectID);
        }
        
        public static void
            TriggerInteractionStart(InteractionType interactionType,
                Rigidbody2D initiatorRb) 
        {
            if (initiatorRb == null)
            {
                Debug.LogWarning(
                    $"EventManager: TriggerInteractionStart called with null initiatorRb for type '{interactionType}'. Aborting trigger.");
                return;
            }

            switch (interactionType)
            {
                case InteractionType.Rope:
                    OnRopeInteractStart?.Invoke(initiatorRb);
                    break;

                case InteractionType.Ladder:
                    OnLadderInteractStart?.Invoke(initiatorRb);
                    break;
                case InteractionType.None:
                    Debug.LogWarning(
                        $"EventManager: TriggerInteractionStart called with InteractionType.None for player {initiatorRb.name}.");
                    break;
                default:
                    Debug.LogWarning(
                        $"EventManager: Received unhandled interaction START type '{interactionType}' for player {initiatorRb.name}. No event triggered.");
                    break;
            }
        }
        public static void
            TriggerInteractionStop(InteractionType interactionType, Rigidbody2D initiatorRb) // Renamed parameter
        {
            if (initiatorRb == null)
            {
                Debug.LogWarning(
                    $"EventManager: TriggerInteractionStop called with null initiatorRb for type '{interactionType}'. Aborting trigger.");
                return;
            }

            switch (interactionType)
            {
                case InteractionType.Rope:
                    Debug.Log($"EventManager: Triggering Rope Interaction STOP Event for {initiatorRb.name}.");
                    OnRopeInteractStop?.Invoke(initiatorRb);
                    break;

                case InteractionType.Ladder:
                    OnLadderInteractStop?.Invoke(initiatorRb);
                    break;

                case InteractionType.Trajectory:
                    OnTrajectoryInteractStop?.Invoke(initiatorRb); 
                    break;
            
                case InteractionType.None:
                   
                    break;
                default:
                    Debug.LogWarning(
                        $"EventManager: Received unhandled interaction STOP type '{interactionType}' for player {initiatorRb.name}. No event triggered.");
                    break;
            }
        }
    }
}