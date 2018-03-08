using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CubeAxialConverter : MonoBehaviour {

	public Hexagon AxialToCube(Axial axial){
		return new Hexagon(axial.q, axial.r, -axial.q - axial.r);
	}

	Axial CubeToAxial(Hexagon hexagon){
		return new Axial(hexagon.x, hexagon.z);
	}
}

public class Axial : MonoBehaviour  {
	public int q;
	public int r;

	public Axial(int q, int r){
		this.q = q;
		this.r = r;
	}

}

public class Hexagon : MonoBehaviour  {
	[SerializeField]
	public int x;
	[SerializeField]
	public int y;
	[SerializeField]
	public int z;

	public Hexagon(int x, int y, int z){
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public bool Compare(Hexagon hexagon){
		if(this.x == hexagon.x && this.y == hexagon.y && this.z == hexagon.z){
			return true;
		}
		return false;
	}
}

public class Cube {
	public Hexagon[] Neighbor(Hexagon a){
		Hexagon[] neighbor = {
			new Hexagon(a.x+1, a.y-1, a.z), new Hexagon(a.x+1, a.y, a.z-1),new Hexagon(a.x, a.y+1, a.z-1),
			new Hexagon(a.x-1, a.y+1, a.z), new Hexagon(a.x-1, a.y, a.z+1), new Hexagon(a.x, a.y-1, a.z+1)
		};
		return neighbor;
	}

	public int Distance(Hexagon a, Hexagon b){
		return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z))/2;
	}

	Hexagon LinearInterpolation(Hexagon a, Hexagon b, float t){
		return new Hexagon((int)(a.x + (b.x - a.x) * t), (int)(a.y + (b.y - a.y) * t), (int)(a.z + (b.z - a.z) * t));
	}

	public Hexagon[] LineDrawing(Hexagon a, Hexagon b){
		int n = Distance(a, b);
		List<Hexagon> line = new List<Hexagon>();
		for(int i=0; i<n; i++){
			line.Add(LinearInterpolation(a, b, 1 / n * i));
		}
		return line.ToArray();
	}

	public Hexagon[] MovementRange(Hexagon center,int n){
		List<Hexagon> range = new List<Hexagon>();
		for(int dx=-n; dx<=n; dx++){
			int start = Math.Max(-n, -dx-n);
			int stop = Math.Min(n, -dx+n);
			for(int dy=start; dy<=stop; dy++){
				int dz = -dx-dy;
				range.Add(new Hexagon(center.x+dx, center.y+dy, center.z+dz));
			}
		}
		return range.ToArray();
	}

	public Hexagon[] CastRange(Hexagon center, int n){
		List<Hexagon> range = new List<Hexagon>();
		int xmin = center.x - n;
		int xmax = center.x + n;
		int ymin = center.y - n;
		int ymax = center.y + n;
		int zmin = center.z - n;
		int zmax = center.z + n;
		for(int x=xmin; x<=xmax; x++){
			int start = Math.Max(ymin, -x-zmax);
			int stop = Math.Min(ymax, -x-zmin);
			for(int y=start; y<=stop; y++){
				int z = -x-y;
				range.Add(new Hexagon(x, y, z));
			}
		}
		return range.ToArray();
	}

}
