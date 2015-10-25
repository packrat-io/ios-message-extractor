using System;

namespace Extractor.iOS.Entity
{
    class DbMessage
    {
        public int ROWID { get; set; }

        public Guid guid { get; set; }

        public string text { get; set; }

        public string subject { get; set; }

        public string handle { get; set; }

        public string my_handle { get; set; }

        public int date { get; set; }

        public int date_read { get; set; }

        public int date_delivered { get; set; }

        public string service { get; set; }

        public bool is_from_me { get; set; }

        public int item_type { get; set; }

        public int other_handle { get; set; }
    }
}
