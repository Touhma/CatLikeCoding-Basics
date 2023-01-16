using _Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Graph
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] Transform pointPrefab;
        [SerializeField, Range(10, 100)] int resolution = 10;
        [SerializeField] FunctionLibrary.FunctionName function;
        [SerializeField, Min(0f)] float functionDuration = 1f, transitionDuration = 1f;

        public enum TransitionMode
        {
            Cycle,
            Random,
            None
        }

        [SerializeField] TransitionMode transitionMode;
        
        FunctionLibrary.FunctionName transitionFunction;

        bool transitioning;
        
        Transform[] points;
        float duration;

        public void Awake()
        {
            float step = 2f / resolution;
            float3 scale = Vector3.one * step;
            points = new Transform[resolution * resolution];
            for (int i = 0; i < points.Length; i++)
            {
                Transform point = points[i] = Instantiate(pointPrefab);
                point.localScale = scale;
                point.SetParent(transform, false);
            }
        }

        public void Update()
        {
            if (transitionMode == TransitionMode.None)
            {
                UpdateFunction();
            }
            else
            {
                duration += Time.deltaTime;
                if (transitioning) {
                    if (duration >= transitionDuration) {
                        duration -= transitionDuration;
                        transitioning = false;
                    }
                }
                else if (duration >= functionDuration) {
                    duration -= functionDuration;
                    transitioning = true;
                    transitionFunction = function;
                    PickNextFunction();
                }

                if (transitioning) {
                    UpdateFunctionTransition();
                }
                else {
                    UpdateFunction();
                }
            }
           
        }

        void UpdateFunction () {
            FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
            float time = Time.time;
            float step = 2f / resolution;
            float v = 0.5f * step - 1f;
            for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
                if (x == resolution) {
                    x = 0;
                    z += 1;
                    v = (z + 0.5f) * step - 1f;
                }
                float u = (x + 0.5f) * step - 1f;
                points[i].localPosition = f(u, v, time);
            }
        }
        
        void UpdateFunctionTransition () {
            FunctionLibrary.Function
                from = FunctionLibrary.GetFunction(transitionFunction),
                to = FunctionLibrary.GetFunction(function);
            float progress = duration / transitionDuration;
            float time = Time.time;
            float step = 2f / resolution;
            float v = 0.5f * step - 1f;
            for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
                if (x == resolution) {
                    x = 0;
                    z += 1;
                    v = (z + 0.5f) * step - 1f;
                }
                float u = (x + 0.5f) * step - 1f;
                points[i].localPosition = FunctionLibrary.Morph(
                    u, v, time, from, to, progress
                );
            }
        }

        void PickNextFunction () {
            function = transitionMode == TransitionMode.Cycle ?
                FunctionLibrary.GetNextFunctionName(function) :
                FunctionLibrary.GetRandomFunctionNameOtherThan(function);
        }
    }
}