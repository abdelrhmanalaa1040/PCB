using UnityEngine;

public class RealTimeBend : MonoBehaviour
{
    public float bendAmount = 0.2f;  // ãÞÏÇÑ ÇáÇÑÊÝÇÚ ÚäÏ ÇáÊÞÇØÚ
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;
    private bool isBending = false;
    private Transform otherPipe;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
    }

    void Update()
    {
        if (isBending && otherPipe != null)
        {
            for (int i = 0; i < originalVertices.Length; i++)
            {
                Vector3 worldPos = transform.TransformPoint(originalVertices[i]);
                float distance = Vector3.Distance(worldPos, otherPipe.position);

                if (distance < 0.5f) // ÊÚÏíá ÍÓÈ ÍÌã ÇáãÕæÑÉ
                {
                    modifiedVertices[i] = originalVertices[i] + Vector3.up * bendAmount * (1 - distance / 0.5f);
                    print(modifiedVertices[i]);

                }
                else
                {
                    modifiedVertices[i] = originalVertices[i] + Vector3.up * bendAmount * (1 - distance / 0.5f);
                    print(modifiedVertices[i]);

                }
            }

            mesh.vertices = modifiedVertices;
            mesh.RecalculateNormals();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pipe")) // ÊÃßÏ ãä Ãä ÇáãÕæÑÉ ÇáÃÎÑì áÏíåÇ ÇáÊÇÌ "Pipe"
        {
            isBending = true;
            otherPipe = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pipe"))
        {
            isBending = false;
            ResetMesh();
        }
    }

    void ResetMesh()
    {
        print("ResetMesh");
        mesh.vertices = originalVertices;
        mesh.RecalculateNormals();
    }
}
