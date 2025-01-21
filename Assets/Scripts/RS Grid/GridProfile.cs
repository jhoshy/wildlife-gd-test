using Jessinho;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GridProfile : ScriptableObject
{
	public int nodeScale = 1;

	public int width = 4, height = 4;

	public Node[] grid;
}
