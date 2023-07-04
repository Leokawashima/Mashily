using UnityEngine;

public class mash_anim : MonoBehaviour
{
    [SerializeField] Main_Process mainProcess;
    [SerializeField] float fadeTIme = 2f;

    private float needFadeTime;
    public float nowFade;

    [SerializeField]
    private float animBlendSpeed = 5f;
    private float animSpeed;

    private Animator anim;

    void Awake()
    {
        needFadeTime = 1f / fadeTIme / 60f;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float defSpeed = Mathf.Lerp(anim.GetFloat(Name.Anim.defalt), animSpeed, animBlendSpeed * Time.deltaTime) > 0.01 ?
            Mathf.Lerp(anim.GetFloat(Name.Anim.defalt), animSpeed, animBlendSpeed * Time.deltaTime) : 0f;
        anim.SetFloat(Name.Anim.defalt, defSpeed);
    }
    //---------------------------------------------------
    public void Move(float movespeed)
    {        animSpeed = movespeed;    }
    public void Jump()
    {        anim.SetTrigger(Name.Anim.isJump);    }
    public void StayGround(bool stay)
    {        anim.SetBool(Name.Anim.isGround, stay);    }
    public void WallRun(float movespeed)
    {        anim.SetFloat(Name.Anim.wallRun, movespeed);    }
    public void WallRot()
    {        anim.SetTrigger(Name.Anim.lookBack);    }
    public void WallRun_RorL(string RorL, bool onoff)
    {        anim.SetBool(RorL, onoff);    }
    public void JumpFadeStart()
    {
        nowFade += needFadeTime;
        anim.SetFloat(Name.Anim.jumpFade, nowFade); ;
        if (nowFade >= 1f)
            mainProcess.jumpFade = false;
    }
    public void FadeAnim_Reset()
    {
        nowFade = 0f;
        anim.SetFloat(Name.Anim.jumpFade, 0f);
    }
    public void DefAnim_Reset()
    {
        Move(0f);
        anim.SetFloat(Name.Anim.jumpFade, 0f);
    }
    public void WallAnim_Reset()
    {
        WallRun(0f);
        WallRun_RorL(Name.Anim.isRight, false);
        WallRun_RorL(Name.Anim.isLeft, false);
    }
}