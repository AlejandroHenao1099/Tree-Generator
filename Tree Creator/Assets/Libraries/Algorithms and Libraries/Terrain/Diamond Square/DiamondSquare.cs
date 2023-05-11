// This code was converted from Java into C#
// The original Java code was found on Stack Overflow by user M. Jessup
// https://stackoverflow.com/questions/2755750/diamond-square-algorithm?newreg=ee2a40d2fe9f49b9b938151e933860d2

using System;

namespace Terrain
{
    public static class DiamondSquare
    {
        private static int _terrainPoints;
        private static float _roughness;
        private static float _seed; // an initial seed value for the corners of the data

        // public DiamondSquare(int terrainPoints, float roughness, float seed)
        // {
        //     this._terrainPoints = terrainPoints;
        //     this._roughness = roughness;
        //     this._seed = seed;
        // }

        public static float[,] GetData(int terrainPoints, float roughness, float seed)
        {
            _terrainPoints = terrainPoints;
            _roughness = roughness;
            _seed = seed;
            return diamondSquareAlgorithm();
        }

        private static float[,] diamondSquareAlgorithm()
        {
            //size of grid to generate, note this must be a
            //value 2^n+1
            int DATA_SIZE = _terrainPoints + 1;  // must be a power of two plus one e.g. 33, 65, 128, etc

            float[,] data = new float[DATA_SIZE, DATA_SIZE];
            data[0, 0] = data[0, DATA_SIZE - 1] = data[DATA_SIZE - 1, 0] =
              data[DATA_SIZE - 1, DATA_SIZE - 1] = _seed;

            float h = _roughness;//the range (-h -> +h) for the average offset - affects roughness
            Random r = new Random(_terrainPoints);//for the new value in range of h
                                    //side length is distance of a single square side
                                    //or distance of diagonal in diamond

            for (int sideLength = DATA_SIZE - 1;
                //side length must be >= 2 so we always have
                //a new value (if its 1 we overwrite existing values
                //on the last iteration)
                sideLength >= 2;
                //each iteration we are looking at smaller squares
                //diamonds, and we decrease the variation of the offset
                sideLength /= 2, h /= 2.0f)
            {
                //half the length of the side of a square
                //or distance from diamond center to one corner
                //(just to make calcs below a little clearer)
                int halfSide = sideLength / 2;

                //generate the new square values
                for (int x = 0; x < DATA_SIZE - 1; x += sideLength)
                {
                    for (int y = 0; y < DATA_SIZE - 1; y += sideLength)
                    {
                        //x, y is upper left corner of square
                        //calculate average of existing corners
                        float avg = data[x, y] + //top left
                        data[x + sideLength, y] +//top right
                        data[x, y + sideLength] + //lower left
                        data[x + sideLength, y + sideLength];//lower right
                        avg /= 4.0f;

                        //center is average plus random offset
                        data[x + halfSide, y + halfSide] =
                      //We calculate random value in range of 2h
                      //and then subtract h so the end value is
                      //in the range (-h, +h)
                      avg + ((float)r.NextDouble() * 2 * h) - h;
                    }
                }
                //generate the diamond values
                //since the diamonds are staggered we only move x
                //by half side
                //NOTE: if the data shouldn't wrap then x < DATA_SIZE
                //to generate the far edge values
                for (int x = 0; x < DATA_SIZE - 1; x += halfSide)
                {
                    //and y is x offset by half a side, but moved by
                    //the full side length
                    //NOTE: if the data shouldn't wrap then y < DATA_SIZE
                    //to generate the far edge values
                    for (int y = (x + halfSide) % sideLength; y < DATA_SIZE - 1; y += sideLength)
                    {
                        //x, y is center of diamond
                        //note we must use mod  and add DATA_SIZE for subtraction 
                        //so that we can wrap around the array to find the corners
                        float avg =
                          data[(x - halfSide + DATA_SIZE) % DATA_SIZE, y] + //left of center
                          data[(x + halfSide) % DATA_SIZE, y] + //right of center
                          data[x, (y + halfSide) % DATA_SIZE] + //below center
                          data[x, (y - halfSide + DATA_SIZE) % DATA_SIZE]; //above center
                        avg /= 4.0f;

                        //new value = average plus random offset
                        //We calculate random value in range of 2h
                        //and then subtract h so the end value is
                        //in the range (-h, +h)
                        avg = avg + ((float)r.NextDouble() * 2 * h) - h;
                        //update value for center of diamond
                        data[x, y] = avg;

                        //wrap values on the edges, remove
                        //this and adjust loop condition above
                        //for non-wrapping values.
                        if (x == 0) data[DATA_SIZE - 1, y] = avg;
                        if (y == 0) data[x, DATA_SIZE - 1] = avg;
                    }
                }
            }
            return data;
        }
    }
}
