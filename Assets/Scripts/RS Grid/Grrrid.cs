using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Jessinho
{
	public class Grrrid : MonoBehaviour
	{
		public static Grrrid Singleton;

		public GridProfile gridProfile;

		private int nodeScale = 2;

		private int gridSizeX = 4, gridSizeY = 4;

		public Node[,] grid;

		public int MaxSize
		{
			get
			{
				return gridSizeX * gridSizeY;
			}
		}

		private void Awake()
		{
			Singleton = this;
			CreateGrid();
		}

		void Start()
		{
		}

		void CreateGrid()
		{
			if (!gridProfile)
				return;

			gridSizeX = gridProfile.width;
			gridSizeY = gridProfile.height;
			nodeScale = gridProfile.nodeScale;

			if (gridProfile.grid == null || gridProfile.grid.Length == 0)
			{
				grid = new Node[gridSizeX, gridSizeY];
				for (int x = 0; x < gridSizeX; x++)
				{
					for (int y = 0; y < gridSizeY; y++)
					{
						grid[x, y] = new Node(new Vector3(nodeScale * x, 0, nodeScale * y), x, y);
						//Instantiate(vertice_placeholder, requiemGrid[x, y].worldPos, Quaternion.identity);
						//Debug.DrawLine(requiemGrid[x,y].worldPos, requiemGrid[x, y].worldPos + Vector3.up, Color.green, 100);
					}
				}
			}
			else
			{
				grid = new Node[gridSizeX, gridSizeY];
				for (int x = 0; x < gridSizeX; x++)
				{
					for (int y = 0; y < gridSizeY; y++)
					{
						grid[x, y] = gridProfile.grid[x + y * gridSizeX];//new Node(new Vector3(nodeScale * x, 0, nodeScale * y), x, y);
																		 //Instantiate(vertice_placeholder, requiemGrid[x, y].worldPos, Quaternion.identity);
																		 //Debug.DrawLine(requiemGrid[x,y].worldPos, requiemGrid[x, y].worldPos + Vector3.up, Color.green, 100);
					}
				}
			}

			SaveGridToProfile();
		}

		public void SaveGridToProfile()
		{
			gridProfile.grid = new Node[gridSizeX * gridSizeY];

			for (int x = 0; x < gridSizeX; x++)
			{
				for (int y = 0; y < gridSizeY; y++)
				{
					gridProfile.grid[x + y * gridSizeX] = grid[x, y];
				}
			}
		}

		public Node GetNodeFromWorldPoint(Vector3 worldPosition)
		{
			int x = Mathf.RoundToInt(worldPosition.x / nodeScale);
			int y = Mathf.RoundToInt(worldPosition.z / nodeScale);
			return grid[x, y];
		}

		//public Node GetNodeFromWorldPoint(Vector3 worldPosition)
		//{
		//	float percentX = (worldPosition.x +  gridWorldSize.x / 2) / gridWorldSize.x;
		//	float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		//	percentX = Mathf.Clamp01(percentX);
		//	percentY = Mathf.Clamp01(percentY);
		//
		//	int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		//	int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		//	return grid[x, y];
		//}


		public Transform begin, end;
		void Update()
		{
			return;

			//Pathfinding teste
			if (Application.isEditor && Input.GetKeyDown(KeyCode.Space))
			{
				List<Node> pathList = Astar(begin.position, end.position);
				Vector3 lastPosition = pathList[0].worldPosition;
				foreach (var item in pathList)
				{
					Debug.DrawLine(item.worldPosition, lastPosition, Color.blue, 2f);
					lastPosition = item.worldPosition;
				}
			}

			//if (Input.GetMouseButton(1))
			//{
			//    click_placeholder.transform.position = GetWorldPointFromScreen(Input.mousePosition);
			//}

			//Adicionar PAREDE - Isso é pra debugar e depois salvar o grid num arquivo
			if (Application.isEditor && Input.GetMouseButton(1))
			{
				if (vertClick1 == null)
				{
					vertClick1 = GetNodeFromWorldPoint(GetWorldPointFromScreen(Input.mousePosition));
				}
				else if (vertClick2 == null)
				{
					Node aux = GetNodeFromWorldPoint(GetWorldPointFromScreen(Input.mousePosition));
					if (vertClick1 != aux)
					{
						//Seta o vertice 2
						vertClick2 = GetNodeFromWorldPoint(GetWorldPointFromScreen(Input.mousePosition));
						//Coloca os 2 como bloqueados um pro outro
						if (!vertClick1.ContainsWallInNode(vertClick2))
						{
							vertClick1.AddWallToNode(vertClick2);
							vertClick2.AddWallToNode(vertClick1);
						}
						else
						{
							vertClick1.RemoveWallFromNode(vertClick2);
							vertClick2.RemoveWallFromNode(vertClick1);
						}
					}
				}				
			}

			if (Application.isEditor && Input.GetMouseButton(2))
			{
				if (vertClick1 == null)
				{
					vertClick1 = GetNodeFromWorldPoint(GetWorldPointFromScreen(Input.mousePosition));
				}
				else if (vertClick2 == null)
				{
					Node aux = GetNodeFromWorldPoint(GetWorldPointFromScreen(Input.mousePosition));
					if (vertClick1 != aux)
					{
						//Seta o vertice 2
						vertClick2 = GetNodeFromWorldPoint(GetWorldPointFromScreen(Input.mousePosition));
						//Coloca os 2 como bloqueados um pro outro
						if (!vertClick1.ContainsDoorInNode(vertClick2))
						{
							vertClick1.AddDoorToNode(vertClick2);
							vertClick2.AddDoorToNode(vertClick1);
						}
						else
						{
							vertClick1.RemoveDoorFromNode(vertClick2);
							vertClick2.RemoveDoorFromNode(vertClick1);
						}
					}
				}
			}

			//Soltar botão
			if (Application.isEditor && Input.GetMouseButtonUp(1))
			{
				//Bloqueia o node em si ao soltar o mouse em cima se não tiver uma parede selecionada
				if (vertClick2 == null)
				{
					Node clickedVertice = GetNodeFromWorldPoint(GetWorldPointFromScreen(Input.mousePosition));
					clickedVertice.PathUpdate(!clickedVertice.IsWalkable);
				}

				//Limpa os vertices selecionados
				vertClick1 = null;
				vertClick2 = null;
				Debug.Log("Teste");
				SaveGridToProfile();
			}
			if (Input.GetMouseButtonUp(2))
			{
				//Limpa os vertices selecionados
				vertClick1 = null;
				vertClick2 = null;
			}
		}

		[HideInInspector]
		public Node vertClick1, vertClick2;

		public Vector3 GetWorldPointFromScreen(Vector2 ScreenPos/*, bool convertToNodePosition*/)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(ScreenPos);
			Vector3 aux = Vector3.one * 10;//só de teste
			if (Physics.Raycast(ray, out hit, 1000, (1 << LayerMask.NameToLayer("Terreno")) | (1 << 0), QueryTriggerInteraction.Ignore))
			{
				aux = new Vector3(hit.point.x, hit.point.y, hit.point.z);
				try
				{
					//if (convertToNodePosition)
					//{
					aux = GetNodeFromWorldPoint(aux).worldPosition;
					return new Vector3(aux.x, hit.point.y, aux.z);
					//}
					//else
					//    return aux;
				}
				catch (Exception e)
				{
					Debug.Log("Deu ruim, vê ai: " + e.Message);
					return aux;
				}
			}
			else
			{
				////Se o raycast nao pegou eh porque ele clicando ta naqueles terrenos de fora, sem colisao, entao devolvo um ponto na posicao zero
				//Vector3 point = AchaPontoNoChao(ray.origin, ray.direction, 0);
				//Debug.DrawRay(point, Vector3.up * 3, Color.cyan);
				return aux;
			}
		}

		#region Grid_Debug
		void OnDrawGizmos()
		{
			if (grid == null)
				return;

			DrawGrid();
			DrawArestas();
		}
		void DrawGrid()
		{
			for (int i = 0; i < gridSizeX; i++)
			{
				for (int j = 0; j < gridSizeY; j++)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawWireCube(grid[i, j].worldPosition, new Vector3(nodeScale, 0.2f, nodeScale));

					if (!grid[i, j].IsWalkable)
					{
						Gizmos.color = Color.red;
						Gizmos.DrawCube(grid[i, j].worldPosition, new Vector3(nodeScale * 0.7f, 0.2f, nodeScale * 0.7f));
					}
				}
			}
		}
		void DrawArestas()
		{
			for (int i = 0; i < gridSizeX; i++)
			{
				for (int j = 0; j < gridSizeY; j++)
				{
					//Arestas Horizontais
					if (j != gridSizeY - 1)
					{
						Vector3 pos;
						Node vert = grid[i, j];
						Node nextVert = grid[i, j + 1];
						pos = (vert.worldPosition - (vert.worldPosition - nextVert.worldPosition) / 2);

						if (vert.ContainsWallInNode(nextVert))
						{
							Gizmos.color = Color.red;
							Gizmos.DrawCube(pos, new Vector3(nodeScale * 0.99f, 0.3f, nodeScale * 0.1f));

							//Desenha PORTAS
							if (vert.ContainsDoorInNode(nextVert))
							{
								Gizmos.color = Color.yellow;
								Gizmos.DrawCube(pos, new Vector3(nodeScale * 0.5f, 0.2f, nodeScale * 0.3f));
							}
						}
					}
					//Arestas Verticais
					if (i != gridSizeY - 1)
					{
						Vector3 pos;
						Node vert = grid[i, j];
						Node nextVert = grid[i + 1, j];
						pos = (vert.worldPosition - (vert.worldPosition - nextVert.worldPosition) / 2);

						if (vert.ContainsWallInNode(nextVert))
						{
							Gizmos.color = Color.red;
							Gizmos.DrawCube(pos, new Vector3(nodeScale * 0.1f, 0.3f, nodeScale * 0.99f));

							//Desenha PORTAS
							if (vert.ContainsDoorInNode(nextVert))
							{
								Gizmos.color = Color.yellow;
								Gizmos.DrawCube(pos, new Vector3(nodeScale * 0.3f, 0.2f, nodeScale * 0.5f));
							}
						}
					}
				}
			}
		}
		#endregion

		#region A* Pathfinding
		/*Algoritmo A* */
		public List<Node> Astar(Vector3 startPos, Vector3 targetPos, bool includeDiagonals = false)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			Node startNode = this.GetNodeFromWorldPoint(startPos);
			Node targetNode = this.GetNodeFromWorldPoint(targetPos);

			if (startNode == targetNode)
			{
				return null;
			}

			Heap<Node> openSet = new Heap<Node>(this.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			//Procura enquanto a lista de vertices ainda nao tiver acabado        
			while (openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);

				//Se achar a posiçao, faz o caminho 
				if (currentNode == targetNode)
				{
					sw.Stop();
					Debug.Log("Path found: " + sw.ElapsedMilliseconds + " ms");
					return RetracePath(startNode, targetNode);
				}

				foreach (Node neighbour in this.GetNeighbours(currentNode, includeDiagonals))
				{
					if (!currentNode.CanGoToNode(neighbour) /*!neighbour.walkable*/ || closedSet.Contains(neighbour))
					{
						continue;
					}

					float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
						//else
						//	openSet.UpdateItem(neighbour);
					}
				}
			}
			Debug.Log("NULLL");
			return null;
		}

		/*Devolve o caminho do inicio ao fim*/
		List<Node> RetracePath(Node startNode, Node endNode)
		{
			List<Node> path = new List<Node>();
			Node currentNode = endNode;

			while (currentNode != startNode)
			{
				path.Add(currentNode);
				currentNode = currentNode.parent;
			}
			path.Add(startNode);
			path.Reverse();

			//Isso era so pra debug
			//this.path = path;

			return path;
		}


		/*Retorna a distancia sem contar a altura*/
		float GetDistance(Node nodeA, Node nodeB)
		{
			return Vector3.Distance(new Vector3(nodeA.worldPosition.x, 0, nodeA.worldPosition.z), new Vector3(nodeB.worldPosition.x, 0, nodeB.worldPosition.z));
		}

		/*Retrona a distancia considerando os pontos do grafo na matriz e ignorando a posição do mundo*/
		//int GetDistance(Node nodeA, Node nodeB) {
		//    int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		//    int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		//
		//    if (dstX > dstY)
		//        return 14 * dstY + 10 * (dstX - dstY);
		//    return 14 * dstX + 10 * (dstY - dstX);
		//}

		/*Checa cada vertice da lista do caminho atual e confere se algum obstaculo entrou na frente enquanto ele estava sendo utilizado*/
		//public bool checaObstaculoNoCaminho(List<Node> ListaCaminho)
		//{
		//    int count = ListaCaminho.Count;
		//    for (int i = 0; i < count; i++)
		//    {
		//        if (!ListaCaminho[i].walkable)
		//            return true;
		//    }
		//    return false;
		//}

		/*Faz as verificações necessárias para saber qual a melhor forma de encontrar o caminho para uma unidade*/
		//public Node EncontraCaminho(Vector3 origem, Vector3 destino, ref List<Node> listaCaminho, bool inputAlterouDestino)
		//{
		//    Node nodeOrigem = GetNodeFromWorldPoint(origem);
		//    Node nodeDestino = GetNodeFromWorldPoint(destino);
		//
		//    //Se o destino nao for walkable, ele pega o vertice verde mais perto vai direto pra ele
		//    if (!nodeDestino.walkable)
		//    {
		//        //Se for nativo, pega o que ta salvo
		//        if (nodeDestino.vermelhoNativo && nodeDestino.nodeVerdeMaisProximo != null)
		//        {
		//            Debug.Log("To nos nativos");
		//            nodeDestino = nodeDestino.nodeVerdeMaisProximo;
		//            destino = nodeDestino.worldPosition;
		//        }
		//        else
		//        {
		//            //Caso contrario, busca em runtime
		//            Node hit;
		//            RaycastOnGrid(origem, nodeDestino.worldPosition, out hit);
		//            if (hit != null) nodeDestino = hit;
		//
		//            //nodeDestino = BuscaLargura(nodeDestino);//SUBSTITUIR POR ESSE SE O BAKE REALMENTE FOR PRA FRENTE...
		//
		//            destino = nodeDestino.worldPosition;
		//        }
		//    }
		//
		//    /*Checa se o raycast do grid vai bater em algo. Se bater, o A* é calculado, do contrario, o personagem segue a lista de caminho gerada no raycast*/
		//    /*Enche o caminho inicial com os pontos que vierem do raycast*/
		//    Node nodeHit;//Recebe 'null' se não bater em nada, nodeHit recebe o primeiro node VERMELHO encontrado, porem a lista retorna so os verdes, sem o nodeHit
		//    List<Node> listaCaminhoTemp = RaycastOnGrid(nodeOrigem.worldPosition, nodeDestino.worldPosition, out nodeHit);
		//    int listaCount = listaCaminhoTemp.Count;
		//    //if (!nodeDestino.walkable) {
		//    //    //Se o destino nao for andavel
		//    //    nodeDestino = nodeHit;
		//    //    //nodeDestino = 
		//    //}        
		//
		//    //Se o proximo caminho dele for um node vermelho, e o destino selecionado tambem for um node vermelho ele nao pode mais andarandar.
		//    if (listaCount > 0 && !listaCaminhoTemp[0].walkable && !nodeDestino.walkable)
		//    {
		//        return nodeOrigem;
		//    }
		//
		//    if (nodeHit == null && listaCaminhoTemp.Count > 0)
		//    {
		//        //[CAMINHO SEM A*] Se nao precisar do A*, o destino é o ultimo caminho do Raycast
		//        //if (nodeHit == null) {
		//        //Se ele puder andar em frente, vai de boas
		//        listaCaminho = listaCaminhoTemp;
		//        nodeDestino = listaCaminho[listaCaminho.Count - 1];
		//
		//    }
		//    else if (nodeHit != null && nodeDestino.walkable)
		//    {
		//
		//        //Se ja chegou, entao para.
		//        if (nodeOrigem == nodeDestino)
		//        {
		//            listaCaminho = new List<Node>();
		//            return null;
		//        }
		//
		//        ////TENHO QUE ARRUMAR ESSE AQUI AINDA, OS POVOS AINDA PODEM FICAR TUDO NUM MESMO LUGAR, VOU ARMARZENAR NO OBJETO "Node" QUEM JA ESTIVER EM CIMA, SOH VAI PODER TER 1 POR NODE
		//        //Busca por um vizinho livre caso o destino atual dele não seja mais andável
		//        //if(!nodeDestino.walkable){			
		//        //	List<Node> vizinhosDestino = GetNeighbours (GridCreator.PathfindingSingleton.NodeFromWorldPoint (destino));
		//        //	foreach (Node viz in vizinhosDestino ) {
		//        //		if(viz.walkable){
		//        //			destino = viz.worldPosition;
		//        //			listaCaminho = GridCreator.PathfindingSingleton.FindPath (origem, destino);
		//        //			return listaCaminho [0];
		//        //		}
		//        //	}
		//        //}
		//
		//        //Remove nodes da lista sempre que alcança eles.
		//        if (listaCaminho != null && listaCaminho.Count > 0)
		//        {
		//            if (listaCaminho[0] == nodeOrigem)
		//                listaCaminho.Remove(listaCaminho[0]);
		//            nodeDestino = listaCaminho.Count > 0 ? listaCaminho[0] : null;
		//        }
		//
		//        //[CAMINHO COM CALCULO A*]Checa se precisa fazer o calculo do algoritmo A* de novo: Se der o input de clicar, ou se entrar um obstaculo na frente
		//        if (inputAlterouDestino || listaCaminho != null && listaCaminho.Count > 0 && GridCreator.PathfindingSingleton.checaObstaculoNoCaminho(listaCaminho))
		//        {
		//            listaCaminho = GridCreator.PathfindingSingleton.Astar(origem, destino);
		//            //if (listaCaminho == null || listaCaminho[0] == null) {
		//            //    Debug.Log("Raycastou...");
		//            //    listaCaminho = listaCaminhoTemp;
		//            //    nodeDestino = nodeHit;//listaCaminho[listaCaminho.Count - 1];
		//            //} else {
		//            nodeDestino = listaCaminho[0];
		//            //}
		//        }
		//    }
		//    return nodeDestino;
		//}

		/*Traça uma reta em cada ponto do node até encontrar algum obstaculo, retorna o caminho feito pela reta (convertido em nodes)*/
		public List<Node> RaycastOnGrid(Vector3 origem, Vector3 destino, out Node hit)
		{
			hit = null;
			Node nodeOrigem = GetNodeFromWorldPoint(origem);
			Node nodeDestino = GetNodeFromWorldPoint(destino);
			Vector3 aux = (destino - origem).normalized * nodeScale;

			//Lista pra retornar o "raycast"
			List<Node> tempList = new List<Node>();
			//List<Node> listaNegocos = new List<Node>();

			float guardaUltimaMags = (origem - destino).magnitude;

			//Faz a busca ate chegar no ultimo
			while (origem != destino)
			{
				origem += aux;//Anda com o ponto

				float magAtual = (origem - destino).magnitude;

				//Adiciona pra lista do caminho feito
				Node nodeAtual = GetNodeFromWorldPoint(origem);
				tempList.Add(nodeAtual);

				//Se chegar em algum node que não da pra andar, então sai do metodo
				if (!nodeAtual.IsWalkable)
				{
					hit = GetNodeFromWorldPoint(origem);//Assim colocava o hit como o vermelho
														//hit = tempList[tempList.Count - 1];//Assim retorna o ultimo verde batido
					break;
				}
				//Se chegar aqui chegou no fim e não bateu em canto nenhum.
				if (magAtual > guardaUltimaMags)
				{
					break;
				}
				guardaUltimaMags = magAtual;
			}

			//Remove nodes da lista sempre que alcança eles.
			//if (tempList != null && tempList.Count > 0) {
			//        Debug.Log("EIA SIA FIAO ANDA" + (tempList[0].worldPosition - nodeOrigem.worldPosition).magnitude);
			//    if (tempList[0] == nodeOrigem) {
			//        tempList.Remove(tempList[0]);
			//    }
			//    nodeDestino = tempList.Count > 0 ? tempList[0] : null;
			//}

			////Debug
			//if (amostraRaycast)
			//{
			//    for (int x = 0; x < tempList.Count; x++)
			//    {
			//        if (x != tempList.Count - 1)
			//            Debug.DrawLine(tempList[x].worldPosition, tempList[x + 1].worldPosition, Color.blue);
			//    }
			//    //Mostra o hit
			//    if (hit != null)
			//        Debug.DrawRay(hit.worldPosition, Vector3.up * 2, Color.black);
			//}
			return tempList;
		}
		#endregion

		/*Encontra todos os vizinhos de um dado vertice*/
		//public List<Node> GetNeighbours(Node node)
		//{
		//    //ESSE METODO PRECISA DE UM PARAMETRO
		//    //QUE FAÇA O VIZINHO SER COM DIAGONAL OU SEM DIAGONAL
		//    List<Node> neighbours = new List<Node>();
		//
		//    for (int x = -1; x <= 1; x++)
		//    {
		//        for (int y = -1; y <= 1; y++)
		//        {
		//            if (x == 0 && y == 0)
		//                continue;
		//
		//            int checkX = node.x + x;
		//            int checkY = node.y + y;
		//
		//            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
		//            {
		//                neighbours.Add(grid[checkX, checkY]);
		//            }
		//        }
		//    }
		//    return neighbours;
		//}
		public List<Node> GetNeighbours(Node node, bool includeDiagonals = false)
		{
			List<Node> neighbours = new List<Node>();

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					//Pula o proprio node
					if (x == 0 && y == 0)
						continue;
					//Pula diagonais, se necessario
					if (!includeDiagonals)
					{
						if (x == -1 && y == 1)
							continue;
						if (x == 1 && y == 1)
							continue;
						if (x == -1 && y == -1)
							continue;
						if (x == 1 && y == -1)
							continue;
					}

					int checkX = node.x + x;
					int checkY = node.y + y;

					if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
					{
						neighbours.Add(grid[checkX, checkY]);
					}
				}
			}
			return neighbours;
		}

		/*Busca em largura que retorna todos os nodes 'verdes' encontrados até um certo nivel*/
		public List<Node> BreadthFirstSearch(Node origem, int nivelMaximoProfundidade, bool buscarSomenteWalkables)
		{
			List<Node> visitados = new List<Node>();
			Queue<Node> fila = new Queue<Node>();
			List<Node> listaVistadosDesseNivel = new List<Node>();

			fila.Enqueue(origem);
			visitados.Add(origem);

			//Parametros para achar a profundidade da busca
			List<Node> walkableNodes = new List<Node>();
			int nivelQueEstou = 0;
			int proximoNivel = 1;
			int i = 0;

			//Procura ate todos da fila sejam analisados
			while (fila.Count != 0)
			{
				//Guarda primeiro elemento da fila e tira ele de la
				Node vAtual = fila.Dequeue();

				//Pega os vizinhos desse node
				List<Node> vizinhos = GetNeighbours(vAtual, true);

				//Checa em que nivel estou, e avanca sempre que pode.
				if (i >= proximoNivel)
				{
					nivelQueEstou++;
					proximoNivel += 8 * nivelQueEstou; // Pra achar o proximo nivel, sempre procuro por (SUM[n*i++])+1;
				}
				//Se chegar no nivel maximo pedido, sai do metodo
				if (nivelQueEstou >= nivelMaximoProfundidade)
				{
					break;
				}

				//Para cada vertice vizinho do meu vertice atual, procuro por algum que ainda nao foi visitado
				foreach (Node item in vizinhos)
				{
					if (!visitados.Contains(item))
					{
						fila.Enqueue(item);
						visitados.Add(item);

						//Adiciona os vertices buscados para a lista de retorno
						if (item.IsWalkable)
						{
							walkableNodes.Add(item);
						}
						else
						{
							if (!buscarSomenteWalkables)
								walkableNodes.Add(item);
						}
					}
				}
				i++;
			}
			return walkableNodes;
		}

		/*Busca em largura que retorna somente o node encontrado mais perto da origem*/
		public Node BreadthFirstSearch(Node origem)
		{
			List<Node> visitados = new List<Node>();
			Queue<Node> fila = new Queue<Node>();

			fila.Enqueue(origem);
			visitados.Add(origem);

			//Parametros para achar a profundidade da busca
			List<Node> walkableNodes = new List<Node>();
			int nivelQueEstou = 0;
			int proximoNivel = 1;
			int i = 0;
			//Encontrar node com menor distancia
			Node nodeMaisPertoDaOrigem = null;
			float menorDist = float.MaxValue;

			//Procura ate todos da fila sejam analisados
			while (fila.Count != 0)
			{
				//Guarda primeiro elemento da fila e tira ele de la
				Node vAtual = fila.Dequeue();

				//Pega os vizinhos desse node
				List<Node> vizinhos = GetNeighbours(vAtual, true);

				//Checa em que nivel estou, e avanca sempre que pode.
				if (i >= proximoNivel)
				{
					nivelQueEstou++;
					proximoNivel += 8 * nivelQueEstou; // Pra achar o proximo nivel, sempre procuro por (SUM[n*i++])+1;

					//Se chegar na trocagem de nivel mas ja tiver pelo menos um node mais perto ja sai daqui.
					if (nodeMaisPertoDaOrigem != null || nivelQueEstou >= 50)
					{
						//Debug.Log("GOGO VAZA");
						break;
					}
				}

				//Para cada vertice vizinho do meu vertice atual, procuro por algum que ainda nao foi visitado
				foreach (Node item in vizinhos)
				{
					if (!visitados.Contains(item))
					{
						fila.Enqueue(item);
						visitados.Add(item);

						//Encontra o item com menor distancia
						if (item.IsWalkable)
						{
							//Procura pelo node walkable mais perto da origem
							float distanciaAtual = Vector3.Distance(origem.worldPosition, item.worldPosition);
							if (distanciaAtual < menorDist)
							{
								nodeMaisPertoDaOrigem = item;
								menorDist = distanciaAtual;
							}
						}
					}
				}

				i++;
			}
			return nodeMaisPertoDaOrigem;
		}
	}
}