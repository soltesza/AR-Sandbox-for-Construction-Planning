# AR-Sandbox-for-Construction-Planning

The AR sandbox is meant to augment a physical sandbox with additional information overlaid with a projector. Specifically, this project aims to aid in the visualization of civil and construction engineering concepts both for collaboration and teaching.

**NOTE: This project requires a Kinect V2 sensor to operate.**

# SEE HERE FOR THE UPDATED INSTALL GUIDE FOR TRAFFIC SIMULATION FEATURES

https://github.com/spencjon/AR-Sandbox-for-OSU-Civil-Construction-Engineering/blob/master/Documents/2018_Documents/ARSandbox%20Traffic%20Simulation%20Install%20Guide/ARSandbox_Traffic_Simulation_Install_Guide.pdf

# Installation

This project requires the Unity game engine and the Microsoft Kinect SDK to operate. After installing Unity and the Kinect SDK, run the Unity editor. Open a new project, and navigate to the src directory in the local GitHub repository. Unity should recognize the AR_Sandbox directory contained in the src directory as a Unity project. After opening the project, the program can be run in the editor by pressing the play button, or as an executable by navigating to **File > Build & Run** or pressing **CTRL + B**.

# User Manual

## Navigation

The navigation menu is opened by pressing the **ESC** key. The menu gives the option to change mode, calibrate the system, or quit the application. The mode can also be changed by pressing **Q** for depth mode, **W** for design mode, **E** for cut and fill mode, and **R** for calibration mode.

## System Setup

When the system starts, a number of parameters must be set in order to ensure proper height measurement and display. These parameters are set by opening the navigation menu and pressing **Calibrate** or pressing the **R** key. Begin by dragging the gray control points at each edge of the projection area so that the entire surface of the sandbox is covered by the projection. Clicking and dragging anywhere on the projection area itself will move entire projection area. To align the projected image with the physical features of the sandbox, use the **Arrow Keys** to translate and the **+** and **-** keys to scale it. Finally, set the lowest and highest points on the sand by adjusting the two sliders labeled maximum and minimum height that appear in the lower left-hand corner. Dig a hole in the sand down to the bottom of the sandbox, or to the desired lowest point. Build a hill to the highest desired point. Start with the Minimum Height slider at 0 and lower the Maximum Height slider until the lowest point on the sand turns red but before orange equipotential lines appear. Raise the Minimum Height slider until the highest point on the sand turns blue, but before cyan equipotential lines appear. Once you have calibrated the system, you can click the **Save** button to save your settings and the **Load** button to load your saved settings.

## Depth Mode

Depth mode is used for displaying strictly the height of the sand using a color gradient. Red areas are the lowest, and blue areas are the highest.

## Design Mode

Design Mode is used for designing a road segment that will be used for cut and fill calculations. When Design Mode is selected, a road will appear on the sand. This road is constrained to the bounds of the sandbox. The path of the road can be changed by clicking and dragging the orange diamond shaped control points. Holding the **Shift** key causes a cross-sectional view of the road and terrain to appear, and allows the height of the control points to be changed by clicking and dragging with the mouse. This cross-sectional view can also be toggled on or off using the **H** key. Points can be added or removed by pressing the **Add Point** or **Remove Point** buttons. Unwanted changes to control point positions can be undone by pressing **CTRL + Z**. You can also press the **Z** key to make the road flat which can be useful when testing the sandbox.

## Cut/Fill Mode

Cut/Fill Mode is used to display a table containing information about the road segment such as cut and fill areas and volumes. When design mode is selected, the road segment will be visible. To open the cut/fill table, press the **E** key. The table updates every 5 seconds, and can be scrolled using the horizontal scrollbar to the bottom and vertical scrollbar to the right.
