var changeDelay = 1.0;
var changeSpeed = 3.0;

private var goalColor : Color;
private var myRenderer : Renderer;

function Start () {
    myRenderer = GetComponent.<Renderer>();
	while (true) {
		goalColor = Color(Random.value, Random.value, Random.value, 1.0);
		yield WaitForSeconds(changeDelay);
	}
}

function Update () {
	myRenderer.material.color = Colorx.Slerp(myRenderer.material.color, goalColor, changeSpeed * Time.deltaTime);
}
