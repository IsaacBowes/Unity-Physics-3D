using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBody : MonoBehaviour
{
    public int numLinks;

    // Use this for initialization
    void Start ()
    {
        GameObject ball = Resources.Load<GameObject>("PhysicsSphere");
        Vector3 pos = transform.position;
        //instantiate an anchor and make it our child
        GameObject anchor = Instantiate(ball, pos, transform.rotation) as GameObject;
        anchor.transform.parent = gameObject.transform;
        anchor.name = "Anchor";
        anchor.GetComponent<Rigidbody>().isKinematic = true;

        GameObject[] links = new GameObject[numLinks];
        for (int i = 0; i < numLinks; i++)
        {
            // TODO spawn a link similar to how we spawned the anchor, and make it not Kinematic
            pos = pos - new Vector3(0, 1.0f, 0);
            GameObject link = Instantiate(ball, pos, transform.rotation) as GameObject;
            link.transform.parent = gameObject.transform;
            link.name = "PhysicsSphere";
            link.GetComponent<Rigidbody>().isKinematic = false;
            links[i] = link;
            // add a hinge to the anchor (for the first link) or the previous link
            HingeJoint joint = links[i].AddComponent<HingeJoint>();
            joint.connectedBody = (i == 0 ? anchor : links[i - 1]).GetComponent<Rigidbody>();
            // set up the anchors manually from centre to centre
            joint.anchor = new Vector3(0, 0, 0);
            joint.connectedAnchor = new Vector3(0, -1.0f, 0);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
