using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Bonus : MonoBehaviour
{
    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Power Up

    // To prevent double bonus
    public bool givingBonus = false;

    /* In the maze we have differente bonus
        0 - +10 seconds
        1 - walls disable for 5 seconds
        2 - color distortion for 30 seconds
        3 - the ball will stretch, just like in Event Horizon for 20 seconds
        4 - light walls will appear in spaces between walls for 30 seconds
     */

    // The type of bonus
    public int bonus;

    // The message of each bonus
    private string[] bonusMsg;

    // Start is called before the first frame update
    void Start()
    {
        bonusMsg = new string[5] {
            "O Buraco Negro afeta as noções de tempo e espaço que conhecemos, seu relógio ganhou um bônus por conta disso.",
            "As paredes do labirinto foram distorcidas, aproveite e tente pegar um atalho. Essa anomalia não deve dura muito.",
            "A atividade do Buraco Negro deforma a luz, nossos sensores foram afetados e devem ficar assim por alguns segundos.",
            "O Horizonte de Eventos possui um forte campo gravitacional que gera o efeito de alargamento dos objetos ali, estamos passando pela Espaguetificação.",
            "Com essa gravidade toda, poeira e gás são comprimidas num processo de acreção formando barreiras luminosas que vão te atrapalhar, cuidado."
        };

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //To collide give a bonus and destroy the object
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            if (!givingBonus) 
            {
                // Show the respective message
                StartCoroutine(GameObject.FindWithTag("Puzzle").GetComponent<Maze>().ShowMessage(bonusMsg[bonus]));

                // Apply the message
                StartCoroutine(GiveBonus());
            }
        }
    }

    IEnumerator GiveBonus() 
    {
        // To prevent double bonus
        givingBonus = true;

        audioSrc.clip = clips[0];
        audioSrc.Play();

        // Hide the bonus item
        gameObject.GetComponent<Renderer>().enabled = false;

        // Aplly the bonus in the game
        if (bonus == 0)
        {
            // Send to API
            GameObject.FindWithTag("Puzzle").GetComponent<Maze>().SendBonusInfo("bonusTime");

            // +20 seconds in the timer
            GameObject.FindWithTag("Puzzle").GetComponent<Maze>().BonusTime(20);
        }
        else if (bonus == 1)
        {
            // Send to API
            GameObject.FindWithTag("Puzzle").GetComponent<Maze>().SendBonusInfo("bonusWall");

            // Hide the walls for 5 seconds
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

            foreach (GameObject wall in walls)
            {
                wall.SetActive(false);
            }

            yield return new WaitForSeconds(5f);

            foreach (GameObject wall in walls)
            {
                wall.SetActive(true);
            }
        }
        else if (bonus == 2)
        {
            // Send to API
            GameObject.FindWithTag("Puzzle").GetComponent<Maze>().SendBonusInfo("bonusLights");

            // Distortion of the vision for 30 seconds
            //Apply Black and White filter with postprocessing
            PostProcessVolume ppVolume = GameObject.FindWithTag("effects").GetComponent<PostProcessVolume>();
            ColorGrading colors;

            if (ppVolume.profile.TryGetSettings(out colors))
            {
                colors.saturation.value = -100f;

                yield return new WaitForSeconds(30f);

                colors.saturation.value = 0f;
            }
        }
        else if (bonus == 3)
        {
            // Send to API
            GameObject.FindWithTag("Puzzle").GetComponent<Maze>().SendBonusInfo("bonusSpaghetti");

            //Stretch ball for 20 seconds
            //Keep the actual scale
            Vector3 initialScale = GameObject.FindWithTag("Ball").transform.localScale;

            GameObject.FindWithTag("Ball").transform.transform.localScale = new Vector3(10f, 3f, 3f);

            yield return new WaitForSeconds(20f);

            GameObject.FindWithTag("Ball").transform.transform.localScale = initialScale;
        }
        else if (bonus == 4) 
        {
            // Send to API
            GameObject.FindWithTag("Puzzle").GetComponent<Maze>().SendBonusInfo("bonusLightWall");

            GameObject.FindWithTag("Puzzle").GetComponent<Maze>().TurnLightWalls(true);

            yield return new WaitForSeconds(30f);

            GameObject.FindWithTag("Puzzle").GetComponent<Maze>().TurnLightWalls(false);
        }

        //Time to play the sound
        yield return new WaitForSeconds(1f);
        
        // To prevent double bonus
        givingBonus = false;

        // Destroy the bonus item
        Destroy(gameObject);
    }
}
