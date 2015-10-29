Here’s one way to build and run it:
1.	Make sure you have a Java 8 JDK installed.
•	If not, you can get it from \\agi\common\LicensedApps\Java\Java 1.8 (JDK and JRE) 
2.	Copy an AGI Components license file into the src\main\resources directory
3.	Open a command prompt and navigate to the extracted directory
4.	Run this command to build the project: gradlew build
•	You will need access to the internet to run this as it downloads some third-party libraries.
•	If you get an error like “Could not find tools.jar” then you need to set the JAVA_HOME environment variable to where you installed the JDK. You can set this by:
i.	Right click on the Computer icon on your desktop and select Properties
ii.	Click Advanced system settings on the left panel
iii.	Click the Environment Variables… button
iv.	In the bottom section, click the New… button
v.	Enter JAVA_HOME for the variable name
vi.	Paste in the directory of your JDK installation for the variable value
5.	Run this command to start the server: java –jar build\libs\orbit-propagation.jar
•	Technically you can double-click on the JAR file to run it, but shutting down the server later is more difficult.
6.	Open a web browser and go to http://localhost:8080/
7.	When you are finished, go back to the command prompt and press Ctrl+C to shut down the server.

There are two ways to load this project into Eclipse. First way is to install the Gradle plugin for Eclipse and import the extracted directory as a Gradle project. Second way is, from the command prompt, to run "gradlew eclipse", which turns it into a regular Java project that can be imported into Eclipse. Once you have the project imported into Eclipse, you can run it by right clicking on the OrbitPropagationTutorial class, and selecting Run As -> Java Application.
