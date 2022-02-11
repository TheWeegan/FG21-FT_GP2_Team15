using System;
using System.ComponentModel;
using UnityEngine;

namespace PlayerScripts
{
	public class PlayerEatingMode : MonoBehaviour
	{
		public ParticleSystem suctionEffect;
		private ParticleSystem spawnedEffect;
		private Transform mouth; 
		private Rigidbody door;
		private LayerMask ignoreRaycast;
		private bool blockSuction;
		private bool gameStarted;
		[NonSerialized] public static bool IsEating;

		[Header("Suction settings")]
		[Range(0.5f, 10f)] public float pullDistance; //the range of the suction.
		[Range(0.5f, 5f)] public float pullRadius;  //how wide the radius is.
		[Description("How strong the suction power is")]
		[Range(0.1f, 1f)]public float pullObjectForce; //how strong the suction is.
		void Start()
		{
			GameManager.instance.deActivate += DeActivate; 
			GameManager.instance.suctionBlocket += BlockSuctionEvent; 
			GameManager.instance.shootableShot += UnblockSuction;
			GameManager.instance.startGame += DelayedStart;
			
			door = GameObject.Find("Door")?.GetComponent<Rigidbody>();
			mouth = GameObject.Find("Hole")?.GetComponent<Transform>();
			ignoreRaycast = LayerMask.GetMask("Ignore Raycast");
			if (mouth is { }) spawnedEffect = Instantiate(suctionEffect, mouth.transform); 
		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0) && gameStarted)
			{
				if (!blockSuction)
				{
					IsEating = true;
					spawnedEffect.Play();
					AudioManager.Instance.PlaySuckingSound(gameObject);
				}
				if(blockSuction)
				{
					GameManager.instance.FailedSuction();
					StopSuctionEffects();
				}
			}
			if(Input.GetKeyUp(KeyCode.Mouse0))
			{
				IsEating = false;
				StopSuctionEffects();
			}
		}
		private void FixedUpdate()
		{
			if (IsEating && !blockSuction)
			{
				PullObjects();
			}
		}

		private void PullObjects()
		{
			spawnedEffect.transform.position = mouth.transform.position;
			
			var cols = Physics.OverlapSphere(mouth.position + mouth.forward * pullDistance/2, pullRadius);
            
			foreach (var c in cols) {
				if (c.TryGetComponent<Rigidbody> (out var r) && !Physics.Linecast(mouth.transform.position, r.transform.position, 
					ignoreRaycast)) //added linecast to check if object is behind masked walls.
				{
					if (!r.CompareTag("IgnoreForce"))
					{
						r.AddForce((mouth.position - r.position).normalized * pullObjectForce, ForceMode.Impulse);
					}
				}
			}
			if (door != null)
			{
				door.AddForce(transform.right -(transform.forward +(transform.right*-1)).normalized * pullObjectForce *10, ForceMode.Force);
			} 
        }
		private void StopSuctionEffects()
		{
			spawnedEffect.Stop();
			AudioManager.Instance.StopPlayingSuckingSound();
		}

		private void BlockSuctionEvent() //mag full, cant suck.
		{
			blockSuction = true;
			StopSuctionEffects();
		}
		private void UnblockSuction() //fire resets block.
		{
			blockSuction = false;
		}

		private void DelayedStart() //temp fix for preventing "eating" in new menu.
		{
			gameStarted = true;
		}

		private void DeActivate() //menu triggers, should 
		{
			gameStarted = false;
			StopSuctionEffects();
		}
	}
}

