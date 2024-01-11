using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject ImpactPrefab;

    // Start is called before the first frame update
    private bool _isAttacking;
    private bool _isBlocking;

    private UpDown UpOrDown;

    private bool CanAttack => !_isAttacking && !_isBlocking;
    private bool CanBlock => !_isAttacking && !_isBlocking;

    public bool Dead => _dead;




    #region AnimationParamNames
    const string SPEED = "Speed";
    const string ATTACK_HIGH_QUICK = "AttackHighQuick";
    const string ATTACK_HIGH_SLOW = "AttackHighSlow";
    const string ATTACK_LOW_QUICK = "AttackLowQuick";
    const string ATTACK_LOW_SLOW = "AttackLowSlow";

    const string BLOCK_HIGH = "BlockHigh";
    const string BLOCK_LOW = "BlockLow";


    const string DIE = "Die";
    const string WIN = "Win";



    #endregion

    public Animator hydraAnimator;
    private Animator _animator;
    private AudioSource _audio;
    private Transform _otherPlayer;
    public bool hydraHit = false;//check if hydra is hitting

    //Ataques magellan
    public float startVolume;
    public AudioClip attackLowSlow;
    public AudioClip movementClip;
    public AudioClip attackQuickLow;
    public AudioClip attackHighSlow;
    public AudioClip win;
    public AudioClip die;
    public AudioClip attackQuick;
    public AudioClip hitClip;

    static int _playercount;
    int _id;
    private bool _dead;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        startVolume = _audio.volume;
        _id = _playercount++;
    }

    private void Update()
    {
        if ((_isAttacking || _isBlocking) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            Restart();
        }
    }

    public void SetOtherPlayer(Transform other)
    {
        _otherPlayer = other;
    }

    internal void SetAttacking(bool value, UpDown upDown)
    {
        _isAttacking = value;
        UpOrDown = upDown;
    }



    internal void SetBlocking(bool value, UpDown upDown)
    {
        _isBlocking = value;
        UpOrDown = upDown;
    }

    public void TryHighQuickAttack()
    {
        if (CanAttack)
        {
            _animator.SetTrigger(ATTACK_HIGH_QUICK);
            //play audio
            _audio.clip = attackQuick;
            _audio.PlayDelayed(0.0f);
            SetAttacking(true, UpDown.Up);
            Debug.Log(_isAttacking);
        }
    }
    public void TryHighSlowAttack()
    {
        if (CanAttack)
        {
            _animator.SetTrigger(ATTACK_HIGH_SLOW);
            //play audio
            _audio.clip = attackHighSlow;

            if (hydraAnimator)
            {
                hydraAnimator.SetTrigger(ATTACK_HIGH_SLOW);
                _audio.PlayDelayed(0.0f);
                StartCoroutine(FadeOut(_audio, 1.5f, startVolume));
            }
            else
            {
                _audio.PlayDelayed(0.0f);//pon delay si quieres
            }
             
            SetAttacking(true, UpDown.Up);
            Debug.Log(_isAttacking);
        }
    }
    public void TryLowQuickAttack()
    {
        if (CanAttack)
        {
            _animator.SetTrigger(ATTACK_LOW_QUICK);
            SetAttacking(true, UpDown.Down);
            if (hydraAnimator)
            {
                hydraAnimator.SetTrigger(ATTACK_LOW_QUICK);
                SetAttacking(false, UpDown.Down);
            }
            //play audio
            _audio.clip = attackQuickLow;
            _audio.PlayDelayed(0.0f);
            StartCoroutine(FadeOut(_audio, 1.3f, startVolume));
            Debug.Log(_isAttacking);
        }
    }
    public void TryLowSlowAttack()
    {
        if (CanAttack)
        {
            _animator.SetTrigger(ATTACK_LOW_SLOW);
            //play audio
            _audio.clip = attackLowSlow;

            if (hydraAnimator)
            {
                hydraAnimator.SetTrigger(ATTACK_LOW_SLOW);
                _audio.PlayDelayed(0.2f);
                StartCoroutine(FadeOut(_audio, 1.7f, startVolume));
            }
            else
            {
                _audio.PlayDelayed(0.0f);
            }
            SetAttacking(true, UpDown.Down);
            Debug.Log(_isAttacking);
        }
    }

    internal void TryHighBlock()
    {
        if (CanBlock)
        {
            _animator.SetTrigger(BLOCK_HIGH);
            SetBlocking(true, UpDown.Up);
            Debug.Log(_isBlocking);
        }
    }

    internal void TryLowBlock()
    {
        if (CanBlock)
        {
            _animator.SetTrigger(BLOCK_LOW);
            SetBlocking(true, UpDown.Down);
            Debug.Log(_isBlocking);

        }
    }


    public void OnHit(Transform hit)
    {
        var hitBy = hit.root.GetComponent<PlayerController>();
        if (hitBy.transform == _otherPlayer && (hitBy._isAttacking || hydraHit) && !_dead && !hitBy._dead)
        {
            //play audio
            _audio.clip = hitClip;
            _audio.PlayDelayed(0.0f);
            if (!_isBlocking || hitBy.UpOrDown != this.UpOrDown || hitBy.Dead)
            {
                Die();
                hitBy.Win();
                Instantiate(ImpactPrefab, hit.position, Quaternion.identity);
            }
            hydraHit = false;
        }
    }

    private void Die()
    {
        _animator.SetTrigger(DIE);
        //play audio
        _audio.clip = die;
        _audio.PlayDelayed(0.5f);
        StartCoroutine(DieLater());
    }

    IEnumerator DieLater()
    {
        yield return new WaitForSeconds(0.1f);
        _dead = true;
        yield return new WaitForSeconds(5);
        PlayerStart.nPLayers = 0;
        _playercount = 0;
        MovementController._playercount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Win()
    {
        _animator.SetTrigger(WIN);
        //play audio
        StopAllCoroutines();
        StopAudio();//arreglito feo
        _audio.volume = startVolume;
        _audio.clip = win;
        _audio.PlayDelayed(0.2f);
        if (hydraAnimator)
        {
            hydraAnimator.SetTrigger(WIN);
        }
    }

    void Restart()
    {
        if (_isAttacking)
        {
            _isAttacking = false;
            Debug.Log(_isAttacking);
        }
        else
        {
            _isBlocking = false;
            Debug.Log(_isBlocking);
        }
    }
    public void PlayForTime(float time)
    {
        _audio.Play();
        Invoke("StopAudio", time);
    }

    private void StopAudio()
    {
        _audio.Stop();
        _audio.volume = startVolume;
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime, float startVol)
    {

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVol * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVol;
    }
}

public enum UpDown
{
    Up,
    Down
}

