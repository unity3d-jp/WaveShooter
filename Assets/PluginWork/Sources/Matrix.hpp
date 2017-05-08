/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * Matrix.hpp - Project AnotherThread2
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
 * since Sun Feb 12 15:54:19 2017
 */
#ifndef	PLUGINSOURCES_MATRIX_HPP_SUN_FEB_12_15_54_19_2017
#define	PLUGINSOURCES_MATRIX_HPP_SUN_FEB_12_15_54_19_2017

#include <utility>
#include "Vector.hpp"

namespace UTJ {

class Matrix4x4
{
private:
	typedef Matrix4x4 ThisClass;
	float m00_;
	float m10_;
	float m20_;
	float m30_;
	float m01_;
	float m11_;
	float m21_;
	float m31_;
	float m02_;
	float m12_;
	float m22_;
	float m32_;
	float m03_;
	float m13_;
	float m23_;
	float m33_;

public:
	Matrix4x4()
	: m00_(1.0f), m10_(0.0f), m20_(0.0f), m30_(0.0f)
	, m01_(0.0f), m11_(1.0f), m21_(0.0f), m31_(0.0f)
	, m02_(0.0f), m12_(0.0f), m22_(1.0f), m32_(0.0f)
	, m03_(0.0f), m13_(0.0f), m23_(0.0f), m33_(1.0f){}
	static inline void swap(float& a, float& b) { float t = a; a = b; b = t; }
	void transpose() {
		swap(m01_, m10_);
		swap(m02_, m20_);
		swap(m03_, m30_);
		swap(m12_, m21_);
		swap(m13_, m31_);
		swap(m23_, m32_);
	}
};

} // namespace UTJ {

#endif	/* PLUGINSOURCES_MATRIX_HPP_SUN_FEB_12_15_54_19_2017 */
/*
 * End of Matrix.hpp
 */
