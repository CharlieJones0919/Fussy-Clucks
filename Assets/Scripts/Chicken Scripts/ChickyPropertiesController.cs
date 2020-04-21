using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script specifies properties that all the chicken objects have. It's attached to the SceneMaintenanceObject so it won't destroyed on load, as it doesn't need to change between levels.
public class ChickyPropertiesController : MonoBehaviour
{
    //This structure contains values which are consistant for all of the chickens. They're all specified in this script to make small game design edits.
    public struct ConstProps
    {
        public float timeBetweenCoinDrop;   //The frequency at which chickens should drop coins.
        public int maxCoinDropAmount;       //The maximum amount of gold a chicky type chicken can drop.
        public float seedFeedAmount;

        public float eggTouchRadius;        //The distance from the egg's position (centre), the player can touch on the screen to pick said egg up. This is smaller to avoid multiple eggs being picked up.
        public float chickyTouchRadius;     //The distance from the chickens's position (centre), the player can touch on the screen to pick said chicken up.
        public float chickyColliderRadius;  //Radius to set the chicken's sphere collider object to.
        public Vector3 eggColliderCentre;//Centre to set the egg's sphere collider object to.
        public Vector3 chickyColliderCentre;//Centre to set the chicken's sphere collider object to.

        public float timeToStayEgg;         //Amount of time to spend as an egg;

        public float eggColdLimit;          //If the egg's temperature is under this value, its "cold" boolean property will be set as true.
        public float eggHotLimit;           //If the egg's temperature is over this value, its "hot" boolean property will be set as true.
        public float eggFreezeDeathTemp;    //If the egg's temperature is under this value its it'll die (be deactivated).
        public float eggOverheatDeathTemp;  //If the egg's temperature is over this value its it'll die (be deactivated).

        public float chuckColdLimit;        //If the chicken's temperature is under this value, its "cold" boolean property will be set as true.
        public float chuckHotLimit;         //If the chicken's temperature is over this value, its "hot" boolean property will be set as true.
        public float chuckFreezeDeathTemp;  //If the egg's temperature is under this value its it'll die (be deactivated).
        public float chuckOverheatDeathTemp;//If the egg's temperature is over this value its it'll die (be deactivated).

        public float speed;                //Wandering speed.
        public float maxVel;               //Maximum velocity while wandering.

        public float wanderTimeLimit;      //Number of seconds before chicken changes direction when wandering.
        public float fenceKnockback;       //Force the chicken is knocked back from the fence.

        public float pickUpHeight;         //Position of the chicken on the Y-axis when picked up.
        public float fingerFollowSpeed;    //Speed at which the chicken is moved by force towards the touch position.
        public float thrownTime;           //Seconds which should pass after the player unclicks or stops touching the chicken. 
    };

    //This structure contains properties related to the time the object spends in its EggState.
    public struct EggProps
    {
        public bool isEgg;                  //Denotes if the object is currently an egg or not.     
        public float timeUntilHatch;        //Amount of time the object has spent as an egg since it was activated from the pool.
    }

    //This structure contains variables shared by all the chickens which will be changed during runtime and reset to a default state upon activation.
    public struct ChickyVariableProps
    {
        public string name;                 //Optional name of the chicken as set by the text input field in the UI.
        public string type;                 //The "type" this object is. (Chicky or Roosir). This is included to present the concept of implementing other types. Roosirs cost more, has different models, and produce more coin for demonstration.
        public Vector2 randomDir;            //The random direction on the X and Z axis that the chicken wanders.
        public float wanderTimer;            //Amount of time the chicken has spent wandering since the last direction change.
        public bool isBeingHeld;             //Has chicken been clicked/touched. 
        public float timeSinceCoinDrop;      //How much time has passed since the chicken last dropped gold.

        //The chicken's health property values/levels.
        public float temp;
        public float hunger;
        public float attention;
        public float rest;

        //Boolean values dependant on the health property values. (Implact the amount of gold the chicken drops).
        public bool inNest;
        public bool inHutch;
        public bool cold;
        public bool hot;
        public bool hungry;
        public bool lonely;
        public bool sleepy;
    };

    //The base instance of the constant properties which all of the chicken's use.
    private ConstProps controllerProps;

    //Ran before level load to define the constant values shared between all the chickens.
    private void Awake()
    {
        controllerProps.timeBetweenCoinDrop = 5.0f;
        controllerProps.maxCoinDropAmount = 5;
        controllerProps.seedFeedAmount = 30; //How much of a chicken's hunger a seed pile restores.

        controllerProps.eggTouchRadius = 0.65f;
        controllerProps.chickyTouchRadius = 1.75f;
        controllerProps.chickyColliderRadius = 1.5f;
        controllerProps.eggColliderCentre = new Vector3(0.0f, 1.0f, 0.0f);
        controllerProps.chickyColliderCentre = new Vector3(0.0f, -0.15f, 0.0f);

        controllerProps.timeToStayEgg = 15.0f;

        controllerProps.eggColdLimit = 23.0f;
        controllerProps.eggHotLimit = 40.0f;
        controllerProps.chuckColdLimit = 8.0f;
        controllerProps.chuckHotLimit = 30.0f;

        controllerProps.eggFreezeDeathTemp = 16.0f;
        controllerProps.eggOverheatDeathTemp = 49.0f;
        controllerProps.chuckFreezeDeathTemp = 0.0f;
        controllerProps.chuckOverheatDeathTemp = 45.0f;

        controllerProps.speed = 25.0f;
        controllerProps.maxVel = 10.0f;

        controllerProps.wanderTimeLimit = 2.5f;
        controllerProps.fenceKnockback = 5.0f;

        controllerProps.pickUpHeight = 2.5f;
        controllerProps.fingerFollowSpeed = 450.0f;
        controllerProps.thrownTime = 1.0f;
    }

    //Used by the Chuck script to get a reference of the constant chicken variable values.
    public ConstProps GetConstValues()
    {
        return controllerProps;
    }
}
