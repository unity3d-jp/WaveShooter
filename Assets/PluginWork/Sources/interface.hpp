/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * interface.hpp - Project AnotherThread2
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
 * since Wed Jan 25 09:47:46 2017
 */
#ifndef	PLUGINSOURCES_INTERFACE_HPP_WED_JAN_25_09_47_46_2017
#define	PLUGINSOURCES_INTERFACE_HPP_WED_JAN_25_09_47_46_2017

#include "types.h"

extern "C" {
	DLLAPI float foo(float value);
	DLLAPI int ConvertArrayFromList(int unit_max,
									int32_t* alive_table_items_ptr,
									int32_t* positions_items_ptr,
									int32_t* velocities_items_ptr,
									int32_t* uv2_items_ptr,
									int32_t* vertices_items_ptr,
									int32_t* normals_items_ptr,
									int32_t* uv2s_items_ptr);
}

#endif	/* PLUGINSOURCES_INTERFACE_HPP_WED_JAN_25_09_47_46_2017 */
/*
 * End of interface.hpp
 */
