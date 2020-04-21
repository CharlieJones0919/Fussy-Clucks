using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;  //Specifiying that the "Random" function call should be from Unity.Engine and not System.

//An instance of this script is attached to each Chicky prefab object and controls their behaviour and property values.
public class Chuck : MonoBehaviour
{
    public LevelController levelController;                             //Reference to the LevelController script to get universal game values and functions.
    public ChickyPropertiesController.ConstProps constPropsRef;         //A copy of ChickyPropertiesController's specified constant variable values.
    public ChickyPropertiesController.EggProps eggProps;                //For holding this object instance's egg properties data.
    public ChickyPropertiesController.ChickyVariableProps chickyProps;  //For holding this object instance's chicken properties data.

    private GameObject currentModelClone;                               //Stores a reference to the clone of a model prefab which is instantiated when this object is activated depending on the type of chuck activated.
    public GameObject chickyModel;                                      //Stores a reference to the Chicky model this object's model should be replaced with depending on its specified type when activated.
    public GameObject roosirModel;                                      //Stores a reference to the Roosir model this object's model should be replaced with depending on its specified type when activated.

    public Rigidbody rigidBody;             //This chicken's rigidbody component.
    public SphereCollider chuckCollider;    //This chicken's collider component.
    public float currentTouchRadius;        //The current radius from the the egg/chicken the player can touch on the screen to pick it up. It's smaller when it's an egg to try prevent picking up multiple eggs. 

    public Vector3 seedPosition;            //Position to go toward to get seed, as retrieved from the list of active positions in LevelController.
    public GameObject activeCoin;           //The current coin from the list of coins in the coin object pool being operated on.
    public int coinDropAssignedValue;       //How much to make the coin pile worth when it's picked up based on the chicken's current stats.

    //Ran before runtime to initialise variables and to initialise the chicken's FSM.
    private void Awake()
    {
        InitializeStateMachine(); //Runs the InitalizeStateMachine() function as defined below.
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();   
        constPropsRef = levelController.chickyPropController.GetConstValues();  //Setting the copy of controlProps to the values of the original.
    }

    //Adds all the behaviour states to the states dictionary and sets these to the state machine.
    private void InitializeStateMachine()
    {
        Dictionary<Type, ChuckBaseState> states = new Dictionary<Type, ChuckBaseState>(); //Dictionary to hold the states used in the FSM.
        states.Add(typeof(PickedUpState), new PickedUpState(this));     //Adds PickedUpState to states. (Starting State). Has the chicken follow the touch position.
        states.Add(typeof(EggState), new EggState(this));               //Adds EggState to states. Checks if the object is still an egg, and calls the "Hatch" when the timer for such is done.
        states.Add(typeof(WanderState), new WanderState(this));         //Adds WanderState to states. Has the chicken drop coins, go to seeds, and wander around the map. 
        GetComponent<ChuckStateMachine>().SetStates(states);            //Passes the added states into the state machine's setting function.
    }

    //Everytime the object is activated from its object pool, set its variable values to these default values.
    public void OnEnable()
    {
        currentTouchRadius = constPropsRef.eggTouchRadius;          
        chuckCollider.center = constPropsRef.eggColliderCentre;
        chuckCollider.radius = constPropsRef.chickyColliderRadius;

        //Egg's Initial Properties
        eggProps.isEgg = true;
        eggProps.timeUntilHatch = 0.0f;

        //Chicky's Initial Variable Properties when Activated
        chickyProps.name = null;
        chickyProps.randomDir = UnityEngine.Random.insideUnitCircle; //Sets the Vector2 its initial 2 random floats between -1 and 1.
        chickyProps.wanderTimer = 0.0f;
        chickyProps.isBeingHeld = false;
        chickyProps.timeSinceCoinDrop = 0.0f;

        chickyProps.temp = 25.0f;
        chickyProps.hunger = 100.0f;
        chickyProps.attention = 100.0f;
        chickyProps.rest = 100.0f;

        chickyProps.inNest = false;
        chickyProps.cold = false;
        chickyProps.hot = false;
        chickyProps.hungry = false;
        chickyProps.lonely = false;
        chickyProps.sleepy = false;
    }

