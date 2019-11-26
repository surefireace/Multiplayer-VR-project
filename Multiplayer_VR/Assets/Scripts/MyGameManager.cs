// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to handle typical game management requirements
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun; 
using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
	#pragma warning disable 649

	/// <summary>
	/// Game manager.
	/// Connects and watch Photon Status, Instantiate Player
	/// Deals with quiting the room and the game
	/// Deals with level loading (outside the in room synchronization)
	/// </summary>
	public class MyGameManager : MonoBehaviourPunCallbacks
    {

		#region Public Fields

		static public MyGameManager Instance;
        public static GameObject LocalPlayerInstance;
        public GameObject testSpawn;
        public bool m_observer = false;

        private bool m_gameOver = false;

        [SerializeField]
        private LightPrisms[] m_lightPrisms = null;

        [SerializeField]
        private AudioSource m_victorySound;
        #endregion

        #region Private Fields

        private GameObject instance;
        private Color[] m_prismColors;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private GameObject enemyPrefab;

        public ParticleSystem[] victoryParticles;
        public Light victoryLight;
        bool playVictoryOnce = false;
        public GameObject beamEnd;
       
        //[SerializeField]
        //private GameObject playerGunPrefab;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            m_prismColors = new Color[m_lightPrisms.Length];

            for(int i = 0; i < m_prismColors.Length; ++i)
            {
                m_prismColors[i] = m_lightPrisms[i].Color;
            }

        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
		{
			// in case we started this demo with the wrong scene being active, simply load the menu scene
			if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("MyLauncher");

				return;
			}

			if (playerPrefab == null) { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

				Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			}
            else
            {
				if (PlayerManager.LocalPlayerInstance==null && !m_observer)
				{
				    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab.name, ViveManager.m_instance.m_cam.transform.position, ViveManager.m_instance.m_cam.transform.rotation) as GameObject;

                    string playername = PhotonNetwork.LocalPlayer.NickName;



                   if (playername == "Dog")
                    {
                      
                        playerObj.GetComponent<robotAvatar>().headMesh.gameObject.layer = 10;
                        ViveManager.m_instance.m_cam.GetComponent<Camera>().cullingMask &= ~(1 << 10);

                    }
                   else
                    {
                        playerObj.GetComponent<robotAvatar>().headMesh.gameObject.layer = 11;
                        ViveManager.m_instance.m_cam.GetComponent<Camera>().cullingMask &= ~(1 << 11);

                    }


                    //PhotonNetwork.Instantiate(playerGunPrefab.name, ViveManager.m_instance.m_RHand.transform.position, ViveManager.m_instance.m_RHand.transform.rotation);
                    //if (PhotonNetwork.IsMasterClient)
                    //{
                        //PhotonNetwork.Instantiate(enemyPrefab.name, new Vector3(0, 3, 0), Quaternion.identity);
                    //}

                    //PhotonNetwork.Instantiate(testSpawn.name, ViveManager.m_instance.m_cam.transform.position, ViveManager.m_instance.m_cam.transform.rotation);

                }
                if (photonView != null && photonView.IsMine)
                {
                    PlayerManager.LocalPlayerInstance = this.gameObject;
                }
                else
                {

					Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
				}

			}

		}

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity on every frame.
		/// </summary>
		void Update()
		{
			// "back" button of phone equals "Escape". quit app if that's pressed
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}

            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                PhotonNetwork.LeaveRoom();
            }

        }

        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Called when a Photon Player got connected. We need to then load a bigger scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerEnteredRoom( Player other  )
		{
			Debug.Log( "OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

			if ( PhotonNetwork.IsMasterClient )
			{
				Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // called before OnPlayerLeftRoom

                LoadArena();
			}
		}

		/// <summary>
		/// Called when a Photon Player got disconnected. We need to load a smaller scene.
		/// </summary>
		/// <param name="other">Other.</param>
		public override void OnPlayerLeftRoom( Player other  )
		{
			Debug.Log( "OnPlayerLeftRoom() " + other.NickName ); // seen when other disconnects

			if ( PhotonNetwork.IsMasterClient )
			{
				Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // called before OnPlayerLeftRoom

				LoadArena(); 
			}
		}

		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom()
		{
			SceneManager.LoadScene("MyLauncher");
		}

		#endregion

		#region Public Methods

		public void LeaveRoom()
		{
			PhotonNetwork.LeaveRoom();
		}

		public void QuitApplication()
		{
			Application.Quit();
		}

        /// <summary>
        /// to disrupt the prisms when the enemy attacks them
        /// </summary>
        public void DisruptPrisms()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                System.Random rand = new System.Random();
                int n = m_prismColors.Length;

                // shuffle the colors
                for (int i = m_prismColors.Length - 1; i > 1; i--)
                {
                    int r = rand.Next(i + 1);
                    
                    var value = m_prismColors[r];
                    m_prismColors[r] = m_prismColors[i];
                    m_prismColors[i] = value;
                }


                for (int i = 0; i < m_lightPrisms.Length; ++i)
                {
                    m_lightPrisms[i].Disrupt(m_prismColors[i]);
                    //Debug.LogWarning("<Color=cyan>" + m_prismColors[i] + "</color>");
                }
                //Debug.LogWarning("end");

            }
        }

		#endregion

		#region Private Methods

		void LoadArena()
		{
			if ( !PhotonNetwork.IsMasterClient )
			{
				Debug.LogError( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
			}

			Debug.LogFormat( "PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount );

			//PhotonNetwork.LoadLevel("TestLevel");
		}

        public bool GameOver()
        {
            return m_gameOver;
        }

        /// <summary>
        /// to show the user they won
        /// </summary>
        /// <param name="didWin"> if the user won</param>
        public void EndGame(bool didWin)
        {
            print("ONE");

            PhotonView photonView = PhotonView.Get(this);//.photonView.RPC());
           
            print("TWO");

            if (!photonView)
                print("NULLLLLLLLL");

            if (didWin)
            {
                if (!playVictoryOnce)
                {
                    playVictoryOnce = true;
                    photonView.RPC("Victory", RpcTarget.All);
                }
                print("THREE");
               
                
            }
        }

        /// <summary>
        /// to tell the clients the game is over
        /// </summary>
        [PunRPC]
        public void Victory()
        {
            m_gameOver = true;

            beamEnd.transform.position = new Vector3(beamEnd.transform.position.x, beamEnd.transform.position.y, 80f);
            victoryLight.enabled = true;
            foreach (ParticleSystem vicPart in victoryParticles)
                vicPart.Play();

            victoryLight.enabled = true;

            

            m_victorySound.Play();
        }

        #endregion

    }

}