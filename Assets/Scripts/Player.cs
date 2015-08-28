using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    //[SyncVar]
    public GameObject crate;
    public GameObject cratePrefab;
    public float moveSpeed = 0.2f;

    [SyncVar] public int moveX;

    public int moveY;

    [SyncVar] public Color myColor;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartClient()
    {
        //Debug.Log("Player OnStartClient netId:" + netId + " crate:" + this.crate);
        GetComponent<MeshRenderer>().material.color = myColor;
    }

    [ClientRpc]
    private void RpcPoke(int value)
    {
        Debug.Log("value:" + value);
    }

    [Command]
    private void CmdMakeCrate()
    {
        var crate = (GameObject) Instantiate(cratePrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(crate);

        this.crate = crate;
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // input handling for local player only
        var oldMoveX = moveX;
        var oldMoveY = moveY;

        moveX = 0;
        moveY = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdMakeCrate();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CmdLobby();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveX -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveX += 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveY += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveY -= 1;
        }
        if (moveX != oldMoveX || moveY != oldMoveY)
        {
            CmdMove(moveX, moveY);
        }
    }

    [Command]
    public void CmdLobby()
    {
        var lobby = NetworkManager.singleton as NetworkLobbyManager;
        NetworkManager.singleton.ServerChangeScene(lobby.lobbyScene);
    }

    [Command]
    public void CmdMove(int x, int y)
    {
        moveX = x;
        moveY = y;
        transform.Translate(moveX*moveSpeed, moveY*moveSpeed, 0);
        SetDirtyBit(1);
    }

    [ServerCallback]
    public void FixedUpdate()
    {
        transform.Translate(moveX*moveSpeed, moveY*moveSpeed, 0);
    }
}