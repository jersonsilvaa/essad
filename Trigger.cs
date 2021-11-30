using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

void OnTriggerEnter2D(Collider2D other)
{
    var controller = transform.parent.GetComponent<Controller>();

    if (other.gameObject.tag == "Shell") {
        var shell = other.gameObject.GetComponent<Shell>();

        if (shell.is_moving()) {
            var ray = controller.transform.Find("ray");
            var ray_start = ray.transform.position;
            var ray_top = new Vector2(ray.transform.position.x, ray.transform.position.y - 8);

            RaycastHit2D[] top_hits = Physics2D.LinecastAll(ray_start, ray_top);


           foreach (RaycastHit2D hit in top_hits)
                {
                    var collider = hit.collider;
                    if (collider != null)
                    {
                        if (collider.gameObject.tag == "Shell")
                        {
                            var player_rigid_body = controller.GetComponent<Rigidbody2D>();
                            player_rigid_body.velocity = new Vector2(player_rigid_body.velocity.x, 0);

                            if (Input.GetKey(KeyCode.Space))
                            {
                                player_rigid_body.AddForce(new Vector2(0, controller.saltoMaximo * 2f));
                            }
                            else
                            {
                                player_rigid_body.AddForce(new Vector2(0, controller.saltoMinimo * 1.25f));
                            }
                            shell.is_moving(false);
                            var rigid_body = shell.GetComponent<Rigidbody2D>();
                            rigid_body.velocity = new Vector2();

                        }
                    }
                }
        } else {
            if (Input.GetButton("Fire1")) {
                controller.pick_item_up(other.gameObject);
            } else {
                controller.kick_item(other.gameObject);
            }
        }
    }
}

}