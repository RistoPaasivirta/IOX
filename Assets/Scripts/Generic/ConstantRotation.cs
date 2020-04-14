using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField] private Vector3 Speed = new Vector3(0,0,360);

    private void Update() => 
        transform.Rotate(Speed * Time.deltaTime);
}
