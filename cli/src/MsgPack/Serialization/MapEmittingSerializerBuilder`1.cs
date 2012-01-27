﻿#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010 FUJIWARA, Yusuke
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
#endregion -- License Terms --

using System;
using NLiblet.Reflection;

namespace MsgPack.Serialization
{
	/// <summary>
	///		<see cref="EmittingSerializerBuilder{T}"/> implementation which emits as map.
	/// </summary>
	/// <typeparam name="TObject">The type of the target object.</typeparam>
	internal sealed class MapEmittingSerializerBuilder<TObject> : EmittingSerializerBuilder<TObject>
	{
		public MapEmittingSerializerBuilder( SerializationContext context )
			: base( context ) { }

		protected sealed override void EmitPackMembers( SerializerEmitter emitter, TracingILGenerator packerIL, SerializingMember[] entries )
		{
			packerIL.EmitAnyLdarg( 1 );
			packerIL.EmitAnyLdc_I4( entries.Length );
			packerIL.EmitAnyCall( Metadata._Packer.PackMapHeader );
			packerIL.EmitPop();

			foreach ( var entry in entries )
			{
				packerIL.EmitAnyLdarg( 1 );
				packerIL.EmitLdstr( entry.Member.Name );
				packerIL.EmitAnyCall( Metadata._Packer.PackString );
				packerIL.EmitPop();
				Emittion.EmitSerializeValue(
					emitter,
					packerIL,
					1,
					entry.Member.GetMemberValueType(),
					il0 =>
					{
						if ( typeof( TObject ).IsValueType )
						{
							il0.EmitAnyLdarga( 2 );
						}
						else
						{
							il0.EmitAnyLdarg( 2 );
						}

						Emittion.EmitLoadValue( il0, entry.Member );
					}
				);
			}

			packerIL.EmitRet();
		}
	}
}