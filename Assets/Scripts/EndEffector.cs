using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEffector : MonoBehaviour {
    public Vector3 PozycjaGlobalna;
    public GameObject leftfinger;
    public GameObject rightfinger;
    public GameObject center;
    private Rigidbody rightbody;


    public float UstawKąt = 0;
    public float BierzącyKąt;
    public bool Close;
    public bool Open;
    public float speed = 0.1f;
    public GameObject Target;
    public Vector3 TargetPosition;
    public Vector3 NewPosition;
    public Vector3 LastPosition;

    public float DistanceToTarget;
    public float DistanceToTargetC;
    public float ZeroToTargetXYZ;
    public float DefaultLength;
    public float DegreeToTarget;

    public bool SetEfectorOnTarget;
    public bool SetEfectorBase;
    public bool CanMove = false;
    Renderer rend;
    // Use this for initialization
    void Start () {
        rightbody = rightfinger.GetComponent<Rigidbody>();
        //rend = Target.GetComponent<Renderer> ();  
    }

    public Vector3 abc;
    public float r;
    // Update is called once per frame
    void Update() {
        PozycjaGlobalna = transform.position;
        transform.localEulerAngles = new Vector3(UstawKąt, 0, 0);
        BierzącyKąt = transform.localEulerAngles.x;



        if (SetEfectorOnTarget)
        {
        //TargetPosition = Target.transform.position;

        ZeroToTargetXYZ = Vector3.Magnitude(TargetPosition);

        if (ZeroToTargetXYZ > DefaultLength && TargetPosition.y > 0)
        {
            CanMove = false;
           // rend.material.color = Color.red;
        }
        else
        {
           // rend.material.color = Color.green;
            CanMove = true;
        }
    

        DistanceToTarget = Mathf.Sqrt(Mathf.Pow(TargetPosition.z - transform.position.z, 2) + Mathf.Pow(TargetPosition.y - transform.position.y, 2));
        DistanceToTargetC = Mathf.Sqrt(Mathf.Pow(TargetPosition.z - LastPosition.z, 2) + Mathf.Pow(TargetPosition.y - LastPosition.y, 2));
        NewPosition = new Vector3(TargetPosition.x, TargetPosition.y - transform.position.y, TargetPosition.z);


            abc.x = Vector3.Distance(LastPosition, transform.position);
            abc.y = DistanceToTarget;
            abc.z = DistanceToTargetC;



            r = ((abc.z * abc.z) - (abc.x * abc.x) - (abc.y * abc.y)) / (2 * abc.x * abc.y);


            if (NewPosition.z > 0)
                DegreeToTarget = Mathf.Acos(r) * Mathf.Rad2Deg;
            if (NewPosition.z < 0)
                DegreeToTarget = 0 - Mathf.Acos(r) * Mathf.Rad2Deg;

            UstawKąt = DegreeToTarget;
        }



        if (SetEfectorBase)
        {
            if (UstawKąt > 0)
                UstawKąt -= 1.1f;
            if (UstawKąt < 0)
                UstawKąt += 1.1f;

        }

        //DegreeToTarget = Vector3.Angle(TargetPosition,transform.position);

        if (Close)
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            rightfinger.transform.position = Vector3.MoveTowards(rightfinger.transform.position, center.transform.position, step);
            leftfinger.transform.position = Vector3.MoveTowards(leftfinger.transform.position, center.transform.position, step);
        }
        if (Open)
        {
            rightfinger.transform.localPosition = new Vector3(3,1, 0);
            leftfinger.transform.localPosition  = new Vector3(-3,1, 0);
        }
        //rightbody.AddRelativeForce(Vector3.forward * speed, ForceMode.Force);
    }

    void OnDrawGizmosSelected()
    {
        if (SetEfectorOnTarget)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(LastPosition, transform.position);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(TargetPosition, transform.position);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(LastPosition, TargetPosition);
        }
    }

}
