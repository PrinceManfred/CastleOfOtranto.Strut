using System;
using System.Reflection;

namespace CastleOfOtranto.Strut
{
	public readonly record struct TypeInfoCacheEntry(
		bool HasDefault,
		NullabilityState NullabilityState);

	public readonly record struct TypeInfoMemberCacheKey(
		Type ContainingType,
		string MemberName);

	public readonly record struct TypeInfoParameterCacheKey(
		Type ContainingType,
		string MethodName,
		string ParameterName);
}