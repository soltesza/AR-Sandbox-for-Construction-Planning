
The Sumo folder and its files are a modified version of 'sumo-win64extra-1.1.0'. 
A zip file of the original SUMO source is supplied in this repository. 
______________________________________________________________________________________
______________________________________________________________________________________
Setup Instructions
______________________________________________________________________________________
______________________________________________________________________________________


----------------------------------------Step 1----------------------------------------
In order to run the AR_Sumobox Unity Project you must copy the Sumo folder to the C:
drive of your computer. The resulting path should be "C:\Sumo". 
--------------------------------------------------------------------------------------


----------------------------------------Step 2----------------------------------------
Once Sumo is installed it must be able to be found by Unity. To do this you must add
a System or User variable that defines SUMO_HOME. Follow the steps below... 
--------------------------------------------------------------------------------------
1. Open file explorer and right-click on 'This PC'.
2. Select 'Properties'.
3. Select 'Advanced System Settings'.
4. In the 'System Properties' window that just opened select 'Environment Variables'.
5. Under the 'User Variables' window select the 'New' button.
6. For 'Variable Name' enter "SUMO_HOME"
7. For 'Variable Value' enter "C:\Sumo"
8. Click 'Ok' to save.


----------------------------------------Step 3----------------------------------------
Sumo tools must also be able to be found. To do this follow these steps...
---------------------------------------------------------------------------------------
1. In the 'User variables' window there is a variable named 'Path'. Select this variable
   so that it is highlighted and press the edit button below the window.
2. In the 'Edit Environment Variable' window that just opened select 'New'.
3. Type "C:\Sumo\tools\" and click 'Ok'.
4. Click 'Ok' twice more to close all windows.


