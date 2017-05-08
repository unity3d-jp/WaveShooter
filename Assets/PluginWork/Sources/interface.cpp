/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * interface.cpp - Project AnotherThread2
 *
 * Copyright (c) 2017 Yuji YASUHARA
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * since Wed Jan 25 09:47:23 2017
 */

#include <stdio.h>
#include <math.h>
#include "Vector.hpp"
#include "Matrix.hpp"
#include "interface.hpp"

extern "C" {

// UTJ::Matrix4x4
// transpose(UTJ::Matrix4x4 m)
// {
// 	m.transpose();
// 	return m;
// }

float
foo(float value)
{
	return value * 3.0f;
}

int
ConvertArrayFromList(int unit_max,
					 int32_t* alive_table_items_ptr,
					 int32_t* positions_items_ptr,
					 int32_t* velocities_items_ptr,
					 int32_t* uv2_items_ptr,
					 int32_t* vertices_items_ptr,
					 int32_t* normals_items_ptr,
					 int32_t* uv2s_items_ptr)
{
	int32_t* alive_table = reinterpret_cast<int32_t*>(alive_table_items_ptr);
	UTJ::Vector3* positions = reinterpret_cast<UTJ::Vector3*>(positions_items_ptr);
	UTJ::Vector3* velocities = reinterpret_cast<UTJ::Vector3*>(velocities_items_ptr);
	UTJ::Vector2* uv2_list = reinterpret_cast<UTJ::Vector2*>(uv2_items_ptr);
	UTJ::Vector3* vertices = reinterpret_cast<UTJ::Vector3*>(vertices_items_ptr);
	UTJ::Vector3* normals = reinterpret_cast<UTJ::Vector3*>(normals_items_ptr);
	UTJ::Vector2* uv2s = reinterpret_cast<UTJ::Vector2*>(uv2s_items_ptr);

	int idx = 0;
	for (int i = 0; i < unit_max; ++i) {
		if (alive_table[i] != 0) {
			for (int j = 0; j < 8; ++j) {
				vertices[idx*8+j] = positions[i];
				normals[idx*8+j] = velocities[i];
				uv2s[idx*8+j] = uv2_list[i];
			}
			++idx;
		}
	}
	return idx;
}

} // extern "C" {
/*
 * End of interface.cpp
 */
