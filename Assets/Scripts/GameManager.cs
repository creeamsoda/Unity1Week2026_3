using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DollManager dollManager;
    [SerializeField] private List<GameObject> taskList;
    
    [SerializeField] InputAction checkTaskAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        checkTaskAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkTaskAction.WasPressedThisFrame())
        {
            CheckTask(taskList[0]);
        }
    }

    private void CheckTask(GameObject task)
    {
        Transform[] elementsTransform = task.GetComponentsInChildrenWithoutSelf<Transform>();

        foreach (Transform element in elementsTransform)
        {
            AddSquare(element, dollManager.bodySpriteRenderer, dollManager.bodyMaskController);
            // AddSquare(element, dollManager.leftArm.transform, dollManager.leftArmMaskController);
            // AddSquare(element, dollManager.rightArm.transform, dollManager.rightArmMaskController);
            // AddSquare(element, dollManager.leftLeg.transform, dollManager.leftLegMaskController);
            // AddSquare(element, dollManager.rightLeg.transform, dollManager.rightLegMaskController);
            Debug.LogError("手足の着色処理はまだコメントアウトしてる、各手足にMaskつけてからここのコメントアウト解除する");
        }
    }

    private void AddSquare(Transform taskElement, SpriteRenderer doll, MaskController maskController)
    {
        // 1. スプライト本来のサイズ（Local空間）を取得
        // doll.sprite.bounds は、変換前のスプライト自体のサイズを返します
        Vector2 spriteLocalSize = doll.sprite.bounds.size;

        // 2. dollのTransform（Scale含む）を考慮した、Local軸に沿ったワールドサイズ
        Vector2 dollWorldSize = new Vector2(
            spriteLocalSize.x * doll.transform.lossyScale.x,
            spriteLocalSize.y * doll.transform.lossyScale.y
        );

        // 3. 【重要】taskElementのワールド座標をdollのローカル座標に変換
        // これにより、dollが回転していても「dollの中心から見てどこか」が正確に取れます
        Vector3 localPos = doll.transform.InverseTransformPoint(taskElement.position);

        // 4. ローカル座標をUV空間 (0.0 ~ 1.0) にマッピング
        // localPosは中心が(0,0)なので、サイズで割って0.5を足します
        Vector2 uvPos = new Vector2(
            localPos.x / spriteLocalSize.x + 0.5f,
            localPos.y / spriteLocalSize.y + 0.5f
        );

        // 5. サイズもローカル軸に合わせて計算
        Vector2 uvSize = new Vector2(
            taskElement.lossyScale.x / dollWorldSize.x,
            taskElement.lossyScale.y / dollWorldSize.y
        );

        // 6. 回転は doll から見た相対角度を渡す
        float relativeRotation = taskElement.rotation.eulerAngles.z - doll.transform.rotation.eulerAngles.z;

        // 7. アスペクト比もローカル基準（回転に影響されない）
        float aspect = dollWorldSize.x / dollWorldSize.y;

        // MaskControllerに値を渡す
        maskController.AddRect(uvPos, uvSize, relativeRotation, aspect);
    }
}
