using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using R3;

public class WireMoveManager : MonoBehaviour
{
    // パラメータ
    public static Vector2 MoveSpeedMax = new Vector2(1f, 1f) * 100000f;
    public static float inputIgnoreBorderSqr = 0.01f;
    
    // 変数
    public Rigidbody2D selectingWireRigidBody = null;
    public CableObject selectingCableObject =  null;
    
    // 公開イベント
    private Subject<Vector2> onWireAddForce = new Subject<Vector2>();

    public Observable<Vector2> OnWireAddForce => onWireAddForce;

    // 糸の端点
    [SerializeField] private Rigidbody2D leftHandRigidBody;
    [SerializeField] private Rigidbody2D rightHandRigidBody;
    [SerializeField] private Rigidbody2D leftFootRigidBody;
    [SerializeField] private Rigidbody2D rightFootRigidBody;
    [SerializeField] private CableObject leftHandCableObject;
    [SerializeField] private CableObject rightHandCableObject;
    [SerializeField] private CableObject leftCableObject;
    [SerializeField] private CableObject rightCableObject; 
    private Vector2 wireStayPosition = new Vector2(0f, 3f);
    private Vector2 wireLeftHandStayPosition = new Vector2(0f, 3f);
    private Vector2 wireRightHandStayPosition = new Vector2(0f, 3f);
    private Vector2 wireLeftFootStayPosition = new Vector2(0f, 3f);
    private Vector2 wireRightFootStayPosition = new Vector2(0f, 3f);

    // キーの入力を受ける
    [SerializeField] private InputAction leftAction;
    [SerializeField] private InputAction rightAction;
    [SerializeField] private InputAction upAction;
    [SerializeField] private InputAction downAction;
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction switchLeftAction;
    [SerializeField] private InputAction switchRightAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectingWireRigidBody = leftHandRigidBody;
        selectingCableObject = leftHandCableObject;
        
