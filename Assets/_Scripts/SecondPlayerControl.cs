using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SecondPlayerControl : MonoBehaviour
{
    [SerializeField] private GameObject newPlayerPrefab; // The new Player Prefab to use.
    private PlayerInputManager _inputManager;

    // Start is called before the first frame update
    void Start()
    {
        _inputManager = GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindWithTag("Player1")) //Checking for a game object with the tag
        {
            PlayerInputManager.instance.playerPrefab = newPlayerPrefab; //If yes, changes the player prefab field to your selected prefab
        }
    }
}
