using UnityEngine;

public class DebugAddSquare : MonoBehaviour
{
    [SerializeField] MaskController maskController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maskController.AddRect(new Vector2(-0.01f, 0.5f), new Vector2(0.5f, 2f), 0f, 3f);
    }
}
