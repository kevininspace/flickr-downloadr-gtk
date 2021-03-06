﻿namespace FloydPink.Flickr.Downloadr.Logic.Extensions {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using Model.Constants;

    public static class DictionaryExtensions {
        private static readonly bool runningOnMono = Type.GetType("Mono.Runtime") != null;

        public static object GetValue(this Dictionary<string, object> dictionary, string key) {
            return dictionary.ContainsKey(key) ? dictionary[key] : string.Empty;
        }

        public static object GetSubValue(this Dictionary<string, object> dictionary, string key,
                                         string subKey = AppConstants.FlickrDictionaryContentKey) {
            if (dictionary.ContainsKey(key)) {
                var subDictionary = (Dictionary<string, object>) dictionary[key];
                return subDictionary.ContainsKey(subKey) ? subDictionary[subKey] : null;
            }
            return null;
        }

        public static PhotosResponse GetPhotosResponseFromDictionary(this Dictionary<string, object> dictionary) {
            var photos = new List<Photo>();
            IEnumerable<Dictionary<string, object>> photoDictionary;

            if (runningOnMono) {
                var photoListAsArrayList = (ArrayList) dictionary.GetSubValue("photos", "photo");
                photoDictionary = photoListAsArrayList.Cast<Dictionary<string, object>>();
            } else {
                var photoListAsIEnumerable = (IEnumerable<object>) dictionary.GetSubValue("photos", "photo");
                photoDictionary = photoListAsIEnumerable.Cast<Dictionary<string, object>>();
            }

            photos.AddRange(photoDictionary.Select(BuildPhoto));

            return new PhotosResponse(
                int.Parse(dictionary.GetSubValue("photos", "page").ToString()),
                int.Parse(dictionary.GetSubValue("photos", "pages").ToString()),
                int.Parse(dictionary.GetSubValue("photos", "perpage").ToString()),
                int.Parse(dictionary.GetSubValue("photos", "total").ToString()),
                photos);
        }

        public static IEnumerable<string> ExtractOriginalTags(this Dictionary<string, object> dictionary) {
            IEnumerable<Dictionary<string, object>> tagList;

            var photoJson = (Dictionary<string, object>) dictionary.GetValue("photo");
            var tagsJson = (Dictionary<string, object>) photoJson.GetValue("tags");

            if (runningOnMono) {
                var tagListAsArrayList = (ArrayList) tagsJson.GetValue("tag");
                tagList = tagListAsArrayList.Cast<Dictionary<string, object>>();
            } else {
                var tagListAsIEnumerable = (IEnumerable<object>) tagsJson.GetValue("tag");
                tagList = tagListAsIEnumerable.Cast<Dictionary<string, object>>();
            }

            return (from Dictionary<string, object> tag in tagList
                    select tag.GetValue("raw").ToString()).ToList();
        }

        private static Photo BuildPhoto(Dictionary<string, object> dictionary) {
            return new Photo(dictionary.GetValue("id").ToString(),
                dictionary.GetValue("owner").ToString(),
                dictionary.GetValue("secret").ToString(),
                dictionary.GetValue("server").ToString(),
                int.Parse(dictionary.GetValue("farm").ToString()),
                dictionary.GetValue("title").ToString(),
                Convert.ToBoolean(dictionary.GetValue("ispublic")),
                Convert.ToBoolean(dictionary.GetValue("isfriend")),
                Convert.ToBoolean(dictionary.GetValue("isfamily")),
                dictionary.GetSubValue("description").ToString().Trim(),
                dictionary.GetValue("tags").ToString(),
                dictionary.GetValue("originalsecret").ToString(),
                dictionary.GetValue("originalformat").ToString(),
                dictionary.GetValue("url_sq").ToString(),
                dictionary.GetValue("url_q").ToString(),
                dictionary.GetValue("url_t").ToString(),
                dictionary.GetValue("url_s").ToString(),
                dictionary.GetValue("url_n").ToString(),
                dictionary.GetValue("url_m").ToString(),
                dictionary.GetValue("url_z").ToString(),
                dictionary.GetValue("url_c").ToString(),
                dictionary.GetValue("url_l").ToString(),
                dictionary.GetValue("url_o").ToString());
        }
    }
}
