using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Hello_Triangle : MonoBehaviour
{

    public Material mat;
    public Quaternion currentRotation;

    public int nb_ligne = 2;
    public int nb_colonne = 2;


    // Use this for initialization
    void Start()
    {
        drawTriangle(nb_ligne, nb_colonne);
    }

    void Update()
    {

        drawTriangle(nb_ligne, nb_colonne);
    }
    

    private void drawTriangle(int n, int m)
    {

        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();          // Creation d'un composant MeshFilter qui peut ensuite être visualisé
            gameObject.AddComponent<MeshRenderer>();
        }
        int nb_triangles = n * m * 2;
        Vector3[] vertices = new Vector3[nb_triangles * 3];
        int[] triangles = new int[nb_triangles * 3];


        // Generate vertices
        int vertexIndex = 0;

        for (int i = 0; i <= n; i++)
        {
            for (int j = 0; j <= m; j++)
            {
                vertices[vertexIndex++] = new Vector3((float)i, (float)j, 0.0f);
            }
        }

        // Generate triangles 
        int triangleIndex = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                int vertexA = i * (m + 1) + j;
                int vertexB = vertexA + 1;
                int vertexC = (m + 1) * (i + 1) + j;
                int vertexD = vertexC + 1;

                triangles[triangleIndex++] = vertexA;
                triangles[triangleIndex++] = vertexB;
                triangles[triangleIndex++] = vertexD;

                triangles[triangleIndex++] = vertexA;
                triangles[triangleIndex++] = vertexD;
                triangles[triangleIndex++] = vertexC;

            }
        }



        Mesh msh = new Mesh();                          // Création et remplissage du Mesh

        msh.vertices = vertices;
        msh.triangles = triangles;

        gameObject.GetComponent<MeshFilter>().mesh = msh;           // Remplissage du Mesh et ajout du matériel
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }

}

