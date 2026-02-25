using System.Collections.Generic;
using UnityEngine;

public class CutSurface : MonoBehaviour
{
    [SerializeField] Sprite surfaceSprite;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] GameObject blankPaper;

    int gridW;
    int gridH;
    Mesh originalMesh;
    Texture2D surfaceTexture;


    void Start()
    {
        surfaceTexture = surfaceSprite.texture;
        originalMesh = meshFilter.mesh;
        gridW = surfaceTexture.width / 10;
        gridH = surfaceTexture.height / 10;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Cut();
        }
    }

    void Cut()
    {
        var mask = BuildMask(surfaceTexture, gridW, gridH);
        var regions = ExtractRegions(mask);
        GenerateMeshesFromRegions(regions);
        blankPaper.gameObject.SetActive(false);
    }

    bool[,] BuildMask(Texture2D tex, int width, int height, float alphaThreshold = 0.1f)
    {
        bool[,] mask = new bool[width, height];

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            float u = (x + 0.5f) / width;
            float v = (y + 0.5f) / height;
            Color p = tex.GetPixelBilinear(u, v);
            mask[x, y] = p.a > alphaThreshold;
        }

        return mask;
    }


    void CreateGeneratedMesh(
        string name,
        List<Vector3> verts,
        List<Vector2> uvs,
        List<int> tris)
    {
        GameObject go = new GameObject(name)
        {
            transform =
            {
                position = blankPaper.transform.position,
                rotation = blankPaper.transform.rotation,
                localScale = blankPaper.transform.localScale
            }
        };
        go.transform.SetParent(this.transform);

        Mesh m = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
            vertices = verts.ToArray(),
            uv = uvs.ToArray(),
            triangles = tris.ToArray()
        };

        m.RecalculateNormals();
        m.RecalculateBounds();
        m.RecalculateTangents();

        go.AddComponent<MeshFilter>().mesh = m;
        go.AddComponent<MeshRenderer>().material =
            meshFilter.GetComponent<MeshRenderer>().material;
    }

    List<List<Vector2Int>> ExtractRegions(bool[,] mask)
    {
        bool[,] visited = new bool[gridW, gridH];
        List<List<Vector2Int>> regions = new();

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        for (int y = 0; y < gridH; y++)
        for (int x = 0; x < gridW; x++)
        {
            if (mask[x, y] || visited[x, y])
                continue;

            List<Vector2Int> region = new();
            Queue<Vector2Int> q = new();
            q.Enqueue(new Vector2Int(x, y));
            visited[x, y] = true;

            while (q.Count > 0)
            {
                var p = q.Dequeue();
                region.Add(p);

                for (int i = 0; i < 4; i++)
                {
                    int nx = p.x + dx[i];
                    int ny = p.y + dy[i];

                    if (nx >= 0 && nx < gridW && ny >= 0 && ny < gridH)
                    {
                        if (!mask[nx, ny] && !visited[nx, ny])
                        {
                            visited[nx, ny] = true;
                            q.Enqueue(new Vector2Int(nx, ny));
                        }
                    }
                }
            }

            regions.Add(region);
        }

        return regions;
    }

    void GenerateMeshesFromRegions(List<List<Vector2Int>> regions)
    {
        Bounds b = originalMesh.bounds;
        float width = b.size.x;
        float depth = b.size.z;
        float cellW = width / gridW;
        float cellH = depth / gridH;
        int regionIndex = 0;

        foreach (var region in regions)
        {
            List<Vector3> verts = new();
            List<Vector2> uvs = new();
            List<int> tris = new();

            int vertIndex = 0;

            foreach (var cell in region)
            {
                int x = cell.x;
                int y = cell.y;
                float px = b.min.x + x * cellW;
                float pz = b.min.z + y * cellH;
                Vector3 v0 = new(px, 0, pz);
                Vector3 v1 = new(px + cellW, 0, pz);
                Vector3 v2 = new(px + cellW, 0, pz + cellH);
                Vector3 v3 = new(px, 0, pz + cellH);

                verts.Add(v0);
                verts.Add(v1);
                verts.Add(v2);
                verts.Add(v3);

                Vector2 uv0 = new((float)x / gridW, (float)y / gridH);
                Vector2 uv1 = new((float)(x + 1) / gridW, (float)y / gridH);
                Vector2 uv2 = new((float)(x + 1) / gridW, (float)(y + 1) / gridH);
                Vector2 uv3 = new((float)x / gridW, (float)(y + 1) / gridH);

                uvs.Add(uv0);
                uvs.Add(uv1);
                uvs.Add(uv2);
                uvs.Add(uv3);

                tris.Add(vertIndex + 0);
                tris.Add(vertIndex + 2);
                tris.Add(vertIndex + 1);

                tris.Add(vertIndex + 0);
                tris.Add(vertIndex + 3);
                tris.Add(vertIndex + 2);

                vertIndex += 4;
            }

            CreateGeneratedMesh($"Region_{regionIndex}", verts, uvs, tris);
            regionIndex++;
        }
    }
}