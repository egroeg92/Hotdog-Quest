using UnityEngine;
using System.Collections;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
public class Client : MonoBehaviour{

	public GameController game;
	public NetworkView networkView;
	public NetworkViewID viewID;
	bool connected = false;

	public void destroy(){

		removePlayer (Network.player);
		game.client = null;
		game.destroyCity ();
		Network.Disconnect (250);
		Destroy (this);
	
	}

	public void updatePosition(Vector3 position){
		updatePlayerPosition (position, Network.player);
	}
	[RPC]
	void sendMap(NetworkPlayer p){
	}
	[RPC]
	void receiveMap(NetworkPlayer p, Vector3 sizes, Vector3 pos, Vector3 plane, int count, int size){
		//Debug.Log (pos.Length);
		//Debug.Log (sizes.Length);


		//MemoryStream pms = new MemoryStream (System.Convert.FromBase64String(pos));
		//MemoryStream sms = new MemoryStream (System.Convert.FromBase64String(sizes));

		//BinaryFormatter bf = new BinaryFormatter ();

		//ArrayList po = bf.Deserialize (pms) as ArrayList;
		//ArrayList si = bf.Deserialize (sms) as ArrayList;
		Debug.Log (pos);

		game.addPosition (pos);
		game.addSize (sizes);

		if(count == size -1)
			game.createCity(plane);



		//game.createCity (po, si, plane);

	}

	[RPC]
	void updatePlayerPosition(Vector3 position, NetworkPlayer player){
		networkView.RPC ("updatePlayerPosition", RPCMode.Server, position, Network.player);
	}
		
	[RPC]
	void removePlayer(NetworkPlayer player){
		networkView.RPC ("removePlayer", RPCMode.Server, Network.player);
	}
		
	[RPC]
	public void connectedToServer(NetworkPlayer player, int connections){
		Debug.Log (Network.player == player);
		if (Network.player == player) {
			game.setPlayer(connections);
		}
	}

	[RPC]
	public void receiveUpdate(Vector3 p1Pos, Vector3 p2Pos){
		game.updateOtherPlayer (1, p1Pos);
		game.updateOtherPlayer (2, p2Pos);

	}


}
