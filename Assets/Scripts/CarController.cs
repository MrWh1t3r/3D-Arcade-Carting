using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public float acceleration;
    public float turnSpeed;

    public Transform carModel;
    private Vector3 _startModelOffset;

    public float groundCheckRate;
    private float _lastGroundCheckTime;

    private float _curYRot;

    public bool canControl;

    private bool _accelerateInput;
    private float _turnInput;

    public TrackZone curTrackZone;
    public int zonesPassed;
    public int racePosition;
    public int curLap;
    
    public Rigidbody rig;

    private void Start()
    {
        _startModelOffset = carModel.transform.localPosition;
        GameManager.instance.cars.Add(this);
        transform.position = GameManager.instance.spawnPoints[GameManager.instance.cars.Count - 1].position;
    }
    

    private void Update()
    {
        if (!canControl)
            _turnInput = 0.0f;
        
        float turnRate = Vector3.Dot(rig.velocity.normalized, carModel.forward);
        turnRate = Mathf.Abs(turnRate);
        _curYRot += turnRate * _turnInput * turnSpeed * Time.deltaTime;
        
        carModel.transform.position = transform.position + _startModelOffset;
        //carModel.transform.eulerAngles = new Vector3(0, _curYRot, 0);
        CheckGround();
    }

    private void FixedUpdate()
    {
        if(!canControl)
            return;
        
        if (_accelerateInput == true)
        {
            rig.AddForce(carModel.forward * acceleration, ForceMode.Acceleration);
        }
    }

    public void OnAccelerateInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            _accelerateInput = true;
        else
        {
            _accelerateInput = false;
        }
    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, -0.75f, 0), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f))
        {
            carModel.up = hit.normal;
        }
        else
        {
            carModel.up = Vector3.up;
        }
        
        carModel.Rotate(new Vector3(0,_curYRot,0), Space.Self);
    }

    public void OnTurnInput(InputAction.CallbackContext context)
    {
        _turnInput = context.ReadValue<float>();
    }
}
