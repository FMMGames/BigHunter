using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    public delegate void PlayerDeath();
    public event PlayerDeath OnDeath;
    PlayerCombatController _combatController;
    public Joystick _joystick;
    private CharacterController _controller;
    private Animator _anim;

    // Here we define the values for how the character moves;
    [Header("Character Attributes")]
    [SerializeField] private float _turnSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _baseSpeed;

    // Here we set variables needed for the controller;
    [Header("Controller Configuration")]
    [SerializeField] private float _groundLevel;
    [SerializeField] private LayerMask _groundLayer;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _runFX;

    [Header("Stats")]
    public bool isGrounded;
    public bool canMove;
    public bool isMoving;
    bool dead;

    private Vector3 _move;
    private float _runSpeed;
    private float _dir;
    private Vector3 _addedForce;
    private Vector3 _nextPos;
    private Transform _transform;
    private float _jumptTimer;
    private float _x, _z;

    private void Start()
    {
        _transform = transform;
        _controller = GetComponent<CharacterController>();
        _combatController = GetComponent<PlayerCombatController>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        InputManagement();

        _anim.SetBool("IsMoving", isMoving);
        _anim.SetBool("IsGrounded", isGrounded);

        MovementManager();
        GroundCheck();

        Movement();
        Orientation();
        ForceDamp();
    }

    void Death()
    {
        if (dead)
            return;

        OnDeath?.Invoke();

        _anim.SetTrigger("Death");
        Destroy(gameObject, 3f);

        dead = true;
    }


    // Here we check if the controller's bottom is touching valid Ground;
    private void GroundCheck()
    {
        //Left Ground
        if (!Physics.Raycast(_transform.position, -_transform.up, _groundLevel, _groundLayer) && isGrounded)
        {

        }        

        //Touched Ground
        if (Physics.Raycast(_transform.position, -_transform.up, _groundLevel, _groundLayer) && !isGrounded)
            _move.y = 0;

        RaycastHit hit;
        if (Physics.Raycast(_transform.position, -_transform.up, out hit, _groundLevel, _groundLayer))
            isGrounded = true;
        else
            isGrounded = false;

        //if (isGrounded)
        //{
        //    if (!_runFX.isPlaying)
        //        _runFX.Play();
        //}
        //else
        //    _runFX.Stop();
    }

    // Here we make the character always face the direction of its movement;
    private void Orientation()
    {
        if (!canMove) return;
        if (_x == 0 && _z == 0) return;

        if (_combatController.isAggro)
        {
            Vector3 groundedPos = new Vector3(_combatController._currentCombatTarget.transform.position.x, transform.position.y, _combatController._currentCombatTarget.transform.position.z);

            _transform.LookAt(groundedPos);

            if (Vector3.Angle(transform.forward, _move) <= 45)
            {
                _anim.SetFloat("RunSpeedVertical", 1);
            }
            else if (Vector3.Angle(transform.forward, _move) > 45)
            {
                _anim.SetFloat("RunSpeedVertical", -1);
            }

            if (Vector3.Angle(transform.right, _move) <= 45)
            {
                _anim.SetFloat("RunSpeedHorizontal", 1);
            }
            else if (Vector3.Angle(transform.right, _move) > 45)
            {
                _anim.SetFloat("RunSpeedHorizontal", -1);
            }
        }
        else
        {
            _nextPos = new Vector3(_x, 0, _z);

            if (_nextPos != Vector3.zero)
                _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(_nextPos), _turnSpeed * Time.deltaTime);
        }       
    }

    // Here we pass horizontal input and constant speed to the controller's movement, as well as adding fake gravity;
    private void Movement()
    {
        if (canMove)
            _move = new Vector3((_x * _runSpeed) + _addedForce.x, _move.y, (_z * _runSpeed) + _addedForce.z);
        else
            _move = Vector3.zero;       

        if (!isGrounded)
            _move.y = (Mathf.Lerp(_move.y, -30, 2f* Time.deltaTime));

        _controller.Move(_move * Time.deltaTime);

        if ((_x != 0 || _z != 0) && canMove)
            isMoving = true;
        else
            isMoving = false;

        if (_jumptTimer > 0)
            _jumptTimer -= Time.deltaTime;
        else if (_jumptTimer < 0)
            _jumptTimer = 0;
    }

    // Here we add the jump impulse upwards;
    private void Jump()
    {
        if (!isGrounded || _jumptTimer > 0)
            return;

        ApplyForce(_transform.up * _jumpForce);
        _anim.SetTrigger("Jump");

        _jumptTimer = 0.8f;
    }


    public void Dodge(float distance)
    {

    }

    private void InputManagement()
    {
        _x = _joystick.Horizontal;
        _z = _joystick.Vertical;
    }

    // Here we fake physics forces;
    public void ApplyForce(Vector3 p_dir)
    {
        _addedForce = p_dir;
        _move.y = _addedForce.y;
    }

    // Here we gradually damp the forces inside the fake force vector to 0;
    private void ForceDamp()
    {
        if (_addedForce.x != 0)
        {
            if (_addedForce.x < 0)
                _addedForce.x = Mathf.Lerp(_addedForce.x, 0.0001f, 10 * Time.deltaTime);
            else if (_addedForce.x > 0)
                _addedForce.x = Mathf.Lerp(_addedForce.x, -0.0001f, 10 * Time.deltaTime);
        }

        if (_addedForce.y != 0)
        {
            if (_addedForce.y < 0)
                _addedForce.y = Mathf.Lerp(_addedForce.y, 0.0001f, 10 * Time.deltaTime);
            else if (_addedForce.y > 0)
                _addedForce.y = Mathf.Lerp(_addedForce.y, -0.0001f, 10 * Time.deltaTime);
        }

        if (_addedForce.z != 0)
        {
            if (_addedForce.z < 0)
                _addedForce.z = Mathf.Lerp(_addedForce.z, 0.0001f, 10 * Time.deltaTime);
            else if (_addedForce.z > 0)
                _addedForce.z = Mathf.Lerp(_addedForce.z, -0.0001f, 10 * Time.deltaTime);
        }
    }

    // Here we define what stops movement;
    private void MovementManager()
    {
        if (dead)
            canMove = false;
        else
            canMove = true;

        if (canMove)
        {
            if (_runSpeed < _baseSpeed)
                _runSpeed = Mathf.Lerp(_runSpeed, _baseSpeed+1, 20 * Time.deltaTime);
            else if (_runSpeed > _baseSpeed)
                _runSpeed = _baseSpeed;
        }
        else
        {
            if (_runSpeed > 0)
                _runSpeed = Mathf.Lerp(_runSpeed, -1, 20 * Time.deltaTime);
            else if (_runSpeed < 0)
                _runSpeed = 0;
        }

      
    }

    public void FootR()
    {

    }

    public void FootL() { 
}
}