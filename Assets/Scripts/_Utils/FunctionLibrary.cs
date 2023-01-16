using Unity.Mathematics;
using static UnityEngine.Mathf;
using Random = UnityEngine.Random;

namespace _Utils
{
    public static class FunctionLibrary
    {
        public delegate float3 Function(float u, float v, float t);

        public enum FunctionName
        {
            Wave,
            MultiWave,
            Ripple,
            Sphere,
            Torus
        }

        private static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Torus };

        public static Function GetFunction(FunctionName name)
        {
            return functions[(int)name];
        }
        
        public static FunctionName GetNextFunctionName (FunctionName name) {
            return (int)name < functions.Length - 1 ? name + 1 : 0;
        }
        
        public static FunctionName GetRandomFunctionName () {
            var choice = (FunctionName)Random.Range(0, functions.Length);
            return choice;
        }
        
        public static FunctionName GetRandomFunctionNameOtherThan (FunctionName name) {
            var choice = (FunctionName)Random.Range(1, functions.Length);
            return choice == name ? 0 : choice;
        }

        private static float3 Wave(float u, float v, float t)
        {
            return new float3(u, math.sin(PI * (u + v + t)), v);
        }

        private static float3 MultiWave(float u, float v, float t)
        {
            float3 p;
            p.x = u;
            p.y = math.sin(PI * (u + 0.5f * t));
            p.y += 0.5f * math.sin(2f * PI * (v + t));
            p.y += math.sin(PI * (u + v + 0.25f * t));
            p.y *= 1f / 2.5f;
            p.z = v;
            return p;
        }

        private static float3 Ripple(float u, float v, float t)
        {
            float d = Sqrt(u * u + v * v);
            float3 p;
            p.x = u;
            p.y = math.sin(PI * (4f * d - t));
            p.y /= 1f + 10f * d;
            p.z = v;
            return p;
        }

        private static float3 Sphere(float u, float v, float t)
        {
            float r = 0.9f + 0.1f * math.sin(PI * (6f * u + 4f * v + t));
            float s = r * math.cos(0.5f * PI * v);
            float3 p;
            p.x = s * math.sin(PI * u);
            p.y = r * math.sin(0.5f * PI * v);
            p.z = s * math.cos(PI * u);
            return p;
        }

        private static float3 Torus(float u, float v, float t)
        {
            float r1 = 0.7f + 0.1f * math.sin(PI * (6f * u + 0.5f * t));
            float r2 = 0.15f + 0.05f * math.sin(PI * (8f * u + 4f * v + 2f * t));
            float s = r1 + r2 * math.cos(PI * v);
            float3 p;
            p.x = s * math.sin(PI * u);
            p.y = r2 * math.sin(PI * v);
            p.z = s * math.cos(PI * u);
            return p;
        }

        public static float3 Morph(
            float u, float v, float t, Function from, Function to, float progress
        )
        {
            return math.unlerp(from(u, v, t), to(u, v, t), math.smoothstep(0f, 1f, progress));
        }
    }
}