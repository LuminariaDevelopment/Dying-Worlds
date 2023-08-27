using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] Transform player;
    [SerializeField] Transform orientation;
    float rotationx;
    float rotationy;

    void Update()
    {
        float mousex = Input.GetAxisRaw("Mouse X") * Time.deltaTime * x;
        float mousey = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * y;

        rotationx -= mousey;
        rotationy += mousex;
        rotationx = Mathf.Clamp(rotationx, -90, 90);
        transform.rotation = Quaternion.Euler(new Vector3(rotationx, rotationy, 0f));
        player.rotation = Quaternion.Euler(new Vector3(0f, rotationy, 0f));
        orientation.rotation = Quaternion.Euler(new Vector3(0f, rotationy, 0f));
    }
}
