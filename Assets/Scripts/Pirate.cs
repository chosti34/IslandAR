using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirate : MonoBehaviour
{
	private Animation m_animation;
	private int m_cellIndex;

	float speed = 7;
	float gravity = 15;

	CharacterController m_chctrl;
	Vector3 moveDirection = Vector3.zero;

	void Start()
	{
		m_chctrl = transform.GetComponent<CharacterController>();
	}

	void Update()
	{
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
				// m_animation.Play("Walk");
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
					m_chctrl.Move(relative * Time.deltaTime * speed * 2);
				}
				else
				{
					m_chctrl.Move(relative * Time.deltaTime * speed);
				}
			}
		}
	}

	public void MoveTo(Vector3 pos)
	{
		m_chctrl.transform.position = pos;
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
