# FlightInspection

### SUMMARY
This project is a simulator-interface platform.
This platform allows you upload a good-flight csv, a might problematic flight csv and a rellevant xml for parsering the csv.

The platform shows the flight by the simulator and also imoportant information in a desktio-application.
Moreover, a dll for anomaly detection can be upload in order to visualize the anomalize on the application.

### Hierarchy
In the main folder the are the application files which implements the MVVM architecture.
*should add here*

### What should I download?
A great question.
In order to download this platform, you should clone this repository.
Also, you have to download the flightgear simulator from [here](https://www.flightgear.org/).
You might have two csv files like marked above and a rellevant csv in the simulator directory.
It's prefer that you have an appropiate dll for anomaly detector which imlements the interface describe below.

That's it :)

### An appropiate dll
The dll has to implement 'doAnomaly' function (without inpput and output).
The dll reat "input.txt" file: the first lines will be the learning csv, after that 'done' then the 'test' csv and then another 'done'.
The dll has to learn correlation from the first csv, and detect anomalies in the second csv.
The dll write a new file callde "output.txt" so each line will be a row number, a space, first column name - second column name, for each anamoly that was detected.
For example, if there is an anomaly in the second csv in line 45 between column the correlative columns 'airspeed' and 'high',
the dll will write in the 'output.txt' the line: "45 airspeed-high".

### Download instructions
*what should be here??*

#### please see [UML]() for calrify the documentation and the hierarchy of this platform.

Thank you choose 'EL-AL'
