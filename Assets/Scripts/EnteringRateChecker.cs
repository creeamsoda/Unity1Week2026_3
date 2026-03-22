using UnityEngine;

public class EnteringRateChecker : MonoBehaviour
{
    public static float SamplingMaxWidth = 0.1f;
    
    [SerializeField] LayerMask targetLayer;
    private SpriteRenderer spriteRenderer;
    private Vector2Int samplingCount;
    private Vector2 localStep;
    private Vector2 localStartPoint;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        // 1. スプライト自体のローカルな大きさを取得 (Scaleを含まない)
        // sprite.bounds は SpriteRenderer.bounds と違い、純粋なスプライトのサイズ
        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        Vector2 size = spriteBounds.size;

        // 2. サンプリング数を計算 (ワールド座標でのサイズをもとに計算)
        // SpriteRenderer.bounds.size はスケールが反映されたワールドサイズ
        Vector3 worldSize = spriteRenderer.bounds.size;
        samplingCount.x = Mathf.Max(1, Mathf.RoundToInt(worldSize.x / SamplingMaxWidth));
        samplingCount.y = Mathf.Max(1, Mathf.RoundToInt(worldSize.y / SamplingMaxWidth));

        // 3. ローカル空間での1ステップあたりの距離を計算
        localStep.x = size.x / samplingCount.x;
        localStep.y = size.y / samplingCount.y;

        // 4. 左下隅のローカル座標 (Pivotを考慮)
        localStartPoint = (Vector2)spriteBounds.min;
    }

    public float SamplingArea()
    {
        if (spriteRenderer == null) return 0;

        int enteringPointCount = 0;
        int totalPoints = (samplingCount.x + 1) * (samplingCount.y + 1);

        for (int i = 0; i <= samplingCount.x; i++)
        {
            for (int j = 0; j <= samplingCount.y; j++)
            {
                // ローカル座標を計算
                Vector3 localPos = new Vector3(
                    localStartPoint.x + (i * localStep.x),
                    localStartPoint.y + (j * localStep.y),
                    0
                );

                // ローカル座標を現在のオブジェクトの位置・回転・スケールに合わせてワールド座標に変換
                Vector2 worldPoint = transform.TransformPoint(localPos);

                // 判定
                if (Physics2D.OverlapPoint(worldPoint, targetLayer) != null)
                {
                    enteringPointCount++;
                }
            }
        }

        return (float)enteringPointCount / totalPoints;
    }
}