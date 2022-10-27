using UnityEngine;

namespace KK
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] GameObject floor, upperWallLeft, upperWallRight, lowerWallLeft, lowerWallRight;
        [SerializeField] float tileSize;
        [SerializeField] int roomSize;

        int[,] roomStructure;

        void Awake()
        {
            GenerateLevel();
        }

        void GenerateLevel()
        {
            roomStructure = new int[roomSize, roomSize];

            int start = 0;
            int end = 0;

            for (int i = 0; i < roomSize; i++)
            {
                if (i == 0)
                {
                    start = Random.Range(0, roomSize - 1);

                    end = Random.Range(start + 1, roomSize);
                }
                else
                {
                    int currentLength = end - start;

                    if (GetRandomBool())
                    {
                        //random left offset
                        if (start > 0)
                        {
                            start = start - Random.Range(1, currentLength);
                            if (start < 0)
                                start = 0;
                        }
                        else if (currentLength >= roomSize * 0.8f)
                            start = start + Random.Range(1, currentLength / 2);
                    }

                    if (GetRandomBool())
                    {
                        //random right offset
                        if (end < roomSize - 1)
                        {
                            end = end + Random.Range(1, currentLength);
                            if (end > roomSize - 1)
                                end = roomSize - 1;
                        }
                        else if (currentLength >= roomSize * 0.8f)
                            end = end - Random.Range(1, currentLength / 2);
                    }
                }


                for (int j = start; j < end + 1; j++)
                {
                    roomStructure[i, j] = 1;
                }
            }



            //test
            string test = "";

            for (int i = 0; i < roomStructure.GetLength(0); i++)
            {
                test = "";
                for (int j = 0; j < roomStructure.GetLength(1); j++)
                {
                    test += roomStructure[i, j];
                }
                print(test);
            }

        }

        bool GetRandomBool()
        {
            return Random.Range(0, 2) == 0;
        }
    }
}
