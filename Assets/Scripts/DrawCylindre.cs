using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawCylin : MonoBehaviour
{
    public Material mat;
    public Quaternion currentRotation;

    public float rayon = 2.0f;
    public float h = 5.0f;
    public int meridian = 3;
    

    // Start is called before the first frame update
    void Start()
    {
        DrawCylindre(meridian, rayon, h);
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void DrawCylindre(int m, float r, float h)
    {
        Vector3[] vertices = new Vector3[2 + m*2] ;
        int[] triangles = new int[m*2*3*2];


        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();          // Creation d'un composant MeshFilter qui peut ensuite �tre visualis�
            gameObject.AddComponent<MeshRenderer>();
        }

        // Generate vertices
        int vertexIndex = 0;
        float phi = (2 * Mathf.PI) / m;
        float teta = 0.0f;

        for (int i = 0; i < m; i++)
        {
            
            Vector3 p = new Vector3(r * Mathf.Cos(teta), h/2, r * Mathf.Sin(teta));
            Vector3 pp = new Vector3(r * Mathf.Cos(teta), -1*h/2, r * Mathf.Sin(teta));

            vertices[vertexIndex++] = p;
            vertices[vertexIndex++] = pp;

            teta += phi;
            Debug.Log(teta);
            Debug.Log(p);
            Debug.Log(pp);

        }

        
        // Generate triangles

        int triangleIndex = 0;

        // Draw Body

        for (int i = 0; i < (m-1); i++)
        {
            int vertexA = i * 2;
            int vertexB = vertexA + 1;
            int vertexC = vertexB + 1;
            int vertexD = vertexC + 1;


            triangles[triangleIndex++] = vertexA;
            triangles[triangleIndex++] = vertexD;
            triangles[triangleIndex++] = vertexB;

            triangles[triangleIndex++] = vertexA;
            triangles[triangleIndex++] = vertexC;
            triangles[triangleIndex++] = vertexD;
        }

        triangles[triangleIndex++] = (m * 2) - 2;
        triangles[triangleIndex++] = 1;
        triangles[triangleIndex++] = (m * 2) - 1;

        triangles[triangleIndex++] = (m * 2) - 2;
        triangles[triangleIndex++] = 0;
        triangles[triangleIndex++] = 1;



        Vector3 origin_top = new Vector3(0.0f, h/2, 0.0f);
        Vector3 origin_bot = new Vector3(0.0f, -1 * h / 2, 0.0f);


        vertices[vertexIndex++] = origin_top;
        vertices[vertexIndex++] = origin_bot;

        // Draw Top
        for (int i = 0; i <= m; i+=2)
        {
            triangles[triangleIndex++] = vertices.Length - 2;
            triangles[triangleIndex++] = i + 2;
            triangles[triangleIndex++] = i + 2;
        }

        // Draw Bot
        for (int i = 1; i <= m; i += 2)
        {
            triangles[triangleIndex++] = vertices.Length - 1;
            triangles[triangleIndex++] = i + 2;
            triangles[triangleIndex++] = i;
        }

        Mesh msh = new Mesh();                          // Cr�ation et remplissage du Mesh

        msh.vertices = vertices;
        msh.triangles = triangles;

        gameObject.GetComponent<MeshFilter>().mesh = msh;           // Remplissage du Mesh et ajout du mat�riel
        gameObject.GetComponent<MeshRenderer>().material = mat;

    }
}
