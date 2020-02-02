using UnityEngine;
using System;
using RepairGame;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine.Events;

namespace RepairGame
{
    [Serializable]
    public enum DriveType
    {
        RearWheelDrive,
        FrontWheelDrive,
        AllWheelDrive
    }
}

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(AudioSource))]
public class WheelDriveCustom : MonoBehaviour
{
    [Tooltip("Car will be controlled by Ai")]
    public bool enemyAi = false;

    [Header("Ai settings")] 
    public bool initialisedWhenEnabled = false; 
    public float inputReadDelay = 1.6f;
    public float maxForwardAngle = 20f;
    public float maxPlayerDistance = 45f;
    public float maxCatchUpDistance = 10f;
    public float minCatchUpDistance = 1f;
    public float catchUpAdditionalMaxSpeed = 15f;
    public bool debugAi = false;
    
    [Space]
    public bool onAtStart = true;
    [Tooltip("Is car constantly accelerating without player input?")]
    public bool constantAcceleration = true;
    [Tooltip("Simulated constant vertical input value (constant acceleration)")]
    public float simulatedConstantInput = 1f;
    [Tooltip("Maximum steering angle of the wheels")]
	public float maxAngle = 30f;
	[Tooltip("Maximum torque applied to the driving wheels")]
	public float maxTorque = 300f;
	[Tooltip("Maximum brake torque applied to the driving wheels")]
	public float brakeTorque = 30000f;
	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelShape;

    public float maxSpeed = 30f;

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed = 5f;
	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5;
	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1;

	[Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
	public DriveType driveType;

    private WheelCollider[] m_Wheels;
    public bool useWheelColliderChildAsWheelShape;

    public UnityEvent turnOnEvent;

    Rigidbody rb;

    private List<float> capturedPlayerInput = new List<float>();
    private float captureStartTime = 0f;

    public static GameObject s_player;

    private bool vehicleOn = true;

    private float catchUpAdditionalSpeed = 0f;

    private bool aiOn = false;

    private AudioSource audioSource;

    // Find all the WheelColliders down in the hierarchy.
    void Start()
	{
        audioSource = GetComponent<AudioSource>();

        if (!onAtStart)
            vehicleOn = false;

        if (enemyAi)
        {
            if (initialisedWhenEnabled)
                aiOn = true;
            
            captureStartTime = Time.time;
            StartCoroutine(CapturePlayerInput());
        }else if (!s_player)
                s_player = gameObject;
        else Debug.LogError("More than one player car detected");

        rb = GetComponent<Rigidbody>();

		m_Wheels = GetComponentsInChildren<WheelCollider>();

		for (int i = 0; i < m_Wheels.Length; ++i) 
		{
			var wheel = m_Wheels [i];

			// Create wheel shapes only when needed.
			if (wheelShape != null)
			{
				var ws = Instantiate (wheelShape);
				ws.transform.parent = wheel.transform;
			}
		}
    }

    private IEnumerator PlayerDistanceCheck(float checkRate = 0.1f)
    {
        while(true)
        {
            yield return new WaitForSeconds(checkRate);
            if (vehicleOn)
            {
                float distFromPlayer = Vector3.Distance(s_player.transform.position, transform.position);
                if (distFromPlayer > minCatchUpDistance)
                    catchUpAdditionalSpeed = catchUpAdditionalMaxSpeed / (maxCatchUpDistance / distFromPlayer);
                else catchUpAdditionalSpeed = 0f;

                if (distFromPlayer > maxPlayerDistance)
                    TurnOffVehicle(true, "Too far from player");
                else if (catchUpAdditionalSpeed > catchUpAdditionalMaxSpeed)
                    catchUpAdditionalSpeed = catchUpAdditionalMaxSpeed;
            }
        }
    }

    public void InitialiseAi()
    {
        aiOn = true;
    }

    public void DeactivateAi()
    {
        aiOn = false;
    }

    public void ToggleAi(bool toggle)
    {
        aiOn = toggle;
    }

    private IEnumerator CapturePlayerInput()
    {
        while (enemyAi)
        {
            if (Time.time - captureStartTime > inputReadDelay)
                capturedPlayerInput.RemoveAt(0);

            capturedPlayerInput.Add(Input.GetAxis("Horizontal"));

            yield return null;
        }
    }

    // This is a really simple approach to updating wheels.
    // We simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero.
    // This helps us to figure our which wheels are front ones and which are rear.
    void Update()
    {
        if (debugAi)
            DrawAiDebug();

        m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

        float angle = maxAngle * (enemyAi ? GetDelayedInput() : Input.GetAxis("Horizontal"));
        float appliedTorque = maxTorque;

        if (vehicleOn)
        {
            if (rb.velocity.magnitude < (maxSpeed + catchUpAdditionalSpeed))
            {
                appliedTorque *= constantAcceleration ? simulatedConstantInput : Input.GetAxis("Vertical");
            }
            else appliedTorque = 0f;
        }
        else appliedTorque = 0f;

        float torque = appliedTorque;

		float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;

		foreach (WheelCollider wheel in m_Wheels)
		{
			// A simple car where front wheels steer while rear ones drive.
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;

			if (wheel.transform.localPosition.z < 0)
			{
				wheel.brakeTorque = handBrake;
			}

			if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive)
			{
				wheel.motorTorque = torque;
			}

			if (wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive)
			{
				wheel.motorTorque = torque;
			}

			// Update visual wheels if any.
			if (wheelShape || useWheelColliderChildAsWheelShape) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// Assume that the only child of the wheelcollider is the wheel shape.
				Transform shapeTransform = wheel.transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}
		}

