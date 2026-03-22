using UnityEngine;
using R3;

public class DebugPowerArrow : MonoBehaviour
{
    [SerializeField] WireMoveManager wireMoveManager;
    [SerializeField] Transform powerArrow;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wireMoveManager.OnWireAddForce.Subscribe(force => ShowPowerArrow(force));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void ShowPowerArrow(Vector2 force)
    {
        if (force.sqrMagnitude < 0.001f)
        {
            powerArrow.gameObject.SetActive(false);
            return;
        }
        powerArrow.gameObject.SetActive(true);
        powerArrow.position = wireMoveManager.selectingWireRigidBody.position;
        powerArrow.rotation = Quaternion.FromToRotation(Vector3.up, force);
        powerArrow.localScale = new Vector3(powerArrow.localScale.x, 0.1f * force.magnitude, powerArrow.localScale.z);
    }
}
