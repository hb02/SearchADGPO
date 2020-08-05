# SearchADGPO
Tool for Searching Current User Domain

## Purpose
The main Function is to Search your whole AD-Gpo's vor a defined string. This tool is intended to use on a DomainController or a Computer where you installed the GPEdit-Snapin via FOD.

## Language
This little Tool is written in C#.

## Functionality
### Main Function
This Tool automaticaly reads the Domain of the Current User and Reads all GPO's. It lists all Objects in an separate part. There are two predefined SearchCriterias (only W7/only W10 Policys).

### Cache
The Tool has also implemented a Caching-Function (Report saved as XML). All GPO'S were saved as xml-File. If you Search again during the specified Threshold only Cached-Files are used. Cache is located in a SubFolder beside the actual EXE-File.

### SubMenu on right click / DoubleClick
On DoubleClick the tool generates a HTML-Report which is directly located beside the Cache-Files. On Right-Click you can chose to edit the selected GPO.
