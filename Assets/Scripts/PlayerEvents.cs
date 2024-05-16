using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    PlayerMotion playerMotion;
    private void Awake()
    {
        playerMotion = GetComponentInParent<PlayerMotion>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Land()
    {
        playerMotion.FallEnd();
    }
}
