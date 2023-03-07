using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Pun;
using personnage_class.Personage;


public class MovePlayer : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;

    public float speed = 600.0f;
    public float turnSpeed = 400.0f;
    private Vector3 moveDirection = Vector3.zero;
    public float gravity = 20.0f;
    public Personnage test;
    private PhotonView _photonView;
		
		
    void Start () {
        controller = GetComponent <CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();
        test = new Soldat("test");
        _photonView = GetComponent<PhotonView>();
			
    }

    void Update (){
        if (test.canMove && _photonView.IsMine  )
        {
            if (Input.GetKey ("w")) {
                anim.SetInteger ("AnimationPar", 1);
            }  else {
                anim.SetInteger ("AnimationPar", 0);
            }

            if(controller.isGrounded){
                moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
            }

            test.Took(new Kit_Heal(5,0));
            test.Took(new Kit_Heal(5,0));
            Console.WriteLine(test.Get_Inventory()[0]);
            test.Remove_Life(10);
            test.Use(0);
            test.Trow(1);
            Console.WriteLine(test.Get_Inventory()[0]);
				
            float turn = Input.GetAxis("Horizontal");
            transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
				
            controller.Move(moveDirection * Time.deltaTime);
            moveDirection.y -= gravity * Time.deltaTime;
        }
			
    }
}