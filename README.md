# Assignment 5: Shader Graph Lightsaber

# AI Disclosure
I used claude Opus to help with the wirting and implementation of some of the scripts, and also used it to help put together this readme and format it in a reasonable manner. 

## Controls
- **Space**: ignite / retract the blade
- **Hold right mouse + move**: swing the lightsaber (the hum gets louder and higher as you swing faster)
- **1-5**: change blade color (blue, green, red, purple, yellow)

## Running
Open the project in Unity (URP), open the main scene, and press Play.
Note: Project Settings > Player > Active Input Handling is set to "Both" so the legacy `Input` API works.

## Parts Completed
- **Parts 1-3**: URP scene, Bloom post-processing, custom Lit Shader Graph with BladeColor, OuterGlow, InnerGlow, and a PulseSpeed noise-sine pulse.
- **Part 4**: `Lightsaber.cs` on `BladeBase` scales the blade along its local Y from a pivot at the hilt, and deactivates the blade when it is fully collapsed.
- **Part 5 (customization)**: `LightsaberExtras.cs`
  - A Point Light on the blade lights the surrounding scene in the blade's color, fades with extension, and flickers slightly
  - Ignition, retraction, and hum sounds are generated in code with `AudioClip.Create` (no outside audio files)
  - Mouse-driven swinging, with swing speed controlling the hum's pitch and volume
  - Runtime color switching through a MaterialPropertyBlock, which drives the Shader Graph's BladeColor property and the light color together

## Hierarchy
```
Lightsaber (LightsaberExtras, AudioSource)
├── Hilt
└── BladeBase (Lightsaber)
    └── Blade (Point Light child)
```

## Outside Resources
- Assignment handout and Unity documentation (AudioClip.Create, MaterialPropertyBlock)
- Claude (AI assistant) for help writing scripts

## Video
[link here]
