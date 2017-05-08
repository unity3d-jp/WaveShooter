/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

namespace UTJ {

public class BoxingPool {
	// 44.0KiB for rectangle
	// ??KiB for octagonal
	const int MAX_SIZE_FOR_INT = 512*18;
	public static object[] IntList;
	public static void init()
	{
		IntList = new object[MAX_SIZE_FOR_INT];
		for (var i = 0; i < IntList.Length; ++i) {
			if (i % 8 == 0 || i % 18 == 0) {
				IntList[i] = (object)i;
			}
		}
	}
}

} // namespace UTJ {
/*
 * End of BoxingPool.cs
 */