        switchLeftAction.performed +=  SwitchLeft;
        switchRightAction.performed +=  SwitchRight;
        moveAction.Enable();
        switchLeftAction.Enable();
        switchRightAction.Enable();
        wireStayPosition = selectingWireRigidBody.position;
        wireRightHandStayPosition = rightHandRigidBody.position;
        wireLeftFootStayPosition = leftFootRigidBody.position;
        wireRightFootStayPosition = rightFootRigidBody.position;
    }

    private void OnDestroy()
    {
        switchLeftAction.performed -= SwitchLeft;
        switchRightAction.performed -= SwitchRight;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(moveAction.ReadValue<Vector2>().y) > inputIgnoreBorderSqr)
        {
            MoveWireVertical();
        }
        else
        {
            onWireAddForce.OnNext(Vector2.zero); 
            selectingWireRigidBody.position = new Vector2(selectingWireRigidBody.position.x, wireStayPosition.y);
            selectingWireRigidBody.linearVelocity = new Vector2(selectingWireRigidBody.linearVelocity.x, 0f);
        }

        if (Mathf.Abs(moveAction.ReadValue<Vector2>().x) > inputIgnoreBorderSqr)
        {
            MoveWireHorizontal();
        }
        else
        {
            onWireAddForce.OnNext(Vector2.zero);
            selectingWireRigidBody.position = new Vector2(wireStayPosition.x, selectingWireRigidBody.position.y);
            selectingWireRigidBody.linearVelocity = new Vector2(0f, selectingWireRigidBody.linearVelocity.y);
        }

        if (selectingWireRigidBody != leftHandRigidBody)
        {
            leftHandRigidBody.position = wireLeftHandStayPosition;
            leftHandRigidBody.linearVelocity = Vector2.zero;
        }
        
        if (selectingWireRigidBody != rightHandRigidBody)
        {
            rightHandRigidBody.position = wireRightHandStayPosition;
            rightHandRigidBody.linearVelocity = Vector2.zero;
        }

        if (selectingWireRigidBody != leftFootRigidBody)
        {
            leftFootRigidBody.position = wireLeftFootStayPosition;
            leftFootRigidBody.linearVelocity = Vector2.zero;
        }

        if (selectingWireRigidBody != rightFootRigidBody)
        {
            rightFootRigidBody.position = wireRightFootStayPosition;
            rightFootRigidBody.linearVelocity = Vector2.zero;
        }
    }

    void MoveWireVertical()
    {
        if (moveAction.ReadValue<Vector2>().y > 0.01f)
        {
            selectingCableObject.SlowlyReel();
            wireStayPosition.y = selectingWireRigidBody.position.y;
        }else if (moveAction.ReadValue<Vector2>().y < -0.01f)
        {
            selectingCableObject.SlowlyAddLine();
            wireStayPosition.y = selectingWireRigidBody.position.y;
        }
        onWireAddForce.OnNext(moveAction.ReadValue<Vector2>() *  MoveSpeedMax * Time.deltaTime);
    }

    private void MoveWireHorizontal()
    {
        // 横方向
        selectingWireRigidBody.AddForce(new Vector2(moveAction.ReadValue<Vector2>().x, 0f) * MoveSpeedMax * Time.deltaTime);
        wireStayPosition.x = selectingWireRigidBody.position.x;
        onWireAddForce.OnNext(new Vector2(moveAction.ReadValue<Vector2>().x, 0f) * MoveSpeedMax * Time.deltaTime);
    }

    void SwitchLeft(InputAction.CallbackContext ctx)
    {
        SwitchWireInternal(true);
    }

    void SwitchRight(InputAction.CallbackContext ctx)
    {
        SwitchWireInternal(false);
    }

    private void SwitchWireInternal(bool isLeft)
    {
        var sortedWires =
            new List<Rigidbody2D>() { leftFootRigidBody, leftHandRigidBody, rightHandRigidBody, rightFootRigidBody }.OrderBy(rb => rb.position.x).ToList();
        
        int currentSelectedWireIndex = sortedWires.IndexOf(selectingWireRigidBody);

        if (isLeft)
        {
            if (currentSelectedWireIndex == 0) return;
            if (selectingWireRigidBody == leftHandRigidBody)
            {
                wireLeftHandStayPosition = selectingWireRigidBody.position;
            }
            else if (selectingWireRigidBody == rightHandRigidBody)
            {
                wireRightHandStayPosition = selectingWireRigidBody.position;
            }
            else if (selectingWireRigidBody == leftFootRigidBody)
            {
                wireLeftFootStayPosition = selectingWireRigidBody.position;
            }
            else if (selectingWireRigidBody == rightFootRigidBody)
            {
                wireRightFootStayPosition = selectingWireRigidBody.position;
            }

            selectingWireRigidBody = sortedWires[currentSelectedWireIndex - 1];
            switch (selectingWireRigidBody)
            {
                case var rb when rb == leftHandRigidBody:
                    selectingCableObject = leftHandCableObject;
                    break;
                case var rb when rb == rightHandRigidBody:
                    selectingCableObject = rightHandCableObject;
                    break;
                case var rb when rb == leftFootRigidBody:
                    selectingCableObject = leftCableObject;
                    break;
                case var rb when rb == rightFootRigidBody:
                    selectingCableObject = rightCableObject;
                    break;
            }
            wireStayPosition = selectingWireRigidBody.position;
        }
        else
        {
            if (currentSelectedWireIndex == sortedWires.Count - 1) return;
            
            if (selectingWireRigidBody == leftHandRigidBody)
            {
                wireLeftHandStayPosition = selectingWireRigidBody.position;
            }
            else if (selectingWireRigidBody == rightHandRigidBody)
            {
                wireRightHandStayPosition = selectingWireRigidBody.position;
            }
            else if (selectingWireRigidBody == leftFootRigidBody)
            {
                wireLeftFootStayPosition = selectingWireRigidBody.position;
            }
            else if (selectingWireRigidBody == rightFootRigidBody)
            {
                wireRightFootStayPosition = selectingWireRigidBody.position;
            }
            
            selectingWireRigidBody = sortedWires[currentSelectedWireIndex + 1];
            switch (selectingWireRigidBody)
            {
                case var rb when rb == leftHandRigidBody:
                    selectingCableObject = leftHandCableObject;
                    break;
                case var rb when rb == rightHandRigidBody:
                    selectingCableObject = rightHandCableObject;
                    break;
                case var rb when rb == leftFootRigidBody:
                    selectingCableObject = leftCableObject;
                    break;
                case var rb when rb == rightFootRigidBody:
                    selectingCableObject = rightCableObject;
                    break;
            }
            wireStayPosition = selectingWireRigidBody.position;
        }
    }
}