using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;

public class PlayerMotion : MonoBehaviour
{
    [Header("Camaras")]
    public Transform cam;
    public CinemachineFreeLook cinemachineFreeLook;
    public GameObject targetCam;
    public CinemachineVirtualCamera virtualCamera;
    public bool focus;

    [Header("zTarget")]
    public Transform targetPlayer;
    public Transform follow;

    [Header("Movimiento")]
    public float speed;
    public float speedRotation = 10;
    public float maxSlopeAngle = 40f;
    public float playerHeight = 0.2f;

    [Header("Roll")]
    [SerializeField] private float rollPower;
    [SerializeField] private float dodgePower;
    [SerializeField] private float rollMultiplayer;
    [SerializeField] private bool isRoll;


    [Header("Salto")]
    public float groundDistanceUp;
    public float groundDistance;
    public float gravity = 9.8f;
    public float gravityMultiplayer = 1;
    public float jumpPower = 35;
    public bool onGround, isJump;
    public bool stop;
    public LayerMask groundLayer;

    [Header("Interaction")]
    [SerializeField] public bool interacting;
    [SerializeField] public ItemCollision chest;

    [Header("Mov. Camara")]
    public float rotationSpeedCamX;
    public float rotationSpeedCamY;

    Rigidbody rb;
    Animator anim;
    Vector2 _move, m_look;  //Player Input
    Vector3 move;   //Move player
    float slopeAngle;
    RaycastHit slopeHit;
    zTarget zTarget;
    Sequence s;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        zTarget = GetComponent<zTarget>();
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
        bool onSlope = OnSlope();
        rb.useGravity = !onSlope;
        groundDistanceUp = (onSlope) ? -0.2f : 0.2f;
        onGround = Physics.CheckSphere(transform.position + (Vector3.up * groundDistanceUp), groundDistance, groundLayer);

        if (!onGround && !onSlope)
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

        if (focus)
        {
            UpdateFocus();
        }

