using UnityEngine;
using System.Collections.Generic;

public class Icosahedron : MonoBehaviour {

    private string islandModel = "IslandSimple";

    public List<Vector3> icoVertices = new List<Vector3>();
    List<Edge> edges = new List<Edge>();

    private struct Edge {
        public int v1, v2;

        public Edge(int v1, int v2) {
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    public void Start() {
        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;
        float size = 2.0f;
        t *= size;

        icoVertices.Add(new Vector3(-size, t, 0));
        icoVertices.Add(new Vector3(size, t, 0));
        icoVertices.Add(new Vector3(-size, -t, 0));
        icoVertices.Add(new Vector3(size, -t, 0));

        icoVertices.Add(new Vector3(0, -size, t));
        icoVertices.Add(new Vector3(0, size, t));
        icoVertices.Add(new Vector3(0, -size, -t));
        icoVertices.Add(new Vector3(0, size, -t));

        icoVertices.Add(new Vector3(t, 0, -size));
        icoVertices.Add(new Vector3(t, 0, size));
        icoVertices.Add(new Vector3(-t, 0, -size));
        icoVertices.Add(new Vector3(-t, 0, size));

        

        //around p0
        edges.Add(new Edge(0, 1));
        edges.Add(new Edge(0, 5));
        edges.Add(new Edge(0, 11));
        edges.Add(new Edge(0, 10));
        edges.Add(new Edge(0, 7));

        edges.Add(new Edge(1, 5));
        edges.Add(new Edge(5, 11));
        edges.Add(new Edge(11, 10));
        edges.Add(new Edge(10, 7));
        edges.Add(new Edge(7, 1));

        //middleroll
        edges.Add(new Edge(1, 8));
        edges.Add(new Edge(8, 7));
        edges.Add(new Edge(7, 6));
        edges.Add(new Edge(6, 10));
        edges.Add(new Edge(10, 2));
        edges.Add(new Edge(2, 11));
        edges.Add(new Edge(11, 4));
        edges.Add(new Edge(4, 5));
        edges.Add(new Edge(5, 9));
        edges.Add(new Edge(9, 1));
        //around3
        edges.Add(new Edge(8, 6));
        edges.Add(new Edge(6, 2));
        edges.Add(new Edge(2, 4));
        edges.Add(new Edge(4, 9));
        edges.Add(new Edge(9, 8));

        edges.Add(new Edge(8, 3));
        edges.Add(new Edge(6, 3));
        edges.Add(new Edge(2, 3));
        edges.Add(new Edge(4, 3));
        edges.Add(new Edge(9, 3));

        icoVertices.ForEach(delegate (Vector3 point) {
            GameObject go = Instantiate(Resources.Load(islandModel, typeof(GameObject)), point, Quaternion.identity) as GameObject;
            go.transform.up = go.transform.position - new Vector3(0, 0, 0);
        });
    }

    public void Update() {
        edges.ForEach(delegate (Edge edge) {
            Debug.DrawLine(icoVertices[edge.v1], icoVertices[edge.v2]);
        });
    }
}
