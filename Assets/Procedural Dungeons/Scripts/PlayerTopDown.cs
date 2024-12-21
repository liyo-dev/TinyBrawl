using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTopDown : MonoBehaviour
{
    private Rigidbody componentRigidbody;

    public int TurnSpeed = 2;

    private void Start()
    {
        componentRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            componentRigidbody.AddForce(Vector3.forward* TurnSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            componentRigidbody.AddForce(Vector3.back * TurnSpeed);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            componentRigidbody.AddForce(Vector3.left * TurnSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            componentRigidbody.AddForce(Vector3.right * TurnSpeed);
        }

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene("Demo Dungeon");
    }
}
