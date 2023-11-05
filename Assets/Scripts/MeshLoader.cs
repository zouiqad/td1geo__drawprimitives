using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;
using System;

public class MeshLoader : MonoBehaviour
{
    public string fileName = "cube.off";
    public List<Vector3> Vertices { get; } = new List<Vector3>();
    public List<int> Triangles { get; } = new List<int>();
    public List<Vector3> Normals { get; } = new List<Vector3>();

    private Dictionary<Tuple<int ,int>, int> Edges = new Dictionary<Tuple<int, int>, int>();
    private Dictionary<int, List<Vector3>> normalHash = new Dictionary<int, List<Vector3>>();

    public Material mat;
    private Mesh msh;

    private int vertexCount = 0;
    private int faceCount = 0;
    private int edgeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        LoadOFF(fileName);
        DrawMesh();
        CalculateBounds();
        ComputeSmoothNormals();
        GetMeshInfo();
        WriteOFF("test.off");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            Handles.DrawLine(Vertices[i], Vertices[i] + Normals[i]);
        }
    }


    private void DrawMesh()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();


        msh = new Mesh();                          // Création et remplissage du Mesh

        msh.vertices = Vertices.ToArray();
        msh.triangles = Triangles.ToArray();

        gameObject.GetComponent<MeshFilter>().mesh = msh;           // Remplissage du Mesh et ajout du matériel
        gameObject.GetComponent<MeshRenderer>().material = mat;

    }

    private void ComputeSmoothNormals()
    {
        for (int i = 0; i < Triangles.Count; i += 3)
        {
            int[] verticesIndex = { Triangles[i], Triangles[i + 1], Triangles[i + 2] };


            // Calculate edges 
            Vector3 e1 = Vertices[verticesIndex[1]] - Vertices[verticesIndex[0]]; // b - a
            Vector3 e2 = Vertices[verticesIndex[2]] - Vertices[verticesIndex[0]]; // c - a

            // Compute normal
            Vector3 cross = Vector3.Cross(e1, e2); // N = (b - a) x (c - a)


            foreach(int vertex in verticesIndex)
            {
                if (!normalHash.ContainsKey(vertex))
                {
                    List<Vector3> vertexNormals = new List<Vector3>();
                    vertexNormals.Add(cross);
                    normalHash.Add(vertex, vertexNormals);
                }
                else
                {
                    normalHash[vertex].Add(cross);
                }
            }

        }

        // Remplir les normales
        for (int i = 0; i < Vertices.Count; i++)
        {
            Vector3 averageNormal = Vector3.zero;
            foreach (Vector3 normal in normalHash[i])
            {
                averageNormal += normal;
            }

            averageNormal /= normalHash[i].Count;
            Normals.Add(averageNormal.normalized);
        }

        msh.normals = Normals.ToArray();
        msh.RecalculateNormals();

    }

    private void CalculateBounds()
    {
        Vector3 sum = Vector3.zero;
        Vector3 max = Vector3.zero;

        for(int i = 0; i < Vertices.Count; i++)
        {
            sum += Vertices[i];

            // Calculer le vertex max
            if(Vertices[i].magnitude > max.magnitude)
            {
                max = Vertices[i];
            }
        }

        Vector3 center = sum / (float)Vertices.Count;

        for (int i = 0; i < Vertices.Count; i++)
        {
            Vertices[i] -= center;
            Vertices[i] /= max.magnitude;

        }
        
        msh.vertices = Vertices.ToArray();
        msh.RecalculateBounds();
    }

    private void GetMeshInfo()
    {
        int vertexCount = Vertices.Count;
        int faceCount = Triangles.Count / 3;
        int edgeCount = 0;

        // Calculate edge count
        for (int i = 0; i < Triangles.Count; i += 3)
        {
            Tuple<int, int>[] triangleEdges =
            {
                new Tuple<int, int>(Triangles[i], Triangles[i + 1]),
                new Tuple<int, int>(Triangles[i], Triangles[i + 2]),
                new Tuple<int, int>(Triangles[i + 1], Triangles[i + 2])
            };


            foreach(var edge in triangleEdges)
            {
                Tuple<int, int> mirrorTuple = new Tuple<int, int>(edge.Item2, edge.Item1);
                if (!Edges.ContainsKey(edge))
                {
                    if (!Edges.ContainsKey(mirrorTuple))
                    {
                        Edges.Add(edge, 0);
                    }
                    else
                    {
                        Edges[mirrorTuple] += 1;
                    }

                    
                }
                else
                {
                    Edges[edge] += 1;
                }
            }

        }

        foreach(var edge in Edges)
        {
            Debug.Log("EDGE: " + edge);
        }

        Debug.Log("EDGE COUNT" + Edges.Count);

        Debug.Log("TRIANGLE COUNT" + Triangles.Count / 3);

        Debug.Log("VERTEX COUNT" + Vertices.Count);
    }

    private void WriteOFF(string fileName)
    {
        string filePath = Path.Combine(@"C:\Users\death\Desktop", fileName);

        using (StreamWriter ws = new StreamWriter(filePath))
        {

            int vertexCount = Vertices.Count;
            int faceCount = Triangles.Count / 3;
            int edgeCount = 0;

            // Write Header 
            ws.WriteLine("OFF");
            ws.WriteLine($"{vertexCount} {faceCount} {edgeCount}");

            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-us");
            foreach (var vertex in Vertices)
            {
                ws.WriteLine($"{vertex.x} {vertex.y} {vertex.z}");
            }


            for (int i = 0; i < Triangles.Count; i += 3)
            {
                ws.WriteLine($"3 {Triangles[i]} {Triangles[i + 1]} {Triangles[i + 2]}");
            }

        }
    }

    private void LoadOFF(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, fileName);

        using (StreamReader rs = new StreamReader(filePath))
        {
            string line = "";
            string header;
            int lineCount = 0;
            int vertexCount = 0;
            int faceCount = 0;
            Vector3 sum = Vector3.zero;

            // CHECK HEADER
            if ((header = rs.ReadLine()) != null) {
                lineCount++;
                header = header.Trim();

                if (!header.StartsWith("OFF"))
                {
                    throw new System.Exception("Invalid OFF file format");
                }

                line = rs.ReadLine();
                lineCount++;
                line = line.Trim();

                string[] headerParts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (header.Length != 3)
                {
                    throw new System.Exception("Invalid header in OFF file");
                }

                vertexCount = int.Parse(headerParts[0]);
                faceCount = int.Parse(headerParts[1]);
                

            }


            line = rs.ReadLine();

            // Read vertices
            while (line  != null && lineCount < vertexCount + 2)
            {
                lineCount++;
                line = line.Trim();

                string[] vertexParts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

                if (vertexParts.Length != 3)
                {
                    throw new System.Exception($"Invalid vertex data at line {lineCount}");
                }

                float x = float.Parse(vertexParts[0], System.Globalization.CultureInfo.InvariantCulture);
                float y = float.Parse(vertexParts[1], System.Globalization.CultureInfo.InvariantCulture);
                float z = float.Parse(vertexParts[2], System.Globalization.CultureInfo.InvariantCulture);


                Vertices.Add(new Vector3(x, y, z));

                // Read next line
                line = rs.ReadLine();
            }

            // Read indices
            while (line != null && lineCount <= vertexCount + faceCount + 2)
            {
                lineCount++;
                line = line.Trim();

                string[] faceParts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);


                if (faceParts.Length < 4 || int.Parse(faceParts[0]) != 3)
                {
                    throw new System.Exception($"Invalid face data at line {lineCount}");
                }

                for(int i = 1; i < faceParts.Length; i++)
                {
                    Triangles.Add(int.Parse(faceParts[i]));
                }

                // Read next line 
                line = rs.ReadLine();
            }
        }
    }

}


