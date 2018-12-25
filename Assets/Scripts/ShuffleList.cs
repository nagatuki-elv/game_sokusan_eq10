using System.Collections.Generic;
using UnityEngine;

public static class ShuffleList{
	/// <summary>
	/// シャッフル
	/// </summary>
	/// <param name="list"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static List<T> Shuffle<T>(this List<T> list){
	    for (int i = 0; i < list.Count; i++) {
			T temp = list[i];
			int randomIndex = Random.Range(0, list.Count);
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
	    }

    	return list;
	}
}