    //Called when the object's collider collides with another non-trigger collider.
    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Fence")
        {
            Vector2 pushDir = collider.contacts[0].point - transform.position; //Get position of contact with the fence.
            pushDir = -pushDir.normalized; //Invert and normalise the direction.

            //If the chicken is at the map limit (or wanders into an occupied nest), add an impulse force to it in the opposite direction then change the random direction.
            rigidBody.AddForce(pushDir.x * constPropsRef.fenceKnockback, 0.0f, pushDir.y * constPropsRef.fenceKnockback, ForceMode.Impulse);
            chickyProps.wanderTimer = constPropsRef.wanderTimeLimit;
        }

        if (collider.gameObject.tag == "Chuck") //Increase the chicken's attention when it collides with another chicken;
        {
            if (chickyProps.attention < 100)
            {
                chickyProps.attention++;
            }
        }
    }

    //Called when the object's collider collides with a trigger collider.
    private void OnTriggerEnter(Collider collider)
    {
        //When the chicken collides with a seed, if it isn't an egg, and has less hunger than the amount the seed will recover, deactivate the seed object from the object pool and increase hunger. Then remove the seedPosition from the list.
        if (collider.gameObject.tag == "Seed")  
        {
            if ((!eggProps.isEgg) && ((chickyProps.hunger < (100 - constPropsRef.seedFeedAmount))))
            {
                levelController.DeactivateObject(collider.gameObject, levelController.levelSeed);
                chickyProps.hunger += constPropsRef.seedFeedAmount;
                levelController.seedPositions.Remove(seedPosition);
            }
        }

        //If the chicken is in the hutch (and isn't an egg) or in a nest, reduce velocity to 0, freeze the object's rotation so it's not rolling about, and make the appropriate boolean true.
        if (collider.gameObject.tag == "InsideHutch")
        {
            if (!eggProps.isEgg)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
                chickyProps.inHutch = true;
            }
        }
        else if (collider.gameObject.tag == "Nest")
        {
            rigidBody.velocity = Vector3.zero;
            transform.position = new Vector3(collider.transform.position.x, 0.5f, collider.transform.position.z);
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            chickyProps.inNest = true;
        }
    }

    //When the chicken leaves the hutch or collider, unfreeze the rigidbody's rotation and reset the chickyProps property value to false.
    private void OnTriggerExit(Collider collider)
    { 
        if (collider.gameObject.tag == "InsideHutch")
        {
            if (!eggProps.isEgg)
            {
                rigidBody.constraints = RigidbodyConstraints.None;
                chickyProps.inHutch = false;
            }
        }
        else if (collider.gameObject.tag == "Nest")
        {
            rigidBody.constraints = RigidbodyConstraints.None;
            chickyProps.inNest = false;
        }
    }

    //Called when the egg buttons are pressed and when in the Hatch() function. Sets the chicken's type and instantiates a copy of the specified model prefab  which is made a child to this object.
    public void SetType(string type, GameObject model)
    {
        //Deletes the previous model object (egg model) by making a reference to it and deleting the reference.
        GameObject oldModelClone = currentModelClone;
        Destroy(oldModelClone);

        currentModelClone = Instantiate(model, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity); //Instantiates a clone of the model specified in the parameter.
        currentModelClone.transform.parent = transform;     //Sets the new object as a child to this one.
        chickyProps.type = type;    //Sets the type of the object. (Chicky Egg, Roosir Egg, Chicky or Roosir).
    }

    //Function called on update during the WanderState.
    public void Wandering()
    {
        //If the time the chicken has spent wandering since last changing direction is more than or equal to the time the chicken should spend spend wandering in one direction, set the random direction to a new Vector2 and reset the wander time.
        if (chickyProps.wanderTimer >= constPropsRef.wanderTimeLimit)
        {
            chickyProps.randomDir = UnityEngine.Random.insideUnitCircle;
            chickyProps.wanderTimer = 0.0f;
        }

        //Move the chicken's rigidbody in the direction of the current random direction and increase the wander time passed variable.
        rigidBody.AddForce(chickyProps.randomDir.x * constPropsRef.speed, 0.0f, chickyProps.randomDir.y * constPropsRef.speed, ForceMode.Force);
        //Clamp the rigidbody's velocity to the specified maximum.
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, constPropsRef.maxVel);

        chickyProps.wanderTimer += Time.deltaTime;  //Increment wander time.
    }

    //Called by the PickedUpState's update while BeenPickedUp() returns as true. Moves the chicken towards the position of the player's finger on the screen.
    public void PickedUp()
    {
        //Increase the egg timer while the chicken is picked up.
        if (eggProps.isEgg) 
        {
            IsEggStill();
        }

        rigidBody.velocity = Vector3.zero;        //Sets the chicken's velocity back to 0 so the only movement is the force towards the finger.
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Gets the position of the mouse/finger on the screen as a world co-ordinate.
        touchPos.y = constPropsRef.pickUpHeight;  //Redefines the y-axis of the touch position as the screen hasn't got a depth.
        rigidBody.AddForce((touchPos - transform.position) * constPropsRef.fingerFollowSpeed); //Adds force to the chicken in the direction of the finger/mouse. (The difference between the touch position and its current position).
    }

    //Called every update while the chicken is in its PickedUpState. Checks if the mouse button or the player's finger is [still] down on the chicken. A.K.A. Is the chicken still being held. 
    public bool BeenPickedUp()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //A vector of the player's touching position from the screen position into the world position.

            //If the object isn't in a hutch, set the touch's position on the Y-axis to 1.0f so only one object will be picked up at once. It it is however, set the touch position as higher because they're raised in position while in the hutch.
            if (!chickyProps.inHutch) { touchPosition.y = 1.0f; }
            else { touchPosition.y = 3.0f; }
           
            //If the player's touch position is within the range on each axis of this chicken's position, plus the radius of the touchRadius.
            if (((touchPosition.x > (transform.position.x - currentTouchRadius))) && (touchPosition.x < (transform.position.x + currentTouchRadius))
            && ((touchPosition.y > (transform.position.y - 1.0f))) && (touchPosition.y < (transform.position.y + 1.0f))
            && ((touchPosition.z > (transform.position.z - currentTouchRadius))) && (touchPosition.z < (transform.position.z + currentTouchRadius)))
            {
                chickyProps.isBeingHeld = true;     //If the player is touching in the range of a chicken, set isBeingHeld as true.
                levelController.uiOutput.whichChicky = transform.gameObject;    //Set the whichChicky game object in the UIController script to this chicken object if it has been picked up. (This will make the UI output this chicken's property values).
            }
        }
        else if (Input.GetMouseButtonUp(0)) //If the player stops touching the screen, the chicken isn't being held.
        {
            chickyProps.isBeingHeld = false;
        }

        return chickyProps.isBeingHeld; //Return the result of if the chicken is being held.
    }

    //Increments timer from when the object is activated to check if it's still an egg or not.
    public bool IsEggStill()
    {
        //If the timer is less than the time the chicken should spend as an egg, increase the timer and return true to stay in this state.
        if (eggProps.timeUntilHatch < constPropsRef.timeToStayEgg)
        {
            eggProps.timeUntilHatch += Time.deltaTime;
            return true;
        }
        else    //If the timer is equal to or more than the time the chicken should spend in as an egg, reset the timer and return false to exit this state.
        {      
            eggProps.timeUntilHatch = 0;
            return false;
        }
    }

    //Called when the object is no longer an egg as checked by the IsEggStill() timer function.
    public void Hatch()
    {
        //Set the collider data to the chicky values.
        currentTouchRadius = constPropsRef.chickyTouchRadius;
        chuckCollider.center = constPropsRef.chickyColliderCentre;

        //Depending on what type (Roosir/Chicky) the object was defined as on activation, set the type to the adult form of that type with the corresponding model.
        switch (chickyProps.type)
        {
            case ("Chicky Egg"):
                SetType("Chicky", chickyModel);
                break;
            case ("Roosir Egg"):
                SetType("Roosir", roosirModel);
                break;
            default:
                Debug.Log("Unrecognised Chuck Type");
                break;
        }
    }

    //Calls all the functions which increment/decrement the values of the chicken's stats and check the conditions for if any of their boolean properties are true/false (e.g. cold, sleepy, etc). In these condition checks, some of them also check for if the values should deactivate the object.
    public void CheckStats()
    {
        if (eggProps.isEgg)
        {
            //Checks the object's temperature with the limit/death parameter values for when it's an egg.
            CheckTemperature(constPropsRef.eggColdLimit, constPropsRef.eggHotLimit, constPropsRef.eggFreezeDeathTemp, constPropsRef.eggOverheatDeathTemp);
        }
        else
        {
            //Checks the object's temperature with the limit/death parameter values for when it's an adult chicken. 
            CheckTemperature(constPropsRef.chuckColdLimit, constPropsRef.chuckHotLimit, constPropsRef.chuckFreezeDeathTemp, constPropsRef.chuckOverheatDeathTemp);      
            CheckHunger();      //Check and decrease the chicken's hunger.    
            CheckAttention();   //Check and decrease the chicken's attention.  
            CheckRest();        //Check and decrease the chicken's rest.  
        }

        //If this chicken is the game object the UI is currently outputting the values of, set all the textbox values to this chicken's properties.
        if (levelController.uiOutput.whichChicky == transform.gameObject)
        {
            levelController.uiOutput.SetNameOutput(chickyProps.name);  
            levelController.uiOutput.SetTextOutput(levelController.uiOutput.tempText, chickyProps.temp);
            levelController.uiOutput.SetTextOutput(levelController.uiOutput.hungryText, chickyProps.hunger);
            levelController.uiOutput.SetTextOutput(levelController.uiOutput.attentionText, chickyProps.attention);
            levelController.uiOutput.SetTextOutput(levelController.uiOutput.sleepText, chickyProps.rest);
        }
    }

    private void CheckTemperature(float coldLimit, float heatLimit, float coldDeath, float heatDeath)
    {
        if ((chickyProps.inNest) && (chickyProps.temp <= constPropsRef.eggOverheatDeathTemp))   //If the chicken is in a nest, increase the chicken's temperature.
        {
            chickyProps.temp += Time.deltaTime * 0.5f;
        }
        else if ((!chickyProps.inNest) && (chickyProps.temp > 0.0f))    //If the chicken isn't in the nest, decrease the chicken's temperature. It decreases 15% slower if the object isn't an egg anymore.
        {
            if (eggProps.isEgg)
            {
                chickyProps.temp -= Time.deltaTime ;
            }
            else
            {
                chickyProps.temp -= Time.deltaTime * 0.15f;
            }
        }

        //If the temperature is lower than the current cold limit, set cold as true and hot as false. Display this with the appropriate UI sprite if it's the active UI object. 
        //^ If the chicken's temperature is equal to or lower than the death limit value, deactivate the object from its list and displays a message in the tip box to explain why it died.
        if (chickyProps.temp <= coldLimit)
        {
            chickyProps.cold = true;
            chickyProps.hot = false;

            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.tempIcon, levelController.uiOutput.tempCold); }

            if (chickyProps.temp <= coldDeath)
            {
                KillChuck("A chicken died from the cold. (Put it in a nest next time).");
            }
        }
        else if (chickyProps.temp >= heatLimit)     //The same as the last IF statement, but for the heat limits.
        {
            chickyProps.cold = false;
            chickyProps.hot = true;

            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.tempIcon, levelController.uiOutput.tempHot); }

            if (chickyProps.temp >= heatDeath)
            {
                KillChuck("A chicken died from overheating. (Don't leave your chickens in the nest so long next time).");
            }
        }
        else    //If the chicken's temperature doesn't qualify it as too hot or cold, set the booleans to false and set the temperature UI image to its neutral sprite.
        {
            chickyProps.cold = false;
            chickyProps.hot = false;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.tempIcon, levelController.uiOutput.temp); }
        }
    }

    private void CheckHunger()
    {
        //Decrease the chicken's hunger.
        if (chickyProps.hunger > 0.0f)
        {
            chickyProps.hunger -= Time.deltaTime;
        }

        //If the hunger value is less than 50% the hungry boolean is true, and if at 0 it's dead (deactivated). Change UI icon sprites accordingly.
        if (chickyProps.hunger < 50.0f)
        {
            chickyProps.hungry = true;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.hungryIcon, levelController.uiOutput.hungerLow); }

            if (chickyProps.hunger <= 0)
            {
                KillChuck("A chicken died of hunger.");
            }
        }
        else
        {
            chickyProps.hungry = false;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.hungryIcon, levelController.uiOutput.hunger); }
        }
    }

    private void CheckAttention()
    {
        //If the chicken is being held, increase the attention value by time * 2. If the chicken isn't being held, decrease it. (Attention is also increased by colliding with another chicken).
        if ((chickyProps.isBeingHeld) && (chickyProps.attention < 100.0f))  
        {
            chickyProps.attention += Time.deltaTime * 2;
        }
        else if ((!chickyProps.isBeingHeld) && (chickyProps.attention > 0.0f))
        {
            chickyProps.attention -= Time.deltaTime;
        }

        //If the attention value is less than 50% the lonely boolean is true. Change UI icon sprites accordingly.
        if (chickyProps.attention < 50.0f)
        {
            chickyProps.lonely = true;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.attentionIcon, levelController.uiOutput.attentionLow); }
        }
        else
        {
            chickyProps.lonely = false;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.attentionIcon, levelController.uiOutput.attention); }
        }
    }

    private void CheckRest()
    {
        //If the chicken is in a hutch, increase the sleep value. If the chicken isn't in a hutch, decrease it at 50% speed.
        if ((chickyProps.inHutch) && (chickyProps.rest < 100.0f))
        {
            chickyProps.rest += Time.deltaTime;
        }
        else if ((!chickyProps.inHutch) && (chickyProps.rest > 0.0f))
        {
            chickyProps.rest -= Time.deltaTime * 0.5f;
        }

        //If the rest value is less than 50% the sleepy boolean is true. Change UI icon sprites accordingly.
        if (chickyProps.rest < 50.0f)
        {
            chickyProps.sleepy = true;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.sleepIcon, levelController.uiOutput.sleepLow); }
        }
        else
        {
            chickyProps.sleepy = false;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.sleepIcon, levelController.uiOutput.sleep); }
        }
    }

    //Check if any of the positions in the seedPositions list is true (allocated), there is seed available so return true and set seedPosition to this position so the chicken can go towards it. Otherwise, return false to indicate no seed object have been activated.
    public bool SeedAvailable()
    {
        foreach (KeyValuePair<Vector3, bool> seed in levelController.seedPositions)
        {
            if (seed.Value == true)
            {
                seedPosition = seed.Key;
                return true;
            }
        }
        return false;   
    }

    //If SeedAvailable() returned as true, add force to the object in the direction of the found seedPosition at the speed of the chicken.
    public void GoToSeed()
    {
        rigidBody.AddForce((seedPosition - transform.position).normalized * constPropsRef.speed);
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, constPropsRef.maxVel);      //Clamp the rigidbody's velocity to the specified maximum.
    }

    //Check if the time since the last coin drop has reached it's maximum, if so, reset the timer and activate a coin object from the list with a gold value based on the chicken's current stats. Otherwise, increase the timer.
    public void CheckCoinDrop()
    {
        if (chickyProps.timeSinceCoinDrop >= constPropsRef.timeBetweenCoinDrop)
        {
            chickyProps.timeSinceCoinDrop = 0.0f;

            foreach (KeyValuePair<GameObject, bool> coin in levelController.levelCoins)
            {
                if (coin.Value == true)     
                {
                    //If any of the coin objects in the coin list are true (free for activation), set it to false and activate it after transforming it to the chicken's current position on the X and Z axis.
                    activeCoin = coin.Key;
                    levelController.levelCoins[activeCoin] = false;
                    activeCoin.transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
                    activeCoin.SetActive(true);

                    //Initially set the coin drop amount to the maximum amount, but subtract 1 for every negative stat boolean. (Sleepy, lonely, cold/hot and hungry).
                    coinDropAssignedValue = constPropsRef.maxCoinDropAmount;
                    if (chickyProps.cold || chickyProps.hot) { coinDropAssignedValue--; }
                    if (chickyProps.hungry) { coinDropAssignedValue--; }
                    if (chickyProps.lonely) { coinDropAssignedValue--; }
                    if (chickyProps.sleepy) { coinDropAssignedValue--; }

                    //If this object is a Roosir, multiply the resulting drop amount by 2.
                    if (chickyProps.type == "Roosir")   
                    {
                        coinDropAssignedValue = coinDropAssignedValue * 2;
                    }

                    //Add the gold worth of this coin drop to the coinValue list along with the coin clone just activated to correlate it to.
                    levelController.coinValue.Add(coin.Key, coinDropAssignedValue);
                    break;
                }
            }
        }
        else
        {
            chickyProps.timeSinceCoinDrop += Time.deltaTime;
        }
    }

    //When called, destroy the model attached to this chicken instance, then deactivate this object from the pool, and display the reason it died as passed into this function into the tip textbox.
    private void KillChuck(string deathMessage)
    {
        GameObject oldModelClone = currentModelClone;
        Destroy(oldModelClone);

        levelController.DeactivateObject(transform.gameObject, levelController.levelChickens, deathMessage);
    }
}