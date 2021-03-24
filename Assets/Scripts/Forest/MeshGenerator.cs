using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class to generate voxels given a noise map. This will be used afterwards to generate a mesh using the marching squares algorithm.
public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;
    private List<Vector3> vertices;
    private List<int> triangles;

    public MeshFilter walls;
    public MeshFilter generatedMap;
    public float wallHeight;


    //Dictionary to return the list of triangles a vertex is part of.
    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();

    //Store outlines as list of vertices.
    List<List<int>> outlines = new List<List<int>>();

    //Make sure we don't double-check vertices.
    HashSet<int> checkedVertices = new HashSet<int>();

    struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertices;

        public Triangle(int a, int b, int c)
        {
            this.vertexIndexA = a;
            this.vertexIndexB = b;
            this.vertexIndexC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        public int this[int i]
        {
            get
            {
                return vertices[i];
            }
        }

        public bool Contains(int vertexIndex)
        {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }

    public void GenerateMesh(int[,] map, float squareSize)
    {
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshCollider meshCollider = this.GetComponent<MeshCollider>();

        //Reset outlines before generating a new mesh.
        outlines.Clear();
        checkedVertices.Clear();
        triangleDictionary.Clear();

        squareGrid = new SquareGrid(map, squareSize);
        vertices = new List<Vector3>();
        triangles = new List<int>();

        //Triangulate mesh based on square configuration for each existent square.
        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        generatedMap.mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        //Apply texture to the map.
        ApplyTexture(map, squareSize, mesh);

        //Create walls.
        CreateWallMesh();

        //Create custom collider for generated map.
        GenerateColliders2D();
            
    }

    void CreateWallMesh()
    {
        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
       

        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count - 1; i++)
            {
                int startIndex = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]); // left
                wallVertices.Add(vertices[outline[i + 1]]); // right
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
                wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight); // bottom right

                
                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);


                ///////////////////////////////////
                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 2);

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 3);

                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 1);

                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 3);

                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 1);

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 2);

                /////////////////////////////////////
                ///

                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 2);

                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 3);

                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 0);

                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 3);

                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 0);

                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 2);

                //////////////////////////////////
                ///

                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 1);

                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 3);

                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 0);

                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 3);

                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 0);

                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 1);

                /////////////////////////////////
                ///

                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 1);

                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 0);
                //wallTriangles.Add(startIndex + 2);

                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 0);

                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 1);
                //wallTriangles.Add(startIndex + 2);

                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 0);

                //wallTriangles.Add(startIndex + 3);
                //wallTriangles.Add(startIndex + 2);
                //wallTriangles.Add(startIndex + 1);






            }
        }
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        walls.mesh = wallMesh;

        MeshCollider wallCollider = walls.gameObject.AddComponent<MeshCollider>();
        wallCollider.sharedMesh = wallMesh;
        
       
    }

    void GenerateColliders2D()
    {
        //Reset any existent colliders before generating new custom one.
        EdgeCollider2D[] colliders = gameObject.GetComponents<EdgeCollider2D>();

        for( int i=0; i<colliders.Length; i++)
        {
            Destroy(colliders[i]);
        }

        //CalculateMeshOutlines();

        //Generate edge collider based on extracted outline and add it to map object.
        foreach( List<int> outline in outlines)
        {
            EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            Vector2[] edgeVertices = new Vector2[outline.Count];

            for(int i = 0; i< outline.Count; i++)
            {
                edgeVertices[i] = vertices[outline[i]];
            }

            edgeCollider.points = edgeVertices;
        }


    }

    void ApplyTexture(int[,] map, float squareSize, Mesh mesh)
    {
        int tileAmount = 10;
        Vector2[] uvs = new Vector2[vertices.Count];

        for(int i=0; i< vertices.Count; i++)
        {
            //float percentX = Mathf.InverseLerp(-map.GetLength(0) /2  * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].x) * tileAmount;
            //float percentY = Mathf.InverseLerp(-map.GetLength(0) /2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].y) * tileAmount;
            // uvs[i] = new Vector2(percentX, percentY);
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }

        mesh.uv = uvs;
    }

    public void TriangulateSquare(Square square)
    {
        switch(square.configNo)
        {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centreRight, square.centreTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 6:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                break;
            case 5:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);

                //All surrounding points are walls, so check them directly to increase efficiency.
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;

        }
    }

    void MeshFromPoints(params SecondaryNode[] nodes)
    {
        AssignVertices(nodes);

        if(nodes.Length >= 3)
        {
            CreateTriangle(nodes[0], nodes[1], nodes[2]);
        }
        if(nodes.Length >= 4)
        {
            CreateTriangle(nodes[0], nodes[2], nodes[3]);
        }
        if(nodes.Length >= 5)
        {
            CreateTriangle(nodes[0], nodes[3], nodes[4]);
        }
        if(nodes.Length >= 6)
        {
            CreateTriangle(nodes[0], nodes[4], nodes[5]);
        }
    }

    void AssignVertices(SecondaryNode[] points)
    {
        for(int i = 0; i< points.Length; i++)
        {
            if(points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].pos);
            }
        }
    }

    void CreateTriangle(SecondaryNode vert1, SecondaryNode vert2, SecondaryNode vert3)
    {
        triangles.Add(vert1.vertexIndex);
        triangles.Add(vert2.vertexIndex);
        triangles.Add(vert3.vertexIndex);

        Triangle triangle = new Triangle(vert1.vertexIndex, vert2.vertexIndex, vert3.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
    {
        //Check if triangle already contains vertex index.
        if(triangleDictionary.ContainsKey(vertexIndexKey))
        {
            triangleDictionary[vertexIndexKey].Add(triangle);
        }
        else
        {
            List<Triangle> temp = new List<Triangle>();
            temp.Add(triangle);
            triangleDictionary.Add(vertexIndexKey, temp);
        }
    }

    private bool IsEdgeOnOutline(int vertA, int vertB)
    {
        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertA];
        int sharedTriangleCount = 0;

        for(int i = 0; i< trianglesContainingVertexA.Count; i++)
        {
            if(trianglesContainingVertexA[i].Contains(vertB))
            {
                sharedTriangleCount++; 

                if(sharedTriangleCount > 1)
                {
                    //Found shared triangle! 2 points form an outline edge if and only if they share one single triangle.
                    //See https://stackoverflow.com/questions/63676225/find-the-outline-of-a-roughly-2d-mesh for image for this rule.
                    break;

                }
            }
        }

        return sharedTriangleCount == 1;
    }

    private int GetConnectedOutlineVertex(int first, int vertIndex)
    {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertIndex];

        //Find first vertex that forms an outline edge with vertIndex.
        for( int i = 0; i< trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];

            //Check which vertex from triangles creates an outline edge.
            for(int j = 0; j < 3; j++)
            {
                int secondVert = triangle[j];

                if((secondVert != vertIndex && !checkedVertices.Contains(secondVert) && IsEdgeOnOutline(vertIndex, secondVert))
                    ||(secondVert != vertIndex && secondVert == first && IsEdgeOnOutline(vertIndex, secondVert)))
                {
                    return secondVert;
                }
            }
        }

        return -1;
    }

    private int GetConnectedOutlineVertexExceptFirstPathVertex(int vertIndex)
    {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertIndex];

        //Find first vertex that forms an outline edge with vertIndex.
        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];

            //Check which vertex from triangles creates an outline edge.
            for (int j = 0; j < 3; j++)
            {
                int secondVert = triangle[j];

                if (secondVert != vertIndex && !checkedVertices.Contains(secondVert) && IsEdgeOnOutline(vertIndex, secondVert))
                {
                    return secondVert;
                }
            }
        }

        return -1;
    }

    void CalculateMeshOutlines()
    {
        //Go through every vertex in map. Check if it's outline vertex. If yes, follow outline all way around. Add it to outlines list.
        for(int vertIndex =0; vertIndex < vertices.Count; vertIndex ++)
        {
            if(! checkedVertices.Contains(vertIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertexExceptFirstPathVertex(vertIndex);

                //If we found second point that forms edge with vertIndex.
                if(newOutlineVertex != -1)
                {
                    

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertIndex);
                    outlines.Add(newOutline);

                    checkedVertices.Add(vertIndex);

                    //outlines.Count - 1 is the index for the outline list we have just added to the list of outlines.
                    FollowOutline(vertIndex, newOutlineVertex, outlines.Count - 1);


                    //Mark it as checked.

                    //Somehow check here if vertIndex is connected with last node from path. If it is, add it.
                    int last = outlines[outlines.Count - 1][outlines[outlines.Count - 1].Count - 1];
                    if(IsEdgeOnOutline(last, vertIndex))
                    {
                        outlines[outlines.Count - 1].Add(vertIndex);
                    }
                        
                    //outlines[outlines.Count - 1].Add(vertIndex);

                }
            }
        }
    }

    private void FollowOutline(int first, int vertexIndex, int outlineIndex)
    {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        //int nextVertexIndex = GetConnectedOutlineVertex(first, vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertexExceptFirstPathVertex(vertexIndex);

        if (nextVertexIndex != -1)
        {
            FollowOutline(first, nextVertexIndex, outlineIndex);
        } 
    }

    /*
    private void OnDrawGizmos()
    {
        if (squareGrid != null)
        {
            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    Gizmos.color = (squareGrid.squares[x, y].topLeft.ground) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.pos, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].topRight.ground) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].topRight.pos, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomRight.ground) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomRight.pos, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomLeft.ground) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeft.pos, Vector3.one * .4f);

                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreTop.pos, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreRight.pos, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreBottom.pos, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreLeft.pos, Vector3.one * .15f);

                }
            }

        }
    } */

    public class SquareGrid
    {
        //This class is used to represent the entire map = matrix of voxels squares.
        public Square[,] squares;
        

        public SquareGrid(int[,] noiseMap, float squareSize)
        {
            int mainNodeCountX = noiseMap.GetLength(0);
            int mainNodeCountY = noiseMap.GetLength(1);

            MainNode[,] mainNodes = new MainNode[mainNodeCountX, mainNodeCountY];

            for (int indexX = 0; indexX < mainNodeCountX; indexX++)
            {
                for(int indexY = 0; indexY < mainNodeCountY; indexY++)
                {
                    //Create main nodes.
                    Vector3 pos = new Vector3(indexX * squareSize, indexY * squareSize);
                    mainNodes[indexX, indexY] = new MainNode(pos, noiseMap[indexX, indexY] == 1, squareSize);
                }
            }

            squares = new Square[mainNodeCountX - 1, mainNodeCountY - 1];

            for (int x = 0; x < mainNodeCountX - 1; x++)
            {
                for (int y = 0; y < mainNodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(mainNodes[x, y + 1], mainNodes[x + 1, y + 1], mainNodes[x, y], mainNodes[x + 1, y] );


                }
            }

        }

    }

    public class Square
    {
        /* Has 4 main nodes and 4 secondary nodes in this form:
        *   M(1)     S      M(2)
        *
        *   S               S
        *
        *   M(4)     S      M(3)
        *
        * This square shape will help us create all possible configurations in the Marching Squares algorithm.
        *
        */

        public MainNode topLeft, topRight, bottomLeft, bottomRight;
        public SecondaryNode centreTop, centreRight, centreLeft, centreBottom;
        public int configNo;

        public Square(MainNode topLeft, MainNode topRight, MainNode bottomLeft, MainNode bottomRight)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;

            //Assign secondary nodes now based on main nodes' position.
            this.centreTop = topLeft.right;
            this.centreRight = bottomRight.up;
            this.centreLeft = bottomLeft.up;
            this.centreBottom = bottomLeft.right;

            //Assign configuration no = which marching square configuration it should be.
            if(topLeft.ground)
            {
                configNo += 8;
            }
            if(topRight.ground)
            {
                configNo += 4;
            }
            if(bottomRight.ground)
            {
                configNo += 2;
            }
            if(bottomLeft.ground)
            { 
                configNo += 1; 
            }
        }
    }

    public class SecondaryNode
    {
        public Vector3 pos;
        public int vertexIndex = -1;

        public SecondaryNode(Vector3 position)
        {
            this.pos = position;
        }
    }

    public class MainNode: SecondaryNode
    {
        public SecondaryNode right, up;
        public bool ground;

        public MainNode(Vector3 position, bool isGround, float squareSize): base(position)
        {
            ground = isGround;
            right = new SecondaryNode(position + Vector3.right * squareSize / 2f);
            up = new SecondaryNode(position + Vector3.up * squareSize / 2f);
        }

    }  
}
