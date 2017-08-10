﻿using System.Collections;
using UnityEngine;

public class BallControl : MonoBehaviour {
    private Rigidbody2D m_rb2d;
    /// <summary>
    /// Velocity if pause
    /// </summary>
    private Vector2 m_vel;
    /// <summary>
    /// Basic speed
    /// </summary>
    [SerializeField]
    float m_speed;
    /// <summary>
    /// Speed during game
    /// </summary>
    float m_currentSpeed;
    /// <summary>
    /// how evolve speed
    /// </summary>
    [SerializeField]
    float m_step = 0.05f;
    /// <summary>
    /// Modificator of speed
    /// </summary>
    float m_countBoost = 0;

    void Start()
    {
        m_rb2d = GetComponent<Rigidbody2D>();
        GameManager.m_paused.AddListener(Paused);
        GameManager.m_unpaused.AddListener(UnPaused);
        m_currentSpeed = m_speed;

    }

    void Update () {
        m_rb2d.velocity = m_rb2d.velocity.normalized * m_currentSpeed;
    }

    void Paused() {
       m_vel = m_rb2d.velocity;     
        m_rb2d.velocity = Vector2.zero;
    }

    void UnPaused()
    {
        m_rb2d.velocity = m_vel;
    }


    /// <summary>
    /// The ball started
    /// </summary>
    void GoBall(int a_second)
    {
        StartCoroutine(GoBallCoroutine(a_second));
    }

    IEnumerator GoBallCoroutine(int a_second)
    {
        do { 
        yield return new WaitForSeconds(a_second);
        } while(GameManager.m_isPaused) ;

        GetComponent<TrailRenderer>().enabled = true;
        float rand = GameManager.m_lastWinner == PLAYER.NOBODY ? Random.Range(0, 2) : GameManager.m_lastWinner == PLAYER.PLAYER_1 ? 2 : 0;
        if (rand < 1)
        {
            m_rb2d.AddForce(new Vector2(25, 15));
        }
        else
        {
            m_rb2d.AddForce(new Vector2(-25, -15));
        }

    }

    void ResetBall()
    {
        GetComponent<TrailRenderer>().enabled = false;
        m_rb2d.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        m_currentSpeed = m_speed;
        m_countBoost = 0;
        UpdateColorBall();
        StopAllCoroutines();

    }



    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("Player"))
        {
            //Reset speed modifier
            StopAllCoroutines();
            m_currentSpeed -= m_countBoost;
            m_countBoost = 0;
            UpdateColorBall();


            GameManager.m_lastHit = coll.gameObject.GetComponent<PlayerControl>().m_playerID;
            Vector2 vel;
            vel.x = m_currentSpeed * ( coll.contacts[0].point.x - coll.collider.attachedRigidbody.position.x);
            //Force y velocity to avoid ball move horizontally
            vel.y = Mathf.Abs(m_rb2d.velocity.y) < 0.35f ? (m_rb2d.velocity.y+0.1f) * m_currentSpeed : m_rb2d.velocity.y;
            m_rb2d.velocity = vel;
            m_currentSpeed += m_step;


        }
    }


    /// <summary>
    /// Boost Speed
    /// </summary>
    void Boost(float a_value)
    {
        m_countBoost += a_value;
        m_currentSpeed += a_value;
        UpdateColorBall();
        StartCoroutine(UndoBoost(a_value));
    }


    IEnumerator UndoBoost(float a_value)
    {
        yield return new WaitForSeconds(1.5f);
        m_countBoost += -a_value;
        Debug.Log(m_countBoost);
        m_currentSpeed -= a_value;
        UpdateColorBall();
    }

    void UpdateColorBall()
    {
        Color color = m_countBoost > 0 ? Color.red : m_countBoost == 0 ? Color.white : Color.blue;
        GetComponent<SpriteRenderer>().color = color;

        TrailRenderer trail = GetComponent<TrailRenderer>();
        color.a = trail.startColor.a;
        trail.startColor = color;
        color.a = trail.endColor.a;
        trail.endColor = color;

    }



}
