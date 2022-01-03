What is this?
---

In game console for Unity3D that allows for easy set up of commands with parameters.


Why is this needed?
---

Games need an easy way to issue debug commands while the game is running, and Unity does not provide an out-of-the-box in-game console.


Key features
---

* Easy to add new commands in script
* Highly configurable
* Command auto complete
* Command history
* Command Search


Integration
---
Just drag the uConsole prefab into a scene.

Another option is to instantiate the prefab in script and mark as DoNotDestroy.


Adding Commands
---

Add a command with uConsole.RegisterCommand like:

uConsole.RegisterCommand("fov", "change field of view of main camera", fov);

The first parameter is the name of the command
Second parameter is help text (optional)
Third parameter is the function called when command is issued

To retreive console command parameters, use the API functions:

uConsole.GetInt()
uConsole.GetFloat()
uConsole.GetBool()
uConsole.GetString()

If there are two float parameters expected, just call GetFloat() twice:

float floatA = uConsole.GetFloat();
float floatB = uConsole.GetFLoat();

You can query how many parameters as present for a command using:

uConsole.GetNumParameters()


