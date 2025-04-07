# Jayo's Trigger Queue for VNyan

Provides a convenient way to assign VNyan triggers to timed, named queues. Each queue will only run its next trigger after a time specified with the previous trigger.  

# Table of contents
1. [Installation](#installation)
2. [Usage](#usage)
    1. [Trigger Prefix](#trigger-prefix)
    1. [Utility Triggers](#utility-triggers)
        1. [Next Item in Queue](#next-item-in-queue)
        2. [Clear Items in Queue](#clear-items-in-queue)
        3. [Clear Items in All Queues](#clear-items-in-all-queues)
3. [Development](#development)
4. [Special Thanks](#special-thanks) 

## Installation
Once you've got your plugin files (either from a DLL file from the store, or from your builds from source), installation is simple:

1. In VNyan, make sure you've enabled "Allow 3rd party plugins" from the Settings menu.
2. Copy the DLL file _directly_ into your VNyan installation folder's `Items\Assemblies` folder
3. Launch VNyan, confirm that a button for the plugin now exists in your Plugins window!

## Usage

This plugin contains an OSC Sender and an OSC Receiver that can connect to VRChat and communicate between VRC and VNyan.
This is a complete implementation of the VRChat OSC interface documented here: https://docs.vrchat.com/docs/osc-overview .
Changes to avatar parameters in VRC will be set as VNyan parameters, and will activate a trigger so that your node graph can respond in real time.
VNyan trigger can also be used to change VRC avatar parameters, drive control inputs, populate the chatbox, and send body-tracking and eye-tracking telemetry!

In order to avoid conflict with other plugins or VNyan internals, parameter and trigger names related to this plugin are prefixed with `_jq_` or `_jq:`.

### Trigger Prefix

You can queue a trigger by adding a specially-strucutred prefix to the trigger's name:

`_jq:<queue>;;<time>;;YourTriggerNameHere`

where `<queue>` is teh name of the queue "channel" to assign the trigger to, and <time> is an amount of seconds to wait before another item in the same queue channel will be triggered.

You can use this to do things like spread out spammed redeems over a longer period of time, or prevent identical redeems from "overlapping", or to queue up multiple triggers to happen in a carefully-timed sequence.

### Utility Triggers

Thisplugin also provides a few utility triggers to help manage the trigger queues:

#### Next Item in Queue

Trigger name: `_jq_next`
Text1 value: name of queue channel

This trigger will cancel any current remaining wait time for the queue named in the value on the `text1` socket, and immediately fire the next trigger in the queue.

#### Clear Items in Queue

Trigger name: `_jq_clear`
Text1 value: name of queue channel

This trigger will cancel any current remaining wait time for the queue named in the value on the `text1` socket, and immediately clear out any pending items in the queue without activating them.

#### Clear Items in All Queues

Trigger name: `_jq_clear_all_`

This trigger will cancel any current remaining wait time for all queues, and immediately clear out any pending items in all queues without activating them.

## Development
(Almost) Everything you'll need to develop a fork of this plugin (or some other plugin based on this one)!  The main VS project contains all of the code for the plugin DLL, as well as a `unitypackage` that can be dragged into a project to build and modify the UI and export the modified Custom Object.

Per VNyan's requirements, this plugin is built under **Unity 2020.3.48f1** , so you'll need to develop on this version to maintain compatability with VNyan.
You'll also need the [VNyan SDK](https://suvidriel.itch.io/vnyan) imported into your project for it to function properly.
Your Visual C# project will need to mave the paths to all dependencies updated to match their locations on your machine (i.e. you VNyan installation directory under VNyan_Data/Managed).  Most should point to Unity Engine libraries for the correct Engine version **2020.3.48f1**.

## Special Thanks

Suvidriel for building and maintaining VNyan
The Last Seahorse, 2.0, AliceLunazera and Lumibnuuy for suggesting the concept and testing the results (and generally being hella rad)