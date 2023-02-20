# Minilogue XD Progdump
A tool to read KORG Minilogue XD Program Presets and dump them as a variety of formats.

<img src="docs/002_TyoCityLoop.svg" width="800" />

## Usage

Pass the path to a .mnlgxdprog, .mnlgxdlib, or .prog_bin file into the application and it will output a HTML file into the working directory. Or display an error message.

# userOscillatorDescriptions.json and userUnitMappings.json

Because of the way the Multi-Engine works, the programs themselves don't know _what_ exact Plugin they use. If you install a User Oscillator into slot 3, the Program will note that you use the Slot 3 User Oscillator, but not what that one is.

If I have Tim Shoebridge's PLUCK oscillator installed in mine and you have Sinevibe's Node in yours in Slot 3, the Program will just use Node instead and probably sound completely off. I could also install a different oscillator in Slot 3 tomorrow and now all my existing programs will use that one instead. So that's why it's tricky to know what the currect display name for a User Oscillator is, it's simply impossible to know which one was in use when the program was created.

And since User Oscillators are not included in the library exports (.mnlgxdlib) files, we don't know what they are.

By default, the application will output generic names like "USER OSC" and "Param 2", but you can provide configuration files for your specific synth. `userOscillatorDescriptions.json` is a library of User Oscillators and their parameter names. Without this file, User Oscillator parameters show up as "Param 1" through "Param 6" on the output. (There is no such file for Delay/Reverb/Mod FX because those don't have named parameters in the user interface)

`userUnitMappings.json` is a file that's specific to your synth. Have a look at the example files in the docs/ folder to see what they should look like, it should be pretty straight forward.

## Compatibility Notes

This tool is for the Minilogue XD only. It was tested with Firmware v2.10, which added a bunch of features to the sequencer and others: https://www.youtube.com/watch?v=Y8oLvq9vKes

This is **not** supported for any other logue devices, including the original Minilogue and Minilogue Bass, the Monologue, the Drumlogue, the Prologue, and the NTS-1.

## Development

It's written in .NET 6 and should work across Windows, macOS and Linux, but hasn't been tested on the latter.

The code has two distinct parts: A Parser, and a Report Generator. The Parser lives in the Parser folder and is just reading the Korg Librarian exported files into a strongly typed data structure.

It doesn't try to interpret the data (e.g., mapping the VoiceModeDepth into human readable values).

The second part is the Report Generator, which generates an output file based on the parsed data structures. Currently, there are report generators for SVG and JSON.