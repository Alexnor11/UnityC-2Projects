using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeExample : MonoBehaviour
{
    private void Awake()
    {
        //print(Add(1));
        //print(Add(1,2));
        //print(Add(1,2,3));
        print(Add(1, 2, 3, 4));

        print(Fac(5));
        print(Fac(0));
        print(Fac(-5));

    }

    // params можно позволить функции принимать произвольное количество параметров
    // одного типа.
    

        int Add(params int[] ints)
    {
        int sum = 0;
        foreach (int i in ints) { 
            sum += i;
        }
        return sum;
    }

    // вычисления факториала любого целого числа можно написать рекурсивную
    // функцию Fac():

    int Fac(int n)
    {
        if (n < 0)
        {
            return 0;
        }
        if (n == 0) {  
            return 1; 
        }
        int result = n * Fac(n - 1);
        return result;
    }  
    
}
