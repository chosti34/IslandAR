using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpCharacterMovement : MonoBehaviour
{
	private Animation m_animation;

	float speed = 7;
	float gravity = 15;
	CharacterController controller;
	Vector3 moveDirection = Vector3.zero;

	// Use this for initialization
	void Start()
	{
		// m_animation = GetComponent<Animation>();
		controller = transform.GetComponent<CharacterController>();
		Debug.Log(controller.minMoveDistance);
	}

	// Update is called once per frame
	void Update()
	{
		//APPLY GRAVITY
		if (moveDirection.y > gravity * -1)
		{
			moveDirection.y -= gravity * Time.deltaTime;
		}
		controller.Move(moveDirection * Time.deltaTime);
		var left = transform.TransformDirection(Vector3.left);

		if (controller.isGrounded)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				moveDirection.y = speed;
			}
			else if (Input.GetKey("w"))
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					controller.SimpleMove(transform.forward * speed * 2);
				}
				else
				{
					controller.SimpleMove(transform.forward * speed);
				}
				transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
				// m_animation.Play("Walk");
			}
			else if (Input.GetKey("s"))
			{
				transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
				//m_animation.Play("Walk");
				if (Input.GetKey(KeyCode.LeftShift))
				{
					controller.SimpleMove(transform.forward * speed * 2);
				}
				else
				{
					controller.SimpleMove(transform.forward * speed);
				}
			}
			else if (Input.GetKey("a"))
			{
				transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
				//m_animation.Play("Walk");
				if (Input.GetKey(KeyCode.LeftShift))
				{
					controller.SimpleMove(transform.forward * speed * 2);
				}
				else
				{
					controller.SimpleMove(transform.forward * speed);
				}
			}
			else if (Input.GetKey("d"))
			{
				transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				//m_animation.Play("Walk");
				if (Input.GetKey(KeyCode.LeftShift))
				{
					controller.SimpleMove(transform.forward * speed * 2);
				}
				else
				{
					controller.SimpleMove(transform.forward * speed);
				}
			}
			else
			{
				// m_animation.Play("Idle");
			}
		}
		else
		{
			if (Input.GetKey("w"))
			{
				Vector3 relative = new Vector3();
				relative = transform.TransformDirection(0, 0, 1);
				if (Input.GetKey(KeyCode.LeftShift))
				{
					controller.Move(relative * Time.deltaTime * speed * 2);
				}
				else
				{
					controller.Move(relative * Time.deltaTime * speed);
				}
			}
		}
	}
}
