using UnityEngine;

public class MaskController : MonoBehaviour
{
    public Material characterMaterial; // DollBodyShader_Maskedをセットしたマテリアル
    public Shader rectDrawerShader;    // 上記のRectDrawerシェーダー
    public int textureSize = 512;      // RenderTextureの解像度

    private RenderTexture _maskRT;
    private Material _drawMat;

    void Awake()
    {
        // 1. マスク用のRenderTextureを作成
        _maskRT = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.R8);
        _maskRT.Create();

        // 最初は真っ黒にクリア
        ClearMask();

        // 2. 描き込み用マテリアルの作成
        _drawMat = new Material(rectDrawerShader);

        // 3. キャラのメインマテリアルにこのRTをセット
        characterMaterial.SetTexture("_MaskMap", _maskRT);
    }

    // 四角形を一つ描き足すメソッド
    public void AddRect(Vector2 center, Vector2 size, float angleDeg, float aspectRatio)
    {
        // 横 / 縦 の比率を計算
        float aspect = aspectRatio;
        
        _drawMat.SetVector("_Center", center);
        _drawMat.SetVector("_Size", size);
        _drawMat.SetFloat("_Rotation", angleDeg * Mathf.Deg2Rad);
        _drawMat.SetFloat("_Aspect", aspect);
        
        // Graphics.Blitを使って、現在の_maskRTに新しい四角形を「描き足す」
        // (シェーダー側がBlend One Oneなので加算される)
        RenderTexture temp = RenderTexture.GetTemporary(_maskRT.width, _maskRT.height, 0, _maskRT.format);
        Graphics.Blit(_maskRT, temp);             // 現在の状態をコピー
        Graphics.Blit(null, _maskRT, _drawMat);   // 新しい四角形を描画
        // 注意：加算合成を維持しつつ蓄積させるには、RenderTextureの運用に少し工夫が必要
        // ここでは最もシンプルな「描き足し」の概念を示しています
        RenderTexture.ReleaseTemporary(temp);
    }

    public void ClearMask()
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = _maskRT;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = active;
    }
    
    /// <summary>
    /// 着色された領域の割合を算出する
    /// </summary>
    public float CalculateColoredArea()
    {
        // 1. スプライトの実際のサイズを取得（ワールド単位）
        // SpriteRendererのboundsは、ScaleやSprite自体のサイズをすべて含んだ「世界での大きさ」を返します
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        float worldWidth = bounds.size.x;
        float worldHeight = bounds.size.y;
        float totalWorldArea = worldWidth * worldHeight;

        // 2. RenderTextureのピクセルデータを読み出す
        // WebGLではReadPixelsを使用します。少し重い処理なので注意。
        Texture2D tempTex = new Texture2D(_maskRT.width, _maskRT.height, TextureFormat.R8, false);
        
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = _maskRT;
        tempTex.ReadPixels(new Rect(0, 0, _maskRT.width, _maskRT.height), 0, 0);
        tempTex.Apply();
        RenderTexture.active = active;

        // 3. 白いピクセル（色が変わっている部分）をカウント
        Color32[] pixels = tempTex.GetPixels32();
        int coloredPixelCount = 0;
        int totalPixelCount = pixels.Length;

        for (int i = 0; i < totalPixelCount; i++)
        {
            // Rチャンネルが0より大きければ着色されているとみなす
            // (しきい値を設けて if (pixels[i].r > 128) などにしてもOK)
            if (pixels[i].r > 0) 
            {
                coloredPixelCount++;
            }
        }

        // 使い終わった一時テクスチャを破棄
        Destroy(tempTex);
        
        // 比率 = 着色ピクセル / 全ピクセル
        return (float)coloredPixelCount / totalPixelCount;
    }

    private void OnDestroy()
    {
        if (_maskRT != null) _maskRT.Release();
    }
}