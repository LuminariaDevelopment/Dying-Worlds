using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float launchForce = 10f;

    private void Update()
    {
        // Prüfen, ob die linke Maustaste geklickt wurde
        if (Input.GetMouseButtonDown(0))
        {
            LaunchProjectileFromCenter();
        }
    }

    private void LaunchProjectileFromCenter()
    {
        // Erhalte die Mausposition in der Bildschirmmitte
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Verwandle die Mausposition in eine Ray in der Szene
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        // Erzeuge ein Projektil an der Ray-Startposition
        GameObject newProjectile = Instantiate(projectilePrefab, ray.origin, Quaternion.identity);

        // Erhalte den Rigidbody des Projektils
        Rigidbody projectileRb = newProjectile.GetComponent<Rigidbody>();

        // Verleihe dem Projektil eine Kraft in Richtung des Ray-Richtungsvektors
        projectileRb.velocity = ray.direction * launchForce;
    }
}
