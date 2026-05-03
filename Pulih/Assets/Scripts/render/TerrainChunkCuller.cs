using UnityEngine;

public class TerrainChunkCuller : MonoBehaviour
{
  [SerializeField] private CamerController cameraController;

    private Terrain terrain;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrain.heightmapPixelError = 20; //makin gede makin ringan
        terrain.basemapDistance = 80f;//kecil mkin ringan
        terrain.detailObjectDistance = 80f; ///kecil makin ringan
        terrain.treeDistance = 100f;//kecil ringan
        terrain.treeBillboardDistance = 50f;
    }

    void Update()
    {
        if (cameraController.FrustumPlanes == null) return;

        TerrainData td = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        Bounds fullBounds = new Bounds(
            terrainPos + td.size / 2f,
            td.size
        );

        terrain.enabled = GeometryUtility.TestPlanesAABB(cameraController.FrustumPlanes, fullBounds);
    }
}