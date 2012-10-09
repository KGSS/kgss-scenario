KGSS Scenarios

Installing the plugin:

Move KGSS-Scenario.dll to KSP\Plugins

Installing the Random Failure scenario:

Add the following lines to the bottom of your persistence.sfs, just before the final closing brace:

SCENARIO
{
	name = RandomFailure
	scene = 7
}