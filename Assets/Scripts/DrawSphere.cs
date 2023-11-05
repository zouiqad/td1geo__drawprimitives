using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class DrawSphere : MonoBehaviour
{
    public Material mat;

    public float rayon = 5.0f;
    public int meridian = 3;
    public int paralel = 2;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        Draw(meridian, paralel, rayon);
    }

    private void OnDrawGizmos()
    {
        Draw(meridian, paralel, rayon);
    }

    private void Draw(int m, int p, float r)
    {
        Vector3[] vertices = new Vector3[m * p];
        int[] triangles = new int[m * p * 2 * 3];



        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();          // Creation d'un composant MeshFilter qui peut ensuite être visualisé
            gameObject.AddComponent<MeshRenderer>();
        }

        // Generate vertices
        int indexVertex = 0;
        float teta_slice = (2 * Mathf.PI) / m;
        float teta = 0.0f;

        float phi_slice  = Mathf.PI / p;
        float phi = phi_slice;


        
        for (int i = 1; i < p; i++)
        {
            teta = 0;
            float rayon = r * Mathf.Sin(phi);
            for (int j = 0; j < m; j++)
            {
                Vector3 point = new Vector3(rayon * Mathf.Cos(teta), r * Mathf.Cos(phi), rayon * Mathf.Sin(teta));
                vertices[indexVertex++] = point;
                teta += teta_slice;
            }
            phi += phi_slice;
        }

        // Add poles 
        vertices[indexVertex++] = new Vector3(0.0f, r * Mathf.Cos(0), 0.0f);
        vertices[indexVertex++] = new Vector3(0.0f, r * Mathf.Cos(Mathf.PI), 0.0f);



        
        // Draw triangles
        int indexTriangle = 0;

        // Draw Body
        for (int i = 0; i < (m * p) - (2 * m); i++)
        {
            int vertexA = i;
            int vertexB = ((vertexA + 1) % m == 0 && vertexA != 0) ? vertexA - m + 1 : vertexA + 1;
            int vertexC = vertexA + m;
            int vertexD = vertexB + m;


            triangles[indexTriangle++] = vertexA;
            triangles[indexTriangle++] = vertexD;
            triangles[indexTriangle++] = vertexC;

            triangles[indexTriangle++] = vertexA;
            triangles[indexTriangle++] = vertexB;
            triangles[indexTriangle++] = vertexD;
        }

        // Pole nord
        for (int i = 0; i < m; i++)
        {
            int vertexA = i;
            int vertexB = indexVertex - 2;
            int vertexC = ((vertexA + 1) % m == 0 && vertexA != 0) ? vertexA - m + 1 : vertexA + 1;

            triangles[indexTriangle++] = vertexA;
            triangles[indexTriangle++] = vertexB;
            triangles[indexTriangle++] = vertexC;
        }

        // Pole sud
        for (int i = m * p - 2 * m; i < m * p - m; i++)
        {
            int vertexA = i;
            int vertexB = indexVertex - 1;
            int vertexC = ((vertexA + 1) % m == 0 && vertexA != 0) ? vertexA - m + 1 : vertexA + 1;

            triangles[indexTriangle++] = vertexA;
            triangles[indexTriangle++] = vertexC;
            triangles[indexTriangle++] = vertexB;
        }

        Mesh msh = new Mesh();                          // Création et remplissage du Mesh

        msh.vertices = vertices;
        msh.triangles = triangles;

        gameObject.GetComponent<MeshFilter>().mesh = msh;           // Remplissage du Mesh et ajout du matériel
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }
}
