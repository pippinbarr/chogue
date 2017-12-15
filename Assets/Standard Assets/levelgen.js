#pragma strict

var level:Texture2D;
var sizeX:int = 48;
var sizeY:int = 36;

function Start () {
   
    //Debug.Log("levelgel: "+Ax+","+Ay+","+Bx+","+By);
    GenererNiveau(0, 0, 0, 0);


}

function GenererNiveau(Ax:float, Ay:float, Bx:float, By:float){
	level = new Texture2D(sizeX, sizeY);
	level.filterMode = FilterMode.Point;
	GetComponent.<Renderer>().material.mainTexture = level;
	
	//remplir avec du bruit
	for(var ypos:int = 0; ypos<sizeY; ypos++){
		for(var xpos:int = 0; xpos<sizeX; xpos++){
			if((Random.value > 0.45)){
				level.SetPixel(xpos,ypos,new Color(1,1,1,1));
			}
			else{
				level.SetPixel(xpos,ypos,new Color(0,0,0,1));
			}
		
		}
		
	}



	level.Apply();

	
	CellAuto45(level);		
	//CellAuto45(level);	


	

	level.Apply();
	
	var bytes = level.EncodeToPNG();
	//File.WriteAllBytes(Application.dataPath + "/../niveau.png", bytes);	

}

function Update () {
	/* if (Input.GetMouseButtonDown(0)){ 
	 	CellAuto45(level);
	 }*/
}

function CellAuto45(level:Texture2D){

	for(var ypos:int = 0; ypos<level.height; ypos++){
		for(var xpos:int = 0; xpos<level.width; xpos++){
			//vérifier les 9 pixels de la matrice 3x3, si 5 murs ou plus, alors mur
			var numwalls:int = 0;
			//haut gauche
			var tempx:int = xpos-1;
			var tempy:int = ypos-1;
			if((tempx>-1)&&(tempy>-1)){
				//c'est un mur
				if(level.GetPixel(tempx,tempy).r==0){
					numwalls++;
				}
			}
			else{
				numwalls++;
			}
			//haut
			tempx = xpos;
			tempy = ypos-1;
			if((tempx>-1)&&(tempy>-1)){
				//c'est un mur
				if(level.GetPixel(tempx,tempy).r==0){
					numwalls++;
				}
			}
			else{
				numwalls++;
			}		
			//haut droit
			tempx = xpos+1;
			tempy = ypos-1;
			if((tempx<=level.width)&&(tempy>-1)){
				//c'est un mur
				if(level.GetPixel(tempx,tempy).r==0){
					numwalls++;
				}
			}
			else{
				numwalls++;
			}	
			//gauche
			tempx = xpos-1;
			tempy = ypos;
			if(tempx>-1){
				//c'est un mur
				if(level.GetPixel(tempx,tempy).r==0){
					numwalls++;
				}
			}
			else{
				numwalls++;
			}	
			//gcentre
			tempx = xpos;
			tempy = ypos;
			//c'est un mur
			if(level.GetPixel(tempx,tempy).r==0){
				numwalls++;
			}
			//droit
			tempx = xpos+1;
			tempy = ypos;
			if(tempx<=level.width){
				//c'est un mur
				if(level.GetPixel(tempx,tempy).r==0){
					numwalls++;
				}
			}
			else{
				numwalls++;
			}	
			//bas gauche
			tempx = xpos-1;
			tempy = ypos+1;
			if((tempx>-1)&&(tempy<=level.height)){
				//c'est un mur
				if(level.GetPixel(tempx,tempy).r==0){
					numwalls++;
				}
			}
			else{
				numwalls++;
			}	
			//bas 
			tempx = xpos;
			tempy = ypos+1;
			if(tempy<=level.height){
				//c'est un mur
				if(level.GetPixel(tempx,tempy).r==0){
					numwalls++;
				}
			}
			else{
				numwalls++;
			}	
			//bas droit
			tempx = xpos+1;
			tempy = ypos+1;
			if((tempx<=level.width)&&(tempy<=level.height)){
				//c'est un mur
				if(level.GetPixel(tempx,tempy).r==0){
					numwalls++;
				}
			}
			else{
				numwalls++;
			}	
			if(numwalls>4){
				level.SetPixel(xpos,ypos,new Color(0,0,0,1));
			}	
			else{
				level.SetPixel(xpos,ypos,new Color(1,1,1,1));
			}									
								
		}
		
	}
	level.Apply();

}

