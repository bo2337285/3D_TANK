#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class LowPolyTool : MonoBehaviour {
    //制作Unity顶部菜单栏
    [MenuItem ("Convert Model/LowPoly")]
    static void LowPoly () {
        Transform[] transforms = Selection.transforms;

        for (int i = 0; i < transforms.Length; i++) {
            LowPoly (transforms[i]);
        }

    }

    static void LowPoly (Transform t) {

        MeshFilter meshFilter = t.GetComponent<MeshFilter> ();
        Mesh mesh = meshFilter.sharedMesh;

        Vector3[] oldVerts = mesh.vertices; //保存当前Mesh顶点  
        int[] triangles = mesh.triangles; //三角索引数组  
        Vector2[] olduvs = mesh.uv;
        BoneWeight[] oldBoneWeights = mesh.boneWeights;

        Vector3[] verts = new Vector3[triangles.Length]; //用于保存新的顶点信息 
        Vector2[] uvs = new Vector2[triangles.Length]; //用于保存新的uv信息 
        BoneWeight[] boneWeights = new BoneWeight[triangles.Length]; //用于保存新的骨骼信息 

        for (int i = 0; i < triangles.Length; i++) {
            verts[i] = oldVerts[triangles[i]];
            uvs[i] = olduvs[triangles[i]];
            boneWeights[i] = oldBoneWeights[triangles[i]];
            triangles[i] = i;
        }

        mesh.vertices = verts; //更新Mesh顶点  
        mesh.triangles = triangles; //更新索引 
        mesh.uv = uvs; //更新uv信息
        mesh.boneWeights = boneWeights; //更新骨骼信息

        mesh.RecalculateBounds (); //重新计算边界  
        mesh.RecalculateNormals (); //重新计算法线  
    }

}
#endif