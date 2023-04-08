using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorModelAnimation : MonoBehaviour
{

    private float movementDelta = 0;
    [SerializeField] private float movementSpeed = 0.2f;

    [SerializeField] private float defaultHeight = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movementDelta += Time.deltaTime * movementSpeed;

        if (movementDelta > defaultHeight)
        {
            transform.position = new Vector3(transform.position.x, defaultHeight, transform.position.z);
            movementDelta = 0;
        }

        transform.Translate(new Vector3(0, Time.deltaTime * -movementSpeed, 0));
    }
}
