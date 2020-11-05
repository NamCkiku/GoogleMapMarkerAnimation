﻿using System.Collections.Concurrent;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Android.Factories;
using AndroidBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;

namespace RouteDemo.Droid
{
    public class CachingNativeBitmapDescriptorFactory : IBitmapDescriptorFactory
    {
        private readonly ConcurrentDictionary<string, AndroidBitmapDescriptor> _cache
             = new ConcurrentDictionary<string, AndroidBitmapDescriptor>();

        public AndroidBitmapDescriptor ToNative(BitmapDescriptor descriptor)
        {
            var defaultFactory = DefaultBitmapDescriptorFactory.Instance;

            if (!string.IsNullOrEmpty(descriptor.Id))
            {
                var cacheEntry = _cache.GetOrAdd(descriptor.Id, _ => defaultFactory.ToNative(descriptor));
                return cacheEntry;
            }

            return defaultFactory.ToNative(descriptor);
        }
    }
}