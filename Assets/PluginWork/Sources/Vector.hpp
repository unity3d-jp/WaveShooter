/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * Vector.hpp - Project AnotherThread2
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
 * since Wed Jan 25 09:46:56 2017
 */
#ifndef	PLUGINSOURCES_VECTOR_HPP_WED_JAN_25_09_46_56_2017
#define	PLUGINSOURCES_VECTOR_HPP_WED_JAN_25_09_46_56_2017

#include <math.h>

namespace UTJ {

class Vector3
{
private:
	typedef Vector3 ThisClass;
	float x_, y_, z_;

public:
	Vector3()
		: x_(0.0f)
		, y_(0.0f)
		, z_(0.0f)
	{}
	Vector3(float x, float y, float z)
		: x_(x)
		, y_(y)
		, z_(z)
	{}
	
	float getX() const { return x_; }
	float getY() const { return y_; }
	float getZ() const { return z_; }
	void setX(float v) { x_ = v; }
	void setY(float v) { y_ = v; }
	void setZ(float v) { z_ = v; }
	void set(float x, float y, float z) {
		x_ = x;
		y_ = y;
		z_ = z;
	}
	void setZero() {
		x_ = 0.0f;
		y_ = 0.0f;
		z_ = 0.0f;
	}

	ThisClass& operator=(const ThisClass& another)
	{
		x_ = another.x_;
		y_ = another.y_;
		z_ = another.z_;
		return *this;
	}
	ThisClass& operator+=(const ThisClass& another)
	{
		x_ += another.x_;
		y_ += another.y_;
		z_ += another.z_;
		return *this;
	}
	ThisClass& operator-=(const ThisClass& another)
	{
		x_ -= another.x_;
		y_ -= another.y_;
		z_ -= another.z_;
		return *this;
	}
	ThisClass& operator*=(float value)
	{
		x_ *= value;
		y_ *= value;
		z_ *= value;
		return *this;
	}
	ThisClass& operator/=(float value)
	{
		x_ /= value;
		y_ /= value;
		z_ /= value;
		return *this;
	}

	float getMagnitudeSqr() const
	{
		float len2 = x_*x_ + y_*y_ + z_*z_;
		return len2;
	}

	float getMagnitude() const
	{
		float len2 = getMagnitudeSqr();
		float len = sqrtf(len2);
		return len;
	}

	ThisClass& normalize()
	{
		float value = getMagnitude();
		float rlen = 1.0f / value;
		x_ *= rlen;
		y_ *= rlen;
		z_ *= rlen;
		return *this;
	}
};

inline Vector3 operator+(const Vector3& a, const Vector3& b)
{
	return Vector3(a.getX()+b.getX(),
				   a.getY()+b.getY(),
				   a.getZ()+b.getZ());
}

inline Vector3 operator-(const Vector3& a, const Vector3& b)
{
	return Vector3(a.getX()-b.getX(),
				   a.getY()-b.getY(),
				   a.getZ()-b.getZ());
}

inline Vector3 operator*(const Vector3& a, float value)
{
	return Vector3(a.getX()*value,
				   a.getY()*value,
				   a.getZ()*value);
}

inline Vector3 operator/(const Vector3& a, float value)
{
	return Vector3(a.getX()/value,
				   a.getY()/value,
				   a.getZ()/value);
}

inline float dot(const Vector3& a, const Vector3& b)
{
	return (a.getX() * b.getX() + 
			a.getY() * b.getY() + 
			a.getZ() * b.getZ());
}

inline Vector3 cross(const Vector3& a, const Vector3& b)
{
	float ax = a.getX();
	float ay = a.getY();
	float az = a.getZ();
	float bx = b.getX();
	float by = b.getY();
	float bz = b.getZ();
	Vector3 value(ay*bz - az*by,
				  az*bx - ax*bz,
				  ax*by - ay*bx);
	return value;
}

inline Vector3 getNormal(const Vector3& a, const Vector3& b, const Vector3& c)
{
	float x0 = a.getX() - b.getX();
	float y0 = a.getY() - b.getY();
	float z0 = a.getZ() - b.getZ();
	float x1 = a.getX() - c.getX();
	float y1 = a.getY() - c.getY();
	float z1 = a.getZ() - c.getZ();
	Vector3 value(y0*z1 - z0*y1,
				  z0*x1 - x0*z1,
				  x0*y1 - y0*x1);
	value.normalize();
	return value;
}




class Vector2
{
private:
	typedef Vector2 ThisClass;
	float x_, y_;

public:
	Vector2()
		: x_(0.0f)
		, y_(0.0f)
	{}
	Vector2(float x, float y)
		: x_(x)
		, y_(y)
	{}

	float getX() const { return x_; }
	float getY() const { return y_; }
	void setX(float v) { x_ = v; }
	void setY(float v) { y_ = v; }
	void set(float x, float y) {
		x_ = x;
		y_ = y;
	}
	void setZero() {
		x_ = 0.0f;
		y_ = 0.0f;
	}
	ThisClass& operator=(const ThisClass& another)
	{
		x_ = another.x_;
		y_ = another.y_;
		return *this;
	}
};

} // namespace UTJ {

#endif	/* PLUGINSOURCES_VECTOR_HPP_WED_JAN_25_09_46_56_2017 */
/*
 * End of Vector.hpp
 */
