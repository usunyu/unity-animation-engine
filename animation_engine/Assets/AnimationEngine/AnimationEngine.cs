using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Audio;

namespace AnimationEngine
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class AnimationEngine : MonoBehaviour
    {
        // Playable graph which controlling animations & audios.
        PlayableGraph playableGraph;

        // All animation clip for this engine.
        public AnimationClip[] animationClips;

        // All audio clip for this engine.
        public AudioClip[] audioClips;

        // Animation mixer for switching animation.
        private AnimationMixerPlayable animationMixer;

        // Audio mixer for switching audio.
        private AudioMixerPlayable audioMixer;

        // Animation component list will be play.
        private List<AnimationComponent> currentAnimationList;

        // Current playing animation component.
        private AnimationComponent firstAnimation = null;

        // Next playing animation component.
        private AnimationComponent secondAnimation = null;

        // Tag for debugging.
        private const string TAG = "[AnimationEngine]";

        // Use this for initialization
        private void Start()
        {
            // Create playable graph.
            playableGraph = PlayableGraph.Create();

            // Create playable output node.
            var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());

            // Create animation mixer playerable node.
            int animationClipsLength = animationClips == null ? 0 : animationClips.Length;
            animationMixer = AnimationMixerPlayable.Create(playableGraph, animationClipsLength);

            // Connect the mixer to an output.
            playableOutput.SetSourcePlayable(animationMixer);

            // Create clip playable node for all animations.
            for (int i = 0; i < animationClipsLength; i++)
            {
                // Wrap the clip in a playable.
                var clipPlayable = AnimationClipPlayable.Create(playableGraph, animationClips[i]);

                playableGraph.Connect(clipPlayable, 0, animationMixer, i);
                animationMixer.SetInputWeight(i, 0.0f);
            }

            // Create audio output node.
            var audioOutput = AudioPlayableOutput.Create(playableGraph, "Audio", GetComponent<AudioSource>());

            // Create audio mixer playerable node.
            int audioClipsLength = audioClips == null ? 0 : audioClips.Length;
            audioMixer = AudioMixerPlayable.Create(playableGraph, audioClipsLength);

            // Connect the mixer to an output.
            audioOutput.SetSourcePlayable(audioMixer);

            // Create clip playable node for all audios.
            for (int i = 0; i < audioClipsLength; i++)
            {
                // Wrap the clip in a playable.
                var clipPlayable = AudioClipPlayable.Create(playableGraph, audioClips[i], false);

                playableGraph.Connect(clipPlayable, 0, audioMixer, i);
                audioMixer.SetInputWeight(i, 0.0f);
            }

            // Init the animation component list.
            currentAnimationList = new List<AnimationComponent>();

            // Plays the playable graph.
            playableGraph.Play();

            // Register graph visual client to show debug messages.
            // GraphVisualizerClient.Show(playableGraph, "Animation Engine Graph");
        }

        // Update is called once per frame
        private void Update()
        {
            if (animationMixer.GetInputCount() == 0 || currentAnimationList.Count == 0)
            {
                // No valid clip input.
                return;
            }

            if (firstAnimation != null)
            {
                switch (firstAnimation.CurrentStatus)
                {
                    case AnimationComponent.Status.Pending:
                        firstAnimation.Start();
                        break;
                    case AnimationComponent.Status.Processing:
                        firstAnimation.Tick();
                        break;
                    case AnimationComponent.Status.Done:
                        if (currentAnimationList.Count == 1)
                        {
                            firstAnimation.Reset();
                        }
                        else if (currentAnimationList.Count >= 2)
                        {
                            currentAnimationList.Remove(firstAnimation);
                            firstAnimation = currentAnimationList[0];
                            if (currentAnimationList.Count >= 2)
                            {
                                secondAnimation = currentAnimationList[1];
                            }
                            else
                            {
                                secondAnimation = null;
                            }
                        }
                        break;
                }
                // We can start blend next clip.
                if (firstAnimation.CanStartNextClip)
                {
                    switch (secondAnimation.CurrentStatus)
                    {
                        case AnimationComponent.Status.Pending:
                            secondAnimation.Start();
                            break;
                        case AnimationComponent.Status.Processing:
                            secondAnimation.Tick();
                            break;
                        case AnimationComponent.Status.Done:
                            // Impossibile
                            break;
                    }
                }
            }
        }

        // OnDisable is called when the behaviour becomes disabled () or inactive
        private void OnDisable()
        {
            // Destroys all Playables and Outputs created by the graph.
            playableGraph.Destroy();
        }

        // Add clip to pending animation list.
        public void AddAnimations(int[] animations, int[] audios = null, bool queued = true)
        {
            if (!queued)
            {
                CleanPendingAnimations();
            }
            for (int index = 0; index < animations.Length; index++)
            {
                int audioIndex = -1;
                if (audios != null && index < audios.Length)
                {
                    audioIndex = audios[index];
                }
                AnimationComponent component = new AnimationComponent(animationMixer, audioMixer, index, audioIndex);
                currentAnimationList.Add(component);
            }
        }

        // Clean current animation list in pending state.
        private void CleanPendingAnimations()
        {
            if (currentAnimationList.Count < 2)
            {
                return;
            }
            currentAnimationList.RemoveRange(1, currentAnimationList.Count - 1);
        }
    }
}