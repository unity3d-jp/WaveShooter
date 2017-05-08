/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UTJ {

public class SetListSizeOperator
{
	MethodInfo set_value_internal_method_;
	object[] args_;
	delegate void SetListSizeDelegate(FieldInfo fi, object obj, object value);
	SetListSizeDelegate set_size_delegate_;

	public SetListSizeOperator(FieldInfo fi)
	{
		set_value_internal_method_  = fi.GetType().GetMethod("SetValueInternal",
															 (BindingFlags.NonPublic | 
															  BindingFlags.Static |
															  BindingFlags.FlattenHierarchy));
		args_ = new object[3] {
			null,
			null,
			null,
		};
		set_size_delegate_ = (SetListSizeDelegate)System.Delegate.CreateDelegate(typeof(SetListSizeDelegate),
																				 set_value_internal_method_);
	}

	public void invoke_slower(FieldInfo fi, object list, object size)
	{
		args_[0] = fi;
		args_[1] = list;
		args_[2] = size;
		set_value_internal_method_.Invoke(null, args_);
	}

	public void invoke(FieldInfo fi, object list, object size)
	{
		set_size_delegate_(fi, list, size);
	}

}

} // namespace UTJ {
/*
 * End of SetListSizeOperator.cs
 */
