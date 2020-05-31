using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RenderExtensions 
{
    // Start is called before the first frame update
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
	}
}