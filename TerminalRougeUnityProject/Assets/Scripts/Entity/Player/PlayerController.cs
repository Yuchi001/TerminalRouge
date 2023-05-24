using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerController : Entity
{
    [Header("Props")] 
    [SerializeField] private GameObject bulletPrefab;

    private Dictionary<EHealthState, float> States = new Dictionary<EHealthState, float>();

    private bool? directionX = null;
    private bool? directionY = null;

    private float _attackTimer = 0;

    private void Awake()
    {
        SetupEntity();
    }

    void Update()
    {
        Movement();

        if(Input.GetMouseButton(0))
            Shoot();
    }

    void Shoot()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer < 1f / GetStat(EStatType.AttackSpeed))
            return;

        _attackTimer = 0;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        var diff = transform.position - mousePos;
        var angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg + 90;
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.transform.Rotate(0, 0, angle);
    }
     
    void Movement()
    {
        if (States.TryGetValue(EHealthState.Stuned, out var val))
            return;
        
        var ms = GetStat(EStatType.MovementSpeed) - (GetStat(EStatType.MovementSpeed) * msDisadvantage);

        var inputX_down = (Input.GetKeyDown(KeyCode.A), Input.GetKeyDown(KeyCode.D)) as (bool left, bool right)?;
        var inputY_down = (Input.GetKeyDown(KeyCode.S), Input.GetKeyDown(KeyCode.W)) as (bool down, bool up)?;

        var inputX = (Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D)) as (bool left, bool right)?;
        var inputY = (Input.GetKey(KeyCode.S), Input.GetKey(KeyCode.W)) as (bool down, bool up)?;

        var left = (inputX.Value.left && !inputX_down.Value.right) || inputX_down.Value.left;
        var right = (inputX.Value.right && !inputX_down.Value.left) || inputX_down.Value.right;

        var down = (inputY.Value.down && !inputY_down.Value.up) || inputY_down.Value.down;
        var up = (inputY.Value.up && !inputY_down.Value.down) || inputY_down.Value.up;

        if (left && right && directionX != null)
        {
            right = directionX.Value;
            left = !right;
        }

        if (up && down && directionY != null)
        {
            up = directionY.Value;
            down = !up;
        }

        var msY = down ? -ms : (up ? ms : 0);
        var msX  = left ? -ms : (right ? ms : 0);

        var modifier = msY != 0 && msX != 0 ? Time.deltaTime / Mathf.Sqrt(2) : Time.deltaTime;

        var pos = transform.position;
        transform.position = new Vector2(pos.x + msX * modifier, pos.y + msY * modifier);

        directionX = msX == 0 ? null : msX > 0;
        directionY = msY == 0 ? null : msY > 0;
    }
}