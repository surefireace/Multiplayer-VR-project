// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Valve.VR;
using HTC.UnityPlugin.Vive;

/// <summary>
/// the player class that handles firing the gun and networking it for others
/// </summary>
public class LightPlayer : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float m_reloadTime = 0.05f;
    [SerializeField] private float m_dammage = 50f;
    [SerializeField] private float m_bulletSpeed = 50f;
    [SerializeField] private GameObject m_bullet;
    private GuideLineDrawer m_rightGuidLine;
    private bool m_reloaded = true;  // for capping rate of fire
    private bool m_fired = false;   // for mutiplayer syncing
    private GameObject m_gun = null;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        m_rightGuidLine = ViveManager.m_instance.m_guideLineR;
        m_gun = GameObject.FindGameObjectWithTag("Gun");
        if(m_gun == null)
        {
            Debug.LogError("No Gun");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && !PhotonNetwork.IsMasterClient)
        {
            if(ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.FullTrigger) || ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.FullTrigger))
            {
                this.FireGun();
            }
        }
            
    }

    private void FireGun()
    {
        if (m_reloaded)
        {
            m_fired = true;
            m_reloaded = false;

            // if (m_rightGuidLine.isActiveAndEnabled)
            {
                //var points = m_rightGuidLine.raycaster.BreakPoints;
                //var startPoint = points[0];
                //var endPoint = points[points.Count - 1];

                //Ray ray = new Ray(startPoint, endPoint - startPoint);

                //RaycastHit result;
                //if (Physics.Raycast(ray, out result))
                //{
                //    print(result.transform.gameObject.name);
                //    //var bullet = PhotonNetwork.Instantiate(m_bullet.name, ViveManager.m_instance.m_RHand.transform.position + ViveManager.m_instance.m_RHand.transform.forward, ViveManager.m_instance.m_RHand.transform.rotation);
                //    //bullet.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 50, ForceMode.Impulse);
                //    //bullet.transform.GetChild(0).GetComponent<Bullet>().Dammage = m_dammage;
                //}
                
                photonView.RPC("SpawnBullet", RpcTarget.All, ViveManager.m_instance.m_bulletSpawn.transform.position,
                    ViveManager.m_instance.m_bulletSpawn.transform.rotation, ViveManager.m_instance.m_bulletSpawn.transform.forward, m_dammage, m_bulletSpeed);
            }

            StartCoroutine(CoolDown());

        }
    }

    [PunRPC]
    public void SpawnBullet(Vector3 pos, Quaternion rot, Vector3 forward, float dam, float speed)
    {
        if (PhotonNetwork.IsMasterClient)
        {

            var bullet = PhotonNetwork.Instantiate(m_bullet.name, pos + forward, rot);
            bullet.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(forward * speed, ForceMode.Impulse);
            bullet.transform.GetChild(0).GetComponent<Bullet>().Dammage = dam;
        }
    }


    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(m_reloadTime);
        m_fired = false;
        m_reloaded = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.m_fired);
        }
        else
        {
            // Network player, receive data
            this.m_fired = (bool)stream.ReceiveNext();
        }

    }
}
