
# Auxiliary software for treatment of autism based on facial expression 
## Specification


Our project is an auxiliary software for treatment of autism.

Through facial expression recognition, capturing the facial expression feedback from autistic children. By trying different topics and events, in order to analyze the autistic child’s sensitivity with different topics and how they express their feelings, this could help the therapist to improve their presupposed topics, treatment plan, and the way to encourage the child to interact. 


Another application is translation. When an Autistic child is communicating with the therapist, the program identifies the facial expression of the therapist, showing a corresponding expression icon of the therapist's current facial expressions (such as: a smile) to help autistic children understand others’ expressions (autistic child have difficulty understanding facial expression), thus ease the communication with autism child.

Meanwhile, additional techniques includes intonation analysis, using data visualization to show the dynamic changes of the child’s status data,  and analyzing the psychological state of the child and the information behind instantaneous facial expression.

In the long-term plan, we have implemented the detection of human action by Kinect, to observe the repeating abnormal movements of autistic children, due to unsatisfactory results caused by jumpy human-bone identification, we didn’t add it into our project for now, another is doing statistics research on the child's information and the experiment feedback, establish sharing platform is considered. Through the accumulation of experimental information, to preliminary evaluate the effectiveness of the treatment plan and provide recommended treatment suggestions.

## Referenced Documents

