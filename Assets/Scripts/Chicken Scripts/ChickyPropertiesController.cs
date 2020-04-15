using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickyPropertiesController : MonoBehaviour
{
    public struct ConstProps
    {
        public Vector3 eggColliderCentre;
        public float eggColliderRadius;
        public Vector3 chickyColliderCentre;
        public float chickyColliderRadius;

        public float timeToStayEgg;        //Amount of time to spend as an egg;

        public float eggColdLimit;
        public float chuckColdLimit;
        public float eggHotLimit;
        public float chuckHotLimit;

        public float eggFreezeDeathTemp;
        public float chuckFreezeDeathTemp;
        public float eggOverheatDeathTemp;
        public float chuckOverheatDeathTemp;

        public float speed;                //Wandering speed.
        public float maxVel;               //Maximum velocity while wandering.

        public float wanderTimeLimit;      //Number of seconds before chicken changes direction when wandering.
        public float fenceKnockback;       //Force the chicken is knocked back from the fence.

        public float pickUpHeight;         //Position of the chicken on the Y-axis when picked up.
        public float fingerFollowSpeed;    //Speed at which the chicken is moved by force towards the touch position.
        public float thrownTime;           //Seconds which should pass after the player unclicks or stops touching the chicken. 
    };
    private ConstProps controllerProps;

    public struct EggProps
    {
        public bool isEgg;
        public float timeUntilHatch;
        public uint health;
        public bool inNest;
    }

    public struct ChickyVariableProps
    {
        public string name;
        public string type;
        public uint age;
        public float temp;
        public uint hunger;

        public Vector2 randomDir;       //The random direction on the X and Z axis that the chicken wanders.
        public float wanderTimer;  //Amount of time the chicken has spent wandering since the last direction change.
        public float thrownTimer;              //How long has passed in seconds since the player stopped touching the chicken.
        public bool isBeingHeld;        //Has chicken been clicked/touched. (Global so it can remain true until touch ends).
    };

    private void Start()
    {
        controllerProps.eggColliderCentre = new Vector3(0.0f, 0.1f, 0.0f);
        controllerProps.chickyColliderCentre = new Vector3(0.0f, -0.15f, 0.0f);
        controllerProps.eggColliderRadius = 0.45f;
        controllerProps.chickyColliderRadius = 1.5f;

        controllerProps.timeToStayEgg = 180.0f;

        controllerProps.eggColdLimit = 23.0f;
        controllerProps.chuckColdLimit = 8.0f;
        controllerProps.eggHotLimit = 40.0f;
        controllerProps.chuckHotLimit = 30.0f;

        controllerProps.eggFreezeDeathTemp = 16.0f;
        controllerProps.chuckFreezeDeathTemp = 0.0f;
        controllerProps.eggOverheatDeathTemp = 49.0f;
        controllerProps.chuckOverheatDeathTemp = 45.0f;

        controllerProps.speed = 10.0f;
        controllerProps.maxVel = 20.0f;

        controllerProps.wanderTimeLimit = 2.5f;
        controllerProps.fenceKnockback = 5.0f;

        controllerProps.pickUpHeight = 2.0f;
        controllerProps.fingerFollowSpeed = 200.0f;
        controllerProps.thrownTime = 1.0f;
    }

    public ConstProps GetConstProps()
    {
        return controllerProps;
    }
}
