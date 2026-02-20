using System.Collections.Generic;
using UnityEngine;

public class NPCWalker : MonoBehaviour
{
    private struct MaterialData
    {
        public Renderer renderer;
        public int materialIndex;
        public Color originalColor;
    }

    [SerializeField] private float speed = 2f;
    [SerializeField] private float lifeDistance = 20f;
    [SerializeField] private float fadeDistance = 3f;

    private Transform cachedTransform;
    private Vector3 startPos;

    private MaterialPropertyBlock propBlock;
    private MaterialData[] matData; // Массив всех материалов модели

    private static readonly int baseColorID = Shader.PropertyToID("_BaseColor");
    private float currentAlpha = -1f;

    void Awake()
    {
        cachedTransform = transform;
        propBlock = new MaterialPropertyBlock();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        List<MaterialData> dataList = new List<MaterialData>();

        foreach (Renderer r in renderers)
        {
            Material[] mats = r.sharedMaterials; // Используем sharedMaterials для скорости и экономии памяти
            for (int i = 0; i < mats.Length; i++)
            {
                Color color = Color.white;
                if (mats[i] != null && mats[i].HasProperty(baseColorID))
                {
                    color = mats[i].GetColor(baseColorID);
                }

                dataList.Add(new MaterialData
                {
                    renderer = r,
                    materialIndex = i, // Запоминаем конкретный слот материала (0, 1, 2...)
                    originalColor = color
                });
            }
        }
        matData = dataList.ToArray(); // Сохраняем в быстрый массив
    }

    public void ActivateNPC(Vector3 position, Quaternion rotation)
    {
        cachedTransform.SetPositionAndRotation(position, rotation);
        startPos = position;

        currentAlpha = -1f;
        SetAlpha(0f);

        gameObject.SetActive(true);
    }

    void Update()
    {
        cachedTransform.Translate(Vector3.forward * speed * Time.deltaTime);

        Vector3 displacement = cachedTransform.position - startPos;
        float sqrDistance = displacement.sqrMagnitude;
        float targetAlpha = 1f;

        if (sqrDistance < fadeDistance * fadeDistance)
        {
            targetAlpha = Mathf.Sqrt(sqrDistance) / fadeDistance;
        }
        else if (sqrDistance > (lifeDistance - fadeDistance) * (lifeDistance - fadeDistance))
        {
            targetAlpha = (lifeDistance - Mathf.Sqrt(sqrDistance)) / fadeDistance;
        }

        if (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
        {
            SetAlpha(targetAlpha);
        }

        if (sqrDistance >= lifeDistance * lifeDistance)
        {
            gameObject.SetActive(false);
        }
    }

    private void SetAlpha(float alpha)
    {
        currentAlpha = Mathf.Clamp01(alpha);

        for (int i = 0; i < matData.Length; i++)
        {
            Renderer r = matData[i].renderer;
            int matIndex = matData[i].materialIndex;

            r.GetPropertyBlock(propBlock, matIndex);

            Color c = matData[i].originalColor;
            c.a = currentAlpha;

            propBlock.SetColor(baseColorID, c);

            r.SetPropertyBlock(propBlock, matIndex);
        }
    }
}