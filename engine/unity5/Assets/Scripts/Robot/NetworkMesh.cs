using BulletUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// TODO: Constant lerp.

public class NetworkMesh : MonoBehaviour
{
    private const float CorrectionThreshold = 5f;

    private BRigidBody bRigidBody;

    private float positionCorrectionSpeed;
    private float rotationCorrectionSpeed;

    private Vector3 intPosition;
    private Quaternion intRotation;

    public void UpdateMeshTransform(Vector3 newPosition, Quaternion newRotation)
    {
        positionCorrectionSpeed = (newPosition - intPosition).magnitude * CorrectionThreshold;//transform.position;
        rotationCorrectionSpeed = Quaternion.Angle(newRotation, intRotation) * CorrectionThreshold;//transform.rotation;
    }

    private void Start()
    {
        intPosition = transform.position;
        intRotation = transform.rotation;

        bRigidBody = GetComponent<BRigidBody>();
    }
    
    private void Update()
    {
        
        //interpolationFactor = Math.Max(interpolationFactor - interpolationFactor * CorrectionThreshold * Time.deltaTime, 0);

        //intPosition = bRigidBody.GetCollisionObject().WorldTransform.Origin.ToUnity() - deltaPosition * interpolationFactor;

        //transform.position = intPosition;

        //Quaternion currentRotation = bRigidBody.GetCollisionObject().WorldTransform.Orientation.ToUnity();

        //if (interpolationFactor > 0)
        //    intRotation = Quaternion.Inverse(Quaternion.Lerp(currentRotation, currentRotation * Quaternion.Inverse(deltaRotation), interpolationFactor));
        //else
        //    intRotation = currentRotation;

        //transform.rotation = intRotation;

        Debug.DrawLine(transform.position, bRigidBody.GetCollisionObject().WorldTransform.Origin.ToUnity());
        Debug.DrawLine(transform.position, transform.position + Vector3.up);
    }
}
