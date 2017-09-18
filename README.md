# IlGenerator
#### Online C# assembly decomposition tool like IlDasm. ####

Web-site available: [Ilgenerator online](http://ilgenerator.apphb.com "IlGenerator main page")

This repository provides main source code for IlGenerator.  
It's a simple online decompiler that generates msil code +metadata for types in a given C# assembly.  
It's very simple to use:  
  * First you insert your C# code in the upper editor (1).  
  
![Upper editor](/images/editor.png)    
  
  * Then you expand the generated tree in the bottom-left part of the page (2). 
    
![Tree](/images/tree.png)  
  
  * Then you click on any field/method/type/etc. you want and it's metadata and source IL code (if there is one) will appear in the right editor (3).  
  
![Result editor](/images/resultEditor.png)  
  
  * If there are any **compilation errors** you will see their description instead of the visual tree.  
    
![Error messages](/images/errorMessages.png)  
  
  * You'll also see **annotations** for any errors and warnings on the left bar of upper editor.  
    
![Annotations](/images/annotations.png)  

This project is still unfinished but is already usable - all main functions work correctly.  
Any help would be greatly appreciated.
