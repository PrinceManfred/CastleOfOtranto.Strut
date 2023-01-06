using System;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace CastleOfOtranto.Strut
{
	public class TypeInfoCache
	{
		private readonly ConcurrentDictionary<TypeInfoMemberCacheKey,
            TypeInfoCacheEntry> _memberCache = new();

        private readonly ConcurrentDictionary<TypeInfoParameterCacheKey,
            TypeInfoCacheEntry> _parameterCache = new();

        #region Member Methods
        public bool Contains(Type containingType, string memberName)
		{
            return Contains(new TypeInfoMemberCacheKey(containingType, memberName));
		}

        public bool Contains(TypeInfoMemberCacheKey key)
        {
            return _memberCache.ContainsKey(key);
        }

        public bool TryAdd(Type containingType, string memberName, TypeInfoCacheEntry entry)
        {
            return TryAdd(new TypeInfoMemberCacheKey(containingType, memberName), entry);
        }

        public bool TryAdd(TypeInfoMemberCacheKey key, TypeInfoCacheEntry entry)
        {
            return _memberCache.TryAdd(key, entry);
        }

        public bool TryGet(Type containingType, string memberName, out TypeInfoCacheEntry entry)
        {
            return TryGet(new TypeInfoMemberCacheKey(containingType, memberName), out entry);
        }

        public bool TryGet(TypeInfoMemberCacheKey key, out TypeInfoCacheEntry entry)
        {
            return _memberCache.TryGetValue(key, out entry);
        }
        #endregion

        #region Parameter Methods
        public bool Contains(Type containingType, string methodName, string parameterName)
        {
            return Contains(new TypeInfoParameterCacheKey(containingType, methodName, parameterName));
        }

        public bool Contains(TypeInfoParameterCacheKey key)
        {
            return _parameterCache.ContainsKey(key);
        }

        public bool TryAdd(Type containingType, string methodName, string parameterName, TypeInfoCacheEntry entry)
        {
            return TryAdd(new TypeInfoParameterCacheKey(containingType, methodName, parameterName), entry);
        }

        public bool TryAdd(TypeInfoParameterCacheKey key, TypeInfoCacheEntry entry)
        {
            return _parameterCache.TryAdd(key, entry);
        }

        public bool TryGet(Type containingType, string methodName, string parameterName, out TypeInfoCacheEntry entry)
        {
            return TryGet(new TypeInfoParameterCacheKey(containingType, methodName, parameterName), out entry);
        }

        public bool TryGet(TypeInfoParameterCacheKey key, out TypeInfoCacheEntry entry)
        {
            return _parameterCache.TryGetValue(key, out entry);
        }
        #endregion
    }
}

