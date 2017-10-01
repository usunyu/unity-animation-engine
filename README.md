# unity-animation-engine

#### Introduction
Implement Unity Animation Engine by Playable API.

##### Playables API:
The Playables API provides a way to create tools, effects or other gameplay mechanisms by organizing and evaluating data sources in a tree-like structure known as the PlayableGraph. The PlayableGraph allows you to mix, blend, and modify multiple data sources, and play them through a single output.

The Playables API supports animation, audio and scripts. The Playables API also provides the capacity to interact with the animation system and audio system through scripting.

Although the Playables API is currently limited to animation, audio, and scripts, it is a generic API that will eventually be used by video and other systems.

##### Playable vs Animation:
The animation system already has a graph editing tool, it’s a state machine system that is restricted to playing animation. The Playables API is designed to be more flexible and to support other systems. The Playables API also allows for the creation of graphs not possible with the state machine. These graphs represent a flow of data, indicating what each node produces and consumes. In addition, a single graph is not limited to a single system. A single graph may contain nodes for animation, audio, and scripts.

##### Advantages of using the Playables API:
* The Playables API allows for dynamic animation blending. This means that objects in the scenes could provide their own animations. For example, animations for weapons, chests, and traps could be dynamically added to the PlayableGraph and used for a certain duration.
* The Playables API allows you to play easily play a single animation without the overhead involved in creating and managing an AnimatorController asset.
* The Playables API allows users to dynamically create blending graphs and control the blending weights directly frame by frame.
* A PlayableGraph can be created at runtime, adding playable node as needed, based on conditions. Instead of having a huge “one-size-fit-all” graph where nodes are enabled and disabled, the PlayableGraph can be tailored to fit the exact need of the current situation.

#### API Usage:

```
/// <summary>
/// Add amimation clip index to pending animation list, will be played one by one.
/// </summary>
/// <param name="animations">Animation clips added.</param>
/// <param name="audios">If not null, audio clips will be played with animation in samoe order.</param>
/// <param name="queued">If true, new animations will be played after current animations.</param>
AddAnimations(int[] animations, int[] audios = null, bool queued = true)
```
