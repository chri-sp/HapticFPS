/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>15th June 2017</date>
   <summary>Applues a force to a rigidbod that triggered the collider of the jump pad</summary>*/

using UnityEngine;

namespace RadicalForge.Gameplay
{

    [RequireComponent(typeof(Collider))]
    public class JumpPad : MonoBehaviour
    {

        [SerializeField] private float jumpMultiplyer;

        void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddForce(other.transform.up * jumpMultiplyer);
            }
        }

    
    }


}