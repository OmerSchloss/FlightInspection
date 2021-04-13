# FlightInspection

### SUMMARY
This project is a MVVM-designed desktop app, that implemets a flight controller.
In the program, the user can upload a flight (that represented as a csv file) into it, and see different items of data about the uploaded flight.
The idea is to first upload a normal flight in order to let the program learn the features and the data of a proper flight. Then to upload other flights in order to reseach if there are any anomalies in the flight. If there are any, they will be represented on the screen as red points.
The program inerfaces with the flight-gear simulator so the user can see the uploaded fligth on the flight gear app screen.
Furthermore, on the openning screen there is an option for the user to uplaod dll file for using different algorithms to detect the anomalies. 

### Hierarchy
In the main folder we have all of the cs and xaml files and we have also a directory called 'plugins' which contains two different dll files of two different detection algorithms.
In the main folder you can see the 'FlightgearModel.cs' file which is the model of the program. You can also see all of the view models that are connected with the model. 

### What should I download?
A great question.
In order to download this platform, first you should clone this repository.
Also, you have to download the flightgear simulator from [here](https://www.flightgear.org/).
You might have two csv files - one of a normal flight and the other of some fligth.
You should also drag the provided 'play_back.xml' file into this path in your computer: C:\Program Files\FlightGear 2020.3.6\data\Protocol.
You can use the dll files in the 'plugins' directory or use a dll of your own.
That's it :)

### An appropiate dll
The dll has to implement 'doAnomaly' function (without input and output).
The dll needs to be able to get "input.txt" file in this property: the first lines will be the train csv, after that there will be'done' then the 'test' csv and then another 'done'.
The dll has to learn correlation from the first csv, and detect anomalies in the second csv.
The dll write a new file called "output.txt" int this property: the first line is the name of the algorithm ('Line','Circle' or 'other' if you have dll of your own), then there are pairs of all the correlated features in the csv file with ',' between them and done in the and. The lines afterwards will contain the following properties with ',' between them: row number in the csv where the anomaly occured, the first feature and its correlated feature.
For example, if there is an anomaly in the second csv in line 45 between the correlative columns 'airspeed' and 'high',
the dll will write in the 'output.txt' the line: "45,airspeed,high".

#### Here is UML of the whole project for calrifying the hierarchy of this platform.

![flightgear 1 0](https://user-images.githubusercontent.com/71650499/114557742-a433e000-9c72-11eb-84aa-e934ddc1d911.png)