        if (stop)
        {
            return;
        }
        if (!focus)
        {
            if (_move.x != 0 || _move.y != 0)
            {
                move = cam.forward * _move.y;
                move += cam.right * _move.x;
                move.Normalize();
                move.y = 0;
                rb.velocity = (onSlope) ? GetSlopeMoveDirection() * speed : move * speed;
                Vector3 dir = cam.forward * _move.y;
                dir += cam.right * _move.x;
                dir.Normalize();
                dir.y = 0;
                Quaternion targetR = Quaternion.LookRotation(dir);
                Quaternion playerR = Quaternion.Slerp(transform.rotation, targetR, speedRotation * Time.fixedDeltaTime);
                transform.rotation = playerR;
            }
        }
        else
        {
            move = cam.forward * _move.y;
            move += cam.right * _move.x;
            move.Normalize();
            move.y = 0;
            rb.velocity = (onSlope) ? GetSlopeMoveDirection() * speed : move * speed;
        }

    }

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();

        if (stop || interacting)
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

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight) && onGround)
        {
            slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return slopeAngle <= maxSlopeAngle && slopeAngle != 0;
        }
        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(move, slopeHit.normal).normalized;
    }

    public void OnJump()
    {
        Stopping();
        if (focus)
        {
            if (_move.x != 0 || _move.y != 0)
            {
                if (Mathf.Abs(_move.x) > Mathf.Abs(_move.y))
                {
                    _move.y = 0;
                }
                else if (Mathf.Abs(_move.y) > Mathf.Abs(_move.x))
                {
                    _move.x = 0;
                }
                else if (Mathf.Abs(_move.y) == Mathf.Abs(_move.x))
                {
                    _move.y = 0;
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(cam.forward * rollPower, ForceMode.Impulse);
            }
            if (_move.x != 0)
            {
                _move.x = (_move.x < 0) ? -1f : 1f;
            }
            if (_move.y != 0)
            {
                _move.y = (_move.y < 0) ? -1f : 1f;
            }
            anim.SetFloat("MoveX", _move.x);
            anim.SetFloat("MoveY", _move.y);
            Vector3 move = cam.forward * _move.y;
            move += cam.right * _move.x;
            move.Normalize();
            move.y = 0;
            if (_move.x != 0)
            {
                rb.AddForce(move * dodgePower * rollMultiplayer, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(move * rollPower * rollMultiplayer, ForceMode.Impulse);
            }
            anim.SetTrigger("Jumping");
            isRoll = true;
            s = DOTween.Sequence();
            s.AppendInterval(0.5f).OnComplete(() =>
            {
                if (isRoll)
                {
                    StopEnd();
                }
            });
        }
        else
        {
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

    }

    public void OnCam(InputValue value)
    {
        if (interacting)
        {
            return;
        }
        m_look = value.Get<Vector2>();
        cinemachineFreeLook.m_XAxis.Value += m_look.x * rotationSpeedCamX;
        cinemachineFreeLook.m_YAxis.Value += m_look.y * rotationSpeedCamY * Time.fixedDeltaTime;
    }

    public void OnUse(){
        if (chest)
        {
            chest.Open();
            return;
        }
    }

    public void OnChangeTargetL()
    {
        if (targetPlayer == null)
        {
            return;
        }
        TargetActive(false);
        targetPlayer = zTarget.NextToLeft();
        TargetActive(true);
        UpdateFocus();
    }

    public void OnChangeTargetR()
    {
        if (targetPlayer == null)
        {
            return;
        }
        TargetActive(false);
        targetPlayer = zTarget.NextToRight();
        TargetActive(true);
        UpdateFocus();
    }

    public void selectTarget(Transform objetive){
        virtualCamera.Priority = 10;
        cinemachineFreeLook.Priority = 8;
        targetCam.transform.LookAt(objetive);
        follow.position = targetCam.transform.position;
        follow.rotation = targetCam.transform.rotation;
        transform.localEulerAngles = new Vector3(0,follow.localEulerAngles.y,0);
    }

    public void noTarget(){
        targetPlayer = null;
        UpdateFocus();
        virtualCamera.Priority = 8;
        cinemachineFreeLook.Priority = 10;
        isFocus();
    }

    public void OnFocus(InputValue value)
    {
        focus = value.isPressed;
        if (stop || isJump)
        {
            return;
        }
        isFocus();
    }

    public void isFocus()
    {
        if (focus)
        {
            if (targetPlayer == null)
            {
                targetPlayer = zTarget.FirtsTarger();
            }

            if (targetPlayer == null)
            {
                focus = false;
                return;
            }
            TargetActive(true);
            virtualCamera.Priority = 10;
            cinemachineFreeLook.Priority = 8;
            anim.SetBool("IsFocus", true);
            anim.SetTrigger("Focus");
        }
        else
        {
            if (targetPlayer != null)
            {
                TargetActive(false);
            }
            zTarget.t = null;
            targetPlayer = null;
            virtualCamera.Priority = 8;
            cinemachineFreeLook.Priority = 10;
            anim.SetBool("IsFocus", false);
            anim.SetTrigger("SwitchWeapon");
        }
    }

    public void UpdateFocus()
    {
        targetCam.transform.LookAt(targetPlayer);
        follow.position = targetCam.transform.position;
        follow.rotation = targetCam.transform.rotation;
        transform.localEulerAngles = new Vector3(0, follow.localEulerAngles.y, 0);
    }

    void TargetActive(bool b)
    {
        if (targetPlayer.GetComponent<targetDamage>())
        {
            targetPlayer.GetComponent<targetDamage>().targetPoint.SetActive(b);
        }
    }
    public void FallEnd()
    {
        StopEnd();
    }

    public void Stopping()
    {
        isRoll = false;
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
        isRoll = false;
        rb.velocity = Vector3.zero;
        stop = false;
        isFocus();
    }

    public void RollStop()
    {
        rb.velocity = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3.up * groundDistanceUp), groundDistance);
    }
}
