#pragma strict
private var rot : Quaternion;
function Start () {
rot = transform.rotation;
}

function Update () {
transform.parent.rotation = Quaternion.identity;
this.transform.rotation = rot;
}