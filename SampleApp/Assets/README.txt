=============================
SessionM Sample App for Unity 
=============================
v1.0
License: MIT

This project is designed to demonstrate basic SessionM Integration in Unity.  

For more integration info, please see http://www.sessionm.com/documentation/unity-integration.php 

To download SessionM latest SDKs, please see http://www.sessionm.com/documentation/downloads.php

If you run and compile the project as-is, it will produce a working Android 
or iOS App with a test SessionM app.

The following features are demonstrated:
 -The large gift icon opens the SessionM Portal
 -There is a badge which actively displays the number of unclaimed 
  SessionM Achievements.
 -There are three Action claim buttons which will report three different 
  actions when clicked.
 -There is a toast, which displays and allows users to claim achievements
  as they are earned.

To build and test the app, simply switch to the iOS or Android platforms and 
create a build within Unity.

If you would like to customize the app to work with your own SessionM App, 
you'll need to do the following:

1.) Open the scene "Sample"
2.) Select the "SessionM" GameObject in the scene.
3.) Change the iOS App ID and/or the Android App ID to your app's IDs. (You 
can find this in the developer dashboard.
4.) Select the "SessionM Sample" GameObject in the scene.
5.) Change Actions 1-3 in the inspector to Actions within your app.  This will
cause the three buttons to trigger Actions custom to your app.

That's it, save and build!

Note:
We only tested this project on Mac OS.
