using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBody2D : MonoBehaviour
{
    MeshFilter cloth;
    public int numRows = 10;
    public int numColumns = 10;
    public int damping = 2500;
    public int spring = 5000;
    public Vector3 xAxis = new Vector3(1, 0, 0);
    public Vector3 yAxis = new Vector3(0, 0, 1);
    public bool showBalls;


    GameObject[][] links;
    // Use this for initialization
    void Start()
    {

        // get the prefab we set up earlier
        GameObject ball = Resources.Load<GameObject>("PhysicsSphere");
        Vector3 pos = transform.position;
        links = new GameObject[numColumns][];
        for (int i = 0; i < numColumns; i++)
        {
            links[i] = new GameObject[numRows];
            for (int j = 0; j < numRows; j++)
            {
                // move each one down by one unit (could replace this with spacing)
                Vector3 pos0 = pos + xAxis * i + yAxis * j;
                links[i][j] = Instantiate(ball, pos0, transform.rotation) as GameObject;
                links[i][j].transform.parent = gameObject.transform;
                links[i][j].name = "Link_" + i + "_" + j;
                links[i][j].GetComponent<Rigidbody>().isKinematic = false;
                if (i != 0)
                {
                    links[i][j].GetComponent<MeshRenderer>().enabled = showBalls;
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i - 1][j].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -xAxis;
                    joint.spring = spring;
                    joint.damper = damping;
                }
                if (j != 0)
                {
                    links[i][j].GetComponent<MeshRenderer>().enabled = showBalls;
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i][j-1].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -yAxis;
                    joint.spring = spring;
                    joint.damper = damping;
                }
                //Bend Springs
                if (i >= 2)
                {
                    links[i][j].GetComponent<MeshRenderer>().enabled = showBalls;
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i - 2][j].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -xAxis*2;
                    joint.spring = spring;
                    joint.damper = damping;
                }
                if (j >= 2)
                {
                    links[i][j].GetComponent<MeshRenderer>().enabled = showBalls;
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i][j - 2].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -yAxis*2;
                    joint.spring = spring;
                    joint.damper = damping;
                }

                // diagonals
                if (i != 0 && j != numRows-1)
                {
                    links[i][j].GetComponent<MeshRenderer>().enabled = showBalls;
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i - 1][j + 1].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -xAxis + yAxis;
                    joint.spring = spring;
                    joint.damper = damping;
                }
                if (i != 0 && j!=0)
                {
                    links[i][j].GetComponent<MeshRenderer>().enabled = showBalls;
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i - 1][j - 1].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -xAxis - yAxis;
                    joint.spring = spring;
                    joint.damper = damping;
                }
            }
        }
        cloth = GetComponent<MeshFilter>();
        if (cloth)
            SetUpCloth();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateCloth();
	}
    void SetUpCloth()
    {
        cloth.mesh = new Mesh();
        UpdateCloth();
        // TODO set up UV coordinates
        Vector2[] uvs = new Vector2[numRows * numColumns];
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                uvs[i + j * numColumns] = new Vector2(((float)i)/numRows-1, ((float)j)/numColumns-1);
            }
        }
        cloth.mesh.uv = uvs;

        // set up the topology
        int[] triangles = new int[(numRows - 1) * (numColumns - 1) * 12];
        int k = 0;
        for (int i = 0; i < numColumns - 1; i++)
        {
            for (int j = 0; j < numRows - 1; j++)
            {
                triangles[k] = i + j * numColumns; k++;
                triangles[k] = (i + 1) + j * numColumns; k++;
                triangles[k] = i + (j + 1) * numColumns; k++;
                triangles[k] = i + (j + 1) * numColumns; k++;
                triangles[k] = (i + 1) + j * numColumns; k++;
                triangles[k] = (i + 1) + (j + 1) * numColumns; k++;
                // do the triangles both ways for two-sided rendering
                triangles[k] = i + j * numColumns; k++;
                triangles[k] = i + (j + 1) * numColumns; k++;
                triangles[k] = (i + 1) + j * numColumns; k++;
                triangles[k] = (i + 1) + j * numColumns; k++;
                triangles[k] = i + (j + 1) * numColumns; k++;
                triangles[k] = (i + 1) + (j + 1) * numColumns; k++;
            }
        }
        cloth.mesh.triangles = triangles;
    }

    void UpdateCloth()
    {
        Vector3[] vertices = new Vector3[numRows * numColumns];
        Vector3[] normals = new Vector3[numRows * numColumns];
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                vertices[i + j * numColumns] = links[i][j].transform.position - transform.position;
                links[i][j].GetComponent<MeshRenderer>().enabled = showBalls;
            }
        }
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                Vector3 left = i == 0 ? vertices[i + j * numColumns] : vertices[i - 1 + j * numColumns];
                Vector3 right = i == numColumns - 1 ? vertices[i + j * numColumns] : vertices[i + 1 + j * numColumns];
                Vector3 down = j == 0 ? vertices[i + j * numColumns] : vertices[i + (j - 1) * numColumns];
                Vector3 up = j == numRows - 1 ? vertices[i + j * numColumns] : vertices[i + (j + 1) * numColumns];
                normals[i + j * numColumns] = Vector3.Cross(right - left, up - down);
                normals[i + j * numColumns].Normalize();
            }
        }
        cloth.mesh.vertices = vertices;
        cloth.mesh.normals = normals;
        cloth.mesh.RecalculateBounds();
    }
}
