using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace.Debug
{
    public class DebugSpring : MonoBehaviour
    {
        [SerializeField] private SpringJoint2D springJoint;
        [SerializeField] private Rigidbody2D leftHandRigidBody;
        [SerializeField] private Rigidbody2D wireLeftHandRigidBody;
        [SerializeField] private float springDistance = 0.5f;
        [SerializeField] private InputAction UpDownAction;
        
        private void Start()
        {
            springJoint.connectedBody = leftHandRigidBody;
            springJoint.distance = (leftHandRigidBody.transform.position - wireLeftHandRigidBody.transform.position).magnitude;
            springDistance = springJoint.distance;
            
            UpDownAction.Enable();
        }

        private void Update()
        {
            if (UpDownAction.ReadValue<float>() > 0.01f)
            {
                springJoint.distance -= 0.01f;
            }else if (UpDownAction.ReadValue<float>() < -0.01f)
            {
                springJoint.distance += 0.01f;
            }
            
            if ((leftHandRigidBody.transform.position - wireLeftHandRigidBody.transform.position).magnitude <
                springDistance)
            {
                springJoint.enabled = false;
            }
            else
            {
                springJoint.enabled = true;
            }
            springJoint.distance = springDistance;
        }
    }
}