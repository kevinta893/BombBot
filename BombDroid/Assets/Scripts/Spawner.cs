using UnityEngine;
using System.Collections;

public class Spawner {

	public Server server;

	private int counter;

	public Spawner(Server server) {
		this.server = server;
		counter = 1;
		// start a timer to spawn bombs
	}

	public void AddBomb(int id, int type, float degrees, int solution/*, timer*/)
	{
		server.SpawnBomb(id, type, degrees);
		counter++;
	}

}
