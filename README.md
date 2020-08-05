# SearchADGPO
Tool for searching a specified string within all Domain GPO's

## Purpose
The main Function is to Search your whole AD-Gpo's for a defined string. This tool is intended to use on a DomainController or a Computer where you installed the GPEdit-Snapin via FOD.

## Language
This little Tool is written in C#.

## Functionality
### Main Function
This Tool automaticaly reads all GPO's from the DomainController the user is connected. It lists all GPO's which contain the specified string in an separate listbox. There are two predefined SearchCriterias (only W7/only W10 Policys -> if PolicyName Contains W7 or W10).

### Cache
The Tool has also implemented a Caching-Function therfore all GPO's were saved as xml-File. If you Search again during the specified Threshold only Cached-Files are used. Cache is located in a SubFolder beside the actual EXE-File.

### SubMenu on right click / DoubleClick
On DoubleClick the tool generates a HTML-Report which is directly located beside the Cache-Files. On Right-Click you can chose to edit the selected GPO.
