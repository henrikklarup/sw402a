How to use the Multi Agent System Compiler:

1) Run the MultiAgentSystem.exe
2) Press any key to continue
3) Choose whether to compile with outputs or not, errors will always be printed
4) Choose whether to compile the default file or use a custom.
	- Write "back" to go back from the file chooser.
5) Press any key to continue
	- If there is any compilation errors the user can choose to compile again.
6) When the compilation has completed, the MASSiveBattleField will open.


How to use the MASSiveBattleField:

- Commands can be typed in, in the command center in the bottom of the screen.
- All Commands can be found in the Command List on the right, tabbed with the Combat Log.
- To run a command type it into the field below the WarGame Console and press enter.
	- Press either End Turn or enter (with an empty textfield) to execute the commands.
	- Only one team can move each Turn.
	- Execute, executes all moves untill there is no more moves stored.

Examples:
The default file (mass.txt)
- Compiles without errors
- Contains 4 teams, with agents.
- Contains 5 actionpatterns
	- SearchAndDestroy, searches the area around the unit, best used with "encounter" instead of "move"
	- PatrolHigh, patrols the upper part of the medium battlefield
	- PatrolLow, patrols the lower part of the medium battlefield
	- PatrolLeft, patrols the left part of the medium battlefield
	- PatrolRight, patrols the right part of the medium battlefield
	
The other file (fail-example.txt)
Can be runned by choosing no when asked if you want to run the default file, and write fail-example.txt
- Compiles with errors
- Can be compiled again by pressing "y" when asked if you want to compile again, after the errors have been found.
- Best used, by correcting the found error and press "y".
How to fix the errors:
- "agen" should be "agent".
- "int" should be "num".
- Change "i" to an unused variable name.
- Only one agent can be added at a time.