﻿using System;

namespace SeSkydriveLoad
{
    public class SkydriveContent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; }

        public SkydriveContent()
        {
        }

        public SkydriveContent(string id, string name, string size, string updatedTime)
        {
            Id = id;
            Name = name;
            Size = Convert.ToInt64(size);
            UpdatedTime = Convert.ToDateTime(updatedTime);
        }

        public bool IsFolder { get { return Type.ToLower() == "folder"; } }

        public bool IsFile { get { return Type.ToLower() == "file"; } }
    }
}