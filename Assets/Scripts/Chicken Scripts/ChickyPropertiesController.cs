using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickyPropertiesController : MonoBehaviour
{
    public struct BaseProperties
    {
        public float speed;                //Wandering speed.
        public float maxVel;               //Maximum velocity while wandering.

        public float wanderTimeLimit;      //Number of seconds before chicken changes direction when wandering.
        public float fenceKnockback;       //Force the chicken is knocked back from the fence.

        public float pickUpHeight;         //Position of the chicken on the Y-axis when picked up.
        public float fingerFollowSpeed;    //Speed at which the chicken is moved by force towards the touch position.
        public float thrownTime;           //Seconds which should pass after the player unclicks or stops touching the chicken. 
    };
    private BaseProperties chickyBaseProps;

    private void Start()
    {
        SetBaseProperties();
    }

    private void SetBaseProperties()
    {
        chickyBaseProps.speed = 8.0f;
        chickyBaseProps.maxVel = 20.0f;

        chickyBaseProps.wanderTimeLimit = 2.5f;
        chickyBaseProps.fenceKnockback = 5.0f;

        chickyBaseProps.pickUpHeight = 2.0f;
        chickyBaseProps.fingerFollowSpeed = 200.0f;
        chickyBaseProps.thrownTime = 5.0f;
    }

    public BaseProperties GetBaseProperties()
    {
        return chickyBaseProps;
    }
}
