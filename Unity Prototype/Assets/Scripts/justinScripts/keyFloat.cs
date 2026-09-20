using UnityEngine;

public class keyFloat : MonoBehaviour
{
    [SerializeField] float spinSpeed = 50f;
    [SerializeField] float bobHeight = 0.15f;
    [SerializeField] float bobSpeed = 2f;

    Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;    
    }

    // Update is called once per frame
    void Update()
    {
        //spinning
        transform.Rotate(0f,0f , spinSpeed * Time.deltaTime);

        //floating
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight; 
        transform.position = startPos + Vector3.up * offset;
    }
}
