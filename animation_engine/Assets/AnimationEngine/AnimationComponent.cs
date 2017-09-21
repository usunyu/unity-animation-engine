using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Audio;

namespace Animations
{
    /// <summary>
    /// Animation Component.
    /// Example usage:
    /// /* Initial */
    /// AnimationComponent component = 
    /// new AnimationComponent(mixer, animation, audio)
    ///      .SetStartBlendingTime()
    ///      .Start();
    ///
    /// /* Update */
    /// component.SetEndBlendingTime().Tick();
    /// </summary>
    public class AnimationComponent
    {
        public enum Status
        {
            Pending,
            Processing,
            Done
        }

        private AnimationMixerPlayable animationMixer;
        private AudioMixerPlayable audioMixer;
        private int animationIndex;
        private int audioIndex;

        private float speed = 1.0f;

        //                  totalTime
        // +----------+--------------------+----------+
        //   startBlendingTime      endBlendingTime
        private float startBlendingTime = -1.0f;
        private float endBlendingTime = 99999.0f;
        private float startBlendingStep = -1.0f;
        private float endBlendingStep = -1.0f;

        private float processingTime = 0.0f;
        private float totalTime = 0.0f;
        private float weight = 0.0f;

        private bool canStartNextClip = false;

        private Status status = Status.Pending;

        private const float DEFAULT_START_BLENDING_PERCENTAGE = 0.25f;
        private const float DEFAULT_END_BLENDING_PERCENTAGE = 0.75f;

        public AnimationComponent(AnimationMixerPlayable animationMixer,
                                  AudioMixerPlayable audioMixer,
                                  int animation,
                                  int audio = -1)
        {
            this.animationMixer = animationMixer;
            this.audioMixer = audioMixer;
            this.animationIndex = animation;
            this.audioIndex = audio;

            var clip = (AnimationClipPlayable)animationMixer.GetInput(animationIndex);
            // Calculate time for processing current clip.
            totalTime = clip.GetAnimationClip().length;
            // Reset the time so that the clip starts at the correct position.
            clip.SetTime(0);
            // Reverse the animation if necessary.
            clip.SetSpeed(speed);
        }

        public void Reset()
        {
            processingTime = 0.0f;
            status = Status.Pending;
        }

        public void Start()
        {
            if (status != Status.Pending)
            {
                return;
            }

            if (startBlendingTime <= 0.0f)
            {
                // Move weight to 1 directly.
                weight = 1.0f;
            }
            else
            {
                // Calculate blending step.
                startBlendingStep = 1.0f / startBlendingTime;
            }

            status = Status.Processing;
        }

        public void Tick()
        {
            if (status != Status.Processing)
            {
                return;
            }

            processingTime += Time.deltaTime;

            if (processingTime > endBlendingTime)
            {
                if (endBlendingStep <= 0.0f)
                {
                    endBlendingStep = 1.0f / (totalTime - endBlendingTime);
                }
                if (!canStartNextClip)
                {
                    canStartNextClip = true;
                }
                weight -= endBlendingStep;
                if (processingTime >= totalTime)
                {
                    // Make sure we clean current animation clip.
                    weight = 0.0f;
                }
            }
            else if (processingTime < startBlendingTime)
            {
                weight += startBlendingStep;
            }

            animationMixer.SetInputWeight(animationIndex, weight);

            if (processingTime >= totalTime)
            {
                status = Status.Done;
            }
        }

        public AnimationComponent SetAnimationSpeed(float speed = 1.0f)
        {
            this.speed = speed;
            return this;
        }

        public AnimationComponent SetStartBlendingTime()
        {
            this.startBlendingTime = totalTime * DEFAULT_START_BLENDING_PERCENTAGE;
            return this;
        }

        public AnimationComponent SetStartBlendingTime(float blendingTime)
        {
            this.startBlendingTime = blendingTime;
            return this;
        }

        public AnimationComponent SetEndBlendingTime()
        {
            this.endBlendingTime = totalTime * DEFAULT_END_BLENDING_PERCENTAGE;
            return this;
        }

        public AnimationComponent SetEndBlendingTime(float blendingTime)
        {
            this.endBlendingTime = blendingTime;
            return this;
        }

        public int Animation { get { return animationIndex; } }

        public int Audio { get { return audioIndex; } }

        public float Speed { get { return speed; } }

        public Status CurrentStatus { get { return status; } }

        public bool CanStartNextClip { get { return canStartNextClip; } }

        public override string ToString()
        {
            return string.Format(
                "[AnimationComponent: Animation={0}, Audio={1}, Speed={2}, Status={3}]",
                animationIndex,
                audioIndex,
                speed,
                status);
        }
    }
}