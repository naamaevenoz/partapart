using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Player References")]
    [Tooltip("Assign the Transform of Player 1.")]
    [SerializeField] private Transform player1Transform;
    [Tooltip("Assign the Transform of Player 2.")]
    [SerializeField] private Transform player2Transform;

    [Header("Parallax Effect Strength")]
    [Tooltip("How much the layer moves relative to the players' midpoint movement. X for horizontal, Y for vertical.")]
    [SerializeField] private Vector2 parallaxFactor = Vector2.one; 

    [Header("Player-Driven Horizontal Oscillation")] 
    [Tooltip("Enable the horizontal oscillation based on player-driven parallax travel.")]
    [SerializeField] private bool enableHorizontalOscillation = false;
    [Tooltip("The total horizontal distance the layer will travel due to parallax in one direction before its parallax response inverts.")]
    [SerializeField] private float oscillationTravelDistanceX = 5.0f;

    private Vector3 layerInitialAnchorPosition;     
    private Vector3 playersMidpointStartPosition;
    private bool setupComplete = false;

    private float currentOscillationOriginX;       
    private int currentParallaxDirectionX = 1; 
                                               

    void Start()
    {
        if (player1Transform == null || player2Transform == null)
        {
            Debug.LogError($"ParallaxLayer '{gameObject.name}': Player transforms are not assigned. Disabling script.", this);
            enabled = false;
            return;
        }

        layerInitialAnchorPosition = transform.position;
        playersMidpointStartPosition = CalculateMidpoint(player1Transform.position, player2Transform.position);
        
        if (enableHorizontalOscillation)
        {
            currentOscillationOriginX = layerInitialAnchorPosition.x; 
            if (oscillationTravelDistanceX <= 0)
            {
                Debug.LogWarning($"ParallaxLayer '{gameObject.name}': Oscillation Travel Distance X is zero or negative. Oscillation will be disabled.", this);
                enableHorizontalOscillation = false;
            }
        }
        setupComplete = true;
    }

    void LateUpdate()
    {
        if (!setupComplete) return; 

        Vector3 currentPlayersMidpoint = CalculateMidpoint(player1Transform.position, player2Transform.position);
        Vector3 playersMidpointDisplacement = currentPlayersMidpoint - playersMidpointStartPosition;

        float rawParallaxDisplacementX = playersMidpointDisplacement.x * parallaxFactor.x;
        float parallaxDisplacementY = playersMidpointDisplacement.y * parallaxFactor.y; // Y parallax is direct

        float effectiveParallaxDisplacementX = rawParallaxDisplacementX;

        if (enableHorizontalOscillation)
        {
            float currentLegTravelX = (rawParallaxDisplacementX * currentParallaxDirectionX);
            
            float potentialLayerX = currentOscillationOriginX + currentLegTravelX;
            float distanceFromOscillationOrigin = Mathf.Abs(potentialLayerX - currentOscillationOriginX);

            if (distanceFromOscillationOrigin >= oscillationTravelDistanceX)
            {
                float overshoot = distanceFromOscillationOrigin - oscillationTravelDistanceX;
                if (potentialLayerX > currentOscillationOriginX) 
                {
                    effectiveParallaxDisplacementX = currentOscillationOriginX + oscillationTravelDistanceX - layerInitialAnchorPosition.x;
                }
                else 
                {
                    effectiveParallaxDisplacementX = currentOscillationOriginX - oscillationTravelDistanceX - layerInitialAnchorPosition.x;
                }
                
                currentParallaxDirectionX *= -1;

                currentOscillationOriginX = layerInitialAnchorPosition.x + effectiveParallaxDisplacementX;

                Debug.Log($"ParallaxLayer '{gameObject.name}': Oscillation limit reached. New DirectionX: {currentParallaxDirectionX}, New OscillationOriginX: {currentOscillationOriginX}");
            }
            else
            {
                effectiveParallaxDisplacementX = rawParallaxDisplacementX * currentParallaxDirectionX;
            }
        }

        float targetX = layerInitialAnchorPosition.x + effectiveParallaxDisplacementX;
        float targetY = layerInitialAnchorPosition.y + parallaxDisplacementY;

        Vector3 targetPosition = new Vector3(targetX, targetY, layerInitialAnchorPosition.z);
        transform.position = targetPosition;
    }

    private Vector3 CalculateMidpoint(Vector3 pos1, Vector3 pos2)
    {
        return (pos1 + pos2) / 2.0f;
    }
}