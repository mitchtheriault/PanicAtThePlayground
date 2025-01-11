using UnityEngine;
using UnityEngine.SceneManagement;

public class PunchCollider : MonoBehaviour
{
    public Collider punchCollider;

    void Start()
    {
        punchCollider.enabled = false; // Ensure collider starts disabled
    }

    public void EnableCollider()
    {
        punchCollider.enabled = true;
    }

    public void DisableCollider()
    {
        punchCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NerdKid"))
        {
            print("hit");
            // Call the Nerd's hit reaction
            other.GetComponent<NerdKidController>().ReactToHit();
            // Award points or other feedback
            GameManager.instance.AddScore(100);

            

        }
    }



}