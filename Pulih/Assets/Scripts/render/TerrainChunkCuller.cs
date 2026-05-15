using UnityEngine;

public class TerrainChunkCuller : MonoBehaviour
{
    [SerializeField] private CamerController cameraController;

    private Terrain terrain;
    private TerrainData terrainData;
    private Vector3 terrainPos;
    private Bounds terrainBounds;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrain.heightmapPixelError = 20; //kecil makin detail, tapi makin berat
        terrain.basemapDistance = 80f; // kecil makin ringan
        terrain.detailObjectDistance = 80f; // kecil makin ringan
        terrain.treeDistance = 100f; // kecil makin ringan
        terrain.treeBillboardDistance = 50f; //kecil ringan, tapi ngebuat pohon jadi 2D saat jauh

        // Ngecache terrain data saat start buat ngehindari overhead setiap frame
        terrainData = terrain.terrainData;
        terrainPos = terrain.transform.position;
        terrainBounds = new Bounds(
            terrainPos + terrainData.size / 2f,
            terrainData.size
        );
    }

    void Update()
    {
        if (cameraController.FrustumPlanes == null) return;

        // Pake chaced terrain bounds dan frustum planes buat ngecek visibility, daripada ngitung ulang setiap frame
        terrain.enabled = GeometryUtility.TestPlanesAABB(cameraController.FrustumPlanes, terrainBounds);
    }
}