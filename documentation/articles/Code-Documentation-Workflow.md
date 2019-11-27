### Getting Started
To keep FUSEE documentation up to date several tools are used in addition to Microsoft Visual Studio.
In order to create documentation .chm's and webpages the following software needs to be installed:
* Sandcastle Help File Builder: [https://github.com/EWSoftware/SHFB](https://github.com/EWSoftware/SHFB)  
and, optional:
* Ghostdoc (free version): [http://submain.com/products/ghostdoc.aspx](http://submain.com/products/ghostdoc.aspx)

## Setup and create documentation workflow for FUSEE
### Enable XML Documentation Files in target project
![Enable XML comments in Visual Studio](https://raw.githubusercontent.com/wiki/FUSEEProjectTeam/Fusee/Images/Code-Documentation-Workflow/Enable-XML-Docs.png)

### Document your work with Visual Studio
1. Place the cursor in the line above a method.
2. Write three slashes ``///``

### Document your work with Ghostdoc
![Create Comments with Ghostdoc](https://raw.githubusercontent.com/wiki/FUSEEProjectTeam/Fusee/Images/Code-Documentation-Workflow/ghostDoc-add-comment.png)


### Open Engine.shfbproj inside FUSEE's main directory with Sandcastle Help File Builder
* Under “Project Properties” – “Build”:  select “HTML” and “Website” as file format.
* Make sure all Projects are selected. To change settings of a Project, simply single click it and options will appear at the bottom.
![select project to change settings](https://raw.githubusercontent.com/FUSEEProjectTeam/Fusee/develop_lh/Help/HowTo_Images/Documentation/documentationWorkflow_2.jpg)
* _Selected Projects, effective Jan 2015: Common, Core, KeyframeAnimation, Lidgren, Math.Core, OpenTK, SerializationContainer, SFMLAudio, SimpleScene and Xirkit_
* Go to “Project Properties” – “Help File” and select “VS2013” as “Presentation Style”.
![Presentation style](https://raw.githubusercontent.com/FUSEEProjectTeam/Fusee/develop_lh/Help/HowTo_Images/Documentation/documentationWorkflow_3.jpg)
* Now build it (“Documentation” – “Build Project”).


### Hints for Building documentation with Sandcastle Help File Builder
* Before Building the Project make sure that the output folder is as desired, you can configure this setting in Sandcastle Help File Builder -> Project Properties -> Paths
* Try not to output documentation files into folders that already contain files. Your old files could get lost.
* After pressing Build Project in Sandcastle Help File Builder observe the error output, it shows when files are not properly documented and comments are missing.
* Ignore warnings that look like that: "Warn: ResolveReferenceLinksComponent2". This warnings do not affect the quality of the documentation files output.
