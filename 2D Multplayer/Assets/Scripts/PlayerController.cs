using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;

    [Header("Component")]
    public Rigidbody2D rig;
    public Player photonPlayer;
    public Text playerNickname;

    [SerializeField]
    private float speed = 0.8f;

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        speed = 0.8f;
        GameManager.instance.players[id - 1] = this;
        if(!photonView.IsMine)
        {
            //rig.isKinematic = true;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        speed = 0.8f;
        //rig.isKinematic = true;
        //playerNickname.text = photonPlayer.NickName;
    }

    private void Update()
    {
        if(photonPlayer.IsLocal)
        {
            Movements();
            //if (Input.GetKey(KeyCode.LeftControl) || CrossPlatformInputManager.GetButton("Shoot")) ;
            //{
                //photonView.RPC("Fire", RpcTarget.All);
            //}
        }
    }

    void Movements()
    {
        float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");
        float hori = Input.GetAxis("Horizontal");
        float verti = Input.GetAxis("Vertical");

        if(horizontal != 0 ||  vertical != 0  || hori !=0 || verti !=0)
        {
            speed = 0.8f;
        }
        else
        {
            speed = 0;
        }
        if((horizontal > 0  && vertical > 0) || (hori > 0 && verti > 0))
        {
               
            transform.Translate(new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, 0f) );
        }
        else if((horizontal > 0 && vertical < 0) || (hori > 0 && verti < 0))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, -speed * Time.deltaTime, 0f));
        }
        else if ((horizontal < 0 && vertical < 0) || (hori < 0 && verti < 0))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, -speed * Time.deltaTime, 0f));
        }
        else if ((horizontal < 0 && vertical > 0) || (hori < 0 && verti > 0))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, speed * Time.deltaTime, 0f));
        }
        else if ((horizontal > 0 && vertical == 0) || (hori > 0 && verti == 0))
        {
            //right
            
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else if ((horizontal < 0 && vertical == 0) || (hori < 0 && verti == 0))
        {
            //left
            
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        else if ((vertical > 0 && horizontal == 0) || (verti > 0 && hori == 0))
        {
            //down
            
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        else if ((vertical < 0 && horizontal == 0) || (verti < 0 && hori == 0))
        {
            //up
            
            transform.Translate(Vector3.down * speed * Time.deltaTime );
        }
    }

    [PunRPC]
    void Fire()
    {
        GameObject bullet = Instantiate(Resources.Load("bullet",typeof(GameObject)) as GameObject);
        bullet.name = photonPlayer.NickName;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.localPosition = transform.position;
        bullet.transform.localScale = new Vector3(0.01f, 0.0f, 0.0f);
        rb.AddForce(this.transform.forward * 300f); 
        Destroy(bullet, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (photonPlayer.IsLocal)
            {
                GameManager.instance.joinBtn.SetActive(true);
            }               
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (photonPlayer.IsLocal)
        {
            GameManager.instance.joinBtn.SetActive(false);
        }
    }
    IEnumerator PlayerColorChange()
    {
        this.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(2);
        this.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
    }
}
