using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Jessinho
{
	[System.Serializable]
	public class Node : IHeapItem<Node>
	{
		public bool walkable; //Além de paredes, pode ser bloqueado manualmente por aqui, caso as unidades em cima dele ultrapassem o limite ou caso haja algum objeto por cima
							   //Retorna se esse node é 'walkable' (sem contar as paredes) - GET
		public bool IsWalkable
		{
			get { return walkable; }
		}

		public Vector3 worldPosition;
		public int x;
		public int y;

		//Custos e variaveis usadas no A*
		public float gCost;
		public float hCost;
		public Node parent;
		int heapIndex;

		private MovementManager entity;

		/*Construtor para o grid de pathfinding*/
		public Node(Vector3 _worldPos, int x, int y, bool _walkable = true)
		{
			walkable = _walkable;
			worldPosition = _worldPos;
			this.x = x;
			this.y = y;
		}
		/*Construtor para o grid de colisão*/
		public Node(Vector3 _worldPos, bool _isCollisionNode)
		{
			worldPosition = _worldPos;
			//Só inicialisa essa lista de colisores se estiver num grid de colisores
			//if(isCollisionNode) 
			//    colisoresNesseNode = new List<ColisorCirculo>();
		}

		public float fCost
		{
			get
			{
				return gCost + hCost;
			}
		}

		public int HeapIndex
		{
			get
			{
				return heapIndex;
			}
			set
			{
				heapIndex = value;
			}
		}

		public int CompareTo(Node nodeToCompare)
		{
			int compare = fCost.CompareTo(nodeToCompare.fCost);
			if (compare == 0)
			{
				compare = hCost.CompareTo(nodeToCompare.hCost);
			}
			return -compare;
		}

		//Checa se é possivel ir pra esse node - Se houver parede ou bloqueio no node, não deixa ir.
		public bool CanGoToNode(Node node)
		{
			return !wallNodes.Contains(node) && node.walkable;
		}

		//Atualiza se é 'walkable' ou não - SET
		public void PathUpdate(bool walkable)
		{
			this.walkable = walkable;
		}

		//Lista de nodes que tem caminho bloqueado saindo do ponto deste node
		List<Node> wallNodes = new List<Node>();
		public void AddWallToNode(Node node)
		{
			//Adiciona o vertice na lista caso ele ainda não exista
			if (!wallNodes.Contains(node))
				wallNodes.Add(node);
		}
		public void RemoveWallFromNode(Node node)
		{
			//Remove o vertice da lista caso ele ainda exista
			if (wallNodes.Contains(node))
				wallNodes.Remove(node);
			//Remove a porta, se houver
			if (doorNodes.Contains(node))
				doorNodes.Remove(node);
		}
		//Verificação se contem um node bloqueado por parede, pra caso precise
		public bool ContainsWallInNode(Node node)
		{
			return wallNodes.Contains(node);
		}

		//Porta
		List<Node> doorNodes = new List<Node>();
		public void AddDoorToNode(Node node)
		{
			//Adiciona uma parede caso ainda não exista (pois toda porta precisa de uma parede)
			if (!wallNodes.Contains(node))
				wallNodes.Add(node);
			//E depois adiciona uma porta
			if (!doorNodes.Contains(node))
				doorNodes.Add(node);
		}
		public void RemoveDoorFromNode(Node node)
		{
			//Remove a porta
			if (doorNodes.Contains(node))
				doorNodes.Remove(node);
			//E depois remove a parede
			//if (wallNodes.Contains(node))
			//    wallNodes.Remove(node);
		}
		public bool ContainsDoorInNode(Node node)
		{
			return doorNodes.Contains(node);
		}

		//Para usar In-Game (os de cima é pra editar fase, vou organizar isso tudo depois)
		public void OpenDoor(Node node)
		{
			if (doorNodes.Contains(node))
			{
				if (wallNodes.Contains(node))
				{
					wallNodes.Remove(node);
				}
			}
		}
	}
}