        float loweredRpm = m_Wheels[0].rpm / 200f;

        float pitch = loweredRpm > 3 ? 3 : loweredRpm;
        audioSource.pitch = pitch;
	}



    private float GetDelayedInput(bool maxForwardAngleCheck = true)
    {
        if (!aiOn)
            return 0f;
        
        if(maxForwardAngleCheck)
            if(s_player)
            {
                Vector3 toPlayerDir = s_player.transform.position - transform.position;

                if(Vector3.Angle(toPlayerDir, transform.forward) > maxForwardAngle)
                {
                    return Vector3.SignedAngle(transform.forward, toPlayerDir, Vector3.up) >= 0 ? 1f : -1f;
                }
            }

        if(Time.time - captureStartTime > inputReadDelay)
        {
            if (capturedPlayerInput.Count > 0)
                return capturedPlayerInput[0];
            else return 0f;
        }
        else return 0f;
        //Time.time - captureStartTime > inputReadDelay ? capturedPlayerInput[0] : 
    }

    private void DrawAiDebug()
    {
        Vector3 rayOrigin = transform.position + (transform.up * 1);
        Vector3 toPlayerDir = s_player.transform.position - transform.position;
        Debug.DrawRay(rayOrigin, toPlayerDir, Color.blue);

        Vector3 leftMaxForwardAngle = transform.forward * 3f;

        leftMaxForwardAngle = Quaternion.AngleAxis(-maxForwardAngle, transform.up) * leftMaxForwardAngle;

        Debug.DrawRay(transform.position + (transform.up * 1), leftMaxForwardAngle, Color.green);

        Vector3 rightCollisionDirection = transform.forward * 3f;

        rightCollisionDirection = Quaternion.AngleAxis(maxForwardAngle, transform.up) * rightCollisionDirection;

        Debug.DrawRay(transform.position + (transform.up * 1), rightCollisionDirection, Color.green);

        if (s_player)
        {
            //Vector3 toPlayerDir = s_player.transform.position - transform.position;

            if (Vector3.Angle(toPlayerDir, transform.forward) > maxForwardAngle)
            {
                if (Vector3.SignedAngle(transform.forward, toPlayerDir, Vector3.up) >= 0)
                    Debug.DrawRay(transform.position + (transform.up * 1), rightCollisionDirection, Color.red);
                else Debug.DrawRay(transform.position + (transform.up * 1), leftMaxForwardAngle, Color.red);
            }
        }
    }

    public void TurnOnVehicle(float aiInitialiseDelay)
    {
        vehicleOn = true;
        turnOnEvent?.Invoke();
        Invoke(nameof(InitialiseAi), aiInitialiseDelay);
    }

    public void TurnOffVehicle(bool disableObject = false, string reason = "Reason Unknown")
    {
        Debug.Log("Turning vehicle off: " + reason, gameObject);
        vehicleOn = false;
        if(enemyAi)
            GameController.s_Instance.EnemyKilled(this);

        if(disableObject)
        {
            Debug.Log("Despawning enemy", gameObject);
            rb.velocity = Vector3.zero;
            LeanPool.Despawn(this, 5f);
        }
    }

    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxSpeed = newMaxSpeed;
    }

    void OnEnable()
    {
        //TurnOffVehicle();
        if (enemyAi)
        {
            StartCoroutine(PlayerDistanceCheck());
        }
    }

    void OnDisable()
    {
        //if(enemyAi)
            //GameController.s_Instance.EnemyKilled(this);
    }
}
