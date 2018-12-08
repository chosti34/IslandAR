using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pirate : NetworkBehaviour
{
	private Animator m_animator;
	private int m_cellIndex;

	float speed = 5;
	float gravity = 15;

	CharacterController m_chctrl;
	Vector3 moveDirection = Vector3.zero;

	Vector3 m_destination;
	bool m_moving;

	System.Action m_action;

	public void SetCallback(System.Action action)
	{
		m_action = action;
	}

	void Start()
	{
		m_chctrl = transform.GetComponent<CharacterController>();
		m_animator = GetComponent<Animator>();
		m_moving = false;
	}

	void Update()
	{
		if (isLocalPlayer)
		{
			//return;
		}

		//APPLY GRAVITY
		if (moveDirection.y > gravity * -1)
		{
			moveDirection.y -= gravity * Time.deltaTime;
		}

		m_chctrl.Move(moveDirection * Time.deltaTime);
		var left = transform.TransformDirection(Vector3.left);

		if (m_chctrl.isGrounded)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				moveDirection.y = speed;
				//m_animator.SetTrigger("RunTrigger");
			}
			else if (Input.GetKey("w"))
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					m_chctrl.SimpleMove(transform.forward * speed * 2);
				}
				else
				{
					m_chctrl.SimpleMove(transform.forward * speed);
				}
				transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
			}
			else if (Input.GetKey("s"))
			{
				transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
				//m_animation.Play("Walk");
				if (Input.GetKey(KeyCode.LeftShift))
				{
					m_chctrl.SimpleMove(transform.forward * speed * 2);
				}
				else
				{
					m_chctrl.SimpleMove(transform.forward * speed);
				}
				//m_animator.SetTrigger("RunTrigger");
			}
			else if (Input.GetKey("a"))
			{
				transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
				//m_animation.Play("Walk");
				if (Input.GetKey(KeyCode.LeftShift))
				{
					m_chctrl.SimpleMove(transform.forward * speed * 2);
				}
				else
				{
					m_chctrl.SimpleMove(transform.forward * speed);
				}
				//m_animator.SetTrigger("RunTrigger");
			}
			else if (Input.GetKey("d"))
			{
				transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				//m_animation.Play("Walk");
				if (Input.GetKey(KeyCode.LeftShift))
				{
					m_chctrl.SimpleMove(transform.forward * speed * 2);
				}
				else
				{
					m_chctrl.SimpleMove(transform.forward * speed);
				}
			//m_animator.SetTrigger("RunTrigger");
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
					m_chctrl.Move(relative * Time.deltaTime * speed * 2);
				}
				else
				{
					m_chctrl.Move(relative * Time.deltaTime * speed);
				}
			}
		}

		if (m_moving)
		{
			transform.position = Vector3.Lerp(transform.position, m_destination, speed * Time.deltaTime);
			Debug.Log(Vector2.Distance(transform.position, m_destination));
			if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(m_destination.x, m_destination.z)) <= 0.35f)
			{
				m_moving = false;
				m_animator.SetTrigger("Trigger");
				m_action();
			}
		}
	}

	public void MoveTo(Vector3 pos)
	{
		// m_chctrl.transform.position = pos;
		m_animator.SetTrigger("Trigger");
		m_destination = new Vector3(pos.x, transform.position.y, pos.z);
		m_moving = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Chest")
		{
			Destroy(other.gameObject);
		}
	}

	public void SetCellIndex(int index)
	{
		m_cellIndex = index;
	}

	public int GetCellIndex()
	{
		return m_cellIndex;
	}
}
