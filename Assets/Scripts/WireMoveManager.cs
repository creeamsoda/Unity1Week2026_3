using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using R3;

public class WireMoveManager : MonoBehaviour
{
    // パラメータ
    public static Vector2 MoveSpeedMax = new Vector2(1f, 1f) * 10000f;
    public static float inputIgnoreBorderSqr = 0.01f;
    
    // 変数
    public Rigidbody2D selectingWireRigidBody = null;
    
    // 公開イベント
    private Subject<Vector2> onWireAddForce = new Subject<Vector2>();

    public Observable<Vector2> OnWireAddForce => onWireAddForce;

    // 糸の端点
    [SerializeField] private Rigidbody2D leftHandRigidBody;
    [SerializeField] private Rigidbody2D rightHandRigidBody;
    [SerializeField] private Rigidbody2D leftFootRigidBody;
    [SerializeField] private Rigidbody2D rightFootRigidBody;

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
        
        switchLeftAction.performed +=  SwitchLeft;
        switchRightAction.performed +=  SwitchRight;
        moveAction.Enable();
        switchLeftAction.Enable();
        switchRightAction.Enable();
    }

    private void OnDestroy()
    {
        switchLeftAction.performed -= SwitchLeft;
        switchRightAction.performed -= SwitchRight;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAction.ReadValue<Vector2>().sqrMagnitude > inputIgnoreBorderSqr)
        {
            Debug.Log("moving wire " + moveAction.ReadValue<Vector2>());
            MoveWire();
        }
        else
        {
            Debug.Log("no moving wire " + moveAction.ReadValue<Vector2>());
            onWireAddForce.OnNext(Vector2.zero);
        }
    }

    void MoveWire()
    {
        selectingWireRigidBody.AddForce(moveAction.ReadValue<Vector2>() * MoveSpeedMax * Time.deltaTime);
        onWireAddForce.OnNext(moveAction.ReadValue<Vector2>() *  MoveSpeedMax * Time.deltaTime);
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

            selectingWireRigidBody = sortedWires[currentSelectedWireIndex - 1];
        }
        else
        {
            if (currentSelectedWireIndex == sortedWires.Count - 1) return;
            
            selectingWireRigidBody = sortedWires[currentSelectedWireIndex + 1];
        }
    }
}