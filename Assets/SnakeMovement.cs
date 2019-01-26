using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour {
    [Header("Managers")]
    public GameController gameController;

    [Header("Snake Variables")]
    public List<Transform> bodyParts = new List<Transform>();
    public float initialAmount;
    public float minDist = 0.25f;
    public float speed = 1;
    public float rotationSpeed = 50;
    public float lerpTimeX;
    public float lerpTimeY;

    [Header("Snake Head")]
    public GameObject bodypreFeb;

    [Header("Text Amount Manag")]
    public TextMesh partAmtTextMesh;

    [Header("Private Fields")]
    private float distance;
    private Vector3 refVel;

    private Transform curBodyPart;
    private Transform prevBodyPart;

    private bool firstPart;

    [Header("MouseControl Variable")]
    Vector2 mousePrevPos;
    Vector2 mouseCurPos;

    [Header("Particles System Manag")]
    public ParticleSystem snakeParticle;

    private void Start()
    {
        firstPart = true;
        for(int i = 0; i < initialAmount; i++)
        {
            Invoke("AddBodyPart", 0.1f);
        }
    }

    public void SpawnBodyPart()
    {
        firstPart = true;
        for (int i = 0; i < initialAmount; i++)
        {
            Invoke("AddBodyPart", 0.1f);
        }
    }

    private void Update()
    {
        if (GameController.gamestate == GameController.GameState.GAME)
        {
            Move();
            if (bodyParts.Count == 0)
            {
                gameController.SetGameOver();
            }

            if (partAmtTextMesh != null)
            {
                partAmtTextMesh.text = transform.childCount + "";
            }
        }
    }
    public void Move()
    {
        float curSpeed = speed;
        if (bodyParts.Count > 0)
        {
            bodyParts[0].Translate(Vector2.up * curSpeed * Time.smoothDeltaTime);
        }
        float maxX = Camera.main.orthographicSize * Screen.width / Screen.height;

        if(bodyParts.Count > 0)
        {
            if(bodyParts[0].position > maxX)
            {
                bodyParts[0].position = new Vector3(maxX - 0.01f, bodyParts[0].position.y, bodyParts[0].position.z);
            }
            else if(bodyParts[0].position.x < -maxX)
            {
                bodyParts[0].position = new Vector3(-maxX + 0.01f, bodyParts[0].position.z);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            mousePrevPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if(Input.GetMouseButtonDown(0))
        {
            if(bodyParts.Count > 0 && Mathf.Abs(bodyParts[0].position.x) < maxX)
            {
                mouseCurPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float deltaMousePos = Mathf.Abs(mousePrevPos.x - mouseCurPos.x);
                float sign = Mathf.Sign(mousePrevPos.x - mouseCurPos.x);

                bodyParts[0].GetComponent<Rigidbody2D>().AddForce(Vector2.right * rotationSpeed * deltaMousePos * -sign);
                mousePrevPos = mouseCurPos;
            }
            else if(bodyParts.Count > 0 && bodyParts[0].position.x > maxX)
            {
                bodyParts[0].position = new Vector3(maxX - 0.01f, bodyParts[0].position.y, bodyParts[0].position.z);
            }
            else if (bodyParts.Count > 0 && bodyParts[0].position.x < maxX)
            {
                bodyParts[0].position = new Vector3(-maxX + 0.01f, bodyParts[0].position.y, bodyParts[0].position.z);
            }
        }

        for(int i = 1; i < bodyParts.Count; i++)
        {
            curBodyPart = bodyParts[i];
            prevBodyPart = bodyParts[i - 1];
            distance = Vector3.Distance(prevBodyPart.position, curBodyPart.position);

            Vector3 newPos = prevBodyPart.position;
            newPos.z = bodyParts[0].position.z;

            Vector3 pos = curBodyPart.position;
            pos.x = Mathf.Lerp(pos.x, newPos.x, lerpTimeX);
            pos.y = Mathf.Lerp(pos.y, newPos.y, lerpTimeY);

            curBodyPart.position = pos;
        }
    }

    public void AddBodyPart()
    {
        Transform newPart;
        if(firstPart)
        {
            newPart = (Instantiate(bodypreFeb, new Vector3(0,0,0), Quaternion.identity) as GameObject).transform;

            partAmtTextMesh.transform.parent = newPart.position + new Vector3(0, 0.5f, 0);
            firstPart = false;
        }
        else
        {
            newPart = (Instantiate(bodypreFeb, bodyParts[bodyParts.Count - 1].position, bodyParts[bodyParts.Count - 1].rotation) as GameObject).transform;
            newPart.SetParent(transform);
            bodyParts.Add(newPart);
        }
    }
}
