# MNR Prop Mover

A utility that adds extra adjustments to props.

## How to download

Either clone this repository and build yourself, or download one of the [precompiled binaries](../..//releases/latest).

## How to use

Firstly start RPCS3, then run this program. It will automatically hook to RPCS3 and start working immediately.

> **Linux notice:** If the program cannot read the prop selection from RPCS3, Linux may be blocking access to another process. You can temporarily allow it with:
>
> ```bash
> sudo sysctl -w kernel.yama.ptrace_scope=0
> ```
>
> This reduces process isolation for the whole system and resets after reboot. Restore the usual setting with `sudo sysctl -w kernel.yama.ptrace_scope=1` when finished.

Moving a prop will move it off of the prop origin point rather than move the prop stamp.
This can be handy for getting props in difficult places (including out of bounds) but please do not go too crazy or you may struggle to reselect the prop.

Please note that if an adjustment does not work on a certain prop, it is being prevented by the game and is not a bug with the application.
Please do not report these unless you have good reason to believe that it is an issue with the app.

## Controls

**IF YOU ARE USING LINUX:** Please note that you MUST have the console window selected for controls to work.

**Up Arrow** - Tilt Forwards

**Down Arrow** - Tilt Backwards

**Left Arrow** - Tilt Left

**Right Arrow** - Tilt Right


**Q** - Rotate Left

**E** - Rotate Right


90-degree rotation per press:

**U / O** - X axis

**J / L** - Y axis

**I / K** - Z axis


**Page Up** - Scale Up

**Page Down** - Scale Down


**W** - Move Forwards

**S** - Move Backwards

**A** - Move Left

**D** - Move Right

**R** - Move Up

**F** - Move Down
