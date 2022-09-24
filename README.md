# I3DShapesTool
Tool used for extracting the binary .i3d.shapes files used by the GIANTS engine

# Usage
1. Download the pre-built binaries under releases (click on I3DShapesTool.exe) https://github.com/Donkie/I3DShapesTool/releases
2. Extract the zip to some folder on for example your desktop
3. Open windows command line in this folder
* Run `I3DShapesTool -h` to see a list of all available options
* Example extraction: `I3DShapesTool someShapesFile.i3d.shapes` where `someShapesFile.i3d.shapes` is located in the same folder as the I3DShapesTool application.
* You can also simply drag-drop a `.i3d.shapes` file onto the extracted .exe in order to directly export it to `.obj`.

# Modification by me
I added export of all 4 possible UV maps (using a dirty OBJ file hack) - works fine.

The VertexColor export is probably corrupted. I'm working on it ;-)
![about](https://user-images.githubusercontent.com/106232621/192085081-cc8caf66-982d-4da5-beb1-1b41357e619c.png)
You need to load the exported obj using this blender plugin: https://github.com/apickwick/blender-io-obj-VC-4UVs (works on 2.93).
