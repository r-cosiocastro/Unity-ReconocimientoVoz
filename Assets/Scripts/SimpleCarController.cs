using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // está éste eje conectado al motor?
    public bool steering; // pueden estás ruedas girar con la dirección del vehículo?
}

public class SimpleCarController : MonoBehaviour
{
    [SerializeField]
    private List<AxleInfo> axleInfos; // la información acerca de cada eje individual
    [SerializeField]
    private float maxMotorTorque; // torque máximo que el motor puede aplicar a la rueda
    [SerializeField]
    private float maxBrakeTorque; // torque de freno máximo que se puede aplicar a la rueda
    [SerializeField]
    private float maxSteeringAngle; // ángulo de dirección máximo que puede tener la rueda
    [SerializeField]
    private bool useVoiceRecognition; // si se usa o no reconocimiento de voz

    private float posicionPedal = 0;
    private float posicionFreno = 0;
    private float posicionDireccion = 0;

    // encuentra el correspondiente modelo de la rueda
    // y le aplica correctamente el transform segun los datos del collider
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        float motor;
        float steering;
        float brake;
        if (useVoiceRecognition)
        {
            motor = maxMotorTorque * posicionPedal;
            steering = maxSteeringAngle * posicionDireccion;
            brake = maxBrakeTorque * posicionFreno;
        }
        else
        {
            motor = maxMotorTorque * Input.GetAxis("Vertical");
            steering = maxSteeringAngle * Input.GetAxis("Horizontal");
            if (Input.GetButton("Jump"))
            {
                brake = maxBrakeTorque;
            } else
            {
                brake = 0;
            }
        }

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
                axleInfo.leftWheel.brakeTorque = brake;
                axleInfo.rightWheel.brakeTorque = brake;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    //Acciones de Movimiento para reconocimiento de Voz
    public void MoveForward()
    {
        posicionFreno = 0;
        posicionPedal += 0.2f;
        posicionPedal = Mathf.Clamp(posicionPedal, -1, 1);
    }

    public void MoveBackward()
    {
        posicionFreno = 0;
        posicionPedal -= 0.2f;
        posicionPedal = Mathf.Clamp(posicionPedal, -1, 1);
    }

    public void SteerLeft()
    {
        posicionDireccion -= 0.2f;
        posicionDireccion = Mathf.Clamp(posicionDireccion, -1, 1);
    }

    public void SteerRight()
    {
        posicionDireccion += 0.2f;
        posicionDireccion = Mathf.Clamp(posicionDireccion, -1, 1);
    }

    public void SteerAllRight()
    {
        posicionDireccion = 1f;
    }

    public void SteerAllLeft()
    {
        posicionDireccion = -1f;
    }

    public void StopCar()
    {
        posicionFreno = 1f;
        posicionPedal = 0f;
    }

    public void SteerStraightAhead()
    {
        posicionDireccion = 0;

    }

    public void MaxSpeed()
    {
        posicionPedal *= 100f;
        posicionPedal = Mathf.Clamp(posicionPedal, -1, 1);
    }

    public void ResetCar()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        SteerStraightAhead();
        posicionFreno = 100f;
        posicionPedal = 0f;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.up;
    }
}