* Project Oxford Emotion API
[Link](https://www.projectoxford.ai/emotion)
* Emovoice SSI Open Source Project
[Link](https://www.informatik.uniaugsburg.de/lehrstuehle/hcm/projects/tools/emovoice/)
* Kinect for Windows SDK v1.8
[Link](https://www.microsoft.com/en-us/download/details.aspx?id=40278)

## Current System or Situation
###Background, Objectives, and Scope
The project was done to satisfy the requirements for the medical auxiliary diagnosis of autism. 

In the present world, the incidence of autism in children showed a sharp upward trend. Those children commonly has similar characteristics such as ineloquent, lacking in communication and having language barriers, constantly repeating stereotyped behaviors and only interested in certain kinds of activities.

Our auxiliary software for treatment of autism based on facial expression recognition is intended to help the children with autism to correctly understand other’s facial expression and help therapist collect and analysis the information about the reaction of autistic children. 

###Previous Situation
In the procedure of curing autism, there is no single effective way in autism treatment methods, so carrying out the treatment in multiaspect is widely recognized. Training education is the preferred method.

In current treatment, there has been some developed software working on helping children recognizing the name of different objects, with pictures such as fruits and animals, and display the corresponding name to each, as well as distinguish different colors, and there are corresponding game to help children learn while playing, but so far the software target at helping the therapist to do auxiliary analysis for autism assisted treatment has not been dealt with yet.

##Motivation
###Justification for change
According to the situation that current software designed for children with autistic disorder are aimed at learning basic knowledge, and majority of which are with the method of playing games, not for therapist assisted diagnosis applications. So in our software, aiming at several main characteristics of autistic children, we do the following innovative design:


_**Children with autism often have the problem of correctly understanding the meaning of others’ expression.**_

To solve this problem, we design a bidirectional expression recognition feedback system. One side for autistic children, one side for the therapist.

On one hand, the therapist can collect statistics on the expression changes of autistic children in time line. With therapist’s note and statistical analysis, the therapist can adjust treatment strategy according to the child’s interest point and avoid sensitive topic to reduce the possibilities to irritate the child. This can grasp the children's interest, as well as avoid the chance that therapists miss the children's expression changes while recording the note or distraction, which would be beneficial for the treatment being more efficient.

On the other hand, children can compare therapist's facial expression in reality with the corresponding facial expression recognition results on the handheld tablet devices like surface in the child’s hands, through which they can correctly understand the meaning of each expression actually corresponds to. This would do a significant improvement on helping to solve the realistic problem that some children don't understand the meaning of sadness, anger and other expressions.

_**Children with autism spectrum disorders are sometimes emotionally abnormal.**_

Sometimes when encountered with sensitive topic, or specific factors, children will be emotionally abnormal. As part of an auxiliary evaluation, we also consider the results of the intonation analysis, which can determine whether the mood is excited or mild, positive or negative. In face of some children do not like to look at others directly, the intonation analysis become an additional important basis for emotion analysis and distinguishing the child’s preferences on topics.

_**Children with autism tend to have repetitive stereotyped movements.**_

Body language is another basis for emotion analysis, through the recognition of frequent drastic body movements in a short period of time, we can preliminary estimate that the child have violent mood swings in this period of time, such as excited, happy or angry. In offline analysis, therapists should pay special attention to the time point where those violent mood swings occur.

_**Children with autism are only interested in a few specific topics**_

With the combination of the three auxiliary identification method above, facial expression recognition, voice recognition, activity recognition and the help with therapist’s notes, in one hand, the therapist can estimate where the children's interests lies in.  The topic design in the later treatment can be adjusted closer to the area of the child’s interests.


On the other hand, the recognition result of the facial expression, intonation and action information will do real-time information processing and be displayed real-time on the therapist's screen for therapist’s reference, including mood swings in time-line, results of child’s individually selected emotion status feedback and real-time emotion classification results.

## Target Solution
###Primary objectives satisfied by auxiliary software for treatment of autism based on facial expression recognition 
The purpose of this project was to develop the auxiliary software for treatment of autism that satisfies the following objectives:  


* Provide an alternative to the manual procedures, recording data for facial and body information and intonation, as well as made the preliminary emotion status judgment. 
* Provide functionalities for training autistic children to correctly recognize facial expressions.
* The software provides an intuitive and visual interface with capability of real time emotion-status data updating.
* The system provides both “two-way communication” and “interactive” characteristics.
* Automate the process.
* The system is made with user friendly interfaces, such that young-age users could access the system without having difficulty learning it

##Approach
The software system was developed utilizing C#, C++ and JavaScript. In order to ensure the quality of the end product, an extreme programming methodology was applied to the software life cycle.
###Technical
#### Feasibility Study
Before the development process officially gets started, a Project Pre-Proposal was formulated. From it, a Project Plan was derived, including a work breakdown structure, effort estimation, time consume estimation, and other project planning and quality assurance components. A high-level Operational Concept Description (OCD) was developed. Prototype design by Axure RP was conducted on the developing procedure and was revised accordingly.
####Guidelines for Design Quality
A hierarchical organization of system components was defined. Modularity was achieved through logical partitioning of components possessing independent functions.  Separate data and procedure representations were developed through the use of Entity Relationship Diagrams and Data Flow Diagrams. System messages and common functions were recognized and designed.
####Project Implementation Details 
Program Structure Charts were developed and system messages were designed. The Structure Charts are shown below: 
![](/picture/structure.png)

As mentioned above, through logical partitioning of components possessing independent functions we achieved modularity. For each module, technical realization features are listed below and was sorted in execution order:

* Getting image and voice data
First installing Kinect for Windows SDK v1.8 on Kinect 1517, the procedure of acquiring the information of image and human voice was able to complete. The output format is .png, size 640*480 and .wav in sampling rate 16000.

* Facial expression recognition
After getting the input image stream, by referring the Project Oxford Emotion API, asynchronous uploading the image file in C#, we would get emotion analysis result in eight dimension—anger, contempt, disgust, fear, happiness, neutral, sadness and surprise in json.  The value ranges from 0 to 1, in general cases there will only be one to three kinds of detection results bigger than 0.1, and usually the first is way bigger than the second one in ratio. 
![](/picture/emotion.png)
After tests, those emotion detected results is trustworthy, and the order of which transmitting back is usually in the original sending order, because the occurrence rate of wrong order incident is much too low compared to the normally working ones, we can omit the interrupt of these exception.

* Intonation recognition
In the process of intonation recognition, we utilized an Open Source Platform in C++ for Social Signal Interpretation named SSI, by which we can get preliminary emotion analysis results based on human voices. After getting the input voice stream from Kinect, at an interval of 2 seconds, the voice data was send repeatedly to SSI API. The returning data is classified depending on the results into five types: negativeActive, negativePassive, neutral, positivePassive and positiveActive in .xml. The value ranges from 0 to 1 and the analysis result also has the feature of good resolving power.
![](/picture/intonation.png)
* Data Combination
Combining the returned results from facial expression recognition in json and the results from intonation recognition in .xml, we convert the format into csv in order to satisfy the input requirements for d3.js in the visualization process.
* Visualization
After data combination, with calling d3.js, we can visualize the data we got from .csv. We got flow graph, gradually changing linear graph,   . With visualization, we can intuitively see the trend of changes in emotion of autistic child in time-line and his instantaneous emotion status as well.  
![](/picture/childrenSurface.png)
![](/picture/childrenSurface2.png)


### Testing
Unit tests were conducted successfully by the developer. For the results of facial expression recognition, including the time cost for both-way Internet transmitting and processing procedure, the total delay is about 3 seconds, the subsequent delay is stable while the transmission interval is 1 second. For intonation recognition, delay time is about 4 seconds, and the total delay won’t change violently either.


System testing was conducted and source code was uploaded to github, https://github.com/tjuImage.  

### Results 
#### Summary of implementation results
User connects to the System by both opening the program for therapist on pc and the program for autistic children on surface, and the Kinect is connected to the pc. Due to we just own one Kinect currently, so we can only monitor the child’s facial expression, not the therapist’s. Thus the corresponding expression icon of the therapist's current facial expressions won’t be shown on surface for now, instead, we provide an input area for the child to enter his/her thoughts on surface, and the therapist can see the message on pc.


Autistic children is presented with user friendly interfaces on which they can easily express their feelings by taping lively cartoon faces with different facial expressions on surface, in the meantime the therapist will see the corresponding results on pc. 


With data collected from Kinect monitoring autistic children and returned results from Emovoice and Facial Expression API, the therapist is presented with three kinds of visualized dynamic display of trend of emotion changes of autistic child in time-line and his instantaneous emotion status as well. The therapist can take notes about the topic.


