# Modification by VidhosticeSDK

## March 2025: Added support for FS25 (for testing).

Same as below, we added to the export: 4xUV, VertexColor, multiple Materials

```
# Wavefront OBJx file (extension: 4xUV, VertexColor, multiple Materials)
# Creator: I3DShapesTool by Donkie (edited by VidhosticeSDK)
# Name: untitled
# Scale: 1.00

g default

v 1.000000 1.000000 1.000000 1.000000 1.000000 1.000000           <- Vertex Color
v -1.000000 1.000000 1.000000 1.000000 1.000000 1.000000
...
...
vt 0.625000 0.750000            <- UV1
vt 0.625000 1.000000
...
...
vt2 0.333333 0.333333           <- UV2
vt2 0.000000 0.333333
...
...
vt3 1.000000 0.000000           <- UV3
vt3 1.000000 1.000000
...
...
vt4 0.319734 0.619069           <- UV4
vt4 0.093471 0.848276
...
...
vn 0.000000 0.000000 1.000000
vn 0.000000 0.000000 1.000000
...
...
s off
g Cube
usemtl 1                       <- Material 1
f 1/1/1 2/2/2 3/3/3
f 4/4/4 5/5/5 6/6/6
f 1/1/1 3/3/3 7/7/7
usemtl 2                       <- Material 2
f 8/8/8 9/9/9 10/10/10
f 8/8/8 10/10/10 11/11/11
```

<br/>

># I3DShapesTool
>Tool used for extracting the binary .i3d.shapes files used by the GIANTS engine
>
># Usage
>1. Download the pre-built binaries under releases (click on I3DShapesTool.exe) https://github.com/Donkie/I3DShapesTool/releases
>2. Extract the zip to some folder on for example your desktop
>3. Open windows command line in this folder
>* Run `I3DShapesTool -h` to see a list of all available options
>* Example extraction: `I3DShapesTool someShapesFile.i3d.shapes` where `someShapesFile.i3d.shapes` is located in the same folder as the I3DShapesTool application.
>* You can also simply drag-drop a `.i3d.shapes` file onto the extracted .exe in order to directly export it to `.obj`.
>
># Modification by VidhosticeSDK
>Wavefront OBJx file (extension: 4xUV, VertexColor, multiple Materials)
