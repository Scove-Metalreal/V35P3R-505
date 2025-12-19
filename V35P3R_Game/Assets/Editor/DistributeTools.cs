using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class DistributeTools
    {
        [MenuItem("Tools/Distribute/Along X Axis")]
        static void DistributeX() => Distribute(0);

        [MenuItem("Tools/Distribute/Along Y Axis")]
        static void DistributeY() => Distribute(1);

        [MenuItem("Tools/Distribute/Along Z Axis")]
        static void DistributeZ() => Distribute(2);

        static void Distribute(int axis) // 0=x, 1=y, 2=z
        {
            Transform[] selection = Selection.transforms;
            if (selection.Length < 3) return;

            Undo.RecordObjects(selection, "Distribute Objects");

            // Sort selection by position on that axis so we distribute in order
            var sorted = selection.OrderBy(t => t.position[axis]).ToList();

            float start = sorted.First().position[axis];
            float end = sorted.Last().position[axis];
            float distance = end - start;
            float step = distance / (sorted.Count - 1);

            for (int i = 0; i < sorted.Count; i++)
            {
                Vector3 newPos = sorted[i].position;
                newPos[axis] = start + (step * i);
                sorted[i].position = newPos;
            }
        }
    }
}