using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*TODO: dynamically randomize position for the cubes between >=-5 and <=5 for x and z positions around the sphere */
/* tagged unit moves at a speed of 5 * deltaTime) and moved around with arrow keys
 * NPC units movement speed is at (1 * deltaTime) 
 * NPC continuouslly moves away from the tagged target unit if they are closer than 10 units away
 * NPC continuouslly moves closer to the tagged target unit if they are further than 11 units away (always face tagged unit) 
 * render the object to green if been tagged before
 * render any previously tagged NPCs in red and teleport to position (0,0,0)
 * when all NPCs have been tagged, game is over -> return to main menu screen */

public class GameController : MonoBehaviour
{
    public static string tag; // represents the tagged object (playable object)
    public static bool[] isTaggedBefore;
    
    // Start is called before the first frame update
    void Start()
    {
        if (this.transform.name.Equals("GameObject"))
        {
            //array of objects of one sphere and 10 objects while 10 represents the sphere (tagged object)
            isTaggedBefore = new bool[11];
            isTaggedBefore[10] = true;
            for(int i = 0; i < 10; i++)
            {
                isTaggedBefore[i] = false;
            }

            // created the sphere dynamically
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            
            // set the sphere color to green
            Color c = Color.green;

            //rendered sphere
            sphere.GetComponent<Renderer>().material.color = c;

            // set the sphere to center
            sphere.transform.position = Vector3.zero;
            // dynamically attach script to sphere
            sphere.AddComponent<GameController>();

            sphere.name = "Sphere";
            tag = "Sphere";

            //creates all 10 cubes
            for (int i = 0; i <= 9; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = sphere.transform.position + new Vector3(Random.Range(-5.0f, 5.0f), 0.0f, Random.Range(-5.0f, 5.0f));
                cube.transform.name = "Cube" + i;
                // dynamically attach script to each cube
                cube.AddComponent<GameController>();
                /*Instantiate(cube, i * cube.transform.position, Quaternion.identity);*/
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        //instantiates tagObject to the tagged object
        GameObject tagObject = GameObject.Find(tag);

        //checks if the name equals to the playable object
        if (this.transform.name.Equals(tag))
        {

            Vector3 upDir = new Vector3(0, 0, 5 * Time.deltaTime);
            Vector3 downDir = new Vector3(0, 0, -5 * Time.deltaTime);
            Vector3 leftDir = new Vector3(-5 * Time.deltaTime, 0, 0);
            Vector3 rightDir = new Vector3(5 * Time.deltaTime, 0, 0);

            if (Input.GetKey(KeyCode.UpArrow))
            {
                 this.transform.Translate(upDir);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                this.transform.Translate(downDir);
            }

            // keeps left diagonal movement constant
            Vector3 leftDiag = (upDir + leftDir).normalized;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                this.transform.Translate(leftDir);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                this.transform.Translate(rightDir);
            }

            // keeps right diagonal movement constant
            Vector3 rightDiag = (upDir + rightDir).normalized;
        }

        // checks for the non-playable object and controls the npc's movement
        if (!this.transform.name.Equals(tag) && !this.transform.name.Equals("GameObject")) 
        {
            
            Vector3 dist = this.transform.position - tagObject.transform.position;
            if (dist.magnitude <= 10)
            {
                //this.transform.position += new Vector3(1 * Time.deltaTime, 0, 1 * Time.deltaTime);
                Vector3 pos = this.transform.position;
                Vector3 dest = tagObject.transform.position;
                Vector3 dir = (pos - dest).normalized;
                Vector3 newPos = pos + (dir * (1 * Time.deltaTime));
                this.gameObject.transform.position = newPos;
            }
            else
            {
                Vector3 pos = this.transform.position;
                Vector3 dest = tagObject.transform.position;
                Vector3 dir = (dest - pos).normalized;
                Vector3 newPos = pos + (dir * (1 * Time.deltaTime));
                this.gameObject.transform.position = newPos;
            }
        }

        // checks if objects collide and change color accordingly
        if (tagObject.GetComponent<Collider>().bounds.Contains(this.transform.position) && 
            !this.transform.name.Equals(tag))
        {
             Color c = Color.green;
             Color c2 = Color.red;

             tagObject.GetComponent<Renderer>().material.color = c2;
             this.GetComponent<Renderer>().material.color = c;

             Vector3 tempPos = Vector3.zero;
             transform.position = tagObject.transform.position;
             tagObject.transform.position = tempPos;
             tag = this.transform.name;

            int extractedNum = 10;

            //checks if tag is a Sphere 
            if(!tag.Equals("Sphere"))
            {
                try
                {
                    string num = tag.Substring(4,1);
                    extractedNum = int.Parse(num);
                    isTaggedBefore[extractedNum] = true;
                }
                catch (System.Exception ex) 
                {
                    Debug.Log(ex.Message);
                }
            }

            bool isAllTagged = true;
            for (int i = 0; i < isTaggedBefore.Length; i++)
            {
                if (!isTaggedBefore[i])
                {
                    isAllTagged = false;
                    break;
                }
            }
            if(isAllTagged)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

}