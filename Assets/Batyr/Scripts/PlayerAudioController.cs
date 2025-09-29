using StarterAssets;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource movementAudioSource;
    public AudioSource jumpAudioSource;

    [Header("Audio Clips")]
    public AudioClip[] footstepClips;     // Массив звуков шагов (для разнообразия)
    public AudioClip jumpClip;

    [Header("Settings")]
    public float baseStepInterval = 0.5f; // Интервал между шагами при обычной ходьбе
    public float sprintStepMultiplier = 0.6f; // Ускорение шагов при беге (меньше = чаще шаги)
    public float minMoveThreshold = 0.1f; // Минимальная скорость для воспроизведения шагов

    private StarterAssetsInputs _input;
    private CharacterController _characterController;
    private float _nextStepTime;
    private bool _isGroundedLastFrame;

    private void Awake()
    {
        _input = GetComponent<StarterAssetsInputs>();
        // Или, если StarterAssetsInputs на другом объекте:
        // _input = FindObjectOfType<StarterAssetsInputs>();

        // Если CharacterController используется для определения grounded:
        _characterController = GetComponent<CharacterController>();
        // Или ищите его, если на другом объекте
    }

    private void Update()
    {
        var isMoving = _input.move.magnitude > minMoveThreshold;
        var isSprinting = _input.sprint;
        var isJumping = _input.jump;

        // Определяем, на земле ли игрок (если используете CharacterController)
        var isGrounded = !_characterController || _characterController.isGrounded;

        // Воспроизведение прыжка (только при начале прыжка)
        if (isJumping && _isGroundedLastFrame && !isGrounded)
        {
            PlayJumpSound();
        }

        // Воспроизведение шагов
        if (isMoving && isGrounded)
        {
            var stepInterval = baseStepInterval * (isSprinting ? sprintStepMultiplier : 1f);
            if (Time.time >= _nextStepTime)
            {
                PlayFootstep();
                _nextStepTime = Time.time + stepInterval;
            }
        }

        _isGroundedLastFrame = isGrounded;
    }

    private void PlayFootstep()
    {
        if (footstepClips.Length == 0 || movementAudioSource == null) return;

        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        movementAudioSource.PlayOneShot(clip);
    }

    private void PlayJumpSound()
    {
        if (jumpClip && jumpAudioSource)
        {
            jumpAudioSource.PlayOneShot(jumpClip);
        }
    }
}