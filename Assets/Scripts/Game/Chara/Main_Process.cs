using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Process  : MonoBehaviour
{
    #region Cache
    Rigidbody _rb;
    Transform _tf;
    CapsuleCollider _capCol;
    CinemaChanger _cChanger;
    mash_anim player_anim;
    #endregion Cache

    #region RayObj
    [Header("レイ割り当て")]
    [SerializeField] Ray_Ground _rayGround;
    [SerializeField] Ray_Wall _rayWall_Front;
    [SerializeField] Ray_Wall _rayWall_Left;
    [SerializeField] Ray_Wall _rayWall_Right;
    #endregion RayObj

    #region Cameras
    [Header("カメラ割り当て")]
    [SerializeField] PauseScript _pause;
    [SerializeField] MultifunctionFollowingCamera _mainCam;
    [SerializeField] MultifunctionFollowingCamera _rightCam;
    [SerializeField] MultifunctionFollowingCamera _leftCam;
    #endregion Cameras

    #region Chara
    [Header("速度、力の調整")]
    [SerializeField, InspectorName("通常速度")] float mainSpeed = 7f;
    [SerializeField, InspectorName("壁ジャンプ後の慣性毎率")] float mulWallJumpRb = 200f;
    [SerializeField, InspectorName("床ジャンプ力")] float floorJumpForce = 7f;
    [SerializeField, InspectorName("壁ジャンプ力")] float wallJumpForce = 7f;
    [SerializeField, InspectorName("特殊床の力倍率")] float mulHighJumpFloor = 2f;
    [SerializeField, InspectorName("特殊壁の力倍率")] float mulHighJumpWall = 2f;
    #endregion Chara

    //もらったレイの情報から　走る座標　壁の法線　法線から走る方向　加速壁なのか　その壁の名前　を格納する
    struct WallData
    {
        public Vector3 pos, normal, rotate;
        public bool isHigh;
        public string name;
    }
    WallData _wallData;

    //プレイヤーの状態を簡易識別するためのステータス　Update関数でswitch分岐させたりする
    enum IsState {
        Normal, Fly, WallJumpFly, WallRunOfRight, WallRotRight, WallRunOfLeft, WallRotLeft
    };
    IsState PlayerState = IsState.Normal;

    bool padOn;

    bool canJump = true;
    bool canWallDown = false;

    //毎回ifで判定するのでUniRxやコルーチンイベントによるコールバック化を目指したい
    public bool jumpFade = false;

    [SerializeField]float rotSpeed = 150f;
    bool endLookBack;

    public bool ivent = false;
    //------------------------------------------------------------------------
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _tf = GetComponent<Transform>();
        _capCol = GetComponent<CapsuleCollider>();
        _cChanger = GetComponent<CinemaChanger>();
        player_anim = GetComponentInChildren<mash_anim>();
    }

    void Update()
    {
        if(ivent) return;

        if(Mathf.Approximately(Time.timeScale, 0f)) return;

        if(_tf.transform.position.y < -40f)
        {
            _tf.transform.position = Vector3.up * 5f;
            _tf.transform.rotation = Quaternion.Euler(Vector3.zero);
            _rb.velocity = Vector3.zero;
            _mainCam.ResetCam();
        }

        padOn = _pause.padmode;

        //縦入力　横入力
        float _inputX = padOn ? Input.GetAxisRaw(Name.Input.InputPadX) : Input.GetAxisRaw(Name.Input.Horizontal);
        float _inputY = padOn ? Input.GetAxisRaw(Name.Input.InputPadY) : Input.GetAxisRaw(Name.Input.Vertical);
        //カメラのYを軸とした正面の向きを取得
        var _horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        //カメラの向きを基準に水平移動軸を作成
        var _velocity = _horizontalRotation * new Vector3(_inputX, 0, _inputY).normalized;
        //常にカメラの向きを正面に向く回転軸を作成
        var _lookfront = _horizontalRotation * Vector3.forward;

        bool _isGround = _rayGround.RayCastCheckGround();
        player_anim.StayGround(_isGround);

        float _addFloorJumpPower = _rayGround._floorIsHighJump ? mulHighJumpFloor : 1f;
        float _addWallJumpPower = _wallData.isHigh ? mulHighJumpWall : 1f;
        float _downForce = 0f;
        string jumpKindOfInput = padOn ? Name.Input.JumpPad : Name.Input.Jump;

        switch(PlayerState)
        {
            case IsState.Normal:
                Move(_velocity);
                LookFront(_velocity, _lookfront);
                Jump(jumpKindOfInput, floorJumpForce * _addFloorJumpPower);
                player_anim.Move(_velocity.magnitude);
                if(!_isGround)
                {
                    PlayerState = IsState.Fly;
                    _rb.useGravity = true;
                }
                break;

            case IsState.Fly:
                Move(_velocity);
                LookFront(_velocity, _lookfront);
                if (jumpFade)
                    player_anim.JumpFadeStart();
                WallRunStartCheck(_inputX);
                if(_isGround)
                    ResetByHitGround();
                IfStartWallRun();
                break;

            case IsState.WallJumpFly:
                _rb.AddForce(_velocity * mulWallJumpRb * Time.deltaTime);
                LookFront(_velocity, _lookfront);
                //慣性が壁側にあるかどうかを判定できる　normalizeしているので0.8f位の値で関数の中で評価してみたい
                //var normalveloX = new Vector3(_rb.velocity.x, 0, _rb.velocity.z).normalized;
                //WallRunStartCheck(normalveloX.x);
                WallRunStartCheck(_inputX);
                if(_isGround)
                    ResetByHitGround();
                IfStartWallRun();
                break;

            case IsState.WallRunOfRight:
                if (_inputX <= 0f)                    canWallDown = true;
                if(canWallDown && _inputX > 0f)       _downForce = -_inputX;
                IsWallRun(_downForce);

                if(_inputY > -0.7f)   Move(_wallData.rotate * Mathf.Max(_inputY, 0f));
                else
                {
                    PlayerState = IsState.WallRotRight;
                    StartCoroutine(RotateLookBack(PlayerState));
                    player_anim.WallRot();
                    player_anim.WallRun_RorL(Name.Anim.isRight, false);
                    player_anim.WallRun_RorL(Name.Anim.isLeft, true);
                    break;
                }
                WallJump(jumpKindOfInput, wallJumpForce * _addWallJumpPower);

                player_anim.WallRun(Mathf.Abs(Mathf.Max(_inputY, 0f)) + Mathf.Abs(_downForce / 2f));
                if(_isGround)
                {
                    ResetByHitGround();
                    _mainCam.ResetCam();
                }
                WallRunContinueCheck();
                break;

            case IsState.WallRotRight:
                _rb.velocity = Vector3.zero;
                if(endLookBack)
                {
                    _rightCam.ResetCam();
                    _cChanger.ChangeVCamera(_cChanger.VCam[1]);
                    EndLookBack(PlayerState);
                }
                break;

            case IsState.WallRunOfLeft:
                if (_inputX >= 0f)                    canWallDown = true;
                if (canWallDown && _inputX < 0f)      _downForce = _inputX;
                IsWallRun(_downForce);

                if(_inputY > -0.7f) Move(_wallData.rotate * Mathf.Max(_inputY, 0f));
                else
                {
                        PlayerState = IsState.WallRotLeft;
                        StartCoroutine(RotateLookBack(PlayerState));
                        player_anim.WallRot();
                        player_anim.WallRun_RorL(Name.Anim.isLeft, false);
                        player_anim.WallRun_RorL(Name.Anim.isRight, true);
                        break;
                }
                WallJump(jumpKindOfInput, wallJumpForce * _addWallJumpPower);

                player_anim.WallRun(Mathf.Abs(Mathf.Max(_inputY, 0f)) + Mathf.Abs(_downForce / 2f));
                if(_isGround)
                {
                    ResetByHitGround();
                    _mainCam.ResetCam();
                }
                WallRunContinueCheck();
                break;

            case IsState.WallRotLeft:
                _rb.velocity = Vector3.zero;
                if (endLookBack)
                {
                    _leftCam.ResetCam();
                    _cChanger.ChangeVCamera(_cChanger.VCam[2]);
                    EndLookBack(PlayerState);
                }
                break;
        }

        if(Input.GetAxisRaw(jumpKindOfInput) == 0f)
            canJump = true;
    }
    //------------------------------------------------------------------------
    private void Move(Vector3 MoveVelocity)
    {
        //移動軸に応じてスピードを使って実際に移動させる
        _rb.velocity = new Vector3(MoveVelocity.x * mainSpeed, _rb.velocity.y, MoveVelocity.z * mainSpeed);
    }
    private void LookFront(Vector3 MoveVelocity, Vector3 FrontVec)
    {
        //移動軸に力が加わっているなら　用は移動入力しているときだけカメラの正面を向く
        if(MoveVelocity.magnitude != 0)
        {
            transform.rotation = Quaternion.LookRotation(FrontVec, Vector3.up);
        }
    }
    private void Jump(string jumpkind, float jumpForce)
    {
        if(Input.GetAxisRaw(jumpkind) > 0f && canJump)
        {
            canJump = false;
            _rb.useGravity = true;
            player_anim.Jump();
            player_anim.Move(0f);
            player_anim.WallAnim_Reset();
            jumpFade = true;
            PlayerState = IsState.Fly;
            _rb.velocity = _rb.velocity + Vector3.up * jumpForce;
        }
    }
    private void WallJump(string jumpkind, float jumpForce)
    {
        if(Input.GetAxisRaw(jumpkind) > 0f && canJump)
        {
            _rb.useGravity = true;
            //キャラの正面　rotate　　ウォールランしている面の法線の正面　normal 二つを足して斜めに飛ばしている
            //_rb.velocity = _rb.velocity + Vector3.up * JumpForce + (wallRunNormal + wallRunRotate).normalized * JumpForce;
            // この処理は壁の法線方向へ飛ぶ、用は壁を蹴っ飛ばすような処理
            _rb.velocity = _rb.velocity / 2.0f + Vector3.up * jumpForce + _wallData.normal.normalized  * jumpForce * 0.8f;
            PlayerState = IsState.WallJumpFly;
            _mainCam.ResetCam();
            _cChanger.ChangeVCamera(_cChanger.VCam[0]);
            player_anim.WallAnim_Reset();
        }
    }

    void WallUpdate(ref Ray_Wall ray)
    {
        _wallData.name = ray._rayInfo.collider.name;
        _wallData.isHigh = ray._wallIsHighJump;
        _wallData.pos = ray._rayInfo.point;
        _wallData.normal = ray._rayInfo.normal.normalized;
    }
    void WallUpdateRight(Ray_Wall ray)
    {
        WallUpdate(ref ray);
        _wallData.rotate = RotateRight90(_wallData.normal);
    }
    void WallUpdateLeft(Ray_Wall ray)
    {
        WallUpdate(ref ray);
        _wallData.rotate = RotateLeft90(_wallData.normal);
    }
    void WallUpdateFront(Ray_Wall ray)
    {
        WallUpdate(ref ray);
    }

    private void WallRunStartCheck(float _vertical)
    {
        if(_vertical > 0f)
        {
            if(_rayWall_Right.RayCastCheck())
            {
                if(_rayWall_Right._rayInfo.collider.gameObject.name != _wallData.name)
                {
                    WallUpdateRight(_rayWall_Right);
                    canWallDown = false;
                    PlayerState = IsState.WallRunOfRight;
                }
            }
            else if(_rayWall_Front.RayCastCheck())
            {
                if(_rayWall_Front._rayInfo.collider.gameObject.name != _wallData.name)
                {
                    WallUpdateFront(_rayWall_Front);
                    _wallData.rotate = RotateRight90(_wallData.normal);
                    canWallDown = false;
                    PlayerState = IsState.WallRunOfRight;
                }
            }
        }

        if(_vertical < 0f)
        {
            if(_rayWall_Left.RayCastCheck())
            {
                if(_rayWall_Left._rayInfo.collider.gameObject.name != _wallData.name)
                {
                    WallUpdateLeft(_rayWall_Left);
                    canWallDown = false;
                    PlayerState = IsState.WallRunOfLeft;
                }
            }
            else if(_rayWall_Front.RayCastCheck())
            {
                if(_rayWall_Front._rayInfo.collider.gameObject.name != _wallData.name)
                {
                    WallUpdateFront(_rayWall_Front);
                    _wallData.rotate = RotateLeft90(_wallData.normal);
                    canWallDown = false;
                    PlayerState = IsState.WallRunOfLeft;
                }
            }
        }
    }
    private void IfStartWallRun()
    {
        if(PlayerState == IsState.WallRunOfRight)
        {
            _leftCam.ResetCam();
            _cChanger.ChangeVCamera(_cChanger.VCam[2]);
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.useGravity = false;
            _rb.transform.position = _wallData.pos + _wallData.normal * _capCol.radius;
            player_anim.Move(0f);
            player_anim.WallRun_RorL(Name.Anim.isRight, true);
        }
        else if(PlayerState == IsState.WallRunOfLeft)
        {
            _rightCam.ResetCam();
            _cChanger.ChangeVCamera(_cChanger.VCam[1]);
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.useGravity = false;
            _rb.transform.position = _wallData.pos + _wallData.normal * _capCol.radius;
            player_anim.Move(0f);
            player_anim.WallRun_RorL(Name.Anim.isLeft, true);
        }
    }

    private void EndLookBack(IsState fromState)
    {
        if (fromState == IsState.WallRotRight)
        {
            PlayerState = IsState.WallRunOfLeft;
            _wallData.rotate = RotateLeft90(_wallData.normal);
        }
        else
        {
            PlayerState = IsState.WallRunOfRight;
            _wallData.rotate = RotateRight90(_wallData.normal);
        }
        _tf.rotation = Quaternion.LookRotation(_wallData.rotate, Vector3.up);
        endLookBack = false;
    }
    private IEnumerator RotateLookBack(IsState fromState)
    {
        float i = 0;
        float rotate = fromState == IsState.WallRotRight ? rotSpeed : -rotSpeed;

        while(i < 180 / rotSpeed)
        {
            i += Time.deltaTime;
            _tf.Rotate(0f, rotate * Time.deltaTime, 0f);
            yield return null;
        }
        endLookBack = true;
    }

    private void WallRunContinueCheck()
    {
        if (PlayerState == IsState.WallRunOfRight)
        {
            bool continueWallRun = _rayWall_Right.RayCastCheck();
            
            if(continueWallRun)
            {
                if (_wallData.rotate != RotateRight90(_rayWall_Right._rayInfo.normal))
                {
                    _leftCam.FollowCam(_wallData.normal, _rayWall_Right._rayInfo.normal.normalized);

                    WallUpdateRight(_rayWall_Right);
                    _rb.transform.position = _wallData.pos + _wallData.normal * _capCol.radius;
                }
                if (_wallData.isHigh != _rayWall_Right._wallIsHighJump)
                    _wallData.isHigh = _rayWall_Right._wallIsHighJump;
            }
            else
            {
                _rb.useGravity = true;
                player_anim.WallRun_RorL(Name.Anim.isRight, false);
                _wallData.name = "";
                PlayerState = IsState.Fly;
            }
        }

        if(PlayerState == IsState.WallRunOfLeft)
        {
            bool continueWallRun = _rayWall_Left.RayCastCheck();

            if(continueWallRun)
            {
                if(_wallData.rotate != RotateLeft90(_rayWall_Left._rayInfo.normal))
                {
                    _rightCam.FollowCam(_wallData.normal, _rayWall_Left._rayInfo.normal.normalized);

                    WallUpdateLeft(_rayWall_Left);
                    _rb.transform.position = _wallData.pos + _wallData.normal * _capCol.radius;
                }
                if(_wallData.isHigh != _rayWall_Left._wallIsHighJump)
                    _wallData.isHigh = _rayWall_Left._wallIsHighJump;
            }
            else
            {
                _rb.useGravity = true;
                player_anim.WallRun_RorL(Name.Anim.isLeft, false);
                _wallData.name = "";
                PlayerState = IsState.Fly;
            }
        }
    }

    private void IsWallRun(float downForce)
    {
        _rb.velocity = new Vector3(_rb.velocity.x * mainSpeed, downForce, _rb.velocity.z * mainSpeed);
        _tf.rotation = Quaternion.LookRotation(_wallData.rotate, Vector3.up);
    }
    private void ResetByHitGround()
    {
        PlayerState = IsState.Normal;
        player_anim.WallAnim_Reset();
        player_anim.FadeAnim_Reset();
        _wallData.name = "";
        _cChanger.ChangeVCamera(_cChanger.VCam[0]);
    }
    private Vector3 RotateRight90(Vector3 RotateVector)
    {
        return new Vector3(RotateVector.z , 0, -RotateVector.x);
    }
    private Vector3 RotateLeft90(Vector3 RotateVector)
    {
        return new Vector3(-RotateVector.z, 0, RotateVector.x);
    }
}