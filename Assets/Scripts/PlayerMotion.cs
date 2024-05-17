using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMotion : MonoBehaviour
{
    [Header("Camaras")]
    public Transform cam;
    public CinemachineFreeLook cinemachineFreeLook;
    public GameObject targetCam;

    [Header("Movimiento")]
    public float speed;
    public float speedRotation = 10;

    [Header("Salto")]
    public float groundDistanceUp;
    public float groundDistance;
    public float gravity = 9.8f;
    public float gravityMultiplayer = 1;
    public float jumpPower = 35;
    public bool onGround, isJump;
    public bool stop;
    public LayerMask groundLayer;

    [Header("Mov. Camara")]
    public float rotationSpeedCamX;
    public float rotationSpeedCamY;

    Rigidbody rb;
    Animator anim;
    Vector2 _move, m_look;  //Player Input
    Vector3 move;   //Move player
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() 
    {
        onGround = Physics.CheckSphere(transform.position + (Vector3.up * groundDistanceUp), groundDistance, groundLayer);

        if (!onGround)
        {
            rb.AddForce(-gravity * gravityMultiplayer * Vector3.up, ForceMode.Acceleration);
        }

        if (isJump && onGround)
        {
            isJump = false;
            anim.SetBool("OnAir", false);
            rb.velocity = Vector3.zero;
        }
        else if (!isJump && !onGround)
        {
            anim.SetBool("OnAir", true);
            isJump = true;
            Stopping();
            anim.SetTrigger("Fall");
        }

        if (stop)
        {
            return;
        }

        if (_move.x != 0 || _move.y != 0)
        {
            move = cam.forward * _move.y;
            move += cam.right * _move.x;
            move.Normalize();
            move.y = 0;
            rb.velocity = move * speed;
            Vector3 dir = cam.forward * _move.y;
            dir += cam.right * _move.x;
            dir.Normalize();
            dir.y = 0;
            Quaternion targetR = Quaternion.LookRotation(dir);
            Quaternion playerR = Quaternion.Slerp(transform.rotation, targetR, speedRotation * Time.fixedDeltaTime);
            transform.rotation = playerR;
        }
    }

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();

        if (stop)
        {
            return;
        }

        anim.SetBool("Move", (_move.x == 0 && _move.y == 0) ? false : true);
        anim.SetFloat("Moving", (_move.x == 0 && _move.y == 0) ? 0 : 1);
        if (_move.x == 0 && _move.y == 0)
        {
            rb.velocity = Vector3.zero;
        }
        anim.SetFloat("MoveX", _move.x);
        anim.SetFloat("MoveY", _move.y);
    }

    public void OnJump()
    {
        Stopping();
        isJump = true;
        Vector2 movDir = _move;
        anim.SetTrigger("Jumping");
        if (movDir != Vector2.zero)
        {
            Vector3 dir = cam.forward * movDir.y;
            dir += cam.right * movDir.x;
            dir.Normalize();
            dir.y = 0;
            Quaternion targetR = Quaternion.LookRotation(dir);
            transform.rotation = targetR;
            rb.AddForce((transform.forward + Vector3.up) * jumpPower, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        anim.SetBool("OnAir", true);
    }

    public void OnCam(InputValue value)
    {
        m_look = value.Get<Vector2>();
        cinemachineFreeLook.m_XAxis.Value += m_look.x * rotationSpeedCamX;
        cinemachineFreeLook.m_YAxis.Value += m_look.y * rotationSpeedCamY * Time.fixedDeltaTime;
    }

    public void FallEnd()
    {
        StopEnd();
    }

    void Stopping()
    {
        if (onGround)
        {
            rb.velocity = Vector3.zero;
        }
        stop = true;
        anim.SetFloat("MoveX", 0);
        anim.SetFloat("MoveY", 0);
        anim.SetFloat("Moving", 0);
        anim.SetBool("Move", false);
    }

    public void StopEnd()
    {
        anim.SetBool("Move", (_move.x == 0 && _move.y == 0) ? false : true);
        anim.SetFloat("Moving", (_move.x == 0 && _move.y == 0) ? 0 : 1);
        anim.SetFloat("MoveX", _move.x);
        anim.SetFloat("MoveY", _move.y);
        rb.velocity = Vector3.zero;
        stop = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3.up * groundDistanceUp), groundDistance);
    }
}
