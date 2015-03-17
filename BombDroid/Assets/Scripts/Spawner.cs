using UnityEngine;
using System.Collections;

public class Spawner {

	public Server server;

	private int id_count;

	public Spawner(Server server) {
		this.server = server;
		id_count = 1;
		// start a timer to spawn bombs
	}

	public void AddBomb(int id, int type, float degrees, int solution/*, timer*/)
	{
		server.SpawnBomb(id, Random.Range (0, 2), (float) Random.Range(0, 360));
		id_count++;
	}

}
