using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public static class HydraulicErosion
    {
        private static KernelType kernelType;
        private static float[,] waterMap;
        private static float[,] sedimentMap;

        private static float rain = 0.25f;
        private static float solubility = 0.1f;
        private static float evaporation = 0.4f;
        private static float capacity = 0.01f;
        private static int rows;
        private static int cols;

        // public HydraulicErosion(KernelType kt,
        //         float k_rain, float k_solubility, float k_evaporation, float k_capacity)
        // {
        //     this.k_rain = k_rain;
        //     this.k_solubility = k_solubility;
        //     this.k_evaporation = k_evaporation;
        //     this.k_capacity = k_capacity;

        //     kernelType = kt;
        // }

        public static void ApplyErosion(float[,] heightMap, int iterations, KernelType kt,
                float k_rain, float k_solubility, float k_evaporation, float k_capacity)
        {
            //Asignamos los valores del user
            rain = k_rain;
            solubility = k_solubility;
            evaporation = k_evaporation;
            capacity = k_capacity;

            kernelType = kt;
            //Obtenemos las filas y columnas
            rows = heightMap.GetLength(0);
            cols = heightMap.GetLength(1);
            //Creamos un mapa de agua
            waterMap = new float[rows, cols];
            //Y un mapa de sedimento
            sedimentMap = new float[rows, cols];

            //Aqui Realizamos todas las iteraciones
            for (int pass = 0; pass < iterations; ++pass)
            {
                Rain();
                Erosion(heightMap);
                Transfer(heightMap);
                Evaporate(heightMap);
            }
        }

        /**
         Step 1: Add new water.
        */
        //Aqui llenamos el watermap con el valor de rain (0.25f)
        private static void Rain()
        {
            for (int i = 0; i < waterMap.GetLength(0); i++)
                for (int j = 0; j < waterMap.GetLength(1); j++)
                    waterMap[i, j] += rain;
        }

        /**
        Step 2: Erode
        */
        private static void Erosion(float[,] heightMap)
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    //Restamos a cada uno de los valores del heightmap 
                    //solubility(0.1f) * watermap(0.25f) = 0.025f  
                    // 0, 0.1,....0.9,1 - 0.1f * 0.25
                    heightMap[i, j] -= solubility * waterMap[i, j];

                    //Sumamos el valor que restamos del Heightmap al SedimentMap
                    sedimentMap[i, j] += solubility * waterMap[i, j];
                }
            }
        }

        /**
         Step 3: Transfer materials 
        */
        private static void Transfer(float[,] heightMap)
        {
            //Creamos estos Delta Maps, del mismo tamaño
            float[,] delta_watermap = new float[heightMap.GetLength(0), heightMap.GetLength(1)];
            float[,] delta_sedimentmap = new float[heightMap.GetLength(0), heightMap.GetLength(1)];

            //Hacemos este proceso para cada elemento en el HeightMap
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    //Calculamos el centro
                    Vector2Int center = new Vector2Int(i, j);
                    //Obtenemos w del waterMap que contiene el valor de rain(0.25) en todos sus elementos
                    float w = waterMap[center.y, center.x];
                    //Obtenemos h del Heightmap, que ya ha sido reducido un poco
                    float h = heightMap[center.y, center.x];
                    //Obtenemos m del sedimentMap que es el valor resultante, que removimos del HeightMap(0.025f)
                    float m = sedimentMap[center.y, center.x];

                    //a = heightMap + waterMap
                    //a = (0.1,...1) + 0.25
                    float a = h + w;
                    
                    //Creamos listas de ayuda
                    // difference List, Aqui van las diferencias de altura entre cada vecino y su centro
                    List<float> dlist = new List<float>();

                    //Aqui agregamos la altura de cada vecino + el WaterMap, que es una constante(0,25 default)
                    List<float> alist = new List<float>();

                    //Calculamos los vecinos de acuerdo al KernelType
                    var nbs = Neighbours(center, rows, cols);

                    //Iteramos sobre cada vecino, 
                    //Aqui basicamente verificamos cuales de nuestros vecinos estan por debajo de nosostros
                    // Y si lo estan sacamos 2 datos, la diferencia de altura entre el centro
                    //y el vecino(deltaHeight), y la altura del Vecino
                    //Notar que estos 2 valores tambien tienen sumado la Watermap
                    //Pero como es una constante para todos, podemos ignorarlo
                    for (int it = 0; it != nbs.Count;)
                    {
                        //ai = waterMap[vecino] + heightMap[vecino];
                        //ai = 0.25 + (0,...,1)
                        float ai = waterMap[nbs[it].y, nbs[it].x] + heightMap[nbs[it].y, nbs[it].x];

                        //ai < a => (a = (0.1,...1) + 0.25)
                        //Como el WaterMap es constante, basicamente aqui
                        //Vemos si la altura de cada vecino es menor a la del centro
                        if (ai < a)
                        {
                            //Si es menor la altura el vecino
                            //Agregamos la diferencia de altura
                            //a la dlist
                            dlist.Add(a - ai);

                            //Agregamos a la aList la altura del vecino + su waterMap(0.25)
                            alist.Add(ai);
                            //Iteramos el siguiente vecino
                            ++it;
                        }
                        // si la altura del vecino es mayor o igual al del centro
                        else
                        //Removemos este vecino
                            nbs.RemoveAt(it);
                    }

                    //Este valor es la Suma total de las diferencias de altura, entre el centro y sus
                    //vecinos(los que estan por debajo del centro en terminos de altura)
                    float d_total = Utils.Sum(dlist);

                    //Diferencia entre el Promedio total de las alturas(altura + waterMap(0.25)), de los 
                    //vecinos seleccionados, y heightmap[center] + watermap[center]
                    //Basicamente => delta_a =  altura central - altura promedio de los vecinos
                    float delta_a = a - Utils.Average(alist);

                    //Obtenemos el valor menor entre Watermap(0.25) y delta_a
                    float min_val = Mathf.Min(w, delta_a);

                    //Iteramos entre todos los vecinos restantes(que su altura es menor a la del centro)

                    for (int k = 0; k < nbs.Count; ++k)
                    {
                        // Esto es igual a  
                        //al valor Minimo * diferencia[Vecino] / La suma de las diferencias totales
                        float delta_wi = min_val * dlist[k] / d_total;

                        
                        //En el delta_waterMap hacemos el transporte de material
                        //del centro al vecino actual, por la cantidad de delta_wi
                        MoveMaterial(delta_watermap, center, nbs[k], delta_wi);


                        //delta_mi = 0.025 * delta_wi / 0.25
                        float delta_mi = m * delta_wi / w;

                        //En el delta_sedimentMap hacemos el transporte de material
                        //del centro al vecino actual, por la cantidad de delta_mi
                        MoveMaterial(delta_sedimentmap, center, nbs[k], delta_mi);
                    }
                }
            }
            // Apply difference operation
            //Sobre el WaterMap Sumamos el WaterMap + delta_waterMap
            Add(waterMap, delta_watermap, waterMap);
            //Sobre el Sediment_map sumamos el delta_sedimentmap
            Add(sedimentMap, delta_sedimentmap, sedimentMap);
        }

        /**
        * Step 4: Evaporation
        */
        private static void Evaporate(float[,] heightMap)
        {
            for (int i = 0; i < waterMap.GetLength(0); i++)
                for (int j = 0; j < waterMap.GetLength(1); j++)
                    //Multiplicamos el waterMap * el inverso de evaporation(0.4)
                    //Esto basicamente escala el WaterMap
                    waterMap[i, j] *= (1.0f - evaporation);


            //Creamos un delta_sedimentmap
            float[,] delta_sedimentmap = new float[heightMap.GetLength(0), heightMap.GetLength(1)];

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    // Esto es 0.01 * waterMap escalado
                    float max_sediment = capacity * waterMap[i, j];
                    
                    //Esto es el valor maximo entre 0 y la diferencia entre sedimentMap y max_sediment
                    //Sera 0 si max_sediment es mayor a sedimentMap
                    float delta_sediment = Mathf.Max(0f, sedimentMap[i, j] - max_sediment);

                    //Restamos el delta_sediment en el delta_sedimentMap
                    delta_sedimentmap[i, j] -= delta_sediment;

                    //Sumamos al heightMap delta_sediment
                    heightMap[i, j] += delta_sediment;
                }
            }

            //Sumamos el delta_sedimentMap al sedimentMap
            Add(sedimentMap, delta_sedimentmap, sedimentMap);
        }


        /**
         * Convenience function to transfer 
        */
        private static void MoveMaterial(float[,] heightMap, Vector2Int move_from, Vector2Int move_to, float amount)
        {
            heightMap[move_from.y, move_from.x] -= amount;
            heightMap[move_to.y, move_to.x] += amount;
        }

        public static float[,] GetSedimentMap()
        {
            float[,] sedimentmap_norm = null;
            Normalize(sedimentMap, sedimentmap_norm);
            return sedimentmap_norm;
        }

        public static float[,] GetWaterMap()
        {
            float[,] watermap_norm = null;
            Normalize(waterMap, watermap_norm);
            return watermap_norm;
        }

        private static void Add(float[,] mapOne, float[,] mapTwo, float[,] mapThree)
        {
            for (int i = 0; i < mapThree.GetLength(0); i++)
                for (int j = 0; j < mapThree.GetLength(1); j++)
                    mapThree[i, j] = mapOne[i, j] + mapTwo[i, j];
        }

        private static void Normalize(float[,] map, float[,] container)
        {
            container = new float[map.GetLength(0), map.GetLength(1)];
            float maxValue = float.MinValue;
            float minValue = float.MaxValue;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] > maxValue)
                        maxValue = map[i, j];
                    if (map[i, j] < minValue)
                        minValue = map[i, j];
                }
            }

            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    container[i, j] = Mathf.InverseLerp(minValue, maxValue, map[i, j]);
        }

        private static List<Vector2Int> Neighbours(Vector2Int point, int rows, int cols)
        {
            int x = point.x;
            int y = point.y;
            List<Vector2Int> points = new List<Vector2Int>();

            //Obtenemos los vecinos de acuerdo al Kernel
            switch (kernelType)
            {
                case KernelType.MOORE:
                    points.Add(new Vector2Int(x - 1, y - 1));
                    points.Add(new Vector2Int(x, y - 1));
                    points.Add(new Vector2Int(x + 1, y - 1));
                    points.Add(new Vector2Int(x - 1, y));
                    points.Add(new Vector2Int(x + 1, y));
                    points.Add(new Vector2Int(x - 1, y + 1));
                    points.Add(new Vector2Int(x, y + 1));
                    points.Add(new Vector2Int(x + 1, y + 1));
                    // points = new Vector2Int[] {
                    //     new Vector2Int(x - 1, y - 1), new Vector2Int(x, y - 1), new Vector2Int(x + 1, y - 1),
                    //     new Vector2Int(x - 1, y),                  new Vector2Int(x + 1, y),
                    //     new Vector2Int(x - 1, y + 1), new Vector2Int(x, y + 1), new Vector2Int(x + 1, y + 1)};
                    break;

                case KernelType.VON_NEUMANN:
                    points.Add(new Vector2Int(x, y - 1));
                    points.Add(new Vector2Int(x - 1, y));
                    points.Add(new Vector2Int(x + 1, y));
                    points.Add(new Vector2Int(x, y + 1));
                    // points = new Vector2Int[] {
                    //     new Vector2Int(x, y - 1), new Vector2Int(x - 1, y),new Vector2Int(x + 1, y),
                    //            new Vector2Int(x, y + 1)};
                    break;

                case KernelType.VON_NEUMANN2:
                    points.Add(new Vector2Int(x - 1, y - 1));
                    points.Add(new Vector2Int(x + 1, y - 1));
                    points.Add(new Vector2Int(x - 1, y + 1));
                    points.Add(new Vector2Int(x + 1, y + 1));
                    // points = new Vector2Int[] {
                    //     new Vector2Int(x - 1, y - 1), new Vector2Int(x + 1, y - 1),
                    //     new Vector2Int(x - 1, y + 1), new Vector2Int(x + 1, y + 1)};
                    break;
            }

            //Iteramos todos los puntos, y verificamos que esten dentro de los limites
            for (int it = 0; it != points.Count;)
            {
                if (!PointInRange(points[it], rows, cols))
                    points.RemoveAt(it);
                else
                    ++it;
            }
            return points;
        }

        private static bool PointInRange(Vector2Int point, int rows, int cols)
        {
            return point.x < cols && point.x >= 0 && point.y < rows && point.y >= 0;
        }
    }
}